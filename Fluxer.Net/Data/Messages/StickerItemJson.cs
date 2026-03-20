using Newtonsoft.Json;

namespace Fluxer.Net;

public class StickerItemJson
{
    [JsonProperty("id")]
    public ulong Id { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("format_type")]
    public int FormatType { get; set; }
}
