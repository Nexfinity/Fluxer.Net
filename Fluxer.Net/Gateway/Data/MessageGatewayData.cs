using Fluxer.Net.Data.Models;
using Newtonsoft.Json;

namespace Fluxer.Net.Gateway.Data;

/// <summary>
/// Gateway message data matching the MessageResponse API model
/// </summary>
public class MessageGatewayData : IGatewayData
{
	[JsonProperty("id")]
	public ulong Id { get; set; }

	[JsonProperty("channel_id")]
	public ulong ChannelId { get; set; }

	[JsonProperty("author")]
	public UserPartialResponse Author { get; set; } = null!;

	[JsonProperty("webhook_id")]
	public ulong? WebhookId { get; set; }

	[JsonProperty("type")]
	public int Type { get; set; }

	[JsonProperty("flags")]
	public int Flags { get; set; }

	[JsonProperty("content")]
	public string Content { get; set; } = string.Empty;

	[JsonProperty("timestamp")]
	public DateTime Timestamp { get; set; }

	[JsonProperty("edited_timestamp")]
	public DateTime? EditedTimestamp { get; set; }

	[JsonProperty("pinned")]
	public bool IsPinned { get; set; }

	[JsonProperty("mention_everyone")]
	public bool MentionsEveryone { get; set; }

	[JsonProperty("mentions")]
	public List<UserPartialResponse>? Mentions { get; set; }

	[JsonProperty("mention_roles")]
	public List<ulong>? MentionRoles { get; set; }

	[JsonProperty("embeds")]
	public List<Embed>? Embeds { get; set; }

	[JsonProperty("attachments")]
	public List<Attachment>? Attachments { get; set; }

	[JsonProperty("stickers")]
	public List<StickerItem>? Stickers { get; set; }

	[JsonProperty("reactions")]
	public List<MessageReaction>? Reactions { get; set; }

	[JsonProperty("message_reference")]
	public MessageRef? MessageReference { get; set; }

	[JsonProperty("message_snapshots")]
	public List<MessageSnapshot>? MessageSnapshots { get; set; }

	[JsonProperty("nonce")]
	public string? Nonce { get; set; }

	[JsonProperty("referenced_message")]
	public MessageGatewayData? ReferencedMessage { get; set; }

	/// <summary>
	/// Guild member data for the author (only present in guild messages)
	/// </summary>
	[JsonProperty("member")]
	public GuildMemberPartialResponse? Member { get; set; }

	/// <summary>
	/// ID of the guild where the message was sent (null for DMs)
	/// </summary>
	[JsonProperty("guild_id")]
	public ulong? GuildId { get; set; }
}

/// <summary>
/// Partial user response matching UserPartialResponse from the API
/// </summary>
public class UserPartialResponse
{
	[JsonProperty("id")]
	public ulong Id { get; set; }

	[JsonProperty("username")]
	public string Username { get; set; } = null!;

	[JsonProperty("discriminator")]
	public string Discriminator { get; set; } = null!;

	[JsonProperty("avatar")]
	public string? Avatar { get; set; }

	[JsonProperty("bot")]
	public bool IsBot { get; set; }

	[JsonProperty("system")]
	public bool IsSystem { get; set; }

	[JsonProperty("flags")]
	public int Flags { get; set; }

	[JsonProperty("banner")]
	public string? Banner { get; set; }

	[JsonProperty("accent_color")]
	public int? AccentColor { get; set; }
}

/// <summary>
/// Partial guild member response for message context
/// </summary>
public class GuildMemberPartialResponse
{
	[JsonProperty("nick")]
	public string? Nick { get; set; }

	[JsonProperty("avatar")]
	public string? Avatar { get; set; }

	[JsonProperty("roles")]
	public List<ulong> Roles { get; set; } = new();

	[JsonProperty("joined_at")]
	public DateTime JoinedAt { get; set; }
}
