using Newtonsoft.Json;

namespace Fluxer.Net.Gateway.Data;

/// <summary>
/// Gateway data for updating the current user's voice state.
/// Sent as part of VOICE_STATE_UPDATE opcode (4).
/// </summary>
public class VoiceStateUpdatePayload : IGatewayData
{
	[JsonProperty("guild_id")]
	public ulong GuildId { get; set; }

	[JsonProperty("channel_id")]
	public ulong? ChannelId { get; set; }

	[JsonProperty("self_mute")]
	public bool SelfMute { get; set; }

	[JsonProperty("self_deaf")]
	public bool SelfDeaf { get; set; }

	public VoiceStateUpdatePayload(ulong guildId, ulong? channelId, bool selfMute, bool selfDeaf)
	{
		GuildId = guildId;
		ChannelId = channelId;
		SelfMute = selfMute;
		SelfDeaf = selfDeaf;
	}
}
