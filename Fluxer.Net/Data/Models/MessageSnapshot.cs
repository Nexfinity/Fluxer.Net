using Newtonsoft.Json;

namespace Fluxer.Net.Data.Models;

public class MessageSnapshot
{
	[JsonProperty("content")]
	public string? Content { get; set; }

	[JsonProperty("timestamp")]
	public DateTime Timestamp { get; set; }

	[JsonProperty("edited_timestamp")]
	public DateTime? EditedTimestamp { get; set; }

	[JsonProperty("mention_users")]
	public HashSet<ulong>? MentionedUserIds { get; set; }

	[JsonProperty("mention_roles")]
	public HashSet<ulong>? MentionedRoleIds { get; set; }

	[JsonProperty("mention_channels")]
	public HashSet<ulong>? MentionedChannelIds { get; set; }

	[JsonProperty("attachments")]
	public List<Attachment>? Attachments { get; set; }

	[JsonProperty("embeds")]
	public List<Embed>? Embeds { get; set; }

	[JsonProperty("sticker_items")]
	public List<StickerItem>? Stickers { get; set; }

	[JsonProperty("type")]
	public int Type { get; set; }

	[JsonProperty("flags")]
	public int Flags { get; set; }
}
