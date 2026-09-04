namespace Fluxer.Net;

/// <inheritdoc />
public class Presence : Entity, IPresence
{
    /// <inheritdoc />
    public ulong UserId { get; private set; }

    /// <inheritdoc />
    public ulong? GuildId { get; private set; }

    /// <inheritdoc />
    public string Status { get; private set; }

    /// <inheritdoc />
    public List<Activity>? Activities { get; private set; }

    /// <inheritdoc />
    public ClientStatus? ClientStatus { get; private set; }

    IEnumerable<IActivity>? IPresence.Activities => Activities;

    IClientStatus? IPresence.ClientStatus => ClientStatus;

    internal Presence(FluxerBaseClient client) : base(client)
    {

    }

    /// <summary>
    /// Create a Presence object from json.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="json"></param>
    /// <returns></returns>
    public static Presence Create(FluxerBaseClient client, PresenceJson json)
    {
        Presence data = new Presence(client);
        data.Update(client, json);
        return data;
    }

    internal void Update(FluxerBaseClient client, PresenceJson json)
    {
        UserId = json.UserId;
        GuildId = json.GuildId;
        Status = json.Status;
        Activities = json.Activities != null ? json.Activities.Select(x => Activity.Create(client, x)).ToList() : null;
        ClientStatus = ClientStatus.Create(client, json.ClientStatus);
    }
}
