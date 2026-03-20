namespace Fluxer.Net;

/// <inheritdoc />
public class Guild : PartialGuild, IGuild
{
    /// <inheritdoc />
    public ulong OwnerId { get; internal set; }

    /// <inheritdoc />
    public string? VanityUrlCode { get; internal set; }

    /// <inheritdoc />
    public int VerificationLevel { get; internal set; }

    /// <inheritdoc />
    public int MfaLevel { get; internal set; }

    /// <inheritdoc />
    public int NsfwLevel { get; internal set; }

    /// <inheritdoc />
    public int ExplicitContentFilter { get; internal set; }

    /// <inheritdoc />
    public int DefaultMessageNotifications { get; internal set; }

    /// <inheritdoc />
    public ulong? SystemChannelId { get; internal set; }

    /// <inheritdoc />
    public int SystemChannelFlags { get; internal set; }

    /// <inheritdoc />
    public ulong? RulesChannelId { get; internal set; }

    /// <inheritdoc />
    public ulong? AfkChannelId { get; internal set; }

    /// <inheritdoc />
    public int AfkTimeout { get; internal set; }

    /// <inheritdoc />
    public int DisabledOperations { get; internal set; }

    /// <inheritdoc />
    public int? MaxPresences { get; internal set; }

    /// <inheritdoc />
    public int MemberCount { get; internal set; }

    /// <inheritdoc />
    public DateTime? AuditLogsIndexedAt { get; internal set; }

    /// <inheritdoc />
    public DateTime? MessageHistoryCutoff { get; internal set; }

    internal Guild(BaseClient client) : base(client)
    {

    }

    public static Guild Create(BaseClient client, GuildJson json)
    {
        var data = new Guild(client);
        data.Update(client, json);
        return data;
    }

    internal void Update(BaseClient client, GuildJson json)
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
        MaxPresences = json.MaxPresences;
        MemberCount = json.MemberCount;
        AuditLogsIndexedAt = json.AuditLogsIndexedAt;
        MessageHistoryCutoff = json.MessageHistoryCutoff;
    }
}
