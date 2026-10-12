using Newtonsoft.Json;

namespace Fluxer.Net.Gateway;

internal class UserNoteUpdateGatewayData
{
    [JsonProperty("id")]
    public ulong UserId { get; set; }

    [JsonProperty("note")]
    public string? Note { get; set; }
}