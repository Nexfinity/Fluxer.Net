namespace Fluxer.Net.RateLimiting;

internal sealed class RollingWindow
{
    private readonly object _lock = new();
    private readonly Queue<DateTimeOffset> _uses = new();
    private readonly int _limit;
    private readonly TimeSpan _window;

    public RollingWindow(int limit, TimeSpan window)
    {
        _limit = limit;
        _window = window;
    }

    public TimeSpan TimeUntilFree()
    {
        DateTimeOffset now = DateTimeOffset.UtcNow;
        lock (_lock)
        {
            Expire(now);
            return _uses.Count < _limit ? TimeSpan.Zero : _uses.Peek() + _window - now;
        }
    }

    public void Record()
    {
        lock (_lock)
            _uses.Enqueue(DateTimeOffset.UtcNow);
    }

    public void Clear()
    {
        lock (_lock)
            _uses.Clear();
    }

    private void Expire(DateTimeOffset now)
    {
        while (_uses.Count > 0 && _uses.Peek() + _window <= now)
            _uses.Dequeue();
    }
}
