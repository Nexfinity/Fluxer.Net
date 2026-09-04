namespace Fluxer.Net;

/// <inheritdoc />
public class GuildMember : Entity, IGuildMember
{
    /// <inheritdoc />
    public ulong Id => User.Id;

    /// <inheritdoc />
    public DateTimeOffset CreatedAt => User.CreatedAt;

    /// <inheritdoc />
    public string Mention => $"<@{Id}>";

    /// <inheritdoc />
    public ulong GuildId { get; private set; }

    /// <inheritdoc />
    public User User { get; private set; }

    /// <inheritdoc />
    public DateTimeOffset JoinedAt { get; private set; }

    /// <inheritdoc />
    public string? Nickname { get; private set; }

    /// <inheritdoc />
    public string? AvatarHash { get; private set; }

    /// <inheritdoc />
    public string? BannerHash { get; private set; }

    /// <inheritdoc />
    public int? AccentColor { get; private set; }

    /// <inheritdoc />
    public bool IsDeaf { get; private set; }

    /// <inheritdoc />
    public bool IsMute { get; private set; }

    /// <inheritdoc />
    public DateTimeOffset? CommunicationDisabledUntil { get; private set; }

    /// <inheritdoc />
    public HashSet<ulong> RoleIds { get; private set; }

    /// <inheritdoc />
    public string GetCurrentName()
    {
        return Nickname ?? User.DisplayName ?? User.Username;
    }

    /// <inheritdoc />
    public string GetDefaultAvatarUrl()
    {
        return $"{Client.Config.StaticUrl}/avatars/{Id % 6}.png";
    }

    /// <inheritdoc />
    public string? GetAvatarUrl(int size = 160)
    {
        if (string.IsNullOrEmpty(AvatarHash))
            return User.GetAvatarUrl();

        return $"{Client.Config.MediaUrl}/avatars/{Id}/{AvatarHash}.png?size={size}";
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

        return $"{Client.Config.MediaUrl}/guilds/{GuildId}/users/{Id}/banners/{BannerHash}.webp?size={size}";
    }

    IUser IGuildMember.User => User;

    internal GuildMember(FluxerBaseClient client) : base(client)
    {

    }

    /// <summary>
    /// Create a GuildMember object from json.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="json"></param>
    /// <returns></returns>
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
