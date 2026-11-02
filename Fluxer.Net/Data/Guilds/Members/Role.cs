namespace Fluxer.Net;

/// <inheritdoc />
public class Role : Entity, IRole
{
    /// <inheritdoc />
    public ulong GuildId { get; internal set; }

    /// <inheritdoc />
    public ulong Id { get; private set; }

    /// <inheritdoc />
    public DateTimeOffset CreatedAt => SnowflakeUtils.FromSnowflake(Id);

    /// <inheritdoc />
    public string Mention => $"<@&{Id}>";

    /// <summary>
    /// Returns the name, or the ID when the name is not set.
    /// </summary>
    public override string ToString() => Name ?? Id.ToString();

    /// <inheritdoc />
    public string Name { get; private set; }

    /// <inheritdoc />
    public GuildPermissions Permissions { get; private set; }

    /// <inheritdoc />
    public int Position { get; private set; }

    /// <inheritdoc />
    public int? HoistPosition { get; private set; }

    /// <inheritdoc />
    public int Color { get; private set; }

    /// <inheritdoc />
    public string? IconHash { get; private set; }

    /// <summary>
    /// Gets the role icon URL, or null when the role has no icon.
    /// </summary>
    /// <param name="size">The size of the image.</param>
    public string? GetIconUrl(int size = 64)
        => string.IsNullOrEmpty(IconHash) ? null : $"{Client.Config.MediaUrl}/role-icons/{Id}/{IconHash}.webp?size={size}";

    /// <inheritdoc />
    public string? UnicodeEmoji { get; private set; }

    /// <inheritdoc />
    public bool IsHoisted { get; private set; }

    /// <inheritdoc />
    public bool IsMentionable { get; private set; }

    internal Role(FluxerBaseClient client) : base(client)
    {

    }

    /// <summary>
    /// Creates a role from its JSON representation in the given guild.
    /// </summary>
    /// <returns><see cref="Role"/></returns>
    public static Role Create(FluxerBaseClient client, RoleJson json, ulong guildId)
    {
        Role data = new Role(client)
        {
            GuildId = guildId
        };
        data.Update(json);
        return data;
    }

    internal virtual void Update(RoleJson json)
    {
        Id = json.Id;
        Name = json.Name;
        Permissions = json.Permissions;
        Position = json.Position;
        HoistPosition = json.HoistPosition;
        Color = json.Color;
        IconHash = json.IconHash;
        UnicodeEmoji = json.UnicodeEmoji;
        IsHoisted = json.IsHoisted;
        IsMentionable = json.IsMentionable;
    }
}
