namespace Fluxer.Net.Gateway;

public enum FluxerOpCode
{
    Dispatch = 0,
    Heartbeat = 1,
    Identify = 2,
    PresenceUpdate = 3,
    Resume = 4,
    Reconnect = 5,
    InvalidSession = 6, // ?????
    Hello = 7,
    HeartbeatAck = 8
}
