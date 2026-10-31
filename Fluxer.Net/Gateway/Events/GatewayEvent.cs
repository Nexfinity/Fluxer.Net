using System.Diagnostics;

namespace Fluxer.Net.Gateway;

/// <summary>
/// A gateway event. Subscribe with <c>+=</c> and unsubscribe with <c>-=</c>, using a normal or an <c>async</c> handler.
/// Async handlers are awaited: they never run as <c>async void</c>, and their exceptions are caught and logged.
/// </summary>
/// <remarks>
/// Handlers of one event run together, and each handler's failure only affects itself. How events are ordered against
/// each other is set by <see cref="FluxerGatewayConfig.Events"/>.
/// </remarks>
public abstract class GatewayEventBase
{
    private protected GatewayEventBase()
    {
    }

    internal FluxerGatewayClient? Owner { get; set; }

    /// <summary>
    /// The name of the event.
    /// </summary>
    public string Name { get; internal set; } = string.Empty;

    /// <summary>
    /// The number of subscribed handlers.
    /// </summary>
    public abstract int HandlerCount { get; }

    internal async Task RunHandlerAsync(Delegate handler, Func<Task> call)
    {
        long started = Stopwatch.GetTimestamp();
        try
        {
            await call();
        }
        catch (Exception ex)
        {
            Owner?.LogHandlerError(Name, ex);
        }
        finally
        {
            Owner?.CheckHandlerDuration(Name, handler, started);
        }
    }
}

/// <summary>
/// Shared handler bookkeeping for the gateway event types.
/// </summary>
/// <typeparam name="THandler">The awaitable handler type the event invokes.</typeparam>
public abstract class GatewayEventBase<THandler> : GatewayEventBase where THandler : Delegate
{
    private readonly object _lock = new object();
    private (Delegate Key, THandler Handler)[] _handlers = Array.Empty<(Delegate, THandler)>();

    private protected GatewayEventBase()
    {
    }

    /// <inheritdoc />
    public override int HandlerCount => Volatile.Read(ref _handlers).Length;

    internal void Add(Delegate key, THandler handler)
    {
        lock (_lock)
        {
            (Delegate, THandler)[] next = new (Delegate, THandler)[_handlers.Length + 1];
            Array.Copy(_handlers, next, _handlers.Length);
            next[^1] = (key, handler);
            Volatile.Write(ref _handlers, next);
        }
    }

    internal void Remove(Delegate key)
    {
        lock (_lock)
        {
            int index = Array.FindLastIndex(_handlers, x => x.Key.Equals(key));
            if (index < 0)
                return;

            (Delegate, THandler)[] next = new (Delegate, THandler)[_handlers.Length - 1];
            Array.Copy(_handlers, 0, next, 0, index);
            Array.Copy(_handlers, index + 1, next, index, _handlers.Length - index - 1);
            Volatile.Write(ref _handlers, next);
        }
    }

    internal void AddNamed(Delegate handler, Func<Delegate, THandler> wrap)
    {
        foreach (Delegate single in handler.GetInvocationList())
            Add(single, wrap(single));
    }

    internal void RemoveNamed(Delegate handler)
    {
        foreach (Delegate single in handler.GetInvocationList())
            Remove(single);
    }

    internal void Raise(Func<THandler, Task> call)
    {
        if (Volatile.Read(ref _handlers).Length == 0 || Owner == null)
            return;

        Owner.QueueEvent(this, () => RunAllAsync(call));
    }

    private Task RunAllAsync(Func<THandler, Task> call)
    {
        (Delegate Key, THandler Handler)[] handlers = Volatile.Read(ref _handlers);
        if (handlers.Length == 0)
            return Task.CompletedTask;
        if (handlers.Length == 1)
            return RunHandlerAsync(handlers[0].Key, () => call(handlers[0].Handler));

        Task[] running = new Task[handlers.Length];
        for (int i = 0; i < handlers.Length; i++)
        {
            THandler handler = handlers[i].Handler;
            running[i] = RunHandlerAsync(handlers[i].Key, () => call(handler));
        }

        return Task.WhenAll(running);
    }

    internal async Task<TResult> WaitCoreAsync<TResult>(Func<TaskCompletionSource<TResult>, THandler> watch, TResult timedOut,
        TimeSpan? timeout, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Owner?.ReleaseEventLane();

        TaskCompletionSource<TResult> result = new TaskCompletionSource<TResult>(TaskCreationOptions.RunContinuationsAsynchronously);
        THandler handler = watch(result);
        Add(handler, handler);
        try
        {
            using CancellationTokenSource? timer = timeout == null ? null : new CancellationTokenSource(timeout.Value);
            using CancellationTokenRegistration expired = (timer?.Token ?? CancellationToken.None).Register(() => result.TrySetResult(timedOut));
            using CancellationTokenRegistration cancelled = cancellationToken.Register(() => result.TrySetCanceled(cancellationToken));
            return await result.Task.ConfigureAwait(false);
        }
        finally
        {
            Remove(handler);
        }
    }

    internal static TDelegate Bind<TDelegate>(Delegate single) where TDelegate : Delegate
    {
        try
        {
            return (TDelegate)Delegate.CreateDelegate(typeof(TDelegate), single.Target, single.Method);
        }
        catch (ArgumentException ex)
        {
            throw new ArgumentException($"A handler for this event must take the event's arguments and return void or Task, not {single.Method.ReturnType.Name}.", ex);
        }
    }

    internal static bool ReturnsTask(Delegate single) => typeof(Task).IsAssignableFrom(single.Method.ReturnType);
}

/// <inheritdoc cref="GatewayEventBase" />
public sealed class GatewayEvent : GatewayEventBase<Func<Task>>
{
    internal GatewayEvent()
    {
    }

    /// <summary>Subscribes a handler.</summary>
    public static GatewayEvent operator +(GatewayEvent e, Action handler) { e.Add(handler, () => { handler(); return Task.CompletedTask; }); return e; }

    /// <summary>Subscribes an async handler.</summary>
    public static GatewayEvent operator +(GatewayEvent e, Func<Task> handler) { e.Add(handler, handler); return e; }

    /// <summary>Subscribes a handler of the event's named delegate type.</summary>
    public static GatewayEvent operator +(GatewayEvent e, Delegate handler) { e.AddNamed(handler, Wrap); return e; }

    /// <summary>Unsubscribes a handler.</summary>
    public static GatewayEvent operator -(GatewayEvent e, Action handler) { e.Remove(handler); return e; }

    /// <summary>Unsubscribes an async handler.</summary>
    public static GatewayEvent operator -(GatewayEvent e, Func<Task> handler) { e.Remove(handler); return e; }

    /// <summary>Unsubscribes a handler of the event's named delegate type.</summary>
    public static GatewayEvent operator -(GatewayEvent e, Delegate handler) { e.RemoveNamed(handler); return e; }

    private static Func<Task> Wrap(Delegate single)
    {
        if (ReturnsTask(single))
            return Bind<Func<Task>>(single);

        Action action = Bind<Action>(single);
        return () => { action(); return Task.CompletedTask; };
    }

    /// <summary>
    /// Waits for the next time this event is raised. Later events of the calling handler's guild go ahead meanwhile.
    /// </summary>
    /// <param name="timeout">How long to wait. Null waits until <paramref name="cancellationToken"/> is cancelled.</param>
    /// <param name="cancellationToken">Cancels the wait.</param>
    /// <returns>True when the event was raised, false when the timeout passed first.</returns>
    public Task<bool> WaitAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
        => WaitCoreAsync<bool>(result => () => { result.TrySetResult(true); return Task.CompletedTask; }, false, timeout, cancellationToken);

    internal void Invoke() => Raise(h => h());
}

/// <inheritdoc cref="GatewayEventBase" />
public sealed class GatewayEvent<T1> : GatewayEventBase<Func<T1, Task>>
{
    internal GatewayEvent()
    {
    }

    /// <summary>Subscribes a handler.</summary>
    public static GatewayEvent<T1> operator +(GatewayEvent<T1> e, Action<T1> handler) { e.Add(handler, a => { handler(a); return Task.CompletedTask; }); return e; }

    /// <summary>Subscribes an async handler.</summary>
    public static GatewayEvent<T1> operator +(GatewayEvent<T1> e, Func<T1, Task> handler) { e.Add(handler, handler); return e; }

    /// <summary>Subscribes a handler of the event's named delegate type.</summary>
    public static GatewayEvent<T1> operator +(GatewayEvent<T1> e, Delegate handler) { e.AddNamed(handler, Wrap); return e; }

    /// <summary>Unsubscribes a handler.</summary>
    public static GatewayEvent<T1> operator -(GatewayEvent<T1> e, Action<T1> handler) { e.Remove(handler); return e; }

    /// <summary>Unsubscribes an async handler.</summary>
    public static GatewayEvent<T1> operator -(GatewayEvent<T1> e, Func<T1, Task> handler) { e.Remove(handler); return e; }

    /// <summary>Unsubscribes a handler of the event's named delegate type.</summary>
    public static GatewayEvent<T1> operator -(GatewayEvent<T1> e, Delegate handler) { e.RemoveNamed(handler); return e; }

    private static Func<T1, Task> Wrap(Delegate single)
    {
        if (ReturnsTask(single))
            return Bind<Func<T1, Task>>(single);

        Action<T1> action = Bind<Action<T1>>(single);
        return a => { action(a); return Task.CompletedTask; };
    }

    /// <summary>
    /// Waits for the next time this event is raised with arguments matching <paramref name="predicate"/>. Later events
    /// of the calling handler's guild go ahead meanwhile.
    /// </summary>
    /// <param name="predicate">Which arguments to wait for. Null takes the next ones.</param>
    /// <param name="timeout">How long to wait. Null waits until <paramref name="cancellationToken"/> is cancelled.</param>
    /// <param name="cancellationToken">Cancels the wait.</param>
    /// <returns>The argument, or the default value when the timeout passed first.</returns>
    public Task<T1?> WaitAsync(Func<T1, bool>? predicate = null, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
        => WaitCoreAsync<T1?>(result => a =>
        {
            if (predicate == null || predicate(a))
                result.TrySetResult(a);
            return Task.CompletedTask;
        }, default, timeout, cancellationToken);

    internal void Invoke(T1 a1) => Raise(h => h(a1));
}

/// <inheritdoc cref="GatewayEventBase" />
public sealed class GatewayEvent<T1, T2> : GatewayEventBase<Func<T1, T2, Task>>
{
    internal GatewayEvent()
    {
    }

    /// <summary>Subscribes a handler.</summary>
    public static GatewayEvent<T1, T2> operator +(GatewayEvent<T1, T2> e, Action<T1, T2> handler) { e.Add(handler, (a, b) => { handler(a, b); return Task.CompletedTask; }); return e; }

    /// <summary>Subscribes an async handler.</summary>
    public static GatewayEvent<T1, T2> operator +(GatewayEvent<T1, T2> e, Func<T1, T2, Task> handler) { e.Add(handler, handler); return e; }

    /// <summary>Subscribes a handler of the event's named delegate type.</summary>
    public static GatewayEvent<T1, T2> operator +(GatewayEvent<T1, T2> e, Delegate handler) { e.AddNamed(handler, Wrap); return e; }

    /// <summary>Unsubscribes a handler.</summary>
    public static GatewayEvent<T1, T2> operator -(GatewayEvent<T1, T2> e, Action<T1, T2> handler) { e.Remove(handler); return e; }

    /// <summary>Unsubscribes an async handler.</summary>
    public static GatewayEvent<T1, T2> operator -(GatewayEvent<T1, T2> e, Func<T1, T2, Task> handler) { e.Remove(handler); return e; }

    /// <summary>Unsubscribes a handler of the event's named delegate type.</summary>
    public static GatewayEvent<T1, T2> operator -(GatewayEvent<T1, T2> e, Delegate handler) { e.RemoveNamed(handler); return e; }

    private static Func<T1, T2, Task> Wrap(Delegate single)
    {
        if (ReturnsTask(single))
            return Bind<Func<T1, T2, Task>>(single);

        Action<T1, T2> action = Bind<Action<T1, T2>>(single);
        return (a, b) => { action(a, b); return Task.CompletedTask; };
    }

    /// <summary>
    /// Waits for the next time this event is raised with arguments matching <paramref name="predicate"/>. Later events
    /// of the calling handler's guild go ahead meanwhile.
    /// </summary>
    /// <param name="predicate">Which arguments to wait for. Null takes the next ones.</param>
    /// <param name="timeout">How long to wait. Null waits until <paramref name="cancellationToken"/> is cancelled.</param>
    /// <param name="cancellationToken">Cancels the wait.</param>
    /// <returns>The arguments, or null when the timeout passed first.</returns>
    public Task<(T1, T2)?> WaitAsync(Func<T1, T2, bool>? predicate = null, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
        => WaitCoreAsync<(T1, T2)?>(result => (a, b) =>
        {
            if (predicate == null || predicate(a, b))
                result.TrySetResult((a, b));
            return Task.CompletedTask;
        }, null, timeout, cancellationToken);

    internal void Invoke(T1 a1, T2 a2) => Raise(h => h(a1, a2));
}

/// <inheritdoc cref="GatewayEventBase" />
public sealed class GatewayEvent<T1, T2, T3> : GatewayEventBase<Func<T1, T2, T3, Task>>
{
    internal GatewayEvent()
    {
    }

    /// <summary>Subscribes a handler.</summary>
    public static GatewayEvent<T1, T2, T3> operator +(GatewayEvent<T1, T2, T3> e, Action<T1, T2, T3> handler) { e.Add(handler, (a, b, c) => { handler(a, b, c); return Task.CompletedTask; }); return e; }

    /// <summary>Subscribes an async handler.</summary>
    public static GatewayEvent<T1, T2, T3> operator +(GatewayEvent<T1, T2, T3> e, Func<T1, T2, T3, Task> handler) { e.Add(handler, handler); return e; }

    /// <summary>Subscribes a handler of the event's named delegate type.</summary>
    public static GatewayEvent<T1, T2, T3> operator +(GatewayEvent<T1, T2, T3> e, Delegate handler) { e.AddNamed(handler, Wrap); return e; }

    /// <summary>Unsubscribes a handler.</summary>
    public static GatewayEvent<T1, T2, T3> operator -(GatewayEvent<T1, T2, T3> e, Action<T1, T2, T3> handler) { e.Remove(handler); return e; }

    /// <summary>Unsubscribes an async handler.</summary>
    public static GatewayEvent<T1, T2, T3> operator -(GatewayEvent<T1, T2, T3> e, Func<T1, T2, T3, Task> handler) { e.Remove(handler); return e; }

    /// <summary>Unsubscribes a handler of the event's named delegate type.</summary>
    public static GatewayEvent<T1, T2, T3> operator -(GatewayEvent<T1, T2, T3> e, Delegate handler) { e.RemoveNamed(handler); return e; }

    private static Func<T1, T2, T3, Task> Wrap(Delegate single)
    {
        if (ReturnsTask(single))
            return Bind<Func<T1, T2, T3, Task>>(single);

        Action<T1, T2, T3> action = Bind<Action<T1, T2, T3>>(single);
        return (a, b, c) => { action(a, b, c); return Task.CompletedTask; };
    }

    /// <summary>
    /// Waits for the next time this event is raised with arguments matching <paramref name="predicate"/>. Later events
    /// of the calling handler's guild go ahead meanwhile.
    /// </summary>
    /// <param name="predicate">Which arguments to wait for. Null takes the next ones.</param>
    /// <param name="timeout">How long to wait. Null waits until <paramref name="cancellationToken"/> is cancelled.</param>
    /// <param name="cancellationToken">Cancels the wait.</param>
    /// <returns>The arguments, or null when the timeout passed first.</returns>
    public Task<(T1, T2, T3)?> WaitAsync(Func<T1, T2, T3, bool>? predicate = null, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
        => WaitCoreAsync<(T1, T2, T3)?>(result => (a, b, c) =>
        {
            if (predicate == null || predicate(a, b, c))
                result.TrySetResult((a, b, c));
            return Task.CompletedTask;
        }, null, timeout, cancellationToken);

    internal void Invoke(T1 a1, T2 a2, T3 a3) => Raise(h => h(a1, a2, a3));
}

/// <inheritdoc cref="GatewayEventBase" />
public sealed class GatewayEvent<T1, T2, T3, T4> : GatewayEventBase<Func<T1, T2, T3, T4, Task>>
{
    internal GatewayEvent()
    {
    }

    /// <summary>Subscribes a handler.</summary>
    public static GatewayEvent<T1, T2, T3, T4> operator +(GatewayEvent<T1, T2, T3, T4> e, Action<T1, T2, T3, T4> handler) { e.Add(handler, (a, b, c, d) => { handler(a, b, c, d); return Task.CompletedTask; }); return e; }

    /// <summary>Subscribes an async handler.</summary>
    public static GatewayEvent<T1, T2, T3, T4> operator +(GatewayEvent<T1, T2, T3, T4> e, Func<T1, T2, T3, T4, Task> handler) { e.Add(handler, handler); return e; }

    /// <summary>Subscribes a handler of the event's named delegate type.</summary>
    public static GatewayEvent<T1, T2, T3, T4> operator +(GatewayEvent<T1, T2, T3, T4> e, Delegate handler) { e.AddNamed(handler, Wrap); return e; }

    /// <summary>Unsubscribes a handler.</summary>
    public static GatewayEvent<T1, T2, T3, T4> operator -(GatewayEvent<T1, T2, T3, T4> e, Action<T1, T2, T3, T4> handler) { e.Remove(handler); return e; }

    /// <summary>Unsubscribes an async handler.</summary>
    public static GatewayEvent<T1, T2, T3, T4> operator -(GatewayEvent<T1, T2, T3, T4> e, Func<T1, T2, T3, T4, Task> handler) { e.Remove(handler); return e; }

    /// <summary>Unsubscribes a handler of the event's named delegate type.</summary>
    public static GatewayEvent<T1, T2, T3, T4> operator -(GatewayEvent<T1, T2, T3, T4> e, Delegate handler) { e.RemoveNamed(handler); return e; }

    private static Func<T1, T2, T3, T4, Task> Wrap(Delegate single)
    {
        if (ReturnsTask(single))
            return Bind<Func<T1, T2, T3, T4, Task>>(single);

        Action<T1, T2, T3, T4> action = Bind<Action<T1, T2, T3, T4>>(single);
        return (a, b, c, d) => { action(a, b, c, d); return Task.CompletedTask; };
    }

    /// <summary>
    /// Waits for the next time this event is raised with arguments matching <paramref name="predicate"/>. Later events
    /// of the calling handler's guild go ahead meanwhile.
    /// </summary>
    /// <param name="predicate">Which arguments to wait for. Null takes the next ones.</param>
    /// <param name="timeout">How long to wait. Null waits until <paramref name="cancellationToken"/> is cancelled.</param>
    /// <param name="cancellationToken">Cancels the wait.</param>
    /// <returns>The arguments, or null when the timeout passed first.</returns>
    public Task<(T1, T2, T3, T4)?> WaitAsync(Func<T1, T2, T3, T4, bool>? predicate = null, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
        => WaitCoreAsync<(T1, T2, T3, T4)?>(result => (a, b, c, d) =>
        {
            if (predicate == null || predicate(a, b, c, d))
                result.TrySetResult((a, b, c, d));
            return Task.CompletedTask;
        }, null, timeout, cancellationToken);

    internal void Invoke(T1 a1, T2 a2, T3 a3, T4 a4) => Raise(h => h(a1, a2, a3, a4));
}
