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

    internal RtcRegion(FluxerBaseClient client) : base(client)
    {

    }

    /// <summary>
    /// Create a RtcRegion object from json.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="json"></param>
    /// <returns></returns>
    public static RtcRegion Create(FluxerBaseClient client, RtcRegionJson json)
    {
        RtcRegion data = new RtcRegion(client);
        data.Update(client, json);
        return data;
    }

    internal void Update(FluxerBaseClient client, RtcRegionJson json)
    {
        Id = json.Id;
        Name = json.Name;
        Emoji = json.Emoji;
    }
}
