namespace Fluxer.Net;

/// <inheritdoc />
public class Role : Entity, IRole
{
    /// <inheritdoc />
    public ulong GuildId { get; internal set; }

    /// <inheritdoc />
    public ulong Id { get; internal set; }

    /// <inheritdoc />
    public string Name { get; internal set; }

    /// <inheritdoc />
    public ulong Permissions { get; internal set; }

    /// <inheritdoc />
    public int Position { get; internal set; }

    /// <inheritdoc />
    public int Color { get; internal set; }

    /// <inheritdoc />
    public string? UnicodeEmoji { get; internal set; }

    /// <inheritdoc />
    public bool IsHoisted { get; internal set; }

    /// <inheritdoc />
    public bool IsMentionable { get; internal set; }

    internal Role(FluxerBaseClient client) : base(client)
    {

    }

    public static Role Create(FluxerBaseClient client, RoleJson json, ulong guildId)
    {
        var data = new Role(client);
        data.GuildId = guildId;
        data.Update(client, json);
        return data;
    }

    internal void Update(FluxerBaseClient client, RoleJson json)
    {
        Id = json.Id;
        Name = json.Name;
        Permissions = json.Permissions;
        Position = json.Position;
        Color = json.Color;
        UnicodeEmoji = json.UnicodeEmoji;
        IsHoisted = json.IsHoisted;
        IsMentionable = json.IsMentionable;
    }
}
