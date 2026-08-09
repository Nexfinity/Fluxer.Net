namespace Fluxer.Net;

/// <inheritdoc />
public class PartialInvite : Entity, IPartialInvite
{
    /// <inheritdoc />
    public string Code { get; internal set; }

    /// <inheritdoc />
    public int Type { get; internal set; }

    /// <inheritdoc />
    public PartialGuild? Server { get; internal set; }

    /// <inheritdoc />
    public InviteChannelJson? Channel { get; internal set; }

    /// <inheritdoc />
    public InviteUserJson Inviter { get; internal set; }

    /// <inheritdoc />
    public int MemberCount { get; internal set; }

    /// <inheritdoc />
    public int PresenceCount { get; internal set; }

    /// <inheritdoc />
    public DateTime? ExpiresAt { get; internal set; }

    /// <inheritdoc />
    public bool Temporary { get; internal set; }

    IPartialGuild? IPartialInvite.Server => Server;

    internal PartialInvite(FluxerBaseClient client) : base(client)
    {

    }

    /// <summary>
    /// Create a PartialInvite object from json.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="json"></param>
    /// <returns></returns>
    public static PartialInvite Create(FluxerBaseClient client, PartialInviteJson json)
    {
        PartialInvite data = new PartialInvite(client);
        data.Update(client, json);
        return data;
    }

    internal void Update(FluxerBaseClient client, PartialInviteJson json)
    {
        Code = json.Code;
        Type = json.Type;
        if (json.Server != null)
            Server = PartialGuild.Create(client, json.Server);

        Channel = json.Channel;
        Inviter = json.Inviter;
        MemberCount = json.MemberCount;
        PresenceCount = json.PresenceCount;
        ExpiresAt = json.ExpiresAt;
        Temporary = json.Temporary;
    }
}
