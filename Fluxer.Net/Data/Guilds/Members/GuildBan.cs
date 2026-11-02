namespace Fluxer.Net;

/// <summary>
/// A ban of a user from a guild.
/// </summary>
public class GuildBan : Entity, IGuildBan
{
    /// <inheritdoc />
    public DateTimeOffset BannedAt { get; set; }

    /// <inheritdoc />
    public DateTimeOffset? ExpiresAt { get; set; }

    /// <inheritdoc />
    public ulong ModeratorId { get; set; }

    /// <inheritdoc />
    public string? Reason { get; set; }

    /// <inheritdoc />
    public User User { get; set; }

    IUser IGuildBan.User => User;

    internal GuildBan(FluxerBaseClient client) : base(client)
    {

    }

    /// <summary>
    /// Creates a ban from its JSON representation.
    /// </summary>
    /// <returns><see cref="GuildBan"/></returns>
    public static GuildBan Create(FluxerBaseClient client, GuildBanJson json)
    {
        GuildBan data = new GuildBan(client);
        data.Update(json);
        return data;
    }

    internal void Update(GuildBanJson json)
    {
        BannedAt = json.BannedAt;
        ExpiresAt = json.ExpiresAt;
        ModeratorId = json.ModeratorId;
        Reason = json.Reason;
        User = User.Create(Client, json.User);
    }
}
