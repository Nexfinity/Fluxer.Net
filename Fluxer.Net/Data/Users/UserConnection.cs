namespace Fluxer.Net;

/// <inheritdoc />
public class UserConnection : Entity, IUserConnection
{
    /// <inheritdoc />
    public string Id { get; internal set; }

    /// <inheritdoc />
    public string Type { get; internal set; }

    /// <inheritdoc />
    public string Name { get; internal set; }

    /// <inheritdoc />
    public bool IsVerified { get; internal set; }

    /// <inheritdoc />
    public ulong VisibilityFlags { get; internal set; }

    /// <inheritdoc />
    public int SortOrder { get; internal set; }

    internal UserConnection(BaseClient client) : base(client)
    {

    }

    public static UserConnection Create(BaseClient client, UserConnectionJson json)
    {
        var data = new UserConnection(client);
        data.Update(client, json);
        return data;
    }

    internal void Update(BaseClient client, UserConnectionJson json)
    {
        Id = json.Id;
        Type = json.Type;
        Name = json.Name;
        IsVerified = json.IsVerified;
        VisibilityFlags = json.VisibilityFlags;
        SortOrder = json.SortOrder;
    }
}
