namespace Fluxer.Net;

public interface IUserGuildSettings
{
    IDictionary<ulong, IGuildChannelOverride>? ChannelOverrides { get; }

    ulong GuildId { get; }

    bool HideMutedChannels { get; }

    NotificationType MessageNotifications { get; }

    bool MobilePush { get; }

    IMuteConfiguration? MuteConfig { get; }

    bool Muted { get; }

    bool SuppressEveryone { get; }

    bool SuppressRoles { get; }

    int Version { get; }
}
