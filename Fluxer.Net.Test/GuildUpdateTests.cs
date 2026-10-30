using Fluxer.Net.Rest;
using Newtonsoft.Json;

namespace Fluxer.Net.Test;

public class GuildUpdateTests
{
    private static string Serialize(object value) => JsonConvert.SerializeObject(value, FluxerClient._restSerializer);

    private static (FluxerClient Client, RecordingHandler Handler) Create(params string[] responses)
    {
        RecordingHandler handler = new(responses);
        return (new FluxerClient("Bot test", RecordingHandler.Config(new HttpClient(handler))), handler);
    }

    private static string GuildJsonWith(params string[] features)
        => $"{{\"id\":\"10\",\"name\":\"Test\",\"owner_id\":\"1\",\"features\":[{string.Join(",", features.Select(x => $"\"{x}\""))}]}}";

    [Test]
    public void NewGuildSettingsAreSentWithServerNames()
    {
        UpdateGuildRequest request = new()
        {
            Features = new[] { "INVITES_DISABLED", "BANNER" },
            MfaLevel = GuildMfaLevel.Elevated,
            Nsfw = true,
            ContentWarningLevel = GuildContentWarning.ContentWarning,
            ContentWarningText = null,
            EmbedSplashBase64 = "data",
            SplashCardAlignment = GuildSplashCardAlignment.Right,
            MessageHistoryCutoff = new DateTimeOffset(2025, 3, 4, 5, 6, 7, TimeSpan.FromHours(2)),
            Password = "pw",
            MfaMethod = "totp",
            MfaCode = "123456"
        };

        string json = Serialize(request);

        Assert.Multiple(() =>
        {
            Assert.That(json, Does.Contain("\"features\":[\"INVITES_DISABLED\",\"BANNER\"]"));
            Assert.That(json, Does.Contain("\"mfa_level\":1"));
            Assert.That(json, Does.Contain("\"nsfw\":true"));
            Assert.That(json, Does.Contain("\"content_warning_level\":1"));
            Assert.That(json, Does.Contain("\"content_warning_text\":null"));
            Assert.That(json, Does.Contain("\"embed_splash\":\"data\""));
            Assert.That(json, Does.Contain("\"splash_card_alignment\":2"));
            Assert.That(json, Does.Contain("\"message_history_cutoff\":\"2025-03-04T03:06:07.000Z\""));
            Assert.That(json, Does.Contain("\"password\":\"pw\"").And.Contain("\"mfa_method\":\"totp\"").And.Contain("\"mfa_code\":\"123456\""));
        });
    }

    [Test]
    public void UnsetGuildSettingsAreNotSent()
    {
        Assert.Multiple(() =>
        {
            Assert.That(Serialize(new UpdateGuildRequest { Name = "x" }), Is.EqualTo("{\"name\":\"x\"}"));
            Assert.That(Serialize(new UpdateGuildRequest { MessageHistoryCutoff = Optional.Some<DateTimeOffset?>(null) }),
                Is.EqualTo("{\"message_history_cutoff\":null}"));
        });
    }

    [Test]
    public async Task PausingInvitesSendsTheCompleteFeatureSetWithTheFlagAdded()
    {
        (FluxerClient client, RecordingHandler handler) = Create(GuildJsonWith("BANNER", "VERIFIED"), GuildJsonWith("BANNER", "VERIFIED", "INVITES_DISABLED"));

        Guild? guild = await client.Rest.SetGuildInvitesPausedAsync(10, true);

        Assert.Multiple(() =>
        {
            Assert.That(handler.Requests, Has.Count.EqualTo(2));
            Assert.That(handler.Requests[1].Method, Is.EqualTo(HttpMethod.Patch));
            Assert.That(handler.Requests[1].Url, Does.EndWith("/guilds/10"));
            Assert.That(handler.Requests[1].Body, Is.EqualTo("{\"features\":[\"BANNER\",\"VERIFIED\",\"INVITES_DISABLED\"]}"));
            Assert.That(guild!.Features.IsInvitesDisabled, Is.True);
        });
    }

    [Test]
    public async Task ResumingInvitesRemovesOnlyTheFlag()
    {
        (FluxerClient client, RecordingHandler handler) = Create(GuildJsonWith("BANNER", "INVITES_DISABLED", "VERIFIED"), GuildJsonWith("BANNER", "VERIFIED"));

        await client.Rest.SetGuildInvitesPausedAsync(10, false);

        Assert.That(handler.Requests[1].Body, Is.EqualTo("{\"features\":[\"BANNER\",\"VERIFIED\"]}"));
    }

    [Test]
    public async Task InvitesAlreadyInTheRequestedStateSendNothing()
    {
        (FluxerClient client, RecordingHandler handler) = Create(GuildJsonWith("INVITES_DISABLED"));

        Guild? guild = await client.Rest.SetGuildInvitesPausedAsync(10, true);

        Assert.Multiple(() =>
        {
            Assert.That(handler.Requests, Has.Count.EqualTo(1));
            Assert.That(guild, Is.Not.Null);
        });
    }
}
