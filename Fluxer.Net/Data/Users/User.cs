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
    public string GetDefaultAvatarUrl()
    {
        return $"https://fluxerstatic.com/avatars/{Id % 6}.png";
    }

    internal User(FluxerBaseClient client) : base(client)
    {

    }

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
