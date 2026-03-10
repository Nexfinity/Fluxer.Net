using Newtonsoft.Json;

namespace Fluxer.Net;

public class CallInfo
{
	[JsonProperty("participant_ids")]
	public HashSet<ulong>? ParticipantIds { get; set; }

	[JsonProperty("ended_timestamp")]
	public DateTime? EndedTimestamp { get; set; }
}
