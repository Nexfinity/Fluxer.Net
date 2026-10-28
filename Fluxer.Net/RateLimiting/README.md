# Rate limiting

Fluxer.Net follows Fluxer's rate limits for you, on REST and on the gateway. The design and the option names follow
Discord.Net, so `RetryMode`, `RequestOptions.RetryMode`, `RequestOptions.Timeout`, `RequestOptions.RatelimitCallback`,
`IRateLimitInfo` and `RateLimitedException` work the way Discord.Net users expect. The rules underneath are Fluxer's,
from the Rate limits page (topics/rate-limits) and the gateway Limits and rate limits page
(gateway/limits-and-rate-limits) of the Fluxer docs.

## REST

Every request passes through `RequestQueue`:

1. **Global limit.** Fluxer allows 50 requests per second per bot, counted over one second. Requests take a slot from
   a local global bucket of that size (`FluxerConfig.GlobalRateLimit`) before they are sent. Executing a webhook and
   reading, editing or deleting its messages through the webhook token are exempt, as Fluxer exempts them.
2. **Route bucket.** A request belongs to a bucket keyed by its method, its route with ids replaced by placeholders, and
   the channel, guild or webhook it acts on (`BucketId`). Fluxer names each route's bucket in `X-RateLimit-Bucket`; once
   a route's name is known, routes that share it share one bucket, like Discord.Net's hash buckets.
3. **Learning the limit.** Until a bucket has seen a response, only one request is let through. The response's
   `X-RateLimit-Limit`, `X-RateLimit-Remaining` and `X-RateLimit-Reset-After` headers then set the bucket.
4. **Leaky buckets.** Fluxer's buckets refill continuously rather than resetting all at once. `X-RateLimit-Reset-After`
   is the time until the bucket is fully drained, so one slot frees every `Reset-After / (Limit - Remaining)`. A used up
   bucket waits for one slot, not for the whole reset.
5. **429.** A 429 waits the `retry_after` from the response body, which is the delay until one more request is admitted
   and is more precise than the whole-second `Retry-After` header, then sends the request again. A global 429
   (`X-RateLimit-Global: true`) holds every request that counts against the global limit.

### Options

| Setting | Default | Effect |
| --- | --- | --- |
| `FluxerConfig.EnableRateLimiting` | `true` | Turns the REST queue off entirely when false |
| `FluxerConfig.DefaultRetryMode` / `RequestOptions.RetryMode` | `AlwaysRetry` | `RetryRatelimit` waits for buckets and retries 429s; without it a request that would wait throws `RateLimitedException`. `Retry502` and `RetryTimeouts` retry those failures up to three times, for GET, HEAD, PUT, DELETE and OPTIONS only: POST and PATCH are not resent after a timeout or 502 |
| `FluxerConfig.DefaultRequestTimeout` / `RequestOptions.Timeout` | none | Milliseconds a request may spend waiting for rate limits before `RateLimitedException` |
| `FluxerConfig.DefaultRatelimitCallback` / `RequestOptions.RatelimitCallback` | none | Called with the `IRateLimitInfo` of every response |
| `RequestOptions.CancelToken` | none | Cancels the request and any wait |
| `FluxerConfig.GlobalRateLimit` | `50` | Requests per second before the global limit; 1200 for accounts with the high global rate limit flag, 0 to rely on 429s alone |

Limits Fluxer enforces inside a handler, such as username changes, answer 400 with a validation error rather than 429,
so they come back as ordinary errors.

## Gateway

`GatewaySendQueue` holds gateway commands inside Fluxer's budgets instead of letting Fluxer close the connection or drop
them:

- 600 client payloads per connection in a rolling minute, of which 10 are kept free for heartbeats, Identify and
  Resume. Those three skip the queue.
- Five presence updates in a rolling 20 seconds.
- Twelve Request Guild Members commands in a rolling 10 seconds.

The per-connection budgets start again on each new connection, and queued commands wait until Identify or Resume has
been sent on it.
