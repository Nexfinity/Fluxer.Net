namespace Fluxer.Net;

/// <inheritdoc />
public class Gif : Entity
{
    /// <inheritdoc />
    public string Id { get; internal set; }

    /// <inheritdoc />
    public string Title { get; set; }

    /// <inheritdoc />
    public string Url { get; set; }

    /// <inheritdoc />
    public string Source { get; set; }

    /// <inheritdoc />
    public string ProxySource { get; set; }

    /// <inheritdoc />
    public int Width { get; set; }

    /// <inheritdoc />
    public int Height { get; set; }

    internal Gif(BaseClient client) : base(client)
    {

    }

    public static Gif Create(BaseClient client, GifJson json)
    {
        var data = new Gif(client);
        data.Update(client, json);
        return data;
    }

    internal void Update(BaseClient client, GifJson json)
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
