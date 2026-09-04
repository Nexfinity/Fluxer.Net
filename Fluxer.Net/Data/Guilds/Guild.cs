namespace Fluxer.Net;

/// <inheritdoc />
public class Guild : PartialGuild, IGuild
{
    /// <inheritdoc />
    public ulong OwnerId { get; private set; }

    /// <inheritdoc />
    public string? VanityUrlCode { get; private set; }

    /// <inheritdoc />
    public GuildVerificationLevel VerificationLevel { get; private set; }

    /// <inheritdoc />
    public GuildMfaLevel MfaLevel { get; private set; }

    /// <inheritdoc />
    public GuildNsfwLevel NsfwLevel { get; private set; }

    /// <inheritdoc />
    public GuildContentFilter ExplicitContentFilter { get; private set; }

    /// <inheritdoc />
    public GuildDefaultNotifications DefaultMessageNotifications { get; private set; }

    /// <inheritdoc />
    public ulong? SystemChannelId { get; private set; }

    /// <inheritdoc />
    public SystemChannelFlags SystemChannelFlags { get; private set; }

    /// <inheritdoc />
    public ulong? RulesChannelId { get; private set; }

    /// <inheritdoc />
    public ulong? AfkChannelId { get; private set; }

    /// <inheritdoc />
    public int AfkTimeout { get; private set; }

    /// <inheritdoc />
    public GuildOperations DisabledOperations { get; private set; }

    /// <inheritdoc />
    public DateTimeOffset? MessageHistoryCutoff { get; private set; }

    /// <inheritdoc />
    public bool IsNsfw { get; private set; }

    /// <inheritdoc />
    public string ContentWarningText { get; private set; }

    /// <inheritdoc />
    public int? OnlineCount { get; internal set; }

    /// <inheritdoc />
    public int? MemberCount { get; internal set; }

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
        data.Update(json);
        return data;
    }

    internal virtual void Update(GuildJson json)
    {
        base.Update(json);
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
