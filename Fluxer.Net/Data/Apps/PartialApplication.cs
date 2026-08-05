namespace Fluxer.Net;

/// <inheritdoc />
public class PartialApplication : Entity, IPartialApplication
{
    /// <inheritdoc />
    public ulong Id { get; internal set; }

    /// <inheritdoc />
    public string Name { get; internal set; }

    /// <inheritdoc />
    public string Icon { get; internal set; }

    /// <inheritdoc />
    public string Description { get; internal set; }

    /// <inheritdoc />
    public bool IsPublic { get; internal set; }

    /// <inheritdoc />
    public bool RequiresCodeGrant { get; internal set; }

    /// <inheritdoc />
    public ulong Flags { get; internal set; }

    internal PartialApplication(FluxerBaseClient client) : base(client)
    {

    }

    /// <summary>
    /// Create a PartialApplication object from json.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="json"></param>
    /// <returns></returns>
    public static PartialApplication Create(FluxerBaseClient client, PartialApplicationJson json)
    {
        PartialApplication data = new PartialApplication(client);
        data.Update(client, json);
        return data;
    }

    internal void Update(FluxerBaseClient client, PartialApplicationJson json)
    {
        Id = json.Id;
        Name = json.Name;
        Icon = json.Icon;
        Description = json.Description;
        IsPublic = json.IsPublic;
        RequiresCodeGrant = json.RequiresCodeGrant;
        Flags = json.Flags;
    }
}
