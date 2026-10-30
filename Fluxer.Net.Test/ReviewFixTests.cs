using System.Text;
using Fluxer.Net.Gateway;
using Fluxer.Net.Rest;
using Newtonsoft.Json;

namespace Fluxer.Net.Test;

public class ReviewFixTests
{
    private const string Author = "{\"id\":\"1\",\"username\":\"a\"}";

    private static (FluxerClient Client, RecordingHandler Handler) Create(params string[] responses)
    {
        RecordingHandler handler = new(responses.Length == 0 ? new[] { "{}" } : responses);
        return (new FluxerClient("Bot test", RecordingHandler.Config(new HttpClient(handler))), handler);
    }

    private static string Dispatch(string name, string data) => $"{{\"op\":0,\"t\":\"{name}\",\"s\":1,\"d\":{data}}}";

    private static string MessageData(ulong id, ulong channelId = 3)
        => $"{{\"id\":\"{id}\",\"channel_id\":\"{channelId}\",\"guild_id\":\"10\",\"author\":{Author},\"content\":\"m\",\"timestamp\":\"2025-01-01T00:00:00Z\"}}";

    private static FluxerClient GatewayClient(int cacheSize)
    {
        FluxerClient client = new("Bot test");
        SocketGuild guild = new(client);
        guild.Update(new GuildJson { Id = 10, Name = "Test" });
        client.Gateway.Guilds[10] = guild;
        client.Gateway.Channels[3] = Channel.Create(client, new ChannelJson { Id = 3, GuildId = 10, Type = ChannelType.GuildText });
        client.Gateway.Channels[4] = Channel.Create(client, new ChannelJson { Id = 4, GuildId = 10, Type = ChannelType.GuildText });
        TestCache.Use(client, messagesPerChannel: cacheSize);
        return client;
    }

    [Test]
    public async Task LegacyMemberUpdateDoesNotSendProfileFlags()
    {
        (FluxerClient client, RecordingHandler handler) = Create("{\"user\":{\"id\":\"5\",\"username\":\"u\"},\"roles\":[],\"joined_at\":\"2025-01-01T00:00:00Z\"}");

#pragma warning disable CS0618
        await client.Rest.UpdateCurrentMemberAsync(10, new GuildMemberJson { Nickname = "x", User = new UserJson() });
#pragma warning restore CS0618

        Newtonsoft.Json.Linq.JObject body = Newtonsoft.Json.Linq.JObject.Parse(handler.Requests.Single().Body);
        Assert.That(body.ContainsKey("profile_flags") || body.ContainsKey("mention_flags"), Is.False);
    }

    [Test]
    public void DeletedMessagesFreeTheirCacheSlot()
    {
        FluxerClient client = GatewayClient(2);

        client.Gateway.GatewayMessageHandler(Dispatch("MESSAGE_CREATE", MessageData(1)));
        client.Gateway.GatewayMessageHandler(Dispatch("MESSAGE_CREATE", MessageData(2)));
        client.Gateway.GatewayMessageHandler(Dispatch("MESSAGE_DELETE", "{\"id\":\"2\",\"channel_id\":\"3\",\"guild_id\":\"10\"}"));
        client.Gateway.GatewayMessageHandler(Dispatch("MESSAGE_CREATE", MessageData(3)));

        Assert.That(TestCache.Messages(client).GetChannelKeys(3), Is.EqualTo(new ulong[] { 1, 3 }));
    }

    [Test]
    public void LeavingAGuildForgetsItsCachedMessages()
    {
        FluxerClient client = GatewayClient(10);
        client.Gateway.GatewayMessageHandler(Dispatch("MESSAGE_CREATE", MessageData(1, 3)));
        client.Gateway.GatewayMessageHandler(Dispatch("MESSAGE_CREATE", MessageData(2, 4)));

        client.Gateway.GatewayMessageHandler(Dispatch("GUILD_DELETE", "{\"id\":\"10\"}"));

        Assert.That(TestCache.Messages(client).Count, Is.Zero);
    }

    [Test]
    public async Task DeletedThreadsForgetTheirCachedMessages()
    {
        FluxerClient client = GatewayClient(10);
        client.Gateway.GatewayMessageHandler(Dispatch("MESSAGE_CREATE", MessageData(1, 3)));

        client.Gateway.GatewayMessageHandler(Dispatch("THREAD_DELETE", "{\"id\":\"3\",\"guild_id\":\"10\",\"type\":11}"));

        Assert.That(await client.Gateway.GetCachedMessageAsync(1), Is.Null);
    }

    [Test]
    public async Task SinglePartUploadLeavesTheCallersStreamOpen()
    {
        (FluxerClient client, _) = Create(
            "{\"attachments\":[{\"id\":0,\"filename\":\"a.txt\",\"file_size\":2,\"content_type\":\"text/plain\",\"upload_filename\":\"up/2\",\"upload_mode\":\"singlepart\",\"upload_url\":\"https://store.test/one\"}]}",
            "{}");
        MemoryStream stream = new(Encoding.ASCII.GetBytes("hi"));

        await client.Rest.UploadAttachmentAsync(3, stream, "a.txt", "text/plain");

        Assert.That(stream.CanRead, Is.True);
    }

    [Test]
    public void CancellingAnUploadStopsBeforeThePlan()
    {
        (FluxerClient client, RecordingHandler handler) = Create();
        using CancellationTokenSource cancel = new();
        cancel.Cancel();

        Assert.CatchAsync<OperationCanceledException>(() => client.Rest.UploadAttachmentAsync(3,
            new NonSeekableStream(Encoding.ASCII.GetBytes("hi")), "a.txt", options: new RequestOptions { CancelToken = cancel.Token }));
        Assert.That(handler.Requests, Is.Empty);
    }

    [Test]
    public void ConfiguredPresenceIsKeptWhenSettingACustomStatus()
    {
        FluxerClient client = new("Bot test");

        client.Gateway.SeedPresence(new PresenceUpdateGatewayData(Status.Dnd));
        client.Gateway.SetCustomStatus("busy");

        Assert.That(client.Gateway.CurrentStatus, Is.EqualTo(Status.Dnd));
        Assert.That(client.Gateway.CurrentCustomStatus?.Text, Is.EqualTo("busy"));
    }

    private sealed class NonSeekableStream(byte[] data) : MemoryStream(data)
    {
        public override bool CanSeek => false;
    }
}
