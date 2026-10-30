namespace Fluxer.Net.Gateway;

/// <summary>
/// Counters for one cache store, to see how well it works and tune its limits.
/// </summary>
public sealed class CacheStatistics
{
    private long _hits;
    private long _misses;
    private long _writes;
    private long _evictions;
    private long _expirations;
    private long _failures;
    private long _timeouts;

    /// <summary>
    /// Reads that found an entry.
    /// </summary>
    public long Hits => Interlocked.Read(ref _hits);

    /// <summary>
    /// Reads that found nothing, including failed and timed out reads.
    /// </summary>
    public long Misses => Interlocked.Read(ref _misses);

    /// <summary>
    /// Entries stored or replaced.
    /// </summary>
    public long Writes => Interlocked.Read(ref _writes);

    /// <summary>
    /// Entries the built-in store dropped to stay within its limits.
    /// </summary>
    public long Evictions => Interlocked.Read(ref _evictions);

    /// <summary>
    /// Entries the built-in store dropped after they expired.
    /// </summary>
    public long Expirations => Interlocked.Read(ref _expirations);

    /// <summary>
    /// Store calls that threw.
    /// </summary>
    public long Failures => Interlocked.Read(ref _failures);

    /// <summary>
    /// Store calls that took longer than <see cref="FluxerCacheConfig.StoreTimeout"/>.
    /// </summary>
    public long Timeouts => Interlocked.Read(ref _timeouts);

    internal void Hit() => Interlocked.Increment(ref _hits);

    internal void Miss() => Interlocked.Increment(ref _misses);

    internal void Write() => Interlocked.Increment(ref _writes);

    internal void Evict() => Interlocked.Increment(ref _evictions);

    internal void Expire() => Interlocked.Increment(ref _expirations);

    internal void Fail() => Interlocked.Increment(ref _failures);

    internal void TimeOut() => Interlocked.Increment(ref _timeouts);
}

/// <summary>
/// The counters of every gateway cache store.
/// </summary>
/// <param name="Messages">Counters for guild messages.</param>
/// <param name="Users">Counters for users.</param>
/// <param name="Presences">Counters for presences.</param>
/// <param name="Invites">Counters for invites.</param>
public sealed record GatewayCacheStatistics(CacheStatistics Messages, CacheStatistics Users, CacheStatistics Presences, CacheStatistics Invites);
