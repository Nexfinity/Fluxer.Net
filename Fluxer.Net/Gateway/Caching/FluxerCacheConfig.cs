using Serilog;

namespace Fluxer.Net.Gateway;

/// <summary>
/// Limits for the built-in in-memory store. Any mix can be set; with none set nothing is kept.
/// </summary>
public class CacheLimits
{
    /// <summary>
    /// The most entries to keep in total. 0 means no total limit.
    /// </summary>
    public int MaxEntries { get; set; }

    /// <summary>
    /// The most entries to keep per channel. 0 means no per-channel limit.
    /// </summary>
    public int MaxPerChannel { get; set; }

    /// <summary>
    /// How long an entry is kept, or null to keep it until it is pushed out. Expired entries are swept in the background.
    /// </summary>
    public TimeSpan? Expiry { get; set; }

    internal void Validate()
    {
        if (MaxEntries < 0)
            throw new ArgumentOutOfRangeException(nameof(MaxEntries), "MaxEntries can't be negative.");
        if (MaxPerChannel < 0)
            throw new ArgumentOutOfRangeException(nameof(MaxPerChannel), "MaxPerChannel can't be negative.");
        if (Expiry is TimeSpan expiry && expiry <= TimeSpan.Zero)
            throw new ArgumentOutOfRangeException(nameof(Expiry), "Expiry must be longer than zero.");
    }
}

/// <summary>
/// What the gateway remembers to hand over the state from before update and delete events. Each kind uses the built-in
/// in-memory store with its limits, or a store of your own when one is set. Everything is off by default.
/// </summary>
/// <remarks>
/// Guilds, channels, roles, members and voice states are always kept in memory by the gateway and aren't configured here.
/// </remarks>
public class FluxerCacheConfig
{
    /// <summary>
    /// Limits for guild messages, used by <see cref="FluxerGatewayClient.MessageEdited"/>, <see cref="FluxerGatewayClient.MessageRemoved"/>,
    /// <see cref="FluxerGatewayClient.MessagesRemoved"/> and the reaction events.
    /// </summary>
    public CacheLimits Messages { get; set; } = new CacheLimits();

    /// <summary>
    /// Limits for users, used by <see cref="FluxerGatewayClient.UserChanged"/>.
    /// </summary>
    public CacheLimits Users { get; set; } = new CacheLimits();

    /// <summary>
    /// Limits for presences, used by <see cref="FluxerGatewayClient.PresenceChanged"/>.
    /// </summary>
    public CacheLimits Presences { get; set; } = new CacheLimits();

    /// <summary>
    /// Limits for invites, used by <see cref="FluxerGatewayClient.InviteRemoved"/>.
    /// </summary>
    public CacheLimits Invites { get; set; } = new CacheLimits();

    /// <summary>
    /// A store of your own for guild messages. When set, <see cref="Messages"/> is ignored.
    /// </summary>
    public IEntityStore<ulong, MessageGatewayData>? MessageStore { get; set; }

    /// <summary>
    /// A store of your own for users. When set, <see cref="Users"/> is ignored.
    /// </summary>
    public IEntityStore<ulong, UserJson>? UserStore { get; set; }

    /// <summary>
    /// A store of your own for presences, keyed by user ID. When set, <see cref="Presences"/> is ignored.
    /// </summary>
    public IEntityStore<ulong, PresenceGatewayData>? PresenceStore { get; set; }

    /// <summary>
    /// A store of your own for invites, keyed by code. When set, <see cref="Invites"/> is ignored.
    /// </summary>
    public IEntityStore<string, InviteJson>? InviteStore { get; set; }

    /// <summary>
    /// How long a store call may take before the gateway gives up on it and treats it as a miss. Defaults to 2 seconds.
    /// </summary>
    public TimeSpan StoreTimeout { get; set; } = TimeSpan.FromSeconds(2);
}

/// <summary>
/// The stores the gateway uses, built from <see cref="FluxerCacheConfig"/>. Calls through these never throw: a failure
/// or a timeout reads as a miss and is counted in <see cref="Statistics"/>.
/// </summary>
public sealed class GatewayCache : IAsyncDisposable
{
    internal GatewayCache(FluxerCacheConfig config) : this(config, Serilog.Core.Logger.None)
    {
    }

    internal GatewayCache(FluxerCacheConfig config, ILogger logger)
    {
        if (config.StoreTimeout <= TimeSpan.Zero)
            throw new ArgumentOutOfRangeException(nameof(config.StoreTimeout), "StoreTimeout must be longer than zero.");

        Statistics = new GatewayCacheStatistics(new CacheStatistics(), new CacheStatistics(), new CacheStatistics(), new CacheStatistics());
        Messages = Guard(config.MessageStore, config.Messages, Statistics.Messages, config.StoreTimeout, logger, "message");
        Users = Guard(config.UserStore, config.Users, Statistics.Users, config.StoreTimeout, logger, "user");
        Presences = Guard(config.PresenceStore, config.Presences, Statistics.Presences, config.StoreTimeout, logger, "presence");
        Invites = Guard(config.InviteStore, config.Invites, Statistics.Invites, config.StoreTimeout, logger, "invite");
    }

    private static IEntityStore<TKey, TValue> Guard<TKey, TValue>(IEntityStore<TKey, TValue>? custom, CacheLimits limits, CacheStatistics statistics,
        TimeSpan timeout, ILogger logger, string name) where TKey : notnull where TValue : class
        => new GuardedEntityStore<TKey, TValue>(custom ?? new MemoryEntityStore<TKey, TValue>(limits, statistics), statistics, timeout, logger, name);

    /// <summary>
    /// Guild messages by message ID.
    /// </summary>
    public IEntityStore<ulong, MessageGatewayData> Messages { get; }

    /// <summary>
    /// Users by user ID.
    /// </summary>
    public IEntityStore<ulong, UserJson> Users { get; }

    /// <summary>
    /// Presences by user ID.
    /// </summary>
    public IEntityStore<ulong, PresenceGatewayData> Presences { get; }

    /// <summary>
    /// Invites by code.
    /// </summary>
    public IEntityStore<string, InviteJson> Invites { get; }

    /// <summary>
    /// The counters of every store.
    /// </summary>
    public GatewayCacheStatistics Statistics { get; }

    internal async ValueTask RemoveScopeAsync(ulong scopeId)
    {
        await Messages.RemoveScopeAsync(scopeId);
        await Invites.RemoveScopeAsync(scopeId);
    }

    internal async ValueTask ClearBuiltInAsync()
    {
        foreach (Func<ValueTask> clear in BuiltIn())
            await clear();
    }

    private IEnumerable<Func<ValueTask>> BuiltIn()
    {
        if (IsBuiltIn(Messages))
            yield return () => Messages.ClearAsync();
        if (IsBuiltIn(Users))
            yield return () => Users.ClearAsync();
        if (IsBuiltIn(Presences))
            yield return () => Presences.ClearAsync();
        if (IsBuiltIn(Invites))
            yield return () => Invites.ClearAsync();
    }

    private static bool IsBuiltIn<TKey, TValue>(IEntityStore<TKey, TValue> store) where TKey : notnull where TValue : class
        => store is GuardedEntityStore<TKey, TValue> { Inner: MemoryEntityStore<TKey, TValue> };

    /// <summary>
    /// Disposes every store.
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        await Messages.DisposeAsync();
        await Users.DisposeAsync();
        await Presences.DisposeAsync();
        await Invites.DisposeAsync();
    }
}
