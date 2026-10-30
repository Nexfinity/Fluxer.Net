using Fluxer.Net.Rest;
using Newtonsoft.Json;

namespace Fluxer.Net.Test;

public class HelperTests
{
    private const string MemberJson = "{\"guild_id\":\"10\",\"user\":{\"id\":\"5\",\"username\":\"u\"},\"roles\":[],\"joined_at\":\"2025-01-01T00:00:00Z\"}";

    private static (FluxerClient Client, RecordingHandler Handler) Create(params string[] responses)
    {
        RecordingHandler handler = new(responses.Length == 0 ? new[] { "{}" } : responses);
        return (new FluxerClient("Bot test", RecordingHandler.Config(new HttpClient(handler))), handler);
    }

    private static GuildMember Member(FluxerClient client)
        => GuildMember.Create(client, JsonConvert.DeserializeObject<GuildMemberJson>(MemberJson, FluxerClient._restSerializer)!);

    [Test]
    public void JumpUrlsUseTheChannelsRoute()
    {
        (FluxerClient client, _) = Create();
        Message message = Message.Create(client, JsonConvert.DeserializeObject<MessageJson>(
            "{\"id\":\"30\",\"channel_id\":\"20\",\"author\":{\"id\":\"1\",\"username\":\"a\"},\"content\":\"\",\"timestamp\":\"2025-01-01T00:00:00Z\"}",
            FluxerClient._restSerializer)!);

        Assert.Multiple(() =>
        {
            Assert.That(message.GetJumpUrl(10), Is.EqualTo("https://web.fluxer.app/channels/10/20/30"));
            Assert.That(message.GetJumpUrl(null), Is.EqualTo("https://web.fluxer.app/channels/@me/20/30"));
            Assert.That(Member(client).GetProfileUrl(), Is.EqualTo("https://web.fluxer.app/users/5"));
        });
    }

    [Test]
    public async Task SeveralRolesAreAddedOneRequestEach()
    {
        (FluxerClient client, RecordingHandler handler) = Create();

        await Member(client).AddRolesAsync(new ulong[] { 7, 8, 7 });

        Assert.That(handler.Requests.Select(x => (x.Method, x.Url.Substring(x.Url.IndexOf("/guilds", StringComparison.Ordinal)))),
            Is.EqualTo(new[] { (HttpMethod.Put, "/guilds/10/members/5/roles/7"), (HttpMethod.Put, "/guilds/10/members/5/roles/8") }));
    }

    [Test]
    public async Task TimeoutByDurationSendsTheEndTimeAndReason()
    {
        (FluxerClient client, RecordingHandler handler) = Create(MemberJson);

        await Member(client).SetTimeoutAsync(TimeSpan.FromMinutes(10), "spam");

        string body = handler.Requests.Single().Body;
        Assert.Multiple(() =>
        {
            Assert.That(body, Does.Contain("\"communication_disabled_until\":\""));
            Assert.That(body, Does.Contain("\"timeout_reason\":\"spam\""));
        });
    }

    [Test]
    public async Task BanByDaysSendsSeconds()
    {
        (FluxerClient client, RecordingHandler handler) = Create();
        SocketGuild guild = new(client);
        guild.Update(new GuildJson { Id = 10, Name = "Test" });

        await guild.BanUserAsync(5, deleteMessageDays: 2);

        Assert.That(handler.Requests.Single().Body, Does.Contain("\"delete_message_seconds\":172800"));
    }

    [Test]
    public void EmbedBuilderCopiesAReceivedEmbed()
    {
        EmbedJson json = JsonConvert.DeserializeObject<EmbedJson>(
            "{\"type\":\"rich\",\"title\":\"T\",\"description\":\"D\",\"color\":255,\"author\":{\"name\":\"A\",\"icon_url\":\"https://x.test/a.png\"}," +
            "\"footer\":{\"text\":\"F\"},\"image\":{\"url\":\"https://x.test/i.png\"},\"fields\":[{\"name\":\"n\",\"value\":\"v\",\"inline\":true}]}",
            FluxerClient._restSerializer)!;

        EmbedBuilder builder = EmbedBuilder.FromEmbed(json);

        Assert.Multiple(() =>
        {
            Assert.That(builder.Title, Is.EqualTo("T"));
            Assert.That(builder.Color, Is.EqualTo(255));
            Assert.That(builder.Author?.Name, Is.EqualTo("A"));
            Assert.That(builder.Author?.IconUrl, Is.EqualTo("https://x.test/a.png"));
            Assert.That(builder.Footer?.Text, Is.EqualTo("F"));
            Assert.That(builder.ImageUrl, Is.EqualTo("https://x.test/i.png"));
            Assert.That(builder.Fields.Single().IsInline, Is.True);
        });
    }

    [Test]
    public void AllowedMentionPresetsSerializeAsExpected()
    {
        string Serialize(object value) => JsonConvert.SerializeObject(value, FluxerClient._restSerializer);

        Assert.Multiple(() =>
        {
            Assert.That(Serialize(AllowedMentionsRequest.None), Is.EqualTo("{\"parse\":[],\"replied_user\":false}"));
            Assert.That(Serialize(AllowedMentionsRequest.All), Is.EqualTo("{\"parse\":[\"users\",\"roles\",\"everyone\"],\"replied_user\":true}"));
            Assert.That(Serialize(AllowedMentionsRequest.Only(users: new ulong[] { 5 })), Does.Contain("\"parse\":[]").And.Contain("\"users\":["));
        });
    }

    [Test]
    public void ChangingTheStatusKeepsTheCustomStatus()
    {
        (FluxerClient client, _) = Create();

        client.Gateway.SetCustomStatus("brb");
        client.Gateway.SetStatus(Status.Idle);

        Assert.Multiple(() =>
        {
            Assert.That(client.Gateway.CurrentStatus, Is.EqualTo(Status.Idle));
            Assert.That(client.Gateway.CurrentCustomStatus?.Text, Is.EqualTo("brb"));
        });

        client.Gateway.SetCustomStatus(null);
        Assert.That(client.Gateway.CurrentCustomStatus, Is.Null);
        Assert.That(client.Gateway.CurrentStatus, Is.EqualTo(Status.Idle));
    }

    [Test]
    public void WebhookUrlNeedsAToken()
    {
        (FluxerClient client, _) = Create();
        Webhook withToken = Webhook.Create(client, JsonConvert.DeserializeObject<WebhookJson>("{\"id\":\"3\",\"token\":\"abc\"}", FluxerClient._restSerializer)!);
        Webhook withoutToken = Webhook.Create(client, JsonConvert.DeserializeObject<WebhookJson>("{\"id\":\"3\"}", FluxerClient._restSerializer)!);

        Assert.That(withToken.GetUrl(), Is.EqualTo("https://api.fluxer.app/v1/webhooks/3/abc"));
        Assert.That(withoutToken.GetUrl(), Is.Null);
    }
}
