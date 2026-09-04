namespace Fluxer.Net;

/// <inheritdoc />
public class GuildChannelOverride : Entity, IGuildChannelOverride
{
    /// <inheritdoc />
    public bool Collapsed { get; set; }

    /// <inheritdoc />
    public int? MessageNotifications { get; set; }

    /// <inheritdoc />
    public bool Muted { get; set; }

    /// <inheritdoc />
    public MuteConfiguration? MuteConfig { get; set; }

    IMuteConfiguration? IGuildChannelOverride.MuteConfig => MuteConfig;

    internal GuildChannelOverride(FluxerBaseClient client) : base(client)
    {

    }

    /// <summary>
    /// Create a GuildChannelOverride object from json.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="json"></param>
    /// <returns></returns>
    public static GuildChannelOverride Create(FluxerBaseClient client, GuildChannelOverrideJson json)
    {
        GuildChannelOverride data = new GuildChannelOverride(client);
        data.Update(json);
        return data;
    }

    internal void Update(GuildChannelOverrideJson json)
    {
        Collapsed = json.Collapsed;
        MessageNotifications = json.MessageNotifications;
        Muted = json.Muted;
        MuteConfig = MuteConfiguration.Create(Client, json.MuteConfig);
    }
}