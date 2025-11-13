using Fluxer.Net.Gateway.Data;

namespace Fluxer.Net.Example;

/// <summary>
/// Manages voice connection state across the application.
/// </summary>
public static class VoiceStateManager
{
	/// <summary>
	/// Voice endpoint from VOICE_SERVER_UPDATE event.
	/// </summary>
	public static string? VoiceEndpoint { get; set; }

	/// <summary>
	/// Voice token from VOICE_SERVER_UPDATE event.
	/// </summary>
	public static string? VoiceToken { get; set; }

	/// <summary>
	/// Voice session ID from VOICE_STATE_UPDATE event.
	/// </summary>
	public static string? VoiceSessionId { get; set; }

	/// <summary>
	/// Voice guild ID from VOICE_SERVER_UPDATE event.
	/// </summary>
	public static ulong? VoiceGuildId { get; set; }

	/// <summary>
	/// Voice channel ID from VOICE_STATE_UPDATE event.
	/// </summary>
	public static ulong? VoiceChannelId { get; set; }

	/// <summary>
	/// Connection ID from VOICE_SERVER_UPDATE event.
	/// </summary>
	public static string? ConnectionId { get; set; }

	/// <summary>
	/// Ready data containing bot user information.
	/// </summary>
	public static ReadyGatewayData? ReadyData { get; set; }

	/// <summary>
	/// Resets all voice state.
	/// </summary>
	public static void Reset()
	{
		VoiceEndpoint = null;
		VoiceToken = null;
		VoiceSessionId = null;
		VoiceGuildId = null;
		VoiceChannelId = null;
		ConnectionId = null;
	}

	/// <summary>
	/// Checks if all required voice connection data is available.
	/// </summary>
	public static bool IsVoiceDataReady()
	{
		return VoiceEndpoint != null
		       && VoiceToken != null
		       && VoiceSessionId != null
		       && ReadyData?.User != null;
	}
}
