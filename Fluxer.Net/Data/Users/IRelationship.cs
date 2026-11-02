namespace Fluxer.Net;

/// <summary>
/// A relationship between the current user and another user, such as a friendship or a block.
/// </summary>
public interface IRelationship
{
    /// <summary>
    /// The id of the other user.
    /// </summary>
    ulong UserId { get; }

    /// <summary>
    /// The kind of relationship.
    /// </summary>
    RelationshipType Type { get; }

    /// <summary>
    /// The nickname given to the other user, or null when there is none.
    /// </summary>
    string? Nickname { get; }

    /// <summary>
    /// When the relationship started, or null when the API does not send it.
    /// </summary>
    DateTimeOffset? SinceAt { get; }
}
