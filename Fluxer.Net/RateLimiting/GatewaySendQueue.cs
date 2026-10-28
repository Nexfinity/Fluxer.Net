using System.Collections.Concurrent;
using Fluxer.Net.Gateway;
using Serilog;

namespace Fluxer.Net.RateLimiting;

internal sealed class GatewaySendQueue : IDisposable
{
    private const int PayloadLimit = 600;
    private const int ReservedPayloads = 10;
    private static readonly TimeSpan PayloadWindow = TimeSpan.FromSeconds(60);
    private static readonly TimeSpan HandshakePoll = TimeSpan.FromMilliseconds(100);

    private readonly RollingWindow _payloads = new(PayloadLimit - ReservedPayloads, PayloadWindow);
    private readonly RollingWindow _presence = new(5, TimeSpan.FromSeconds(20));
    private readonly RollingWindow _memberRequests = new(12, TimeSpan.FromSeconds(10));
    private readonly ConcurrentQueue<(string Text, FluxerOpCode? OpCode)> _priority = new();
    private readonly ConcurrentQueue<(string Text, FluxerOpCode? OpCode)> _normal = new();
    private readonly SemaphoreSlim _signal = new(0);
    private readonly CancellationTokenSource _stop = new();
    private readonly Action<string> _send;
    private readonly ILogger _logger;
    private readonly Task _loop;
    private volatile bool _awaitingHandshake = true;
    private volatile bool _disposed;

    public GatewaySendQueue(Action<string> send, ILogger logger)
    {
        _send = send;
        _logger = logger;
        _loop = Task.Run(RunAsync);
    }

    public void Enqueue(string text, FluxerOpCode? opCode)
    {
        if (_disposed)
            return;

        if (opCode is FluxerOpCode.Heartbeat or FluxerOpCode.Identify or FluxerOpCode.Resume)
            _priority.Enqueue((text, opCode));
        else
            _normal.Enqueue((text, opCode));

        try
        {
            _signal.Release();
        }
        catch (ObjectDisposedException)
        {
        }
    }

    public void ResetConnection()
    {
        _awaitingHandshake = true;
        _payloads.Clear();
        _presence.Clear();
    }

    public void Dispose()
    {
        _disposed = true;
        _stop.Cancel();
        try
        {
            _loop.Wait(TimeSpan.FromSeconds(2));
        }
        catch (AggregateException)
        {
        }

        _stop.Dispose();
        _signal.Dispose();
    }

    private async Task RunAsync()
    {
        CancellationToken token = _stop.Token;
        (string Text, FluxerOpCode? OpCode)? pending = null;
        bool announced = false;
        try
        {
            while (!token.IsCancellationRequested)
            {
                if (_priority.TryDequeue(out (string Text, FluxerOpCode? OpCode) urgent))
                {
                    Send(urgent.Text, urgent.OpCode);
                    if (urgent.OpCode is FluxerOpCode.Identify or FluxerOpCode.Resume)
                        _awaitingHandshake = false;

                    continue;
                }

                if (pending is null)
                {
                    if (!_normal.TryDequeue(out (string Text, FluxerOpCode? OpCode) next))
                    {
                        await _signal.WaitAsync(token).ConfigureAwait(false);
                        continue;
                    }

                    pending = next;
                }

                if (_awaitingHandshake)
                {
                    await Task.Delay(HandshakePoll, token).ConfigureAwait(false);
                    continue;
                }

                TimeSpan wait = WaitFor(pending.Value.OpCode);
                if (wait > TimeSpan.Zero)
                {
                    if (!announced)
                    {
                        _logger.Debug("Holding gateway {OpCode} for {Wait} ms to stay inside the gateway rate limits",
                            pending.Value.OpCode, (int)wait.TotalMilliseconds);
                        announced = true;
                    }

                    await _signal.WaitAsync(wait, token).ConfigureAwait(false);
                    continue;
                }

                Send(pending.Value.Text, pending.Value.OpCode);
                pending = null;
                announced = false;
            }
        }
        catch (OperationCanceledException)
        {
        }
    }

    private TimeSpan WaitFor(FluxerOpCode? opCode)
    {
        TimeSpan wait = _payloads.TimeUntilFree();
        TimeSpan own = opCode switch
        {
            FluxerOpCode.PresenceUpdate => _presence.TimeUntilFree(),
            FluxerOpCode.RequestGuildMembers => _memberRequests.TimeUntilFree(),
            _ => TimeSpan.Zero
        };
        return own > wait ? own : wait;
    }

    private void Send(string text, FluxerOpCode? opCode)
    {
        _payloads.Record();
        switch (opCode)
        {
            case FluxerOpCode.PresenceUpdate:
                _presence.Record();
                break;
            case FluxerOpCode.RequestGuildMembers:
                _memberRequests.Record();
                break;
        }

        _send(text);
    }
}
