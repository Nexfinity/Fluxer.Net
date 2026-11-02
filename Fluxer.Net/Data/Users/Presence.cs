namespace Fluxer.Net;

/// <summary>
/// The status and activities of a user.
/// </summary>
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
    /// Creates a presence from its JSON data.
    /// </summary>
    /// <param name="client">The client the presence belongs to.</param>
    /// <param name="json">The presence data.</param>
    /// <returns>The presence.</returns>
    public static Presence Create(FluxerBaseClient client, PresenceJson json)
    {
        Presence data = new Presence(client);
        data.Update(json);
        return data;
    }

    internal void Update(PresenceJson json)
    {
        UserId = json.UserId;
        GuildId = json.GuildId;
        Status = json.Status;
        Activities = json.Activities != null ? json.Activities.Select(x => Activity.Create(Client, x)).ToList() : null;
        ClientStatus = ClientStatus.Create(Client, json.ClientStatus);
    }
}
