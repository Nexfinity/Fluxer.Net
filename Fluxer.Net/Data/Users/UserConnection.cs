namespace Fluxer.Net;

/// <summary>
/// An account or domain that a user has connected to their profile.
/// </summary>
public class UserConnection : Entity, IUserConnection
{
    /// <inheritdoc />
    public string Id { get; private set; }

    /// <inheritdoc />
    public string Type { get; private set; }

    /// <inheritdoc />
    public string Name { get; private set; }

    /// <inheritdoc />
    public bool IsVerified { get; private set; }

    /// <inheritdoc />
    public ulong VisibilityFlags { get; private set; }

    /// <inheritdoc />
    public int SortOrder { get; internal set; }

    internal UserConnection(FluxerBaseClient client) : base(client)
    {

    }

    /// <summary>
    /// Creates a connection from its JSON data.
    /// </summary>
    /// <param name="client">The client the connection belongs to.</param>
    /// <param name="json">The connection data.</param>
    /// <returns>The connection.</returns>
    public static UserConnection Create(FluxerBaseClient client, UserConnectionJson json)
    {
        UserConnection data = new UserConnection(client);
        data.Update(json);
        return data;
    }

    internal void Update(UserConnectionJson json)
    {
        Id = json.Id;
        Type = json.Type;
        Name = json.Name;
        IsVerified = json.IsVerified;
        VisibilityFlags = json.VisibilityFlags;
        SortOrder = json.SortOrder;
    }
}
