namespace Fluxer.Net;

/// <inheritdoc />
public class Gif : Entity, IGif
{
    /// <inheritdoc />
    public string Id { get; private set; }

    /// <inheritdoc />
    public string Title { get; private set; }

    /// <inheritdoc />
    public string Url { get; private set; }

    /// <inheritdoc />
    public string Source { get; private set; }

    /// <inheritdoc />
    public string ProxySource { get; private set; }

    /// <inheritdoc />
    public int Width { get; private set; }

    /// <inheritdoc />
    public int Height { get; private set; }

    internal Gif(FluxerBaseClient client) : base(client)
    {

    }

    /// <summary>
    /// Create a Gif object from json.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="json"></param>
    /// <returns></returns>
    public static Gif Create(FluxerBaseClient client, GifJson json)
    {
        Gif data = new Gif(client);
        data.Update(client, json);
        return data;
    }

    internal void Update(FluxerBaseClient client, GifJson json)
    {
        Id = json.Id;
        Title = json.Title;
        Url = json.Url;
        Source = json.Source;
        ProxySource = json.ProxySource;
        Width = json.Width;
        Height = json.Height;
    }
}
