namespace Fluxer.Net;

public interface IUserGuildSettings
{
    Dictionary<ulong, GuildChannelOverrideJson>? ChannelOverrides { get; }

    ulong GuildId { get; }

    bool HideMutedChannels { get; }

    NotificationType MessageNotifications { get; }

    bool MobilePush { get; }

    MuteConfigurationJson? MuteConfig { get; }

    bool Muted { get; }

    bool SuppressEveryone { get; }

    bool SuppressRoles { get; }

    int Version { get; }
}
