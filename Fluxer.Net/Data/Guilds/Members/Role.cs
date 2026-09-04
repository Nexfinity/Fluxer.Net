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
    public string? UnicodeEmoji { get; private set; }

    /// <inheritdoc />
    public bool IsHoisted { get; private set; }

    /// <inheritdoc />
    public bool IsMentionable { get; private set; }

    internal Role(FluxerBaseClient client) : base(client)
    {

    }

    /// <summary>
    /// Create a Role object from json.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="json"></param>
    /// <param name="guildId"></param>
    /// <returns></returns>
    public static Role Create(FluxerBaseClient client, RoleJson json, ulong guildId)
    {
        Role data = new Role(client)
        {
            GuildId = guildId
        };
        data.Update(client, json);
        return data;
    }

    internal virtual void Update(FluxerBaseClient client, RoleJson json)
    {
        Id = json.Id;
        Name = json.Name;
        Permissions = json.Permissions;
        Position = json.Position;
        HoistPosition = json.HoistPosition;
        Color = json.Color;
        UnicodeEmoji = json.UnicodeEmoji;
        IsHoisted = json.IsHoisted;
        IsMentionable = json.IsMentionable;
    }
}
