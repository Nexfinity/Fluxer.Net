using Newtonsoft.Json;

namespace Fluxer.Net.Gateway;

/// <summary>
/// Gateway data for SAVED_MESSAGE_CREATE and SAVED_MESSAGE_DELETE events.
/// </summary>
public class SavedMessageGatewayData
{
    [JsonProperty("saved_message")]
    public SavedMessageJson SavedMessage { get; set; } = null!;
}
