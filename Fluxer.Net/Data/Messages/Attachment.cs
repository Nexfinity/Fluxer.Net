namespace Fluxer.Net;

/// <inheritdoc />
public class Attachment : Entity, IAttachment
{
    /// <inheritdoc />
    public ulong Id { get; set; }

    /// <inheritdoc />
    public string Filename { get; set; }

    /// <inheritdoc />
    public ulong Size { get; set; }

    /// <inheritdoc />
    public string? Title { get; set; }

    /// <inheritdoc />
    public string? Description { get; set; }

    /// <inheritdoc />
    public int? Width { get; set; }

    /// <inheritdoc />
    public int? Height { get; set; }

    /// <inheritdoc />
    public string ContentType { get; set; }

    /// <inheritdoc />
    public string? ContentHash { get; set; }

    /// <inheritdoc />
    public string? Placeholder { get; set; }

    /// <inheritdoc />
    public AttachmentFlag Flags { get; set; }

    /// <inheritdoc />
    public ulong? Duration { get; set; }

    /// <inheritdoc />
    public string? Url { get; set; }

    /// <inheritdoc />
    public string? ProxyUrl { get; set; }

    /// <inheritdoc />
    public bool? IsNsfw { get; set; }

    /// <inheritdoc />
    public string? Waveform { get; set; }

    /// <inheritdoc />
    public DateTime? ExpiresAt { get; set; }

    /// <inheritdoc />
    public bool? IsExpired { get; set; }

    internal ulong ChannelId { get; set; }

    /// <summary>
    /// Get the attachment's url.
    /// </summary>
    public string? GetAttachmentUrl()
    {
        return $"{Client.Config.MediaUrl}/attachments/{ChannelId}/{Id}/{Filename}";
    }

    internal Attachment(FluxerBaseClient client) : base(client)
    {

    }

    /// <summary>
    /// Create a Attachment object from json.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="json"></param>
    /// <param name="channelId"></param>
    /// <returns></returns>
    public static Attachment Create(FluxerBaseClient client, AttachmentJson json, ulong channelId)
    {
        Attachment data = new Attachment(client)
        {
            ChannelId = channelId
        };
        data.Update(client, json);
        return data;
    }

    internal void Update(FluxerBaseClient client, AttachmentJson json)
    {
        Id = json.Id;
        Filename = json.Filename;
        Size = json.Size;
        Title = json.Title;
        Description = json.Description;
        Width = json.Width;
        Height = json.Height;
        ContentType = json.ContentType;
        ContentHash = json.ContentHash;
        Placeholder = json.Placeholder;
        Flags = json.Flags;
        Duration = json.Duration;
        Url = json.Url;
        ProxyUrl = json.ProxyUrl;
        IsNsfw = json.IsNsfw;
        Waveform = json.Waveform;
        ExpiresAt = json.ExpiresAt;
        IsExpired = json.IsExpired;
    }
}
