using System.Reflection;
using Fluxer.Net.Rest;

namespace Fluxer.Net.Test;

public class UserOnlyRouteTests
{
    private static (FluxerClient Client, RecordingHandler Handler) Create(string token, bool enforce = true)
    {
        RecordingHandler handler = new("{}");
        FluxerConfig config = RecordingHandler.Config(new HttpClient(handler));
        config.EnforceUserOnlyRoutes = enforce;
        return (new FluxerClient(token, config), handler);
    }

    [Test]
    public void RoutesAreMatchedBySegmentWithParametersAndQueryStrings()
    {
        Assert.Multiple(() =>
        {
            Assert.That(UserOnlyRoutes.IsUserOnly(HttpMethod.Get, "/users/@me/relationships"), Is.True);
            Assert.That(UserOnlyRoutes.IsUserOnly(HttpMethod.Delete, "/users/@me/relationships/12345"), Is.True);
            Assert.That(UserOnlyRoutes.IsUserOnly(HttpMethod.Get, "/klipy/search?q=cat"), Is.True);
            Assert.That(UserOnlyRoutes.IsUserOnly(HttpMethod.Get, "/gifs/search?q=cat"), Is.True);
            Assert.That(UserOnlyRoutes.IsUserOnly(HttpMethod.Get, "/klipy/trending-gifs"), Is.False);
            Assert.That(UserOnlyRoutes.IsUserOnly(HttpMethod.Get, "/oauth2/applications/@me"), Is.False);
            Assert.That(UserOnlyRoutes.IsUserOnly(HttpMethod.Get, "/oauth2/applications/123"), Is.True);
            Assert.That(UserOnlyRoutes.IsUserOnly(HttpMethod.Get, "/users/@me"), Is.False);
            Assert.That(UserOnlyRoutes.IsUserOnly(HttpMethod.Post, "/channels/3/messages"), Is.False);
            Assert.That(UserOnlyRoutes.IsUserOnly(HttpMethod.Get, "/users/@me/relationships/1/extra"), Is.False);
        });
    }

    [Test]
    public void ABotTokenIsRefusedBeforeAnythingIsSent()
    {
        (FluxerClient client, RecordingHandler handler) = Create("Bot test");

        UserTokenRequiredException? ex = Assert.ThrowsAsync<UserTokenRequiredException>(() => client.Rest.GetCurrentUserRelationshipsAsync<object>());

        Assert.That(ex!.Route, Is.EqualTo("/users/@me/relationships"));
        Assert.That(ex.Method, Is.EqualTo(HttpMethod.Get));
        Assert.That(handler.Requests, Is.Empty);
    }

    [Test]
    public async Task AUserTokenGoesThrough()
    {
        (FluxerClient client, RecordingHandler handler) = Create("flx_test");

        await client.Rest.GetCurrentUserRelationshipsAsync<object>();

        Assert.That(handler.Requests.Single().Url, Does.EndWith("/users/@me/relationships"));
        Assert.That(client.Rest.IsBot, Is.False);
    }

    [Test]
    public async Task TheCheckCanBeTurnedOff()
    {
        (FluxerClient client, RecordingHandler handler) = Create("Bot test", enforce: false);

        await client.Rest.GetCurrentUserRelationshipsAsync<object>();

        Assert.That(handler.Requests, Has.Count.EqualTo(1));
    }

    [Test]
    public async Task BotRoutesAreUntouched()
    {
        (FluxerClient client, RecordingHandler handler) = Create("Bot test");

        await client.Rest.GetCurrentUserAsync();

        Assert.That(handler.Requests, Has.Count.EqualTo(1));
        Assert.That(client.Rest.IsBot, Is.True);
    }

    [Test]
    public void EveryMarkedMethodHitsAUserOnlyRoute()
    {
        IEnumerable<MethodInfo> marked = typeof(FluxerApiClient).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => m.GetCustomAttribute<UserTokenOnlyAttribute>() != null);

        Assert.That(marked.Count(), Is.GreaterThan(50));
    }
}
