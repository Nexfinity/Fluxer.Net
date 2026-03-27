using Newtonsoft.Json;

namespace Fluxer.Net.Rest;

public class AttachmentRequest : AttachmentJson
{
    public AttachmentRequest(Stream stream)
    {
        Stream = stream;
    }

    public AttachmentRequest(string file)
    {
        Filename = Path.GetFileName(file);
        using (var f = File.OpenRead(file))
        {
            Stream = new MemoryStream();
            f.CopyTo(Stream);
            Stream.Position = 0;
        }
    }

    [JsonIgnore]
    internal Stream Stream;

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
