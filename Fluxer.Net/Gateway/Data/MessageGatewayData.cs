using Fluxer.Net.Data.Models;
using Newtonsoft.Json;

namespace Fluxer.Net.Gateway.Data;

public class MessageGatewayData : Message, IGatewayData
{
	/// <summary>
	/// The author of the message (gateway format includes full user object)
	/// </summary>
	[JsonProperty("author")]
	public User? Author { get; set; }

	/// <summary>
	/// Guild member data for the author (only present in guild messages)
	/// </summary>
	[JsonProperty("member")]
	public GuildMember? Member { get; set; }

	/// <summary>
	/// ID of the guild where the message was sent (null for DMs)
	/// </summary>
	[JsonProperty("guild_id")]
	public ulong? GuildId { get; set; }

	/// <summary>
	/// Type of the channel where the message was sent
	/// </summary>
	[JsonProperty("channel_type")]
	public int? ChannelType { get; set; }

	/// <summary>
	/// Unique identifier used by the client for message deduplication
	/// </summary>
	[JsonProperty("nonce")]
	public string? Nonce { get; set; }
}
