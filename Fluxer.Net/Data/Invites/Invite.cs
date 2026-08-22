namespace Fluxer.Net;

/// <inheritdoc />
public class Invite : PartialInvite, IInvite
{
    /// <inheritdoc />
    public DateTimeOffset CreatedAt { get; internal set; }

    /// <inheritdoc />
    public int Uses { get; internal set; }

    /// <inheritdoc />
    public int MaxUses { get; internal set; }

    /// <inheritdoc />
    public int MaxAge { get; internal set; }

    internal Invite(FluxerBaseClient client) : base(client)
    {

    }

    /// <summary>
    /// Create a Invite object from json.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="json"></param>
    /// <returns></returns>
    public static Invite Create(FluxerBaseClient client, InviteJson json)
    {
        Invite data = new Invite(client);
        data.Update(client, json);
        return data;
    }

    internal void Update(FluxerBaseClient client, InviteJson json)
    {
        base.Update(client, json);
        CreatedAt = json.CreatedAt;
        Uses = json.Uses;
        MaxUses = json.MaxUses;
        MaxAge = json.MaxAge;
    }
}
