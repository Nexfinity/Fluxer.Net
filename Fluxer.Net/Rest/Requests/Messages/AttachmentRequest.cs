using Newtonsoft.Json;

namespace Fluxer.Net.Rest;

/// <summary>
/// Create an attachment with a request.
/// </summary>
public class AttachmentRequest : AttachmentJson
{
    /// <summary>
    /// Create attachment using file stream.
    /// </summary>
    /// <param name="stream"></param>
    public AttachmentRequest(Stream stream)
    {
        Stream = stream;
    }

    /// <summary>
    /// Create attachment using file path.
    /// </summary>
    /// <param name="file"></param>
    public AttachmentRequest(string file)
    {
        Filename = Path.GetFileName(file);
        using (FileStream f = File.OpenRead(file))
        {
            Stream = new MemoryStream();
            f.CopyTo(Stream);
            Stream.Position = 0;
        }
    }

    [JsonIgnore]
    internal Stream Stream;

    /// <summary>
    /// Create json from attachment request.
    /// </summary>
    /// <returns></returns>
    public AttachmentJson ToJson()
    {
        return new AttachmentJson
        {
            ContentHash = ContentHash,
            ContentType = ContentType,
            Description = Description,
            Duration = Duration,
            IsExpired = IsExpired,
            Filename = Filename,
            Flags = Flags,
            Height = Height,
            Id = Id,
            IsNsfw = IsNsfw,
            Placeholder = Placeholder,
            ProxyUrl = ProxyUrl,
            Size = Size,
            Title = Title,
            Url = Url,
            Waveform = Waveform,
            Width = Width,
        };
    }
}
