namespace Fluxer.Net.Gateway;

/// <summary>
/// How gateway events are handed to your handlers.
/// </summary>
public enum EventDispatchMode
{
    /// <summary>
    /// Events of one guild, or of one DM or group channel, reach your handlers one at a time and in the order they
    /// arrived. Events of different guilds are handled at the same time.
    /// </summary>
    OrderedPerGuild,

    /// <summary>
    /// Every event reaches your handlers one at a time, in the order it arrived.
    /// </summary>
    Sequential,

    /// <summary>
    /// Every event reaches your handlers as soon as it arrives, without waiting for earlier ones.
    /// </summary>
    Concurrent
}

/// <summary>
/// Settings for how gateway events reach your handlers. They take effect on the first connect.
/// </summary>
public class FluxerEventConfig
{
    /// <summary>
    /// How events are ordered against each other. Defaults to <see cref="EventDispatchMode.OrderedPerGuild"/>.
    /// </summary>
    public EventDispatchMode Mode { get; set; } = EventDispatchMode.OrderedPerGuild;

    /// <summary>
    /// With <see cref="EventDispatchMode.OrderedPerGuild"/>, how many guilds can have handlers running at the same time.
    /// Guilds are spread over this many lanes, and guilds sharing a lane wait for each other. Defaults to 32.
    /// </summary>
    public int Lanes { get; set; } = 32;

    /// <summary>
    /// How many events may wait for handlers before the gateway pauses handling new ones. Receiving, heartbeats and
    /// reconnects carry on while it waits. Defaults to 10,000.
    /// </summary>
    public int MaxQueuedEvents { get; set; } = 10_000;

    /// <summary>
    /// How long later events of a guild wait for the handlers of an earlier one. Past this, they go ahead while those
    /// handlers keep running, and order is only kept for handlers that finish in time. Null keeps strict order, with
    /// later events waiting as long as it takes. Defaults to 2 seconds.
    /// </summary>
    public TimeSpan? LaneHoldLimit { get; set; } = TimeSpan.FromSeconds(2);

    /// <summary>
    /// Logs a warning when a handler runs longer than this. Handlers that wait with <c>WaitAsync</c>, the
    /// <c>WaitFor</c> helpers or <see cref="FluxerGatewayClient.ReleaseEventLane"/> are left out. Null turns the warning
    /// off. Defaults to 3 seconds.
    /// </summary>
    public TimeSpan? SlowHandlerWarning { get; set; } = TimeSpan.FromSeconds(3);

    internal void Validate()
    {
        if (Lanes < 1)
            throw new ArgumentOutOfRangeException(nameof(Lanes), "Lanes must be at least 1.");
        if (MaxQueuedEvents < 1)
            throw new ArgumentOutOfRangeException(nameof(MaxQueuedEvents), "MaxQueuedEvents must be at least 1.");
        if (LaneHoldLimit <= TimeSpan.Zero)
            throw new ArgumentOutOfRangeException(nameof(LaneHoldLimit), "LaneHoldLimit must be longer than zero.");
        if (SlowHandlerWarning <= TimeSpan.Zero)
            throw new ArgumentOutOfRangeException(nameof(SlowHandlerWarning), "SlowHandlerWarning must be longer than zero.");
    }
}
