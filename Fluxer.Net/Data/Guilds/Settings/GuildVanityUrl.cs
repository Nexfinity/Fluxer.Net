namespace Fluxer.Net;

/// <inheritdoc />
public class GuildVanityUrl : Entity, IGuildVanityUrl
{
    /// <inheritdoc />
    public string? Code { get; internal set; }

    /// <inheritdoc />
    public int Uses { get; internal set; }

    internal GuildVanityUrl(BaseClient client) : base(client)
    {

    }

    public static GuildVanityUrl Create(BaseClient client, GuildVanityUrlJson json)
    {
        var data = new GuildVanityUrl(client);
        data.Update(client, json);
        return data;
    }

    internal void Update(BaseClient client, GuildVanityUrlJson json)
    {
        Code = json.Code;
        Uses = json.Uses;
    }
}
