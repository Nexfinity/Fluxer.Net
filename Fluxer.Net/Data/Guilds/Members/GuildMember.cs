namespace Fluxer.Net;

/// <inheritdoc />
public class GuildMember : Entity, IGuildMember
{
    /// <inheritdoc />
    public ulong UserId => User.Id;

    /// <inheritdoc />
    public string Mention => $"<@{UserId}>";

    /// <inheritdoc />
    public ulong GuildId { get; internal set; }

    /// <inheritdoc />
    public User User { get; internal set; }

    /// <inheritdoc />
    public DateTime JoinedAt { get; internal set; }

    /// <inheritdoc />
    public string? Nickname { get; internal set; }

    /// <inheritdoc />
    public string? AvatarHash { get; internal set; }

    /// <inheritdoc />
    public string? BannerHash { get; internal set; }

    /// <inheritdoc />
    public string? Bio { get; internal set; }

    /// <inheritdoc />
    public string? Pronouns { get; internal set; }

    /// <inheritdoc />
    public int? AccentColor { get; internal set; }

    /// <inheritdoc />
    public JoinSource? JoinSourceType { get; internal set; }

    /// <inheritdoc />
    public string? SourceInviteCode { get; internal set; }

    /// <inheritdoc />
    public ulong? InviterId { get; internal set; }

    /// <inheritdoc />
    public bool IsDeaf { get; internal set; }

    /// <inheritdoc />
    public bool IsMute { get; internal set; }

    /// <inheritdoc />
    public DateTime? CommunicationDisabledUntil { get; internal set; }

    /// <inheritdoc />
    public HashSet<ulong>? RoleIds { get; internal set; }

    /// <inheritdoc />
    public bool IsPremiumSanitized { get; internal set; }

    /// <inheritdoc />
    public bool IsTemporary { get; internal set; }

    IUser IGuildMember.User => User;

    internal GuildMember(FluxerBaseClient client) : base(client)
    {

    }

    public static GuildMember Create(FluxerBaseClient client, GuildMemberJson json)
    {
        GuildMember data = new GuildMember(client);
        data.Update(client, json);
        return data;
    }

    internal void Update(FluxerBaseClient client, GuildMemberJson json)
    {
        GuildId = json.GuildId;
        User = User.Create(client, json.User);
        JoinedAt = json.JoinedAt;
        Nickname = json.Nickname;
        AvatarHash = json.AvatarHash;
        BannerHash = json.BannerHash;
        Bio = json.Bio;
        Pronouns = json.Pronouns;
        AccentColor = json.AccentColor;
        JoinSourceType = json.JoinSourceType;
        SourceInviteCode = json.SourceInviteCode;
        InviterId = json.InviterId;
        IsDeaf = json.IsDeaf;
        IsMute = json.IsMute;
        CommunicationDisabledUntil = json.CommunicationDisabledUntil;
        RoleIds = json.RoleIds;
        IsPremiumSanitized = json.IsPremiumSanitized;
        IsTemporary = json.IsTemporary;
    }
}
