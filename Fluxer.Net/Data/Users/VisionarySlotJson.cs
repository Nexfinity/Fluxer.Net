using Newtonsoft.Json;

namespace Fluxer.Net;

public class VisionarySlotJson
{
    [JsonProperty("slot_index")]
    public int SlotIndex { get; set; }

    [JsonProperty("user_id")]
    public ulong? UserId { get; set; }
}
