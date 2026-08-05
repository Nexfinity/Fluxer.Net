namespace Fluxer.Net;

/// <inheritdoc />
public class GuildVanityUrl : Entity, IGuildVanityUrl
{
    /// <inheritdoc />
    public string? Code { get; internal set; }

    /// <inheritdoc />
    public int Uses { get; internal set; }

    internal GuildVanityUrl(FluxerBaseClient client) : base(client)
    {

    }

    /// <summary>
    /// Create a GuildVanityUrl object from json.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="json"></param>
    /// <returns></returns>
    public static GuildVanityUrl Create(FluxerBaseClient client, GuildVanityUrlJson json)
    {
        GuildVanityUrl data = new GuildVanityUrl(client);
        data.Update(client, json);
        return data;
    }

    internal void Update(FluxerBaseClient client, GuildVanityUrlJson json)
    {
        Code = json.Code;
        Uses = json.Uses;
    }
}
