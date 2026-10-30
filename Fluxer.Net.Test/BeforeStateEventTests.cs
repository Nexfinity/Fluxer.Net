using Fluxer.Net.Gateway;

namespace Fluxer.Net.Test;

public class BeforeStateEventTests
{
    private const string Author = "{\"id\":\"1\",\"username\":\"a\"}";

    private static (FluxerClient Client, SocketGuild Guild) Create(int messageCacheSize = 10)
    {
        FluxerClient client = new("Bot test");
        SocketGuild guild = new(client);
        guild.Update(new GuildJson { Id = 10, Name = "Test" });
        client.Gateway.Guilds[10] = guild;
        client.Gateway.Channels[3] = Channel.Create(client, new ChannelJson { Id = 3, GuildId = 10, Type = ChannelType.GuildText });
        client.Gateway.Messages = new MessageCache(messageCacheSize);
        return (client, guild);
    }

    private static string Dispatch(string name, string data) => $"{{\"op\":0,\"t\":\"{name}\",\"s\":1,\"d\":{data}}}";

    private static string MessageData(ulong id, string content)
        => $"{{\"id\":\"{id}\",\"channel_id\":\"3\",\"guild_id\":\"10\",\"author\":{Author},\"content\":\"{content}\",\"timestamp\":\"2025-01-01T00:00:00Z\"}}";

    [Test]
    public void EditedMessageComesWithTheCachedVersion()
    {
        (FluxerClient client, _) = Create();
        (SocketMessage? Before, SocketMessage After)? edit = null;
        client.Gateway.MessageEdited += (before, after) => edit = (before, after);

        client.Gateway.GatewayMessageHandler(Dispatch("MESSAGE_CREATE", MessageData(20, "first")));
        client.Gateway.GatewayMessageHandler(Dispatch("MESSAGE_UPDATE", MessageData(20, "second")));

        Assert.Multiple(() =>
        {
            Assert.That(edit?.Before?.Content, Is.EqualTo("first"));
            Assert.That(edit?.After.Content, Is.EqualTo("second"));
            Assert.That(client.Gateway.GetCachedMessage(20)?.Content, Is.EqualTo("second"));
        });
    }

    [Test]
    public void DeletedMessageComesWithTheCachedVersion()
    {
        (FluxerClient client, _) = Create();
        SocketMessage? removed = null;
        client.Gateway.MessageRemoved += (_, _, message) => removed = message;

        client.Gateway.GatewayMessageHandler(Dispatch("MESSAGE_CREATE", MessageData(20, "gone soon")));
        client.Gateway.GatewayMessageHandler(Dispatch("MESSAGE_DELETE", "{\"id\":\"20\",\"channel_id\":\"3\",\"guild_id\":\"10\"}"));

        Assert.Multiple(() =>
        {
            Assert.That(removed?.Content, Is.EqualTo("gone soon"));
            Assert.That(client.Gateway.GetCachedMessage(20), Is.Null);
        });
    }

    [Test]
    public void BulkDeleteHandsOverOnlyTheCachedMessages()
    {
        (FluxerClient client, _) = Create();
        IReadOnlyList<ulong>? ids = null;
        IReadOnlyList<SocketMessage>? messages = null;
        client.Gateway.MessagesRemoved += (_, i, m) => (ids, messages) = (i, m);

        client.Gateway.GatewayMessageHandler(Dispatch("MESSAGE_CREATE", MessageData(20, "a")));
        client.Gateway.GatewayMessageHandler(Dispatch("MESSAGE_DELETE_BULK", "{\"ids\":[\"20\",\"21\"],\"channel_id\":\"3\",\"guild_id\":\"10\"}"));

        Assert.Multiple(() =>
        {
            Assert.That(ids, Is.EqualTo(new ulong[] { 20, 21 }));
            Assert.That(messages!.Select(x => x.Id), Is.EqualTo(new ulong[] { 20 }));
        });
    }

    [Test]
    public void CacheKeepsOnlyTheNewestMessagesPerChannel()
    {
        (FluxerClient client, _) = Create(messageCacheSize: 2);

        foreach (ulong id in new ulong[] { 1, 2, 3 })
            client.Gateway.GatewayMessageHandler(Dispatch("MESSAGE_CREATE", MessageData(id, "m")));

        Assert.Multiple(() =>
        {
            Assert.That(client.Gateway.GetCachedMessage(1), Is.Null);
            Assert.That(client.Gateway.Messages.GetChannel(3).Select(x => x.Id), Is.EqualTo(new ulong[] { 2, 3 }));
        });
    }

    [Test]
    public void WithoutACacheEditsHaveNoBeforeVersion()
    {
        (FluxerClient client, _) = Create(messageCacheSize: 0);
        bool raised = false;
        SocketMessage? before = null;
        client.Gateway.MessageEdited += (b, _) => (raised, before) = (true, b);

        client.Gateway.GatewayMessageHandler(Dispatch("MESSAGE_CREATE", MessageData(20, "first")));
        client.Gateway.GatewayMessageHandler(Dispatch("MESSAGE_UPDATE", MessageData(20, "second")));

        Assert.That(raised, Is.True);
        Assert.That(before, Is.Null);
    }

    [Test]
    public void MemberChangeKeepsTheOldNicknameAndRoles()
    {
        (FluxerClient client, _) = Create();
        (SocketGuildMember? Before, SocketGuildMember After)? change = null;
        client.Gateway.MemberChanged += (before, after) => change = (before, after);
        string member(string nick, string roles)
            => $"{{\"guild_id\":\"10\",\"user\":{{\"id\":\"5\",\"username\":\"u\"}},\"nick\":\"{nick}\",\"roles\":[{roles}],\"joined_at\":\"2025-01-01T00:00:00Z\"}}";

        client.Gateway.GatewayMessageHandler(Dispatch("GUILD_MEMBER_ADD", member("old", "\"7\"")));
        client.Gateway.GatewayMessageHandler(Dispatch("GUILD_MEMBER_UPDATE", member("new", "\"7\",\"8\"")));

        Assert.Multiple(() =>
        {
            Assert.That(change?.Before?.Nickname, Is.EqualTo("old"));
            Assert.That(change?.Before?.RoleIds, Is.EquivalentTo(new ulong[] { 7 }));
            Assert.That(change?.After.Nickname, Is.EqualTo("new"));
            Assert.That(change?.After.RoleIds, Is.EquivalentTo(new ulong[] { 7, 8 }));
        });
    }

    [Test]
    public void VoiceMoveComesWithThePreviousChannel()
    {
        (FluxerClient client, _) = Create();
        client.Gateway.Channels[40] = Channel.Create(client, new ChannelJson { Id = 40, GuildId = 10, Type = ChannelType.GuildVoice });
        client.Gateway.Channels[41] = Channel.Create(client, new ChannelJson { Id = 41, GuildId = 10, Type = ChannelType.GuildVoice });
        List<(ulong? Before, ulong? After)> changes = new();
        client.Gateway.VoiceStateChanged += (before, after) => changes.Add((before?.ChannelId, after.ChannelId));
        string state(string channel)
            => $"{{\"guild_id\":\"10\",\"channel_id\":{channel},\"user_id\":\"5\",\"session_id\":\"s1\",\"member\":{{\"guild_id\":\"10\",\"user\":{{\"id\":\"5\",\"username\":\"u\"}},\"roles\":[],\"joined_at\":\"2025-01-01T00:00:00Z\"}}}}";

        client.Gateway.GatewayMessageHandler(Dispatch("VOICE_STATE_UPDATE", state("\"40\"")));
        client.Gateway.GatewayMessageHandler(Dispatch("VOICE_STATE_UPDATE", state("\"41\"")));
        client.Gateway.GatewayMessageHandler(Dispatch("VOICE_STATE_UPDATE", state("null")));

        Assert.That(changes, Is.EqualTo(new List<(ulong?, ulong?)> { (null, 40), (40, 41), (41, null) }));
    }
}
