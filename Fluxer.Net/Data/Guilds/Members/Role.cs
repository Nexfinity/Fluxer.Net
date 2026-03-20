namespace Fluxer.Net;

public class Role : Entity, IRole
{
    public ulong GuildId { get; internal set; }

    public ulong Id { get; internal set; }

    public string Name { get; internal set; }

    public ulong Permissions { get; internal set; }

    public int Position { get; internal set; }

    public int Color { get; internal set; }

    public string? IconHash { get; internal set; }

    public string? UnicodeEmoji { get; internal set; }

    public bool IsHoisted { get; internal set; }

    public bool IsMentionable { get; internal set; }

    internal Role(BaseClient client) : base(client)
    {

    }

    public static Role Create(BaseClient client, RoleJson json)
    {
        var data = new Role(client);
        data.Update(client, json);
        return data;
    }

    internal void Update(BaseClient client, RoleJson json)
    {
        GuildId = json.GuildId;
        Id = json.Id;
        Name = json.Name;
        Permissions = json.Permissions;
        Position = json.Position;
        Color = json.Color;
        IconHash = json.IconHash;
        UnicodeEmoji = json.UnicodeEmoji;
        IsHoisted = json.IsHoisted;
        IsMentionable = json.IsMentionable;
    }
}
