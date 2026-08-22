namespace Fluxer.Net;

/// <inheritdoc />
public class Guild : PartialGuild, IGuild
{
    /// <inheritdoc />
    public ulong OwnerId { get; internal set; }

    /// <inheritdoc />
    public string? VanityUrlCode { get; internal set; }

    /// <inheritdoc />
    public GuildVerificationLevel VerificationLevel { get; internal set; }

    /// <inheritdoc />
    public GuildMfaLevel MfaLevel { get; internal set; }

    /// <inheritdoc />
    public GuildNsfwLevel NsfwLevel { get; internal set; }

    /// <inheritdoc />
    public GuildContentFilter ExplicitContentFilter { get; internal set; }

    /// <inheritdoc />
    public GuildDefaultNotifications DefaultMessageNotifications { get; internal set; }

    /// <inheritdoc />
    public ulong? SystemChannelId { get; internal set; }

    /// <inheritdoc />
    public SystemChannelFlags SystemChannelFlags { get; internal set; }

    /// <inheritdoc />
    public ulong? RulesChannelId { get; internal set; }

    /// <inheritdoc />
    public ulong? AfkChannelId { get; internal set; }

    /// <inheritdoc />
    public int AfkTimeout { get; internal set; }

    /// <inheritdoc />
    public GuildOperations DisabledOperations { get; internal set; }

    /// <inheritdoc />
    public DateTimeOffset? MessageHistoryCutoff { get; internal set; }

    /// <inheritdoc />
    public bool IsNsfw { get; internal set; }

    /// <inheritdoc />
    public string ContentWarningText { get; internal set; }

    /// <inheritdoc />
    public int? OnlineCount { get; set; }

    /// <inheritdoc />
    public int? MemberCount { get; set; }

    internal Guild(FluxerBaseClient client) : base(client)
    {

    }

    /// <summary>
    /// Create a Guild object from json.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="json"></param>
    /// <returns></returns>
    public static Guild Create(FluxerBaseClient client, GuildJson json)
    {
        Guild data = new Guild(client);
        data.Update(client, json);
        return data;
    }

    internal virtual void Update(FluxerBaseClient client, GuildJson json)
    {
        base.Update(client, json);
        OwnerId = json.OwnerId;
        VanityUrlCode = json.VanityUrlCode;
        VerificationLevel = json.VerificationLevel;
        MfaLevel = json.MfaLevel;
        NsfwLevel = json.NsfwLevel;
        ExplicitContentFilter = json.ExplicitContentFilter;
        DefaultMessageNotifications = json.DefaultMessageNotifications;
        SystemChannelId = json.SystemChannelId;
        SystemChannelFlags = json.SystemChannelFlags;
        RulesChannelId = json.RulesChannelId;
        AfkChannelId = json.AfkChannelId;
        AfkTimeout = json.AfkTimeout;
        DisabledOperations = json.DisabledOperations;
        MessageHistoryCutoff = json.MessageHistoryCutoff;
        IsNsfw = json.IsNsfw;
        ContentWarningText = json.ContentWarningText;
        OnlineCount = json.OnlineCount;
        MemberCount = json.MemberCount;
    }
}
