namespace Fluxer.Net;

public interface IRelationship
{
    ulong UserId { get; }

    RelationshipType Type { get; }

    string? Nickname { get; }

    DateTimeOffset? SinceAt { get; }
}
