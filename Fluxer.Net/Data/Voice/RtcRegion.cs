namespace Fluxer.Net;

/// <inheritdoc />
public class RtcRegion : Entity, IRtcRegion
{
    /// <inheritdoc />
    public string Id { get; internal set; }

    /// <inheritdoc />
    public string Name { get; internal set; }

    /// <inheritdoc />
    public string Emoji { get; internal set; }

    internal RtcRegion(BaseClient client) : base(client)
    {

    }

    public static RtcRegion Create(BaseClient client, RtcRegionJson json)
    {
        var data = new RtcRegion(client);
        data.Update(client, json);
        return data;
    }

    internal void Update(BaseClient client, RtcRegionJson json)
    {
        Id = json.Id;
        Name = json.Name;
        Emoji = json.Emoji;
    }
}
