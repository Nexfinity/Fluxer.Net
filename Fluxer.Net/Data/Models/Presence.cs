using Newtonsoft.Json;

namespace Fluxer.Net.Data.Models;

public class Presence
{
	[JsonProperty("user_id")]
	public ulong UserId { get; set; }

	[JsonProperty("guild_id")]
	public ulong? GuildId { get; set; }

	[JsonProperty("status")]
	public string Status { get; set; }

	[JsonProperty("activities")]
	public List<Activity>? Activities { get; set; }

	[JsonProperty("client_status")]
	public ClientStatus? ClientStatus { get; set; }
}

public class Activity
{
	[JsonProperty("name")]
	public string Name { get; set; }

	[JsonProperty("type")]
	public int Type { get; set; }

	[JsonProperty("url")]
	public string? Url { get; set; }

	[JsonProperty("created_at")]
	public long CreatedAt { get; set; }

	[JsonProperty("timestamps")]
	public ActivityTimestamps? Timestamps { get; set; }

	[JsonProperty("details")]
	public string? Details { get; set; }

	[JsonProperty("state")]
	public string? State { get; set; }

	[JsonProperty("emoji")]
	public ActivityEmoji? Emoji { get; set; }
}

public class ActivityTimestamps
{
	[JsonProperty("start")]
	public long? Start { get; set; }

	[JsonProperty("end")]
	public long? End { get; set; }
}

public class ActivityEmoji
{
	[JsonProperty("name")]
	public string Name { get; set; }

	[JsonProperty("id")]
	public ulong? Id { get; set; }

	[JsonProperty("animated")]
	public bool Animated { get; set; }
}

public class ClientStatus
{
	[JsonProperty("desktop")]
	public string? Desktop { get; set; }

	[JsonProperty("mobile")]
	public string? Mobile { get; set; }

	[JsonProperty("web")]
	public string? Web { get; set; }
}
