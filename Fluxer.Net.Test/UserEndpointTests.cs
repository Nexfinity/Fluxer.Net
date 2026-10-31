using Fluxer.Net.Rest;

namespace Fluxer.Net.Test;

public class UserEndpointTests
{
    private static (FluxerClient Client, RecordingHandler Handler) Create(string token, string response = "{}")
    {
        RecordingHandler handler = new(response);
        return (new FluxerClient(token, RecordingHandler.Config(new HttpClient(handler))), handler);
    }

    [Test]
    public async Task GifSearchAndTrendingCarryTheQueryAndLocale()
    {
        (FluxerClient client, RecordingHandler handler) = Create("flx_test", "[]");

        await client.Rest.SearchGifsAsync("cat", "en-US");
        await client.Rest.GetGifsTrendingAsync();

        Assert.Multiple(() =>
        {
            Assert.That(handler.Requests[0].Method, Is.EqualTo(HttpMethod.Get));
            Assert.That(handler.Requests[0].Url, Does.EndWith("/gifs/search?q=cat&locale=en-US"));
            Assert.That(handler.Requests[1].Url, Does.EndWith("/gifs/trending"));
        });
    }

    [Test]
    public async Task RegisteringAGifShareSendsTheSlugQueryAndLocale()
    {
        (FluxerClient client, RecordingHandler handler) = Create("flx_test");

        await client.Rest.RegisterGifShareAsync(new RegisterGifShareRequest { Id = "cat-123", Query = "cat", Locale = "en-US" });

        Assert.Multiple(() =>
        {
            Assert.That(handler.Requests[0].Method, Is.EqualTo(HttpMethod.Post));
            Assert.That(handler.Requests[0].Url, Does.EndWith("/gifs/register-share"));
            Assert.That(handler.Requests[0].Body, Does.Contain("\"id\":\"cat-123\"").And.Contain("\"q\":\"cat\"").And.Contain("\"locale\":\"en-US\""));
        });
    }

    [Test]
    public async Task ConnectionsUseTheTypeAndIdInTheRoute()
    {
        (FluxerClient client, RecordingHandler handler) = Create("flx_test");

        await client.Rest.UpdateConnectionAsync("domain", "example.com", new UpdateConnectionRequest { VisibilityFlags = 2, SortOrder = 1 });
        await client.Rest.DeleteConnectionAsync("bluesky", "me.bsky.social");
        await client.Rest.ReorderConnectionsAsync(new ReorderConnectionsRequest { ConnectionIds = new[] { "a", "b" } });

        Assert.Multiple(() =>
        {
            Assert.That(handler.Requests[0].Method, Is.EqualTo(HttpMethod.Patch));
            Assert.That(handler.Requests[0].Url, Does.EndWith("/users/@me/connections/domain/example.com"));
            Assert.That(handler.Requests[0].Body, Does.Contain("\"visibility_flags\":2").And.Contain("\"sort_order\":1"));
            Assert.That(handler.Requests[1].Method, Is.EqualTo(HttpMethod.Delete));
            Assert.That(handler.Requests[1].Url, Does.EndWith("/users/@me/connections/bluesky/me.bsky.social"));
            Assert.That(handler.Requests[2].Url, Does.EndWith("/users/@me/connections/reorder"));
            Assert.That(handler.Requests[2].Body, Does.Contain("\"connection_ids\":[\"a\",\"b\"]"));
        });
    }

    [Test]
    public async Task ClearingAnEntranceSoundSelectionSendsANullSoundId()
    {
        (FluxerClient client, RecordingHandler handler) = Create("flx_test");

        await client.Rest.SetEntranceSoundSelectionAsync(new SetEntranceSoundSelectionRequest { ScopeId = "guild:5", SoundId = null });
        await client.Rest.PlayEntranceSoundAsync(7, 9);

        Assert.Multiple(() =>
        {
            Assert.That(handler.Requests[0].Method, Is.EqualTo(HttpMethod.Put));
            Assert.That(handler.Requests[0].Url, Does.EndWith("/users/@me/entrance-sound-selections"));
            Assert.That(handler.Requests[0].Body, Does.Contain("\"scope_id\":\"guild:5\"").And.Contain("\"sound_id\":null"));
            Assert.That(handler.Requests[1].Method, Is.EqualTo(HttpMethod.Post));
            Assert.That(handler.Requests[1].Url, Does.EndWith("/voice/channels/7/entrance-sound"));
            Assert.That(handler.Requests[1].Body, Does.Contain("\"sound_id\":"));
        });
    }

    [Test]
    public async Task GuildVisitsUseTheirOwnRoutes()
    {
        (FluxerClient client, RecordingHandler handler) = Create("flx_test", "{\"guild_id\":\"4\",\"member\":true,\"visit\":null}");

        GuildVisitStartJson started = await client.Rest.StartDiscoveryGuildVisitAsync(4);
        await client.Rest.StartInviteVisitAsync("abc");
        await client.Rest.EndGuildVisitAsync(4);

        Assert.Multiple(() =>
        {
            Assert.That(started.IsMember, Is.True);
            Assert.That(started.Visit, Is.Null);
            Assert.That(handler.Requests[0].Method, Is.EqualTo(HttpMethod.Post));
            Assert.That(handler.Requests[0].Url, Does.EndWith("/discovery/guilds/4/visit"));
            Assert.That(handler.Requests[1].Url, Does.EndWith("/invites/abc/visit"));
            Assert.That(handler.Requests[2].Method, Is.EqualTo(HttpMethod.Delete));
            Assert.That(handler.Requests[2].Url, Does.EndWith("/users/@me/guild-visits/4"));
        });
    }

    [Test]
    public async Task ReportsSendTheCategoryAndTarget()
    {
        (FluxerClient client, RecordingHandler handler) = Create("flx_test", "{\"report_id\":\"11\",\"status\":\"pending\",\"reported_at\":\"2025-01-01T00:00:00Z\"}");

        ReportJson report = await client.Rest.ReportMessageAsync(new ReportMessageRequest { ChannelId = 2, MessageId = 3, Category = "spam" });

        Assert.Multiple(() =>
        {
            Assert.That(report.Status, Is.EqualTo("pending"));
            Assert.That(handler.Requests[0].Method, Is.EqualTo(HttpMethod.Post));
            Assert.That(handler.Requests[0].Url, Does.EndWith("/reports/message"));
            Assert.That(handler.Requests[0].Body, Does.Contain("\"channel_id\":").And.Contain("\"message_id\":").And.Contain("\"category\":\"spam\""));
        });
    }

    [Test]
    public async Task DeletingAnApplicationSendsTheSudoBodyWithDelete()
    {
        (FluxerClient client, RecordingHandler handler) = Create("flx_test");

        await client.Rest.DeleteApplicationAsync(15, new SudoVerificationRequest { Password = "hunter2" });
        await client.Rest.RevokeOAuthAuthorizationsAsync(new ulong[] { 1, 2 });

        Assert.Multiple(() =>
        {
            Assert.That(handler.Requests[0].Method, Is.EqualTo(HttpMethod.Delete));
            Assert.That(handler.Requests[0].Url, Does.EndWith("/oauth2/applications/15"));
            Assert.That(handler.Requests[0].Body, Does.Contain("\"password\":\"hunter2\""));
            Assert.That(handler.Requests[1].Url, Does.EndWith("/oauth2/@me/authorizations/revoke"));
            Assert.That(handler.Requests[1].Body, Does.Contain("\"application_ids\":["));
        });
    }

    [Test]
    public async Task RelationshipNicknamesAndBulkMessageDeletionUseTheirBodies()
    {
        (FluxerClient client, RecordingHandler handler) = Create("flx_test");

        await client.Rest.UpdateRelationshipNicknameAsync(8, null);
        await client.Rest.BulkDeleteOwnMessagesAsync(new BulkDeleteOwnMessagesRequest
        {
            Password = "pw",
            Scope = "selected",
            IncludeDms = false,
            StartDate = new DateTimeOffset(2025, 1, 2, 3, 4, 5, TimeSpan.Zero)
        });

        Assert.Multiple(() =>
        {
            Assert.That(handler.Requests[0].Method, Is.EqualTo(HttpMethod.Patch));
            Assert.That(handler.Requests[0].Url, Does.EndWith("/users/@me/relationships/8"));
            Assert.That(handler.Requests[0].Body, Does.Contain("\"nickname\":null"));
            Assert.That(handler.Requests[1].Url, Does.EndWith("/users/@me/messages/bulk-delete-mine"));
            Assert.That(handler.Requests[1].Body, Does.Contain("\"password\":\"pw\"").And.Contain("\"scope\":\"selected\"").And.Contain("\"include_dms\":false").And.Contain("\"start_date\":\"2025-01-02T03:04:05.000Z\""));
        });
    }

    [Test]
    public void ANewUserOnlyMethodIsRefusedForABotBeforeAnythingIsSent()
    {
        (FluxerClient client, RecordingHandler handler) = Create("Bot test");

        UserTokenRequiredException? ex = Assert.ThrowsAsync<UserTokenRequiredException>(() => client.Rest.EndCallAsync(3));

        Assert.That(ex!.Route, Is.EqualTo("/channels/3/call/end"));
        Assert.That(handler.Requests, Is.Empty);
    }

    [Test]
    public async Task TrendingGifsWorkWithABotToken()
    {
        (FluxerClient client, RecordingHandler handler) = Create("Bot test", "[]");

        await client.Rest.GetGifsTrendingAsync("en-US");

        Assert.That(handler.Requests.Single().Url, Does.EndWith("/gifs/trending?locale=en-US"));
    }
}
