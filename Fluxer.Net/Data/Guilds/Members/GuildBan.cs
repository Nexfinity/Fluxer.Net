namespace Fluxer.Net;

public class GuildBan : Entity, IGuildBan
{
    /// <inheritdoc />
    public DateTime BannedAt { get; set; }

    /// <inheritdoc />
    public DateTime? ExpiresAt { get; set; }

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
    /// Create a GuildBan object from json.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="json"></param>
    /// <returns></returns>
    public static GuildBan Create(FluxerBaseClient client, GuildBanJson json)
    {
        GuildBan data = new GuildBan(client);
        data.Update(client, json);
        return data;
    }

    internal void Update(FluxerBaseClient client, GuildBanJson json)
    {
        BannedAt = json.BannedAt;
        ExpiresAt = json.ExpiresAt;
        ModeratorId = json.ModeratorId;
        Reason = json.Reason;
        User = User.Create(client, json.User);
    }
}
