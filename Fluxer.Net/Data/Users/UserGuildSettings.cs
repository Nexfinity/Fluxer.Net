namespace Fluxer.Net;

public class UserGuildSettings : Entity, IUserGuildSettings
{
    /// <inheritdoc />
    public Dictionary<ulong, GuildChannelOverride>? ChannelOverrides { get; private set; }

    /// <inheritdoc />
    public ulong GuildId { get; private set; }

    /// <inheritdoc />
    public bool HideMutedChannels { get; private set; }

    /// <inheritdoc />
    public NotificationType MessageNotifications { get; private set; }

    /// <inheritdoc />
    public bool MobilePush { get; private set; }

    /// <inheritdoc />
    public MuteConfiguration? MuteConfig { get; private set; }

    /// <inheritdoc />
    public bool Muted { get; private set; }

    /// <inheritdoc />
    public bool SuppressEveryone { get; private set; }

    /// <inheritdoc />
    public bool SuppressRoles { get; private set; }

    /// <inheritdoc />
    public int Version { get; private set; }

    IDictionary<ulong, IGuildChannelOverride>? IUserGuildSettings.ChannelOverrides => (IDictionary<ulong, IGuildChannelOverride>?)ChannelOverrides;

    IMuteConfiguration? IUserGuildSettings.MuteConfig => MuteConfig;

    internal UserGuildSettings(FluxerBaseClient client) : base(client)
    {

    }

    /// <summary>
    /// Create a UserGuildSettings object from json.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="json"></param>
    /// <returns></returns>
    public static UserGuildSettings Create(FluxerBaseClient client, UserGuildSettingsJson json)
    {
        UserGuildSettings data = new UserGuildSettings(client);
        data.Update(json);
        return data;
    }

    internal void Update(UserGuildSettingsJson json)
    {
        ChannelOverrides = json.ChannelOverrides.ToDictionary(x => x.Key, x => GuildChannelOverride.Create(Client, x.Value));
        GuildId = json.GuildId;
        HideMutedChannels = json.HideMutedChannels;
        MessageNotifications = json.MessageNotifications;
        MobilePush = json.MobilePush;
        MuteConfig = MuteConfiguration.Create(Client, json.MuteConfig);
        Muted = json.Muted;
        SuppressEveryone = json.SuppressEveryone;
        SuppressRoles = json.SuppressRoles;
        Version = json.Version;
    }
}
