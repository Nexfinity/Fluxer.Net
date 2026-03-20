namespace Fluxer.Net;

/// <inheritdoc />
public class Invite : PartialInvite, IInvite
{
    /// <inheritdoc />
    public DateTime CreatedAt { get; internal set; }

    /// <inheritdoc />
    public int Uses { get; internal set; }

    /// <inheritdoc />
    public int MaxUses { get; internal set; }

    /// <inheritdoc />
    public int MaxAge { get; internal set; }

    internal Invite(BaseClient client) : base(client)
    {

    }

    public static Invite Create(BaseClient client, InviteJson json)
    {
        var data = new Invite(client);
        data.Update(client, json);
        return data;
    }

    internal void Update(BaseClient client, InviteJson json)
    {
        base.Update(client, json);
        CreatedAt = json.CreatedAt;
        Uses = json.Uses;
        MaxUses = json.MaxUses;
        MaxAge = json.MaxAge;
    }
}
