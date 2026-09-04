namespace Fluxer.Net;

/// <inheritdoc />
public class PartialInvite : Entity, IPartialInvite
{
    /// <inheritdoc />
    public string Code { get; private set; }

    /// <inheritdoc />
    public int Type { get; private set; }

    /// <inheritdoc />
    public PartialGuild? Guild { get; private set; }

    /// <inheritdoc />
    public InviteChannelJson? Channel { get; private set; }

    /// <inheritdoc />
    public InviteUserJson Inviter { get; private set; }

    /// <inheritdoc />
    public int MemberCount { get; private set; }

    /// <inheritdoc />
    public int PresenceCount { get; private set; }

    /// <inheritdoc />
    public DateTimeOffset? ExpiresAt { get; private set; }

    /// <inheritdoc />
    public bool Temporary { get; private set; }

    IPartialGuild? IPartialInvite.Guild => Guild;

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
        if (json.Guild != null)
            Guild = PartialGuild.Create(client, json.Guild);

        Channel = json.Channel;
        Inviter = json.Inviter;
        MemberCount = json.MemberCount;
        PresenceCount = json.PresenceCount;
        ExpiresAt = json.ExpiresAt;
        Temporary = json.Temporary;
    }
}
