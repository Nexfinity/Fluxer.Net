namespace Fluxer.Net.RateLimiting;

internal sealed class RateLimitBucket
{
    private static readonly TimeSpan DefaultInterval = TimeSpan.FromSeconds(1);
    private static readonly TimeSpan ProbeWait = TimeSpan.FromSeconds(5);

    private readonly object _lock = new();
    private readonly bool _fixed;
    private int? _limit;
    private double _tokens;
    private TimeSpan _interval;
    private DateTimeOffset _updatedAt;
    private DateTimeOffset _blockedUntil;
    private bool _unlimited;
    private bool _probing;
    private TaskCompletionSource<bool> _learned = new(TaskCreationOptions.RunContinuationsAsynchronously);

    public RateLimitBucket(BucketId id)
    {
        Id = id;
        LastUsedAt = DateTimeOffset.UtcNow;
    }

    private RateLimitBucket(BucketId id, int limit, TimeSpan window) : this(id)
    {
        _fixed = true;
        _limit = limit;
        _tokens = limit;
        _interval = TimeSpan.FromTicks(Math.Max(1, window.Ticks / limit));
        _updatedAt = DateTimeOffset.UtcNow;
    }

    public BucketId Id { get; }

    public DateTimeOffset LastUsedAt { get; private set; }

    public bool IsBusy
    {
        get
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;
            lock (_lock)
            {
                Refill(now);
                return _probing || now < _blockedUntil || (_limit.HasValue && _tokens < _limit.Value);
            }
        }
    }

    public static RateLimitBucket Fixed(BucketId id, int limit, TimeSpan window) => new(id, limit, window);

    public bool TryTake(out TimeSpan wait, out Task signal)
    {
        DateTimeOffset now = DateTimeOffset.UtcNow;
        signal = null;
        wait = TimeSpan.Zero;
        lock (_lock)
        {
            LastUsedAt = now;
            if (_unlimited)
                return true;

            if (now < _blockedUntil)
            {
                wait = _blockedUntil - now;
                return false;
            }

            if (_limit is null)
            {
                if (!_probing)
                {
                    _probing = true;
                    return true;
                }

                wait = ProbeWait;
                signal = _learned.Task;
                return false;
            }

            Refill(now);
            if (_tokens >= 1)
            {
                _tokens -= 1;
                return true;
            }

            wait = TimeSpan.FromTicks((long)Math.Ceiling((1 - _tokens) * _interval.Ticks));
            return false;
        }
    }

    public void Update(RateLimitInfo info, bool rateLimited)
    {
        DateTimeOffset now = DateTimeOffset.UtcNow;
        lock (_lock)
        {
            if (!_fixed)
            {
                bool unknown = _limit is null;
                if (!unknown)
                    Refill(now);

                if (info.Limit is int limit && limit > 0)
                {
                    _limit = limit;
                    _unlimited = false;
                    if (info.Remaining is int remaining)
                    {
                        int used = limit - remaining;
                        if (used > 0 && info.ResetAfter is TimeSpan resetAfter && resetAfter > TimeSpan.Zero)
                            _interval = TimeSpan.FromTicks(Math.Max(1, resetAfter.Ticks / used));
                        else if (_interval <= TimeSpan.Zero)
                            _interval = DefaultInterval;

                        _tokens = unknown ? remaining : Math.Min(_tokens, remaining);
                        _updatedAt = now;
                    }
                    else if (unknown)
                    {
                        _tokens = limit;
                        _interval = DefaultInterval;
                        _updatedAt = now;
                    }
                }
                else if (unknown && !rateLimited)
                {
                    _unlimited = true;
                }

                _probing = false;
            }

            if (rateLimited)
            {
                TimeSpan retry = info.RetryAfter is int ms
                    ? TimeSpan.FromMilliseconds(ms)
                    : info.ResetAfter ?? DefaultInterval;
                _tokens = 0;
                _updatedAt = now;
                if (now + retry > _blockedUntil)
                    _blockedUntil = now + retry;
            }

            Signal();
        }
    }

    public void Abandon()
    {
        lock (_lock)
        {
            if (!_probing)
                return;

            _probing = false;
            Signal();
        }
    }

    private void Refill(DateTimeOffset now)
    {
        if (_limit is not int limit || _interval <= TimeSpan.Zero || now <= _updatedAt)
            return;

        _tokens = Math.Min(limit, _tokens + (double)(now - _updatedAt).Ticks / _interval.Ticks);
        _updatedAt = now;
    }

    private void Signal()
    {
        TaskCompletionSource<bool> learned = _learned;
        _learned = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        learned.TrySetResult(true);
    }
}
