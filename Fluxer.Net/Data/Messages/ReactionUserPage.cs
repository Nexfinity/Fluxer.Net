namespace Fluxer.Net;

/// <summary>
/// A page of the users who reacted to a message with one emoji.
/// </summary>
public class ReactionUserPage : Entity
{
    /// <summary>
    /// The users on this page.
    /// </summary>
    public IReadOnlyList<User> Users { get; private set; } = Array.Empty<User>();

    /// <summary>
    /// Whether more users can be fetched by passing <see cref="NextAfterId"/>.
    /// </summary>
    public bool HasMore { get; private set; }

    /// <summary>
    /// The id to fetch the next page after, or null on the last page.
    /// </summary>
    public ulong? NextAfterId { get; private set; }

    internal ReactionUserPage(FluxerBaseClient client) : base(client)
    {

    }

    /// <summary>
    /// Creates a page of reaction users from JSON.
    /// </summary>
    public static ReactionUserPage Create(FluxerBaseClient client, ReactionUserPageJson json)
    {
        ReactionUserPage data = new ReactionUserPage(client);
        data.Users = json.Users.Select(x => User.Create(client, x)).ToArray();
        data.HasMore = json.HasMore;
        data.NextAfterId = json.NextAfterId;
        return data;
    }
}
