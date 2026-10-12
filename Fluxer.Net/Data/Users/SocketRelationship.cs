namespace Fluxer.Net;

/// <inheritdoc />
public class SocketRelationship : Entity, IRelationship
{
    /// <inheritdoc />
    public SocketUser User { get; private set; }

    /// <inheritdoc />
    public ulong UserId { get; private set; }

    /// <inheritdoc />
    public RelationshipType Type { get; private set; }

    /// <inheritdoc />
    public string? Nickname { get; private set; }

    /// <inheritdoc />
    public DateTimeOffset? SinceAt { get; private set; }

    internal SocketRelationship(FluxerBaseClient client) : base(client)
    {

    }

    internal static SocketRelationship Create(FluxerBaseClient client, RelationshipJson json, SocketUser user)
    {
        SocketRelationship data = new SocketRelationship(client)
        {
            User = user
        };
        data.Update(json);
        return data;
    }

    internal void Update(RelationshipJson json)
    {
        UserId = json.UserId;
        Type = json.Type;
        Nickname = json.Nickname;
        SinceAt = json.SinceAt;
    }
}
