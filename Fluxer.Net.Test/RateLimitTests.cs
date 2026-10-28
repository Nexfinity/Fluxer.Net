using System.Diagnostics;
using System.Globalization;
using System.Net;
using Fluxer.Net.RateLimiting;
using Fluxer.Net.Rest;
using Serilog;

namespace Fluxer.Net.Test;

public class RateLimitTests
{
    private static readonly ILogger Logger = Serilog.Core.Logger.None;

    [Test]
    public void BucketsKeepMajorIdsAndTemplateTheRest()
    {
        BucketId message = BucketId.Create("get", "/channels/111/messages/222?limit=5");
        BucketId reaction = BucketId.Create("PUT", "/channels/111/messages/222/reactions/%F0%9F%91%8D/@me");
        BucketId webhook = BucketId.Create("POST", "/webhooks/333/sometoken");
        BucketId member = BucketId.Create("PATCH", "/guilds/444/members/555");

        Assert.Multiple(() =>
        {
            Assert.That(message.RouteKey, Is.EqualTo("GET /channels/{channels}/messages/{id}"));
            Assert.That(message.MajorParameters, Is.EqualTo("111"));
            Assert.That(reaction.RouteKey, Is.EqualTo("PUT /channels/{channels}/messages/{id}/reactions/{emoji}/@me"));
            Assert.That(webhook.RouteKey, Is.EqualTo("POST /webhooks/{webhooks}/{token}"));
            Assert.That(webhook.MajorParameters, Is.EqualTo("333/sometoken"));
            Assert.That(webhook.IsGlobalExempt, Is.True);
            Assert.That(member.IsGlobalExempt, Is.False);
            Assert.That(member.RouteKey, Is.EqualTo("PATCH /guilds/{guilds}/members/{id}"));
            Assert.That(BucketId.Create("GET", "/channels/1/messages/2"), Is.EqualTo(BucketId.Create("GET", "/channels/1/messages/3")));
            Assert.That(BucketId.Create("GET", "/channels/1/messages"), Is.Not.EqualTo(BucketId.Create("GET", "/channels/2/messages")));
        });
    }

    [Test]
    public async Task A429IsWaitedOutAndSentAgain()
    {
        FakeHandler handler = new(
            _ => Limited(retryAfter: 0.15),
            _ => Ok(limit: 5, remaining: 4, resetAfter: 1));
        RequestQueue queue = new(new FluxerConfig(), Logger);

        Stopwatch watch = Stopwatch.StartNew();
        using HttpResponseMessage response = await queue.SendAsync(handler.Client, Get("/channels/1/messages"), "/channels/1/messages");

        Assert.Multiple(() =>
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(handler.Calls, Is.EqualTo(2));
            Assert.That(watch.ElapsedMilliseconds, Is.GreaterThanOrEqualTo(140));
        });
    }

    [Test]
    public void A429ThrowsWhenTheRetryModeDoesNotRetryRateLimits()
    {
        FakeHandler handler = new(_ => Limited(retryAfter: 0.5));
        RequestQueue queue = new(new FluxerConfig(), Logger);
        RequestOptions options = new() { RetryMode = RetryMode.AlwaysFail };

        Assert.ThrowsAsync<RateLimitedException>(() =>
            queue.SendAsync(handler.Client, Get("/channels/1/messages"), "/channels/1/messages", options));
        Assert.That(handler.Calls, Is.EqualTo(1));
    }

    [Test]
    public void A429ThrowsWhenTheWaitWouldPassTheTimeout()
    {
        FakeHandler handler = new(_ => Limited(retryAfter: 5));
        RequestQueue queue = new(new FluxerConfig(), Logger);
        RequestOptions options = new() { Timeout = 500 };

        Assert.ThrowsAsync<RateLimitedException>(() =>
            queue.SendAsync(handler.Client, Get("/channels/1/messages"), "/channels/1/messages", options));
    }

    [Test]
    public async Task AnEmptyBucketWaitsForOneSlotToDrain()
    {
        FakeHandler handler = new(
            _ => Ok(limit: 2, remaining: 1, resetAfter: 0.4),
            _ => Ok(limit: 2, remaining: 0, resetAfter: 0.4),
            _ => Ok(limit: 2, remaining: 0, resetAfter: 0.4));
        RequestQueue queue = new(new FluxerConfig(), Logger);

        (await queue.SendAsync(handler.Client, Get("/guilds/9/roles"), "/guilds/9/roles")).Dispose();
        (await queue.SendAsync(handler.Client, Get("/guilds/9/roles"), "/guilds/9/roles")).Dispose();
        Stopwatch watch = Stopwatch.StartNew();
        (await queue.SendAsync(handler.Client, Get("/guilds/9/roles"), "/guilds/9/roles")).Dispose();

        Assert.Multiple(() =>
        {
            Assert.That(handler.Calls, Is.EqualTo(3));
            Assert.That(watch.ElapsedMilliseconds, Is.InRange(150, 1500));
        });
    }

    [Test]
    public async Task OnlyOneRequestGoesOutBeforeTheLimitIsKnown()
    {
        int inFlight = 0;
        int mostInFlight = 0;
        FakeHandler handler = new(async _ =>
        {
            int now = Interlocked.Increment(ref inFlight);
            mostInFlight = Math.Max(mostInFlight, now);
            await Task.Delay(50);
            Interlocked.Decrement(ref inFlight);
            return Ok(limit: 10, remaining: 9, resetAfter: 1);
        });
        RequestQueue queue = new(new FluxerConfig(), Logger);

        Task first = queue.SendAsync(handler.Client, Get("/channels/7/messages"), "/channels/7/messages");
        await Task.Delay(10);
        Task[] rest = Enumerable.Range(0, 4)
            .Select(_ => queue.SendAsync(handler.Client, Get("/channels/7/messages"), "/channels/7/messages"))
            .ToArray();
        await first;
        Assert.That(mostInFlight, Is.EqualTo(1));
        await Task.WhenAll(rest);
        Assert.That(handler.Calls, Is.EqualTo(5));
    }

    [Test]
    public async Task AGlobal429HoldsEveryRoute()
    {
        FakeHandler handler = new(
            _ => Limited(retryAfter: 0.3, global: true),
            _ => Ok(limit: 5, remaining: 4, resetAfter: 1),
            _ => Ok(limit: 5, remaining: 4, resetAfter: 1));
        RequestQueue queue = new(new FluxerConfig(), Logger);

        Stopwatch watch = Stopwatch.StartNew();
        Task<HttpResponseMessage> limited = queue.SendAsync(handler.Client, Get("/channels/1/messages"), "/channels/1/messages");
        await Task.Delay(50);
        using HttpResponseMessage other = await queue.SendAsync(handler.Client, Get("/guilds/2/roles"), "/guilds/2/roles");
        long otherDone = watch.ElapsedMilliseconds;
        (await limited).Dispose();

        Assert.That(otherDone, Is.GreaterThanOrEqualTo(200));
    }

    [Test]
    public async Task TheCallbackSeesEveryResponse()
    {
        FakeHandler handler = new(_ => Ok(limit: 5, remaining: 3, resetAfter: 1));
        RequestQueue queue = new(new FluxerConfig(), Logger);
        IRateLimitInfo? seen = null;
        RequestOptions options = new() { RatelimitCallback = info => { seen = info; return Task.CompletedTask; } };

        (await queue.SendAsync(handler.Client, Get("/channels/1/messages"), "/channels/1/messages", options)).Dispose();

        Assert.Multiple(() =>
        {
            Assert.That(seen, Is.Not.Null);
            Assert.That(seen!.Limit, Is.EqualTo(5));
            Assert.That(seen.Remaining, Is.EqualTo(3));
            Assert.That(seen.Bucket, Is.EqualTo("abc"));
        });
    }

    [Test]
    public void RollingWindowsFreeUpAsUsesExpire()
    {
        RollingWindow window = new(2, TimeSpan.FromMilliseconds(200));
        window.Record();
        window.Record();
        TimeSpan full = window.TimeUntilFree();
        Thread.Sleep(250);

        Assert.Multiple(() =>
        {
            Assert.That(full, Is.GreaterThan(TimeSpan.Zero));
            Assert.That(window.TimeUntilFree(), Is.EqualTo(TimeSpan.Zero));
        });
    }

    private static HttpRequestMessage Get(string route)
    {
        return new HttpRequestMessage(HttpMethod.Get, "https://api.test" + route);
    }

    private static HttpResponseMessage Ok(int limit, int remaining, double resetAfter)
    {
        HttpResponseMessage response = new(HttpStatusCode.OK) { Content = new StringContent("{}") };
        response.Headers.Add("X-RateLimit-Bucket", "abc");
        response.Headers.Add("X-RateLimit-Limit", limit.ToString(CultureInfo.InvariantCulture));
        response.Headers.Add("X-RateLimit-Remaining", remaining.ToString(CultureInfo.InvariantCulture));
        response.Headers.Add("X-RateLimit-Reset-After", resetAfter.ToString(CultureInfo.InvariantCulture));
        return response;
    }

    private static HttpResponseMessage Limited(double retryAfter, bool global = false)
    {
        string body = $"{{\"code\":\"RATE_LIMITED\",\"message\":\"You're being rate limited.\",\"global\":{(global ? "true" : "false")},\"retry_after\":{retryAfter.ToString(CultureInfo.InvariantCulture)}}}";
        HttpResponseMessage response = new((HttpStatusCode)429) { Content = new StringContent(body) };
        response.Headers.Add("Retry-After", Math.Ceiling(retryAfter).ToString(CultureInfo.InvariantCulture));
        if (global)
        {
            response.Headers.Add("X-RateLimit-Global", "true");
            response.Headers.Add("X-RateLimit-Scope", "global");
        }
        else
        {
            response.Headers.Add("X-RateLimit-Scope", "user");
            response.Headers.Add("X-RateLimit-Bucket", "abc");
            response.Headers.Add("X-RateLimit-Limit", "5");
            response.Headers.Add("X-RateLimit-Remaining", "0");
            response.Headers.Add("X-RateLimit-Reset-After", retryAfter.ToString(CultureInfo.InvariantCulture));
        }

        return response;
    }

    private sealed class FakeHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, Task<HttpResponseMessage>>[] _responses;
        private int _calls;

        public FakeHandler(params Func<HttpRequestMessage, HttpResponseMessage>[] responses)
        {
            _responses = responses.Select(x => (Func<HttpRequestMessage, Task<HttpResponseMessage>>)(r => Task.FromResult(x(r)))).ToArray();
            Client = new HttpClient(this);
        }

        public FakeHandler(Func<HttpRequestMessage, Task<HttpResponseMessage>> response)
        {
            _responses = [response];
            Client = new HttpClient(this);
        }

        public HttpClient Client { get; }

        public int Calls => _calls;

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            int call = Interlocked.Increment(ref _calls) - 1;
            return _responses[Math.Min(call, _responses.Length - 1)](request);
        }
    }
}
