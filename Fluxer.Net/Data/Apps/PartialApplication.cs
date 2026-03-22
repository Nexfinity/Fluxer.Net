using Fluxer.Net.Data.Apps;

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

    internal PartialApplication(BaseClient client) : base(client)
    {

    }

    public static PartialApplication Create(BaseClient client, PartialApplicationJson json)
    {
        var data = new PartialApplication(client);
        data.Update(client, json);
        return data;
    }

    internal void Update(BaseClient client, PartialApplicationJson json)
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
