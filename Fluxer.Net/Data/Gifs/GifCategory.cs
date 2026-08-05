namespace Fluxer.Net;

/// <inheritdoc />
public class GifCategory : Entity, IGifCategory
{
    /// <inheritdoc />
    public string Name { get; internal set; }

    /// <inheritdoc />
    public string Source { get; internal set; }

    /// <inheritdoc />
    public string ProxySource { get; internal set; }

    internal GifCategory(FluxerBaseClient client) : base(client)
    {

    }

    /// <summary>
    /// Create a GifCategory object from json.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="json"></param>
    /// <returns></returns>
    public static GifCategory Create(FluxerBaseClient client, GifCategoryJson json)
    {
        GifCategory data = new GifCategory(client);
        data.Update(client, json);
        return data;
    }

    internal void Update(FluxerBaseClient client, GifCategoryJson json)
    {
        Name = json.Name;
        Source = json.Source;
        ProxySource = json.ProxySource;
    }
}
