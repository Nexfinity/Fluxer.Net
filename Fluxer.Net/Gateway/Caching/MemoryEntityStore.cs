namespace Fluxer.Net.Gateway;

/// <summary>
/// The built-in in-memory store. It keeps entries within a total limit, a per-channel limit, an expiry, or any mix of them,
/// dropping the oldest first. Expired entries are swept in the background. With no limit and no expiry it keeps nothing.
/// </summary>
public sealed class MemoryEntityStore<TKey, TValue> : IEntityStore<TKey, TValue> where TKey : notnull where TValue : class
{
    private readonly int _maxEntries;
    private readonly int _maxPerChannel;
    private readonly TimeSpan? _expiry;
    private readonly Func<DateTimeOffset> _clock;
    private readonly CacheStatistics _statistics;
    private readonly Timer? _sweeper;
    private readonly object _lock = new object();
    private readonly Dictionary<TKey, Entry> _entries = new Dictionary<TKey, Entry>();
    private readonly LinkedList<TKey> _order = new LinkedList<TKey>();
    private readonly Dictionary<ulong, LinkedList<TKey>> _channels = new Dictionary<ulong, LinkedList<TKey>>();
    private readonly Dictionary<ulong, HashSet<TKey>> _guilds = new Dictionary<ulong, HashSet<TKey>>();

    private sealed class Entry
    {
        public TValue Value = null!;
        public CacheScope Scope;
        public DateTimeOffset StoredAt;
        public LinkedListNode<TKey> OrderNode = null!;
        public LinkedListNode<TKey>? ChannelNode;
    }

    /// <summary>
    /// Creates the store with the given limits.
    /// </summary>
    /// <param name="limits">The total and per-channel limits and the expiry.</param>
    public MemoryEntityStore(CacheLimits limits) : this(limits, new CacheStatistics(), () => DateTimeOffset.UtcNow, true)
    {
    }

    internal MemoryEntityStore(CacheLimits limits, CacheStatistics statistics) : this(limits, statistics, () => DateTimeOffset.UtcNow, true)
    {
    }

    internal MemoryEntityStore(CacheLimits limits, Func<DateTimeOffset> clock) : this(limits, new CacheStatistics(), clock, false)
    {
    }

    private MemoryEntityStore(CacheLimits limits, CacheStatistics statistics, Func<DateTimeOffset> clock, bool sweep)
    {
        limits.Validate();
        _maxEntries = limits.MaxEntries;
        _maxPerChannel = limits.MaxPerChannel;
        _expiry = limits.Expiry;
        _clock = clock;
        _statistics = statistics;
        if (sweep && _expiry is TimeSpan expiry)
        {
            TimeSpan period = expiry < TimeSpan.FromMinutes(1) ? expiry : TimeSpan.FromMinutes(1);
            if (period < TimeSpan.FromSeconds(1))
                period = TimeSpan.FromSeconds(1);
            _sweeper = new Timer(_ => Sweep(), null, period, period);
        }
    }

    /// <summary>
    /// Whether the store keeps anything.
    /// </summary>
    public bool IsEnabled => _maxEntries > 0 || _maxPerChannel > 0 || _expiry.HasValue;

    /// <summary>
    /// The counters of this store.
    /// </summary>
    public CacheStatistics Statistics => _statistics;

    /// <summary>
    /// The number of stored entries.
    /// </summary>
    public int Count
    {
        get
        {
            lock (_lock)
                return _entries.Count;
        }
    }

    internal int IndexCount
    {
        get
        {
            lock (_lock)
                return _channels.Count + _guilds.Count;
        }
    }

    /// <summary>
    /// The keys stored for a channel, oldest first.
    /// </summary>
    public IReadOnlyList<TKey> GetChannelKeys(ulong channelId)
    {
        lock (_lock)
            return _channels.TryGetValue(channelId, out LinkedList<TKey>? keys) ? keys.ToList() : new List<TKey>();
    }

    /// <inheritdoc />
    public ValueTask<TValue?> GetAsync(TKey key, CancellationToken cancellationToken = default)
    {
        lock (_lock)
            return new ValueTask<TValue?>(GetLocked(key));
    }

    /// <inheritdoc />
    public ValueTask<IReadOnlyDictionary<TKey, TValue>> GetManyAsync(IEnumerable<TKey> keys, CancellationToken cancellationToken = default)
    {
        Dictionary<TKey, TValue> found = new Dictionary<TKey, TValue>();
        lock (_lock)
        {
            foreach (TKey key in keys)
            {
                if (GetLocked(key) is { } value)
                    found[key] = value;
            }
        }

        return new ValueTask<IReadOnlyDictionary<TKey, TValue>>(found);
    }

    /// <inheritdoc />
    public ValueTask SetAsync(TKey key, TValue value, CacheScope scope, CancellationToken cancellationToken = default)
    {
        if (!IsEnabled)
            return default;

        lock (_lock)
        {
            if (_entries.TryGetValue(key, out Entry? existing))
            {
                if (existing.Scope != scope)
                {
                    RemoveLocked(key, existing);
                }
                else
                {
                    existing.Value = value;
                    existing.StoredAt = _clock();
                    _order.Remove(existing.OrderNode);
                    _order.AddLast(existing.OrderNode);
                    return default;
                }
            }

            Entry entry = new Entry { Value = value, Scope = scope, StoredAt = _clock(), OrderNode = _order.AddLast(key) };
            _entries[key] = entry;

            if (scope.GuildId is ulong guildId)
            {
                if (!_guilds.TryGetValue(guildId, out HashSet<TKey>? guild))
                    _guilds[guildId] = guild = new HashSet<TKey>();
                guild.Add(key);
            }

            if (scope.ChannelId is ulong channelId)
            {
                if (!_channels.TryGetValue(channelId, out LinkedList<TKey>? channel))
                    _channels[channelId] = channel = new LinkedList<TKey>();

                entry.ChannelNode = channel.AddLast(key);
                while (_maxPerChannel > 0 && channel.Count > _maxPerChannel)
                    EvictLocked(channel.First!.Value);
            }

            while (_maxEntries > 0 && _entries.Count > _maxEntries)
                EvictLocked(_order.First!.Value);
        }

        return default;
    }

    /// <inheritdoc />
    public ValueTask<TValue?> RemoveAsync(TKey key, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            if (!_entries.TryGetValue(key, out Entry? entry))
                return new ValueTask<TValue?>((TValue?)null);

            RemoveLocked(key, entry);
            return new ValueTask<TValue?>(entry.Value);
        }
    }

    /// <inheritdoc />
    public ValueTask<IReadOnlyDictionary<TKey, TValue>> RemoveManyAsync(IEnumerable<TKey> keys, CancellationToken cancellationToken = default)
    {
        Dictionary<TKey, TValue> removed = new Dictionary<TKey, TValue>();
        lock (_lock)
        {
            foreach (TKey key in keys)
            {
                if (!_entries.TryGetValue(key, out Entry? entry))
                    continue;

                RemoveLocked(key, entry);
                removed[key] = entry.Value;
            }
        }

        return new ValueTask<IReadOnlyDictionary<TKey, TValue>>(removed);
    }

    /// <inheritdoc />
    public ValueTask RemoveScopeAsync(ulong scopeId, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            List<TKey> keys = new List<TKey>();
            if (_guilds.TryGetValue(scopeId, out HashSet<TKey>? guild))
                keys.AddRange(guild);
            if (_channels.TryGetValue(scopeId, out LinkedList<TKey>? channel))
                keys.AddRange(channel);

            foreach (TKey key in keys)
            {
                if (_entries.TryGetValue(key, out Entry? entry))
                    RemoveLocked(key, entry);
            }
        }

        return default;
    }

    /// <inheritdoc />
    public ValueTask ClearAsync(CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            _entries.Clear();
            _order.Clear();
            _channels.Clear();
            _guilds.Clear();
        }

        return default;
    }

    /// <summary>
    /// Drops every expired entry now. The store also does this in the background.
    /// </summary>
    public void Sweep()
    {
        if (_expiry is not TimeSpan expiry)
            return;

        lock (_lock)
        {
            DateTimeOffset cutoff = _clock() - expiry;
            while (_order.First is { } oldest && _entries[oldest.Value].StoredAt < cutoff)
            {
                RemoveLocked(oldest.Value, _entries[oldest.Value]);
                _statistics.Expire();
            }
        }
    }

    /// <inheritdoc />
    public ValueTask DisposeAsync()
    {
        _sweeper?.Dispose();
        return default;
    }

    private TValue? GetLocked(TKey key)
    {
        if (!_entries.TryGetValue(key, out Entry? entry))
            return null;

        if (_expiry.HasValue && _clock() - entry.StoredAt > _expiry.Value)
        {
            RemoveLocked(key, entry);
            _statistics.Expire();
            return null;
        }

        return entry.Value;
    }

    private void EvictLocked(TKey key)
    {
        RemoveLocked(key, _entries[key]);
        _statistics.Evict();
    }

    private void RemoveLocked(TKey key, Entry entry)
    {
        _entries.Remove(key);
        _order.Remove(entry.OrderNode);

        if (entry.Scope.GuildId is ulong guildId && _guilds.TryGetValue(guildId, out HashSet<TKey>? guild))
        {
            guild.Remove(key);
            if (guild.Count == 0)
                _guilds.Remove(guildId);
        }

        if (entry.ChannelNode != null && entry.Scope.ChannelId is ulong channelId && _channels.TryGetValue(channelId, out LinkedList<TKey>? channel))
        {
            channel.Remove(entry.ChannelNode);
            if (channel.Count == 0)
                _channels.Remove(channelId);
        }
    }
}
