namespace Fluxer.Net;

public interface IGuildChannelOverride
{
    bool Collapsed { get; }

    int? MessageNotifications { get; }

    bool Muted { get; }

    IMuteConfiguration? MuteConfig { get; }
}
