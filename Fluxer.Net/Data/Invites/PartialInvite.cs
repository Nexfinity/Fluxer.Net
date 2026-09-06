namespace Fluxer.Net;

/// <inheritdoc />
public class PartialInvite : Entity, IPartialInvite
{
    /// <inheritdoc />
    public string Code { get; private set; }

    /// <inheritdoc />
    public InviteType Type { get; private set; }

    /// <inheritdoc />
    public PartialGuild? Guild { get; private set; }

    /// <inheritdoc />
    public PartialChannel? Channel { get; private set; }

    /// <inheritdoc />
    public User Inviter { get; private set; }

    /// <inheritdoc />
    public int MemberCount { get; private set; }

    /// <inheritdoc />
    public int PresenceCount { get; private set; }

    /// <inheritdoc />
    public DateTimeOffset? ExpiresAt { get; private set; }

    /// <inheritdoc />
    public bool IsTemporary { get; private set; }

    IPartialGuild? IPartialInvite.Guild => Guild;

    IPartialChannel? IPartialInvite.Channel => Channel;

    IUser IPartialInvite.Inviter => Inviter;

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
        data.Update(json);
        return data;
    }

    internal void Update(PartialInviteJson json)
    {
        Code = json.Code;
        Type = json.Type;
        if (json.Guild != null)
            Guild = PartialGuild.Create(Client, json.Guild);

        Channel = PartialChannel.Create(Client, json.Channel);
        Inviter = User.Create(Client, json.Inviter);
        MemberCount = json.MemberCount;
        PresenceCount = json.PresenceCount;
        ExpiresAt = json.ExpiresAt;
        IsTemporary = json.IsTemporary;
    }
}
