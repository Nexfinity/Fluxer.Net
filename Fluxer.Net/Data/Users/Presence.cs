namespace Fluxer.Net;

/// <inheritdoc />
public class Presence : Entity, IPresence
{
    /// <inheritdoc />
    public ulong UserId { get; set; }

    /// <inheritdoc />
    public ulong? GuildId { get; set; }

    /// <inheritdoc />
    public string Status { get; set; }

    /// <inheritdoc />
    public List<Activity>? Activities { get; set; }

    /// <inheritdoc />
    public ClientStatus? ClientStatus { get; set; }

    IEnumerable<IActivity>? IPresence.Activities => Activities;

    IClientStatus? IPresence.ClientStatus => ClientStatus;

    internal Presence(BaseClient client) : base(client)
    {

    }

    public static Presence Create(BaseClient client, PresenceJson json)
    {
        var data = new Presence(client);
        data.Update(client, json);
        return data;
    }

    internal void Update(BaseClient client, PresenceJson json)
    {
        UserId = json.UserId;
        GuildId = json.GuildId;
        Status = json.Status;
        Activities = json.Activities != null ? json.Activities.Select(x => Activity.Create(client, x)).ToList() : null;
        ClientStatus = ClientStatus.Create(client, json.ClientStatus);
    }
}
