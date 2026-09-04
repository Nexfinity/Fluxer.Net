namespace Fluxer.Net;

/// <inheritdoc />
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
    /// Create a UserConnection object from json.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="json"></param>
    /// <returns></returns>
    public static UserConnection Create(FluxerBaseClient client, UserConnectionJson json)
    {
        UserConnection data = new UserConnection(client);
        data.Update(client, json);
        return data;
    }

    internal void Update(FluxerBaseClient client, UserConnectionJson json)
    {
        Id = json.Id;
        Type = json.Type;
        Name = json.Name;
        IsVerified = json.IsVerified;
        VisibilityFlags = json.VisibilityFlags;
        SortOrder = json.SortOrder;
    }
}
