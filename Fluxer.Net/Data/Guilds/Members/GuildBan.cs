using Fluxer.Net.Gateway.Data;

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
    public UserPartialResponse User { get; set; }

    internal GuildBan(BaseClient client) : base(client)
    {

    }

    public static GuildBan Create(BaseClient client, GuildBanJson json)
    {
        var data = new GuildBan(client);
        data.Update(client, json);
        return data;
    }

    internal void Update(BaseClient client, GuildBanJson json)
    {
        BannedAt = json.BannedAt;
        ExpiresAt = json.ExpiresAt;
        ModeratorId = json.ModeratorId;
        Reason = json.Reason;
        User = json.User;
    }
}
