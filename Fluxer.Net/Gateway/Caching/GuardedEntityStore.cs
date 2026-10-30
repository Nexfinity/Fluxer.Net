using Serilog;

namespace Fluxer.Net.Gateway;

internal sealed class GuardedEntityStore<TKey, TValue> : IEntityStore<TKey, TValue> where TKey : notnull where TValue : class
{
    private readonly IEntityStore<TKey, TValue> _inner;
    private readonly CacheStatistics _statistics;
    private readonly TimeSpan _timeout;
    private readonly ILogger _logger;
    private readonly string _name;

    public GuardedEntityStore(IEntityStore<TKey, TValue> inner, CacheStatistics statistics, TimeSpan timeout, ILogger logger, string name)
    {
        _inner = inner;
        _statistics = statistics;
        _timeout = timeout;
        _logger = logger;
        _name = name;
    }

    public IEntityStore<TKey, TValue> Inner => _inner;

    public async ValueTask<TValue?> GetAsync(TKey key, CancellationToken cancellationToken = default)
    {
        TValue? value = await RunAsync(token => _inner.GetAsync(key, token), null, "get", cancellationToken);
        if (value == null)
            _statistics.Miss();
        else
            _statistics.Hit();
        return value;
    }

    public async ValueTask<IReadOnlyDictionary<TKey, TValue>> GetManyAsync(IEnumerable<TKey> keys, CancellationToken cancellationToken = default)
    {
        List<TKey> list = keys.ToList();
        IReadOnlyDictionary<TKey, TValue> found = await RunAsync(token => _inner.GetManyAsync(list, token), new Dictionary<TKey, TValue>(), "get many", cancellationToken);
        for (int i = 0; i < list.Count; i++)
        {
            if (found.ContainsKey(list[i]))
                _statistics.Hit();
            else
                _statistics.Miss();
        }

        return found;
    }

    public async ValueTask SetAsync(TKey key, TValue value, CacheScope scope, CancellationToken cancellationToken = default)
    {
        if (await RunAsync(async token => { await _inner.SetAsync(key, value, scope, token); return true; }, false, "set", cancellationToken))
            _statistics.Write();
    }

    public ValueTask<TValue?> RemoveAsync(TKey key, CancellationToken cancellationToken = default)
        => RunAsync(token => _inner.RemoveAsync(key, token), null, "remove", cancellationToken);

    public ValueTask<IReadOnlyDictionary<TKey, TValue>> RemoveManyAsync(IEnumerable<TKey> keys, CancellationToken cancellationToken = default)
    {
        List<TKey> list = keys.ToList();
        return RunAsync(token => _inner.RemoveManyAsync(list, token), new Dictionary<TKey, TValue>(), "remove many", cancellationToken);
    }

    public async ValueTask RemoveScopeAsync(ulong scopeId, CancellationToken cancellationToken = default)
        => await RunAsync(async token => { await _inner.RemoveScopeAsync(scopeId, token); return true; }, false, "remove scope", cancellationToken);

    public async ValueTask ClearAsync(CancellationToken cancellationToken = default)
        => await RunAsync(async token => { await _inner.ClearAsync(token); return true; }, false, "clear", cancellationToken);

    public ValueTask DisposeAsync() => _inner.DisposeAsync();

    private async ValueTask<T> RunAsync<T>(Func<CancellationToken, ValueTask<T>> operation, T fallback, string action, CancellationToken cancellationToken)
    {
        if (_inner is MemoryEntityStore<TKey, TValue>)
        {
            try
            {
                return await operation(cancellationToken);
            }
            catch (Exception ex)
            {
                Failed(ex, action);
                return fallback;
            }
        }

        using CancellationTokenSource timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeout.CancelAfter(_timeout);
        Task<T> task;
        try
        {
            task = operation(timeout.Token).AsTask();
        }
        catch (Exception ex)
        {
            Failed(ex, action);
            return fallback;
        }

        try
        {
            if (await Task.WhenAny(task, Task.Delay(Timeout.Infinite, timeout.Token)) != task)
            {
                _ = task.ContinueWith(t => _ = t.Exception, TaskContinuationOptions.OnlyOnFaulted);
                if (!cancellationToken.IsCancellationRequested)
                    TimedOut(action);
                return fallback;
            }

            return await task;
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            TimedOut(action);
            return fallback;
        }
        catch (OperationCanceledException)
        {
            return fallback;
        }
        catch (Exception ex)
        {
            Failed(ex, action);
            return fallback;
        }
        finally
        {
            timeout.Cancel();
        }
    }

    private void TimedOut(string action)
    {
        _statistics.TimeOut();
        _logger.Warning("The {Store} cache store took longer than {Timeout} to {Action}", _name, _timeout, action);
    }

    private void Failed(Exception ex, string action)
    {
        _statistics.Fail();
        _logger.Warning(ex, "The {Store} cache store failed to {Action}", _name, action);
    }
}
