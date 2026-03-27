namespace Fluxer.Net;

/// <inheritdoc />
public class Embed : IEmbed
{
    /// <inheritdoc />
    public string RawType { get; set; }

    /// <inheritdoc />
    public string? Url { get; set; }

    /// <inheritdoc />
    public string Title { get; set; }

    /// <inheritdoc />
    public int? Color { get; set; }

    /// <inheritdoc />
    public DateTime? Timestamp { get; set; }

    /// <inheritdoc />
    public string? Description { get; set; }

    /// <inheritdoc />
    public EmbedAuthor? Author { get; set; }

    /// <inheritdoc />
    public EmbedMedia? Image { get; set; }

    /// <inheritdoc />
    public EmbedMedia? Thumbnail { get; set; }

    /// <inheritdoc />
    public EmbedFooter? Footer { get; set; }

    /// <inheritdoc />
    public EmbedField[]? Fields { get; set; }

    /// <inheritdoc />
    public EmbedAuthor? Provider { get; set; }

    /// <inheritdoc />
    public EmbedMedia? Video { get; set; }

    /// <inheritdoc />
    public EmbedMedia? Audio { get; set; }

    /// <inheritdoc />
    public bool IsNsfw { get; set; }

    IEmbedAuthor? IEmbed.Author => Author;

    IEmbedMedia? IEmbed.Image => Image;

    IEmbedMedia? IEmbed.Thumbnail => Thumbnail;

    IEmbedFooter? IEmbed.Footer => Footer;

    IEmbedField[]? IEmbed.Fields => Fields;

    IEmbedAuthor? IEmbed.Provider => Provider;

    IEmbedMedia? IEmbed.Video => Video;

    IEmbedMedia? IEmbed.Audio => Audio;

    internal Embed(FluxerBaseClient client)
    {

    }

    public static Embed Create(FluxerBaseClient client, EmbedJson json)
    {
        var data = new Embed(client);
        data.Update(client, json);
        return data;
    }

    internal void Update(FluxerBaseClient client, EmbedJson json)
    {
        RawType = json.RawType;
        Url = json.Url;
        Title = json.Title;
        Color = json.Color;
        Timestamp = json.Timestamp;
        Description = json.Description;
        Author = EmbedAuthor.Create(client, json.Author);
        Image = EmbedMedia.Create(client, json.Image);
        Thumbnail = EmbedMedia.Create(client, json.Thumbnail);
        Footer = EmbedFooter.Create(client, json.Footer);
        if (json.Fields != null)
            Fields = json.Fields.Select(x => EmbedField.Create(client, x)).ToArray();
        Provider = EmbedAuthor.Create(client, json.Provider);
        Video = EmbedMedia.Create(client, json.Video);
        Audio = EmbedMedia.Create(client, json.Audio);
        IsNsfw = json.IsNsfw;
    }
}

/// <inheritdoc />
public class EmbedField : IEmbedField
{
    /// <inheritdoc />
    public string Name { get; set; }

    /// <inheritdoc />
    public string Value { get; set; }

    /// <inheritdoc />
    public bool IsInline { get; set; }

    internal EmbedField(FluxerBaseClient client)
    {

    }

    public static EmbedField Create(FluxerBaseClient client, EmbedFieldJson json)
    {
        var data = new EmbedField(client);
        data.Update(client, json);
        return data;
    }

    internal void Update(FluxerBaseClient client, EmbedFieldJson json)
    {
        Name = json.Name;
        Value = json.Value;
        IsInline = json.IsInline;
    }
}
public class EmbedAuthor : IEmbedAuthor
{
    /// <inheritdoc />
    public string? Name { get; set; }

    /// <inheritdoc />
    public string? Url { get; set; }

    /// <inheritdoc />
    public string? IconUrl { get; set; }

    /// <inheritdoc />
    public string? ProxyIconUrl { get; set; }

    internal EmbedAuthor(FluxerBaseClient client)
    {

    }

    public static EmbedAuthor? Create(FluxerBaseClient client, EmbedAuthorJson? json)
    {
        if (json == null)
            return null;

        var data = new EmbedAuthor(client);
        data.Update(client, json);
        return data;
    }

    internal void Update(FluxerBaseClient client, EmbedAuthorJson json)
    {
        Name = json.Name;
        Url = json.Url;
        IconUrl = json.IconUrl;
        ProxyIconUrl = json.ProxyIconUrl;
    }
}
public class EmbedFooter : IEmbedFooter
{
    /// <inheritdoc />
    public string? Text { get; set; }

    /// <inheritdoc />
    public string? IconUrl { get; set; }

    /// <inheritdoc />
    public string? ProxyIconUrl { get; set; }

    internal EmbedFooter(FluxerBaseClient client)
    {

    }

    public static EmbedFooter? Create(FluxerBaseClient client, EmbedFooterJson? json)
    {
        if (json == null)
            return null;

        var data = new EmbedFooter(client);
        data.Update(client, json);
        return data;
    }

    internal void Update(FluxerBaseClient client, EmbedFooterJson json)
    {
        Text = json.Text;
        IconUrl = json.IconUrl;
        ProxyIconUrl = json.ProxyIconUrl;
    }
}
public class EmbedMedia : IEmbedMedia
{
    /// <inheritdoc />
    public string Url { get; set; }

    /// <inheritdoc />
    public ulong Flags { get; set; }

    /// <inheritdoc />
    public string? ProxyUrl { get; set; }

    /// <inheritdoc />
    public string? ContentType { get; set; }

    /// <inheritdoc />
    public string? ContentHash { get; set; }

    /// <inheritdoc />
    public int? Width { get; set; }

    /// <inheritdoc />
    public int? Height { get; set; }

    /// <inheritdoc />
    public string? Description { get; set; }

    /// <inheritdoc />
    public string? Placeholder { get; set; }

    /// <inheritdoc />
    public int? Duration { get; set; }

    internal EmbedMedia(FluxerBaseClient client)
    {

    }

    public static EmbedMedia? Create(FluxerBaseClient client, EmbedMediaJson? json)
    {
        if (json == null)
            return null;

        var data = new EmbedMedia(client);
        data.Update(client, json);
        return data;
    }

    internal void Update(FluxerBaseClient client, EmbedMediaJson json)
    {
        Url = json.Url;
        Flags = json.Flags;
        ProxyUrl = json.ProxyUrl;
        ContentType = json.ContentType;
        ContentHash = json.ContentHash;
        Width = json.Width;
        Height = json.Height;
        Description = json.Description;
        Placeholder = json.Placeholder;
        Duration = json.Duration;
    }
}