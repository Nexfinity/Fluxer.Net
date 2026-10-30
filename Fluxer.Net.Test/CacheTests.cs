using Fluxer.Net.Gateway;

namespace Fluxer.Net.Test;

public class CacheTests
{
    private const string Author = "{\"id\":\"1\",\"username\":\"a\"}";

    private static FluxerClient Create(Action<FluxerClient>? cache = null)
    {
        FluxerClient client = new("Bot test");
        SocketGuild guild = new(client);
        guild.Update(new GuildJson { Id = 10, Name = "Test" });
        client.Gateway.Guilds[10] = guild;
        client.Gateway.Channels[3] = Channel.Create(client, new ChannelJson { Id = 3, GuildId = 10, Type = ChannelType.GuildText });
        (cache ?? (c => TestCache.Use(c, messagesPerChannel: 10, users: 100, presences: 100, invites: 100)))(client);
        return client;
    }

    private static string Dispatch(string name, string data) => $"{{\"op\":0,\"t\":\"{name}\",\"s\":1,\"d\":{data}}}";

    private static string MessageData(ulong id, string content)
        => $"{{\"id\":\"{id}\",\"channel_id\":\"3\",\"guild_id\":\"10\",\"author\":{Author},\"content\":\"{content}\",\"timestamp\":\"2025-01-01T00:00:00Z\"}}";

    private sealed class SlowStore<TKey, TValue> : IEntityStore<TKey, TValue> where TKey : notnull where TValue : class
    {
        private readonly MemoryEntityStore<TKey, TValue> _inner = new(new CacheLimits { MaxEntries = 100 });

        public async ValueTask<TValue?> GetAsync(TKey key, CancellationToken cancellationToken = default)
        {
            await Task.Delay(20, cancellationToken);
            return await _inner.GetAsync(key, cancellationToken);
        }

        public async ValueTask SetAsync(TKey key, TValue value, CacheScope scope, CancellationToken cancellationToken = default)
        {
            await Task.Delay(20, cancellationToken);
            await _inner.SetAsync(key, value, scope, cancellationToken);
        }

        public async ValueTask<TValue?> RemoveAsync(TKey key, CancellationToken cancellationToken = default)
        {
            await Task.Delay(20, cancellationToken);
            return await _inner.RemoveAsync(key, cancellationToken);
        }

        public ValueTask RemoveScopeAsync(ulong scopeId, CancellationToken cancellationToken = default) => _inner.RemoveScopeAsync(scopeId, cancellationToken);

        public ValueTask ClearAsync(CancellationToken cancellationToken = default) => _inner.ClearAsync(cancellationToken);
    }

    [Test]
    public async Task AnAsyncStoreKeepsEventsInOrder()
    {
        FluxerClient client = Create(c => c.Gateway.Cache = new GatewayCache(new FluxerCacheConfig { MessageStore = new SlowStore<ulong, MessageGatewayData>() }));
        TaskCompletionSource<(string? Before, string After)> edited = new();
        client.Gateway.MessageEdited += (before, after) => edited.TrySetResult((before?.Content, after.Content));

        client.Gateway.GatewayMessageHandler(Dispatch("MESSAGE_CREATE", MessageData(20, "first")));
        client.Gateway.GatewayMessageHandler(Dispatch("MESSAGE_UPDATE", MessageData(20, "second")));

        Task finished = await Task.WhenAny(edited.Task, Task.Delay(5000));
        Assert.That(finished, Is.SameAs(edited.Task));
        Assert.That(edited.Task.Result, Is.EqualTo(("first", "second")));
    }

    [Test]
    public void APartialUpdateKeepsTheCachedContentAndAuthor()
    {
        FluxerClient client = Create();
        (string? Content, string? Author)? edited = null;
        (string? Content, string? Author)? removed = null;
        client.Gateway.MessageEdited += (_, after) => edited = (after.Content, after.Author?.Username);
        client.Gateway.MessageRemoved += (_, _, message) => removed = (message?.Content, message?.Author?.Username);

        client.Gateway.GatewayMessageHandler(Dispatch("MESSAGE_CREATE", MessageData(20, "with a link")));
        client.Gateway.GatewayMessageHandler(Dispatch("MESSAGE_UPDATE", "{\"id\":\"20\",\"channel_id\":\"3\",\"guild_id\":\"10\",\"embeds\":[{\"title\":\"unfurl\"}]}"));
        client.Gateway.GatewayMessageHandler(Dispatch("MESSAGE_DELETE", "{\"id\":\"20\",\"channel_id\":\"3\",\"guild_id\":\"10\"}"));

        Assert.Multiple(() =>
        {
            Assert.That(edited, Is.EqualTo(("with a link", "a")));
            Assert.That(removed, Is.EqualTo(("with a link", "a")));
        });
    }

    [Test]
    public void UsersFromPayloadsWithoutFlagsOrDisplayNamesAreNotChanges()
    {
        FluxerClient client = Create();
        int changes = 0;
        client.Gateway.UserChanged += (_, _) => changes++;

        client.Gateway.GatewayMessageHandler(Dispatch("MESSAGE_CREATE", "{\"id\":\"20\",\"channel_id\":\"3\",\"guild_id\":\"10\",\"author\":{\"id\":\"5\",\"username\":\"a\",\"global_name\":\"Ann\",\"flags\":64},\"content\":\"x\",\"timestamp\":\"2025-01-01T00:00:00Z\"}"));
        client.Gateway.GatewayMessageHandler(Dispatch("GUILD_MEMBER_UPDATE", "{\"guild_id\":\"10\",\"user\":{\"id\":\"5\",\"username\":\"a\"},\"roles\":[],\"joined_at\":\"2025-01-01T00:00:00Z\"}"));
        client.Gateway.GatewayMessageHandler(Dispatch("MESSAGE_CREATE", "{\"id\":\"21\",\"channel_id\":\"3\",\"guild_id\":\"10\",\"author\":{\"id\":\"5\",\"username\":\"a\",\"global_name\":\"Ann\",\"flags\":64},\"content\":\"y\",\"timestamp\":\"2025-01-01T00:00:00Z\"}"));
        Assert.That(changes, Is.Zero);

        client.Gateway.GatewayMessageHandler(Dispatch("MESSAGE_CREATE", "{\"id\":\"22\",\"channel_id\":\"3\",\"guild_id\":\"10\",\"author\":{\"id\":\"5\",\"username\":\"a\",\"global_name\":\"Anne\",\"flags\":64},\"content\":\"z\",\"timestamp\":\"2025-01-01T00:00:00Z\"}"));
        Assert.That(changes, Is.EqualTo(1));
    }

    [Test]
    public void PresenceChangeComesWithThePreviousPresence()
    {
        FluxerClient client = Create();
        List<(string? Before, string After)> changes = new();
        client.Gateway.PresenceChanged += (before, after) => changes.Add((before?.Status, after.Status));

        client.Gateway.GatewayMessageHandler(Dispatch("PRESENCE_UPDATE", "{\"guild_id\":\"10\",\"user\":{\"id\":\"5\"},\"status\":\"online\"}"));
        client.Gateway.GatewayMessageHandler(Dispatch("PRESENCE_UPDATE", "{\"guild_id\":\"10\",\"user\":{\"id\":\"5\"},\"status\":\"idle\"}"));

        Assert.That(changes, Is.EqualTo(new List<(string?, string)> { (null, "online"), ("online", "idle") }));
    }

    [Test]
    public void UserChangeIsRaisedWhenANameChanges()
    {
        FluxerClient client = Create();
        (string? Before, string After)? change = null;
        client.Gateway.UserChanged += (before, after) => change = (before.Username, after.Username);
        string member(string name) => $"{{\"guild_id\":\"10\",\"user\":{{\"id\":\"5\",\"username\":\"{name}\"}},\"roles\":[],\"joined_at\":\"2025-01-01T00:00:00Z\"}}";

        client.Gateway.GatewayMessageHandler(Dispatch("GUILD_MEMBER_ADD", member("old")));
        client.Gateway.GatewayMessageHandler(Dispatch("GUILD_MEMBER_UPDATE", member("old")));
        Assert.That(change, Is.Null);

        client.Gateway.GatewayMessageHandler(Dispatch("GUILD_MEMBER_UPDATE", member("new")));
        Assert.That(change, Is.EqualTo(("old", "new")));
    }

    [Test]
    public void ReactionsCarryTheCachedMessage()
    {
        FluxerClient client = Create();
        Message? message = null;
        client.Gateway.MessageReactionAdded += (_, cached, _) => message = cached.Value;

        client.Gateway.GatewayMessageHandler(Dispatch("MESSAGE_CREATE", MessageData(20, "react to me")));
        client.Gateway.GatewayMessageHandler(Dispatch("MESSAGE_REACTION_ADD",
            "{\"channel_id\":\"3\",\"message_id\":\"20\",\"guild_id\":\"10\",\"user_id\":\"5\",\"emoji\":{\"name\":\"👍\"}," +
            "\"member\":{\"guild_id\":\"10\",\"user\":{\"id\":\"5\",\"username\":\"u\"},\"roles\":[],\"joined_at\":\"2025-01-01T00:00:00Z\"}}"));

        Assert.That(message?.Content, Is.EqualTo("react to me"));
    }

    [Test]
    public void PinChangeComesWithThePreviousPinTime()
    {
        FluxerClient client = Create();
        DateTimeOffset? before = DateTimeOffset.MinValue;
        client.Gateway.ChannelPinsChanged += (_, _, previous) => before = previous;

        client.Gateway.GatewayMessageHandler(Dispatch("CHANNEL_PINS_UPDATE", "{\"channel_id\":\"3\",\"guild_id\":\"10\",\"last_pin_timestamp\":\"2025-01-01T00:00:00Z\"}"));
        Assert.That(before, Is.Null);

        client.Gateway.GatewayMessageHandler(Dispatch("CHANNEL_PINS_UPDATE", "{\"channel_id\":\"3\",\"guild_id\":\"10\",\"last_pin_timestamp\":\"2025-02-01T00:00:00Z\"}"));
        Assert.That(before, Is.EqualTo(new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero)));
    }

    [Test]
    public void ThreadUpdateComesWithThePreviousThread()
    {
        FluxerClient client = Create();
        string? before = null;
        string? after = null;
        client.Gateway.ThreadChanged += (_, previous, current) => (before, after) = (previous?.Name, current.Name);

        client.Gateway.GatewayMessageHandler(Dispatch("THREAD_CREATE", "{\"id\":\"30\",\"guild_id\":\"10\",\"parent_id\":\"3\",\"type\":11,\"name\":\"old\"}"));
        client.Gateway.GatewayMessageHandler(Dispatch("THREAD_UPDATE", "{\"id\":\"30\",\"guild_id\":\"10\",\"parent_id\":\"3\",\"type\":11,\"name\":\"new\"}"));

        Assert.That(before, Is.EqualTo("old"));
        Assert.That(after, Is.EqualTo("new"));
    }

    [Test]
    public async Task MemoryStoreDropsTheOldestOverItsTotalLimit()
    {
        MemoryEntityStore<ulong, UserJson> store = new(new CacheLimits { MaxEntries = 2 });

        foreach (ulong id in new ulong[] { 1, 2, 3 })
            await store.SetAsync(id, new UserJson { Id = id }, default);

        Assert.Multiple(async () =>
        {
            Assert.That(await store.GetAsync(1), Is.Null);
            Assert.That((await store.GetAsync(3))?.Id, Is.EqualTo(3UL));
            Assert.That(store.Count, Is.EqualTo(2));
        });
    }

    [Test]
    public async Task MemoryStoreLetsEntriesExpire()
    {
        DateTimeOffset now = DateTimeOffset.UnixEpoch;
        MemoryEntityStore<ulong, UserJson> store = new(new CacheLimits { MaxEntries = 10, Expiry = TimeSpan.FromMinutes(5) }, () => now);

        await store.SetAsync(1, new UserJson { Id = 1 }, default);
        now = now.AddMinutes(4);
        Assert.That(await store.GetAsync(1), Is.Not.Null);

        now = now.AddMinutes(2);
        Assert.That(await store.GetAsync(1), Is.Null);
        Assert.That(store.Count, Is.Zero);
    }

    [Test]
    public async Task MemoryStoreForgetsAGuildOrChannel()
    {
        MemoryEntityStore<ulong, UserJson> store = new(new CacheLimits { MaxEntries = 10 });
        await store.SetAsync(1, new UserJson { Id = 1 }, new CacheScope(10, 3));
        await store.SetAsync(2, new UserJson { Id = 2 }, new CacheScope(11, 4));

        await store.RemoveScopeAsync(10);

        Assert.That(await store.GetAsync(1), Is.Null);
        Assert.That(await store.GetAsync(2), Is.Not.Null);
    }

    [Test]
    public async Task ACacheWithNoLimitsKeepsNothing()
    {
        MemoryEntityStore<ulong, UserJson> store = new(new CacheLimits());

        await store.SetAsync(1, new UserJson { Id = 1 }, default);

        Assert.That(store.IsEnabled, Is.False);
        Assert.That(await store.GetAsync(1), Is.Null);
    }
}
