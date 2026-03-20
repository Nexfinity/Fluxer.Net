using Fluxer.Net.Data.Apps;

namespace Fluxer.Net;

public class PartialApplication : Entity, IPartialApplication
{
    /// <inheritdoc />
    public ulong Id { get; set; }

    /// <inheritdoc />
    public string Name { get; set; }

    /// <inheritdoc />
    public string Icon { get; set; }

    /// <inheritdoc />
    public string Description { get; set; }

    /// <inheritdoc />
    public bool IsPublic { get; set; }

    /// <inheritdoc />
    public bool RequiresCodeGrant { get; set; }

    /// <inheritdoc />
    public ulong Flags { get; set; }

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
