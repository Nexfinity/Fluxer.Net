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
    public int? AccentColor { get; internal set; }

    /// <inheritdoc />
    public bool IsDeaf { get; internal set; }

    /// <inheritdoc />
    public bool IsMute { get; internal set; }

    /// <inheritdoc />
    public DateTime? CommunicationDisabledUntil { get; internal set; }

    /// <inheritdoc />
    public HashSet<ulong> RoleIds { get; internal set; }

    /// <inheritdoc />
    public string GetCurrentName()
    {
        return Nickname ?? User.DisplayName ?? User.Username;
    }

    /// <inheritdoc />
    public string GetDefaultAvatarUrl()
    {
        return $"https://fluxerstatic.com/avatars/{UserId % 6}.png";
    }

    /// <inheritdoc />
    public string? GetAvatarUrl(int size = 160)
    {
        if (string.IsNullOrEmpty(AvatarHash))
            return User.GetAvatarUrl();

        return $"{Client.Config.MediaUrl}/avatars/{UserId}/{AvatarHash}.png?size={size}";
    }

    /// <inheritdoc />
    public string GetAvatarOrDefaultUrl(int size = 160)
    {
        if (string.IsNullOrEmpty(AvatarHash) && string.IsNullOrEmpty(User.AvatarHash))
            return GetDefaultAvatarUrl();

        return GetAvatarUrl(size);
    }

    /// <inheritdoc />
    public string? GetBannerUrl(int size = 1024)
    {
        if (string.IsNullOrEmpty(BannerHash))
            return null;

        return $"https://fluxerusercontent.com/guilds/{GuildId}/users/{UserId}/banners/{BannerHash}.webp?size={size}";
    }

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

    internal virtual void Update(FluxerBaseClient client, GuildMemberJson json)
    {
        GuildId = json.GuildId;
        User = User.Create(client, json.User);
        JoinedAt = json.JoinedAt;
        Nickname = json.Nickname;
        AvatarHash = json.AvatarHash;
        BannerHash = json.BannerHash;
        AccentColor = json.AccentColor;
        IsDeaf = json.IsDeaf;
        IsMute = json.IsMute;
        CommunicationDisabledUntil = json.CommunicationDisabledUntil;
        RoleIds = json.RoleIds != null ? json.RoleIds : new HashSet<ulong>();
    }
}
