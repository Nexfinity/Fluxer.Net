using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Channels;
using Newtonsoft.Json.Linq;

namespace Fluxer.Net.Gateway;

public partial class FluxerGatewayClient
{
    private readonly ConcurrentDictionary<string, GatewayEventBase> _events = new();
    private readonly AsyncLocal<DispatchScope?> _dispatchScope = new();
    private readonly AsyncLocal<LaneRun?> _laneRun = new();
    private readonly object _queueLock = new();
    private readonly List<(int Limit, TaskCompletionSource<bool> Signal)> _queueWaiters = new();
    private FluxerEventConfig _eventConfig = new();
    private EventLane[]? _lanes;
    private bool _dispatchStarted;
    private volatile bool _dispatchStopped;
    private int _queuedEvents;
    private int _queueWaiterCount;
    private int _backlogWarned;
    private int _queueWarned;

    /// <summary>
    /// The number of raised events still waiting for, or running, their handlers.
    /// </summary>
    public int QueuedEvents => Volatile.Read(ref _queuedEvents);

    /// <summary>
    /// Lets later events of the current guild reach their handlers while the calling handler keeps running.
    /// </summary>
    /// <remarks>
    /// Later events go ahead on their own after <see cref="FluxerEventConfig.LaneHoldLimit"/>, and <c>WaitAsync</c> and
    /// the <c>WaitFor</c> helpers call this for you. Call it yourself to let them go ahead at once, for example before
    /// waiting on your own signal or before long work. It does nothing outside a handler.
    /// </remarks>
    public void ReleaseEventLane() => _laneRun.Value?.Release();

    /// <summary>
    /// Waits for the next message matching <paramref name="predicate"/>, letting later events of the calling handler's
    /// guild go ahead meanwhile.
    /// </summary>
    /// <param name="predicate">Which message to wait for.</param>
    /// <param name="timeout">How long to wait. Null waits until <paramref name="cancellationToken"/> is cancelled.</param>
    /// <param name="cancellationToken">Cancels the wait.</param>
    /// <returns>The message, or null when the timeout passed first.</returns>
    public Task<SocketMessage?> WaitForMessageAsync(Func<SocketMessage, bool> predicate, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
        => MessageCreated.WaitAsync(predicate, timeout, cancellationToken);

    /// <summary>
    /// Waits for the next message the author of <paramref name="message"/> sends in its channel, letting later events of
    /// the calling handler's guild go ahead meanwhile.
    /// </summary>
    /// <param name="message">The message to wait for a reply to.</param>
    /// <param name="timeout">How long to wait. Null waits until <paramref name="cancellationToken"/> is cancelled.</param>
    /// <param name="cancellationToken">Cancels the wait.</param>
    /// <returns>The reply, or null when the timeout passed first.</returns>
    public Task<SocketMessage?> WaitForReplyAsync(SocketMessage message, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
        => WaitForMessageAsync(m => m.Id != message.Id && m.ChannelId == message.ChannelId && m.Author != null && m.Author.Id == message.Author?.Id,
            timeout, cancellationToken);

    /// <summary>
    /// Waits for the next guild reaction matching <paramref name="predicate"/>, letting later events of the calling
    /// handler's guild go ahead meanwhile.
    /// </summary>
    /// <param name="predicate">Which reaction to wait for.</param>
    /// <param name="timeout">How long to wait. Null waits until <paramref name="cancellationToken"/> is cancelled.</param>
    /// <param name="cancellationToken">Cancels the wait.</param>
    /// <returns>The reaction, or null when the timeout passed first.</returns>
    public async Task<SocketReaction?> WaitForReactionAsync(Func<SocketReaction, bool> predicate, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
        => (await MessageReactionAdded.WaitAsync((_, _, reaction) => predicate(reaction), timeout, cancellationToken).ConfigureAwait(false))?.Item3;

    private T Event<T>([CallerMemberName] string name = "") where T : GatewayEventBase
    {
        if (_events.TryGetValue(name, out GatewayEventBase? existing))
            return (T)existing;

        return (T)_events.GetOrAdd(name, n =>
        {
            T created = (T)Activator.CreateInstance(typeof(T), nonPublic: true)!;
            created.Owner = this;
            created.Name = n;
            return created;
        });
    }

    private void Keep(GatewayEventBase value, [CallerMemberName] string name = "")
    {
        if (!_events.TryGetValue(name, out GatewayEventBase? current) || !ReferenceEquals(current, value))
            throw new InvalidOperationException($"{name} can only be changed with += and -=.");
    }

    internal void StartEventDispatch(FluxerEventConfig config)
    {
        if (_dispatchStarted)
            return;

        config.Validate();
        _eventConfig = config;
        if (config.Mode != EventDispatchMode.Concurrent)
        {
            EventLane[] lanes = new EventLane[config.Mode == EventDispatchMode.Sequential ? 1 : config.Lanes];
            for (int i = 0; i < lanes.Length; i++)
            {
                EventLane lane = new EventLane(i);
                lanes[i] = lane;
                _ = Task.Run(() => RunLaneAsync(lane));
            }

            _lanes = lanes;
        }

        _dispatchStarted = true;
    }

    private void StopEventDispatch()
    {
        _dispatchStopped = true;
        if (_lanes != null)
        {
            foreach (EventLane lane in _lanes)
                lane.Queue.Writer.TryComplete();
        }

        lock (_queueLock)
        {
            foreach ((int, TaskCompletionSource<bool> signal) waiter in _queueWaiters)
                waiter.signal.TrySetResult(true);
            _queueWaiters.Clear();
            Volatile.Write(ref _queueWaiterCount, 0);
        }
    }

    internal void QueueEvent(GatewayEventBase e, Func<Task> run)
    {
        if (!_dispatchStarted)
        {
            _ = run();
            return;
        }

        Interlocked.Increment(ref _queuedEvents);
        EventLane[]? lanes = _lanes;
        if (lanes == null)
        {
            _ = RunQueuedAsync(new QueuedEvent(e, run), null);
            return;
        }

        ulong key = _dispatchScope.Value?.Key ?? 0;
        int index = lanes.Length == 1 ? 0 : (int)((key * 0x9E3779B97F4A7C15UL >> 32) % (ulong)lanes.Length);
        if (!lanes[index].Queue.Writer.TryWrite(new QueuedEvent(e, run)))
            ReleaseQueued();
    }

    private async Task RunLaneAsync(EventLane lane)
    {
        ChannelReader<QueuedEvent> reader = lane.Queue.Reader;
        while (await reader.WaitToReadAsync().ConfigureAwait(false))
        {
            while (reader.TryRead(out QueuedEvent item))
                await RunQueuedAsync(item, lane);
        }
    }

    private async Task RunQueuedAsync(QueuedEvent item, EventLane? lane)
    {
        Task? running = null;
        try
        {
            LaneRun laneRun = new LaneRun();
            _laneRun.Value = laneRun;
            running = item.Run();
            _ = ReleaseWhenDoneAsync(running);
            if (running.IsCompleted || lane == null)
            {
                await running.ConfigureAwait(false);
                return;
            }

            Task released = laneRun.Released;
            TimeSpan? holdLimit = _eventConfig.LaneHoldLimit;
            if (holdLimit == null)
            {
                await Task.WhenAny(running, released).ConfigureAwait(false);
                return;
            }

            using CancellationTokenSource delay = new CancellationTokenSource();
            Task timer = Task.Delay(holdLimit.Value, delay.Token);
            if (await Task.WhenAny(running, released, timer).ConfigureAwait(false) == timer)
                _logger.Debug("{Event} handlers are still running after {Seconds:F1} seconds. Later events in lane {Lane} go ahead",
                    item.Event.Name, holdLimit.Value.TotalSeconds, lane!.Index);
            delay.Cancel();
        }
        catch (Exception ex)
        {
            LogHandlerError(item.Event.Name, ex);
            if (running == null)
                ReleaseQueued();
        }
    }

    private async Task ReleaseWhenDoneAsync(Task running)
    {
        try
        {
            await running.ConfigureAwait(false);
        }
        catch
        {
        }
        finally
        {
            ReleaseQueued();
        }
    }

    private void ReleaseQueued()
    {
        int queued = Interlocked.Decrement(ref _queuedEvents);
        if (queued == 0)
            Volatile.Write(ref _queueWarned, 0);
        if (Volatile.Read(ref _queueWaiterCount) > 0)
            WakeQueueWaiters(queued);
    }

    private void WakeQueueWaiters(int queued)
    {
        lock (_queueLock)
        {
            for (int i = _queueWaiters.Count - 1; i >= 0; i--)
            {
                (int limit, TaskCompletionSource<bool> signal) = _queueWaiters[i];
                if (queued <= limit || signal.Task.IsCompleted)
                {
                    signal.TrySetResult(true);
                    _queueWaiters.RemoveAt(i);
                }
            }

            Volatile.Write(ref _queueWaiterCount, _queueWaiters.Count);
        }
    }

    private async Task WaitForQueuedEventsAsync(int limit, CancellationToken cancellationToken)
    {
        if (Volatile.Read(ref _queuedEvents) <= limit || _dispatchStopped)
            return;

        TaskCompletionSource<bool> signal = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        lock (_queueLock)
        {
            if (_dispatchStopped)
                return;

            _queueWaiters.Add((limit, signal));
            Volatile.Write(ref _queueWaiterCount, _queueWaiters.Count);
        }

        int queued = Volatile.Read(ref _queuedEvents);
        if (queued <= limit)
            WakeQueueWaiters(queued);

        using (cancellationToken.Register(() => signal.TrySetCanceled()))
            await signal.Task.ConfigureAwait(false);
    }

    private async ValueTask WaitForHandlerRoomAsync()
    {
        if (!_dispatchStarted)
            return;

        int limit = _eventConfig.MaxQueuedEvents;
        if (Volatile.Read(ref _queuedEvents) <= limit)
            return;

        if (Interlocked.Exchange(ref _queueWarned, 1) == 0)
            _logger.Warning("{Queued} events are waiting for handlers. The gateway pauses handling new events until handlers catch up", Volatile.Read(ref _queuedEvents));
        await WaitForQueuedEventsAsync(limit, CancellationToken.None).ConfigureAwait(false);
    }

    private void TrackReceivedBacklog(int pending)
    {
        if (pending > _eventConfig.MaxQueuedEvents && Interlocked.Exchange(ref _backlogWarned, 1) == 0)
            _logger.Warning("{Pending} received events are waiting to be handled. Cache stores or handlers are slower than events arrive", pending);
        else if (pending == 0)
            Volatile.Write(ref _backlogWarned, 0);
    }

    internal void LogHandlerError(string name, Exception ex)
        => _logger.Error(ex, "A {Event} handler threw an exception", name);

    internal void CheckHandlerDuration(string name, Delegate handler, long started)
    {
        TimeSpan? warnAfter = _eventConfig.SlowHandlerWarning;
        if (warnAfter == null || _laneRun.Value?.ReleasedByHandler == true)
            return;

        double seconds = (Stopwatch.GetTimestamp() - started) / (double)Stopwatch.Frequency;
        if (seconds > warnAfter.Value.TotalSeconds)
            _logger.Warning("A {Event} handler ({Handler}) took {Seconds:F1} seconds", name, $"{handler.Method.DeclaringType?.Name}.{handler.Method.Name}", seconds);
    }

    private static ulong PartitionOf(GatewayPacket packet)
    {
        if (packet.Data is not JObject data)
            return 0;

        return Snowflake(data["guild_id"])
            ?? (packet.Dispatch?.StartsWith("GUILD_", StringComparison.Ordinal) == true ? Snowflake(data["id"]) : null)
            ?? Snowflake(data["channel_id"])
            ?? 0;
    }

    private static ulong? Snowflake(JToken? token)
        => token != null && token.Type is JTokenType.String or JTokenType.Integer && ulong.TryParse(token.ToString(), out ulong id) ? id : null;

    private PartitionOverride EventsFor(ulong key) => new PartitionOverride(_dispatchScope.Value, key);

    private sealed class DispatchScope(ulong key)
    {
        public ulong Key = key;
    }

    private readonly struct PartitionOverride : IDisposable
    {
        private readonly DispatchScope? _scope;
        private readonly ulong _previous;

        public PartitionOverride(DispatchScope? scope, ulong key)
        {
            _scope = scope;
            _previous = scope?.Key ?? 0;
            if (scope != null)
                scope.Key = key;
        }

        public void Dispose()
        {
            if (_scope != null)
                _scope.Key = _previous;
        }
    }

    private readonly struct QueuedEvent(GatewayEventBase e, Func<Task> run)
    {
        public GatewayEventBase Event { get; } = e;
        public Func<Task> Run { get; } = run;
    }

    private sealed class EventLane(int index)
    {
        public int Index { get; } = index;
        public Channel<QueuedEvent> Queue { get; } = System.Threading.Channels.Channel.CreateUnbounded<QueuedEvent>(new UnboundedChannelOptions { SingleReader = true });
    }

    private sealed class LaneRun
    {
        private TaskCompletionSource<bool>? _released;

        public Task Released => Signal().Task;

        public bool ReleasedByHandler => Volatile.Read(ref _released)?.Task.IsCompleted == true;

        public void Release() => Signal().TrySetResult(true);

        private TaskCompletionSource<bool> Signal()
        {
            TaskCompletionSource<bool>? current = Volatile.Read(ref _released);
            if (current != null)
                return current;

            Interlocked.CompareExchange(ref _released, new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously), null);
            return _released!;
        }
    }
}
