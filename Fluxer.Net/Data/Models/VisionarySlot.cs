using Newtonsoft.Json;

namespace Fluxer.Net.Data.Models;

public class VisionarySlot
{
	[JsonProperty("slot_index")]
	public int SlotIndex { get; set; }

	[JsonProperty("user_id")]
	public ulong? UserId { get; set; }
}
