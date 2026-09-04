namespace Fluxer.Net;

public interface IGatewaySession
{
    string SessionId { get; }

    string Status { get; }

    bool IsMobile { get; }

    bool IsAfk { get; }
}
