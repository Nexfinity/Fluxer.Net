namespace Fluxer.Net.Gateway;

/// <summary>
/// Where a cached entry belongs. Stores use it to drop everything of a guild or channel at once.
/// </summary>
/// <param name="GuildId">The guild of the entry, if any.</param>
/// <param name="ChannelId">The channel or thread of the entry, if any.</param>
public readonly record struct CacheScope(ulong? GuildId, ulong? ChannelId);

/// <summary>
/// A store for one kind of cached gateway data. Implement it to keep the cache somewhere else, for example in Redis
/// to share it between shards or keep it across restarts. <see cref="SerializedEntityStore{TKey, TValue}"/> does the
/// serialization and scope bookkeeping for key-value backends.
/// </summary>
/// <remarks>
/// <para>Values are the raw JSON models and can be serialized with <see cref="FluxerJson"/>.</para>
/// <para>Methods can be called at the same time from several threads: the gateway calls them one event at a time,
/// but your own code can read the cache whenever it likes. Implementations must be thread safe.</para>
/// <para>The gateway calls every method with a timeout (<see cref="FluxerCacheConfig.StoreTimeout"/>) and treats an
/// exception or a timeout as a miss. A failing store never stops events. Honour the cancellation token to stop
/// work the gateway has given up on.</para>
/// <para>Several bots or shards sharing one backend need their own key namespace, unless they are meant to share entries.</para>
/// </remarks>
/// <typeparam name="TKey">The key, usually an ID.</typeparam>
/// <typeparam name="TValue">The JSON model that is stored.</typeparam>
public interface IEntityStore<TKey, TValue> : IAsyncDisposable where TKey : notnull where TValue : class
{
    /// <summary>
    /// Gets an entry, or null when it isn't stored.
    /// </summary>
    ValueTask<TValue?> GetAsync(TKey key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Stores or replaces an entry.
    /// </summary>
    ValueTask SetAsync(TKey key, TValue value, CacheScope scope, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes an entry and returns it, or null when it wasn't stored.
    /// </summary>
    ValueTask<TValue?> RemoveAsync(TKey key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes every entry whose scope has this guild or channel ID.
    /// </summary>
    ValueTask RemoveScopeAsync(ulong scopeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes every entry.
    /// </summary>
    ValueTask ClearAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets several entries at once. Keys that aren't stored are left out. Override it when the backend can fetch in one round trip.
    /// </summary>
    async ValueTask<IReadOnlyDictionary<TKey, TValue>> GetManyAsync(IEnumerable<TKey> keys, CancellationToken cancellationToken = default)
    {
        Dictionary<TKey, TValue> found = new Dictionary<TKey, TValue>();
        foreach (TKey key in keys)
        {
            if (await GetAsync(key, cancellationToken) is { } value)
                found[key] = value;
        }

        return found;
    }

    /// <summary>
    /// Removes several entries at once and returns the ones that were stored. Override it when the backend can delete in one round trip.
    /// </summary>
    async ValueTask<IReadOnlyDictionary<TKey, TValue>> RemoveManyAsync(IEnumerable<TKey> keys, CancellationToken cancellationToken = default)
    {
        Dictionary<TKey, TValue> removed = new Dictionary<TKey, TValue>();
        foreach (TKey key in keys)
        {
            if (await RemoveAsync(key, cancellationToken) is { } value)
                removed[key] = value;
        }

        return removed;
    }

    ValueTask IAsyncDisposable.DisposeAsync() => default;
}
