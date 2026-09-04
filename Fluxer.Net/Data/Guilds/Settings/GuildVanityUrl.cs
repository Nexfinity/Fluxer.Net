namespace Fluxer.Net;

/// <inheritdoc />
public class GuildVanityUrl : Entity, IGuildVanityUrl
{
    /// <inheritdoc />
    public string? Code { get; private set; }

    /// <inheritdoc />
    public int Uses { get; private set; }

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
        data.Update(json);
        return data;
    }

    internal void Update(GuildVanityUrlJson json)
    {
        Code = json.Code;
        Uses = json.Uses;
    }
}
