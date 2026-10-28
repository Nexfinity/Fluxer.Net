namespace Fluxer.Net;

/// <inheritdoc />
public class User : Entity, IUser
{
    /// <inheritdoc />
    public ulong Id { get; internal set; }

    /// <inheritdoc />
    public DateTimeOffset CreatedAt { get; internal set; }

    /// <inheritdoc />
    public string Mention => $"<@{Id}>";

    /// <inheritdoc />
    public string Username { get; internal set; }

    /// <inheritdoc />
    public string Discriminator { get; internal set; }

    /// <inheritdoc />
    public string DisplayName { get; internal set; }

    /// <inheritdoc />
    public string? AvatarHash { get; internal set; }

    /// <inheritdoc />
    public int? AvatarColor { get; private set; }

    /// <inheritdoc />
    public UserFlags Flags { get; internal set; }

    /// <inheritdoc />
    public bool IsBot { get; private set; }

    /// <inheritdoc />
    public bool IsSystem { get; private set; }

    /// <inheritdoc />
    public MentionPreference MentionPreference { get; private set; }

    /// <inheritdoc />
    public string GetCurrentName()
    {
        return DisplayName ?? Username;
    }

    /// <summary>
    /// Returns the username, followed by the discriminator when the account still has one.
    /// </summary>
    public override string ToString() =>
        string.IsNullOrEmpty(Discriminator) || Discriminator.Trim('0').Length == 0 ? Username : $"{Username}#{Discriminator}";

    /// <inheritdoc />
    public string GetDefaultAvatarUrl()
    {
        return $"{Client.Config.StaticUrl}/avatars/{Id % 6}.png";
    }

    /// <inheritdoc />
    public string? GetAvatarUrl(int size = 160)
    {
        if (string.IsNullOrEmpty(AvatarHash))
            return null;

        return $"{Client.Config.MediaUrl}/avatars/{Id}/{AvatarHash}.png?size={size}";
    }

    /// <inheritdoc />
    public string GetAvatarOrDefaultUrl(int size = 160)
    {
        if (string.IsNullOrEmpty(AvatarHash))
            return GetDefaultAvatarUrl();

        return GetAvatarUrl(size);
    }

    internal User(FluxerBaseClient client) : base(client)
    {

    }

    /// <summary>
    /// Create a User object from json.
    /// </summary>
    /// <returns><see cref="User"/></returns>
    public static User Create(FluxerBaseClient client, UserJson json)
    {
        User data = new User(client);
        data.Update(json);
        return data;
    }

    internal void Update(UserJson json)
    {
        Id = json.Id;
        CreatedAt = SnowflakeUtils.FromSnowflake(Id);
        Username = json.Username;
        Discriminator = json.Discriminator;
        DisplayName = json.DisplayName;
        AvatarHash = json.AvatarHash;
        AvatarColor = json.AvatarColor;
        Flags = json.Flags;
        IsBot = json.IsBot;
        IsSystem = json.IsSystem;
        MentionPreference = json.MentionPreference;
    }

    internal UserJson ToJson() => new()
    {
        Id = Id,
        Username = Username,
        Discriminator = Discriminator,
        DisplayName = DisplayName,
        AvatarHash = AvatarHash,
        AvatarColor = AvatarColor,
        Flags = Flags,
        IsBot = IsBot,
        IsSystem = IsSystem,
        MentionPreference = MentionPreference
    };
}
