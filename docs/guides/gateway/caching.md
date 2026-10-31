---
uid: Guides.Gateway.Caching
title: Caching
---

# Caching

The gateway always keeps guilds, channels, roles, members and voice states in memory. On top of that it can remember
messages, users, presences and invites, and hand over their earlier state in update and delete events. That part is off
by default and you choose how much to keep and where.

## Events with the earlier state

| Event | Earlier state | Needs |
| --- | --- | --- |
| `MessageEdited`, `MessageRemoved`, `MessagesRemoved` | The message before the edit or delete | `Cache.Messages` |
| Reaction events | The message in `Cacheable<Message>.Value` | `Cache.Messages` |
| `UserChanged` | The user before a name, avatar or flag change | `Cache.Users` |
| `PresenceChanged` | The previous presence | `Cache.Presences` |
| `InviteRemoved` | The deleted invite | `Cache.Invites` |
| `MemberChanged`, `VoiceStateChanged`, `ThreadChanged`, `ChannelPinsChanged` | The member, voice state, thread or pin time before | Nothing, always on |

Guild, channel, role, emoji, sticker and current user updates pass the earlier state as well.

## Keeping it in memory

```csharp
await client.Gateway.ConnectAsync(new FluxerGatewayConfig
{
    Cache = new FluxerCacheConfig
    {
        Messages = new CacheLimits { MaxPerChannel = 50, Expiry = TimeSpan.FromHours(6) },
        Users = new CacheLimits { MaxEntries = 50_000 },
        Presences = new CacheLimits { MaxEntries = 50_000 },
        Invites = new CacheLimits { MaxEntries = 5_000 }
    }
});
```

Each kind takes any mix of a total limit, a per-channel limit and an expiry. The oldest entries go first, and expired
entries are swept in the background. A kind with nothing set keeps nothing.

The gateway drops what can no longer change: messages and invites of deleted channels, threads and guilds you leave,
and users and presences of members who left every guild you share with them. A new session (not a resume) clears the
in-memory stores.

## Keeping it somewhere else

Set `MessageStore`, `UserStore`, `PresenceStore` or `InviteStore` to a store of your own, for example to share the
cache between shards or keep it across restarts. Stores hold the raw JSON models.

For a key-value database, derive from `SerializedEntityStore<TKey, TValue>`. It serializes entries, prefixes every key
with a namespace and tracks the keys of each guild and channel. You provide six string operations. With
StackExchange.Redis:

```csharp
public sealed class RedisStore<TKey, TValue>(IDatabase db, string keyNamespace, TimeSpan? expiry = null)
    : SerializedEntityStore<TKey, TValue>(keyNamespace, expiry) where TKey : notnull where TValue : class
{
    protected override async ValueTask<string?> ReadAsync(string key, CancellationToken ct)
        => await db.StringGetAsync(key);

    protected override async ValueTask WriteAsync(string key, string value, TimeSpan? ttl, CancellationToken ct)
        => await db.StringSetAsync(key, value, ttl);

    protected override async ValueTask DeleteAsync(IReadOnlyCollection<string> keys, CancellationToken ct)
        => await db.KeyDeleteAsync(keys.Select(k => (RedisKey)k).ToArray());

    protected override async ValueTask AddToSetAsync(string set, string member, CancellationToken ct)
        => await db.SetAddAsync(set, member);

    protected override async ValueTask RemoveFromSetAsync(string set, string member, CancellationToken ct)
        => await db.SetRemoveAsync(set, member);

    protected override async ValueTask<IReadOnlyCollection<string>> ReadSetAsync(string set, CancellationToken ct)
        => (await db.SetMembersAsync(set)).Select(v => v.ToString()).ToList();
}

Cache = new FluxerCacheConfig
{
    MessageStore = new RedisStore<ulong, MessageGatewayData>(db, "mybot:messages", TimeSpan.FromHours(6)),
    UserStore = new RedisStore<ulong, UserJson>(db, "mybot:users")
}
```

Things to know when you write a store:

- **Namespaces.** Give each store its own namespace. Bots or shards that should not share entries need different ones.
- **Threads.** The gateway calls a store one event at a time, but your own code can read the cache at any moment.
  Stores must be thread safe.
- **Failures.** Every call has a timeout, `StoreTimeout`, 2 seconds by default. An exception or a timeout counts as a
  miss and is logged. Events always arrive, with no earlier state for that call. Honour the cancellation token to stop
  work the gateway gave up on.
- **Bulk calls.** `GetManyAsync` and `RemoveManyAsync` loop over single calls by default. Override them when your
  backend can do them in one round trip.
- **Expiry.** With an expiry, index sets can keep keys that already expired. They read as misses and go away when
  their scope or the store is cleared.
- **Clearing.** The gateway never clears a store of your own. Call `ClearAsync` yourself when you want a fresh start.
- **Disposal.** Disposing the client disposes every store.

`FluxerJson.Serialize` and `FluxerJson.Deserialize` read and write models the same way the library does, for stores
that don't use `SerializedEntityStore`.

## Slow stores and shutdown

Events are applied to the cache in order. While a store is slow, received events wait in line, and `PendingEvents`
shows the current length. Receiving, heartbeats and reconnects carry on, and a warning is logged once the line is
longer than `Events.MaxQueuedEvents`.

Before shutting down, wait for the line to empty and handlers to finish:

```csharp
await client.Gateway.WaitForPendingEventsAsync();
client.Dispose();
```

## Tuning

`client.Gateway.Cache.Statistics` has counters for each store: hits, misses, writes, evictions, expirations, failures
and timeouts. Many evictions with few hits mean a limit is too small for how far back you look. Failures and timeouts
point at the backend.
