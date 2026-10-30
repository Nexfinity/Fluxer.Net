using System.Collections.Concurrent;
using System.Diagnostics;
using Fluxer.Net.Gateway;

namespace Fluxer.Net.Test;

public class CacheReliabilityTests
{
    private const string Author = "{\"id\":\"1\",\"username\":\"a\"}";

    private static FluxerClient Create(FluxerCacheConfig config)
    {
        FluxerClient client = new("Bot test");
        AddGuild(client, 10, 3);
        client.Gateway.Cache = new GatewayCache(config);
        return client;
    }

    private static void AddGuild(FluxerClient client, ulong guildId, ulong channelId)
    {
        SocketGuild guild = new(client);
        guild.Update(new GuildJson { Id = guildId, Name = "Test" });
        client.Gateway.Guilds[guildId] = guild;
        client.Gateway.Channels[channelId] = Channel.Create(client, new ChannelJson { Id = channelId, GuildId = guildId, Type = ChannelType.GuildText });
    }

    private static string Dispatch(string name, string data) => $"{{\"op\":0,\"t\":\"{name}\",\"s\":1,\"d\":{data}}}";

    private static string MessageData(ulong id, string content, ulong guildId = 10, ulong channelId = 3)
        => $"{{\"id\":\"{id}\",\"channel_id\":\"{channelId}\",\"guild_id\":\"{guildId}\",\"author\":{Author},\"content\":\"{content}\",\"timestamp\":\"2025-01-01T00:00:00Z\"}}";

    private sealed class ThrowingStore : IEntityStore<ulong, MessageGatewayData>
    {
        public ValueTask<MessageGatewayData?> GetAsync(ulong key, CancellationToken cancellationToken = default) => throw new InvalidOperationException("down");
        public ValueTask SetAsync(ulong key, MessageGatewayData value, CacheScope scope, CancellationToken cancellationToken = default) => throw new InvalidOperationException("down");
        public ValueTask<MessageGatewayData?> RemoveAsync(ulong key, CancellationToken cancellationToken = default) => throw new InvalidOperationException("down");
        public ValueTask RemoveScopeAsync(ulong scopeId, CancellationToken cancellationToken = default) => throw new InvalidOperationException("down");
        public ValueTask ClearAsync(CancellationToken cancellationToken = default) => throw new InvalidOperationException("down");
    }

    private sealed class HangingStore : IEntityStore<ulong, MessageGatewayData>
    {
        private static async ValueTask<T> Hang<T>(T value)
        {
            await Task.Delay(Timeout.Infinite);
            return value;
        }

        public ValueTask<MessageGatewayData?> GetAsync(ulong key, CancellationToken cancellationToken = default) => Hang<MessageGatewayData?>(null);
        public async ValueTask SetAsync(ulong key, MessageGatewayData value, CacheScope scope, CancellationToken cancellationToken = default) => await Hang(0);
        public ValueTask<MessageGatewayData?> RemoveAsync(ulong key, CancellationToken cancellationToken = default) => Hang<MessageGatewayData?>(null);
        public async ValueTask RemoveScopeAsync(ulong scopeId, CancellationToken cancellationToken = default) => await Hang(0);
        public async ValueTask ClearAsync(CancellationToken cancellationToken = default) => await Hang(0);
    }

    private sealed class DelayedStore(int delay) : IEntityStore<ulong, MessageGatewayData>
    {
        private readonly MemoryEntityStore<ulong, MessageGatewayData> _inner = new(new CacheLimits { MaxEntries = 100 });

        public async ValueTask<MessageGatewayData?> GetAsync(ulong key, CancellationToken cancellationToken = default)
        {
            await Task.Delay(delay, cancellationToken);
            return await _inner.GetAsync(key, cancellationToken);
        }

        public async ValueTask SetAsync(ulong key, MessageGatewayData value, CacheScope scope, CancellationToken cancellationToken = default)
        {
            await Task.Delay(delay, cancellationToken);
            await _inner.SetAsync(key, value, scope, cancellationToken);
        }

        public ValueTask<MessageGatewayData?> RemoveAsync(ulong key, CancellationToken cancellationToken = default) => _inner.RemoveAsync(key, cancellationToken);
        public ValueTask RemoveScopeAsync(ulong scopeId, CancellationToken cancellationToken = default) => _inner.RemoveScopeAsync(scopeId, cancellationToken);
        public ValueTask ClearAsync(CancellationToken cancellationToken = default) => _inner.ClearAsync(cancellationToken);
    }

    private sealed class DictionaryStore<TKey, TValue>(string keyNamespace, TimeSpan? expiry = null) : SerializedEntityStore<TKey, TValue>(keyNamespace, expiry)
        where TKey : notnull where TValue : class
    {
        public ConcurrentDictionary<string, string> Strings { get; } = new();
        public ConcurrentDictionary<string, HashSet<string>> Sets { get; } = new();
        public List<TimeSpan?> Expiries { get; } = new();

        protected override ValueTask<string?> ReadAsync(string key, CancellationToken cancellationToken)
            => new(Strings.TryGetValue(key, out string? value) ? value : null);

        protected override ValueTask WriteAsync(string key, string value, TimeSpan? expiry, CancellationToken cancellationToken)
        {
            Strings[key] = value;
            Expiries.Add(expiry);
            return default;
        }

        protected override ValueTask DeleteAsync(IReadOnlyCollection<string> keys, CancellationToken cancellationToken)
        {
            foreach (string key in keys)
            {
                Strings.TryRemove(key, out _);
                Sets.TryRemove(key, out _);
            }

            return default;
        }

        protected override ValueTask AddToSetAsync(string set, string member, CancellationToken cancellationToken)
        {
            lock (Sets)
                Sets.GetOrAdd(set, _ => new HashSet<string>()).Add(member);
            return default;
        }

        protected override ValueTask RemoveFromSetAsync(string set, string member, CancellationToken cancellationToken)
        {
            lock (Sets)
            {
                if (Sets.TryGetValue(set, out HashSet<string>? members))
                    members.Remove(member);
            }

            return default;
        }

        protected override ValueTask<IReadOnlyCollection<string>> ReadSetAsync(string set, CancellationToken cancellationToken)
        {
            lock (Sets)
                return new(Sets.TryGetValue(set, out HashSet<string>? members) ? members.ToList() : new List<string>());
        }
    }

    [Test]
    public void AThrowingStoreNeverStopsEvents()
    {
        FluxerClient client = Create(new FluxerCacheConfig { MessageStore = new ThrowingStore() });
        bool created = false;
        (bool Raised, SocketMessage? Before) edited = default;
        client.Gateway.MessageCreated += _ => created = true;
        client.Gateway.MessageEdited += (before, _) => edited = (true, before);

        client.Gateway.GatewayMessageHandler(Dispatch("MESSAGE_CREATE", MessageData(20, "first")));
        client.Gateway.GatewayMessageHandler(Dispatch("MESSAGE_UPDATE", MessageData(20, "second")));

        Assert.Multiple(() =>
        {
            Assert.That(created, Is.True);
            Assert.That(edited.Raised, Is.True);
            Assert.That(edited.Before, Is.Null);
            Assert.That(client.Gateway.Cache.Statistics.Messages.Failures, Is.EqualTo(2));
        });
    }

    [Test]
    public async Task AHangingStoreTimesOutAndEventsStillArrive()
    {
        FluxerClient client = Create(new FluxerCacheConfig { MessageStore = new HangingStore(), StoreTimeout = TimeSpan.FromMilliseconds(100) });
        TaskCompletionSource<SocketMessage?> edited = new();
        client.Gateway.MessageEdited += (before, _) => edited.TrySetResult(before);

        client.Gateway.GatewayMessageHandler(Dispatch("MESSAGE_CREATE", MessageData(20, "first")));
        client.Gateway.GatewayMessageHandler(Dispatch("MESSAGE_UPDATE", MessageData(20, "second")));

        Assert.That(await Task.WhenAny(edited.Task, Task.Delay(5000)), Is.SameAs(edited.Task));
        Assert.That(edited.Task.Result, Is.Null);
        Assert.That(client.Gateway.Cache.Statistics.Messages.Timeouts, Is.GreaterThanOrEqualTo(2));
    }

    [Test]
    public async Task ASlowStoreNeverHoldsUpReceiving()
    {
        FluxerClient client = Create(new FluxerCacheConfig { MessageStore = new DelayedStore(150) });

        Stopwatch watch = Stopwatch.StartNew();
        for (ulong id = 20; id < 25; id++)
            client.Gateway.GatewayMessageHandler(Dispatch("MESSAGE_CREATE", MessageData(id, "a")));
        watch.Stop();

        Assert.That(watch.ElapsedMilliseconds, Is.LessThan(150));
        Assert.That(client.Gateway.PendingEvents, Is.GreaterThan(0));
        await client.Gateway.WaitForPendingEventsAsync();
        Assert.That(client.Gateway.PendingEvents, Is.Zero);
    }

    [Test]
    public async Task WaitingForPendingEventsDrainsTheQueue()
    {
        FluxerClient client = Create(new FluxerCacheConfig { MessageStore = new DelayedStore(30) });
        int created = 0;
        client.Gateway.MessageCreated += _ => Interlocked.Increment(ref created);

        for (ulong id = 1; id <= 5; id++)
            client.Gateway.GatewayMessageHandler(Dispatch("MESSAGE_CREATE", MessageData(id, "m")));

        await client.Gateway.WaitForPendingEventsAsync();
        Assert.That(created, Is.EqualTo(5));
        Assert.That(client.Gateway.PendingEvents, Is.Zero);
    }

    [Test]
    public void MemoryStaysFlatAfterManyGuildsLeave()
    {
        FluxerClient client = Create(new FluxerCacheConfig { Messages = new CacheLimits { MaxPerChannel = 50 } });
        for (ulong n = 0; n < 200; n++)
        {
            ulong guildId = 1000 + n, channelId = 5000 + n;
            AddGuild(client, guildId, channelId);
            for (ulong m = 0; m < 5; m++)
                client.Gateway.GatewayMessageHandler(Dispatch("MESSAGE_CREATE", MessageData(n * 10 + m + 100000, "m", guildId, channelId)));
        }

        MemoryEntityStore<ulong, MessageGatewayData> store = TestCache.Messages(client);
        Assert.That(store.Count, Is.EqualTo(1000));

        for (ulong n = 0; n < 200; n++)
            client.Gateway.GatewayMessageHandler(Dispatch("GUILD_DELETE", $"{{\"id\":\"{1000 + n}\"}}"));

        Assert.That(store.Count, Is.Zero);
        Assert.That(store.IndexCount, Is.Zero);
    }

    [Test]
    public async Task AnExpiryAloneKeepsEntriesUntilTheyAreSwept()
    {
        DateTimeOffset now = DateTimeOffset.UnixEpoch;
        MemoryEntityStore<ulong, UserJson> store = new(new CacheLimits { Expiry = TimeSpan.FromMinutes(5) }, () => now);
        Assert.That(store.IsEnabled, Is.True);

        for (ulong id = 1; id <= 3; id++)
            await store.SetAsync(id, new UserJson { Id = id }, default);

        now = now.AddMinutes(10);
        store.Sweep();

        Assert.That(store.Count, Is.Zero);
        Assert.That(store.Statistics.Expirations, Is.EqualTo(3));
    }

    [Test]
    public async Task ReplacingAnEntryRenewsItsExpiry()
    {
        DateTimeOffset now = DateTimeOffset.UnixEpoch;
        MemoryEntityStore<ulong, UserJson> store = new(new CacheLimits { Expiry = TimeSpan.FromMinutes(5) }, () => now);
        await store.SetAsync(1, new UserJson { Id = 1 }, default);
        await store.SetAsync(2, new UserJson { Id = 2 }, default);

        now = now.AddMinutes(4);
        await store.SetAsync(1, new UserJson { Id = 1 }, default);
        now = now.AddMinutes(2);
        store.Sweep();

        Assert.That(await store.GetAsync(1), Is.Not.Null);
        Assert.That(await store.GetAsync(2), Is.Null);
    }

    [Test]
    public void LeavingMembersAreForgottenUnlessTheyShareAnotherGuild()
    {
        FluxerClient client = Create(new FluxerCacheConfig { Presences = new CacheLimits { MaxEntries = 100 }, Users = new CacheLimits { MaxEntries = 100 } });
        AddGuild(client, 11, 4);
        string member(ulong guildId, ulong userId) => $"{{\"guild_id\":\"{guildId}\",\"user\":{{\"id\":\"{userId}\",\"username\":\"u{userId}\"}},\"roles\":[],\"joined_at\":\"2025-01-01T00:00:00Z\"}}";
        foreach ((ulong guildId, ulong userId) in new[] { (10UL, 5UL), (10UL, 6UL), (11UL, 6UL) })
            client.Gateway.GatewayMessageHandler(Dispatch("GUILD_MEMBER_ADD", member(guildId, userId)));
        foreach (ulong userId in new ulong[] { 5, 6 })
            client.Gateway.GatewayMessageHandler(Dispatch("PRESENCE_UPDATE", $"{{\"guild_id\":\"10\",\"user\":{{\"id\":\"{userId}\"}},\"status\":\"online\"}}"));

        client.Gateway.GatewayMessageHandler(Dispatch("GUILD_MEMBER_REMOVE", "{\"guild_id\":\"10\",\"user\":{\"id\":\"5\"}}"));
        client.Gateway.GatewayMessageHandler(Dispatch("GUILD_MEMBER_REMOVE", "{\"guild_id\":\"10\",\"user\":{\"id\":\"6\"}}"));

        MemoryEntityStore<ulong, PresenceGatewayData> presences = TestCache.Unwrap(client.Gateway.Cache.Presences);
        MemoryEntityStore<ulong, UserJson> users = TestCache.Unwrap(client.Gateway.Cache.Users);
        Assert.Multiple(async () =>
        {
            Assert.That(await presences.GetAsync(5), Is.Null);
            Assert.That(await users.GetAsync(5), Is.Null);
            Assert.That(await presences.GetAsync(6), Is.Not.Null);
            Assert.That(await users.GetAsync(6), Is.Not.Null);
        });
    }

    [Test]
    public async Task StatisticsCountHitsMissesAndWrites()
    {
        FluxerClient client = Create(new FluxerCacheConfig { Messages = new CacheLimits { MaxEntries = 1 } });

        client.Gateway.GatewayMessageHandler(Dispatch("MESSAGE_CREATE", MessageData(20, "a")));
        client.Gateway.GatewayMessageHandler(Dispatch("MESSAGE_CREATE", MessageData(21, "b")));
        await client.Gateway.GetCachedMessageAsync(21);
        await client.Gateway.GetCachedMessageAsync(20);

        CacheStatistics stats = client.Gateway.Cache.Statistics.Messages;
        Assert.Multiple(() =>
        {
            Assert.That(stats.Writes, Is.EqualTo(2));
            Assert.That(stats.Evictions, Is.EqualTo(1));
            Assert.That(stats.Hits, Is.EqualTo(1));
            Assert.That(stats.Misses, Is.EqualTo(1));
        });
    }

    [Test]
    public void InvalidSettingsAreRejected()
    {
        Assert.Multiple(() =>
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => _ = new MemoryEntityStore<ulong, UserJson>(new CacheLimits { MaxEntries = -1 }));
            Assert.Throws<ArgumentOutOfRangeException>(() => _ = new MemoryEntityStore<ulong, UserJson>(new CacheLimits { Expiry = TimeSpan.Zero }));
            Assert.Throws<ArgumentOutOfRangeException>(() => _ = new GatewayCache(new FluxerCacheConfig { StoreTimeout = TimeSpan.Zero }));
        });
    }

    [Test]
    public async Task ANewSessionClearsOnlyTheBuiltInStores()
    {
        DictionaryStore<ulong, MessageGatewayData> custom = new("bot:messages");
        GatewayCache cache = new(new FluxerCacheConfig { MessageStore = custom, Users = new CacheLimits { MaxEntries = 10 } });
        await cache.Messages.SetAsync(1, new MessageGatewayData { Id = 1, Content = "kept" }, default);
        await cache.Users.SetAsync(1, new UserJson { Id = 1 }, default);

        await cache.ClearBuiltInAsync();

        Assert.That((await cache.Messages.GetAsync(1))?.Content, Is.EqualTo("kept"));
        Assert.That(await cache.Users.GetAsync(1), Is.Null);
    }

    [Test]
    public async Task SerializedStoreRoundTripsLargeIdsAndTracksScopes()
    {
        DictionaryStore<ulong, MessageGatewayData> store = new("bot:messages", TimeSpan.FromHours(1));
        const ulong large = 18446744073709551000UL;

        await store.SetAsync(large, new MessageGatewayData { Id = large, ChannelId = 3, GuildId = 10, Content = "big" }, new CacheScope(10, 3));
        await store.SetAsync(2, new MessageGatewayData { Id = 2, ChannelId = 4, GuildId = 11, Content = "other" }, new CacheScope(11, 4));

        MessageGatewayData? read = await store.GetAsync(large);
        Assert.Multiple(() =>
        {
            Assert.That(read?.Id, Is.EqualTo(large));
            Assert.That(read?.Content, Is.EqualTo("big"));
            Assert.That(store.Strings.Keys, Has.All.StartsWith("bot:messages:"));
            Assert.That(store.Expiries, Has.All.EqualTo(TimeSpan.FromHours(1)));
        });

        await store.RemoveScopeAsync(10);
        Assert.That(await store.GetAsync(large), Is.Null);
        Assert.That((await store.GetAsync(2))?.Content, Is.EqualTo("other"));

        await store.ClearAsync();
        Assert.That(store.Strings, Is.Empty);
        Assert.That(store.Sets.Values.Sum(x => x.Count), Is.Zero);
    }

    [Test]
    public async Task SerializedStoreRemovesEntriesFromTheirScopes()
    {
        DictionaryStore<ulong, MessageGatewayData> store = new("bot:messages");
        await store.SetAsync(1, new MessageGatewayData { Id = 1, Content = "x" }, new CacheScope(10, 3));

        MessageGatewayData? removed = await store.RemoveAsync(1);

        Assert.That(removed?.Content, Is.EqualTo("x"));
        Assert.That(store.Sets.Values.Sum(x => x.Count), Is.EqualTo(2));
        Assert.That(store.Sets["bot:messages:s:10"], Is.Empty);
        Assert.That(store.Sets["bot:messages:s:3"], Is.Empty);
    }

    [Test]
    public void FluxerJsonRoundTripsModels()
    {
        UserJson user = new() { Id = 18446744073709551000UL, Username = "a", DisplayName = "A" };

        UserJson? read = FluxerJson.Deserialize<UserJson>(FluxerJson.Serialize(user));

        Assert.That(read?.Id, Is.EqualTo(user.Id));
        Assert.That(read?.DisplayName, Is.EqualTo("A"));
    }
}
