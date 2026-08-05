namespace Fluxer.Net;

/// <inheritdoc />
public class User : Entity, IUser
{
    /// <inheritdoc />
    public ulong Id { get; internal set; }

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
    public int? AvatarColor { get; internal set; }

    /// <inheritdoc />
    public UserFlags Flags { get; internal set; }

    /// <inheritdoc />
    public bool IsBot { get; internal set; }

    /// <inheritdoc />
    public bool IsSystem { get; internal set; }

    /// <inheritdoc />
    public string GetCurrentName()
    {
        return DisplayName ?? Username;
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
    /// <param name="client"></param>
    /// <param name="json"></param>
    /// <returns></returns>
    public static User Create(FluxerBaseClient client, UserJson json)
    {
        User data = new User(client);
        data.Update(json);
        return data;
    }

    internal void Update(UserJson json)
    {
        Id = json.Id;
        Username = json.Username;
        Discriminator = json.Discriminator;
        DisplayName = json.DisplayName;
        AvatarHash = json.AvatarHash;
        AvatarColor = json.AvatarColor;
        Flags = json.Flags;
        IsBot = json.IsBot;
        IsSystem = json.IsSystem;
    }
}
