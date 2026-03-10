using Newtonsoft.Json;

namespace Fluxer.Net.Data.Users;

public class FavoriteMeme
{
	[JsonProperty("meme_id")]
	public ulong Id { get; set; }

	[JsonProperty("user_id")]
	public ulong UserId { get; set; }

	[JsonProperty("name")]
	public string Name { get; set; }

	[JsonProperty("alt_text")]
	public string? AltText { get; set; }

	[JsonProperty("tags")]
	public List<string>? Tags { get; set; }

	[JsonProperty("attachment_id")]
	public ulong AttachmentId { get; set; }

	[JsonProperty("filename")]
	public string Filename { get; set; }

	[JsonProperty("content_type")]
	public string ContentType { get; set; }

	[JsonProperty("content_hash")]
	public string? ContentHash { get; set; }

	[JsonProperty("size")]
	public ulong Size { get; set; }

	[JsonProperty("width")]
	public int? Width { get; set; }

	[JsonProperty("height")]
	public int? Height { get; set; }

	[JsonProperty("duration")]
	public double? Duration { get; set; }

	[JsonProperty("storage_key")]
	public string StorageKey { get; set; }

	[JsonProperty("is_gifv")]
	public bool IsGifv { get; set; }

	[JsonProperty("created_at")]
	public DateTime CreatedAt { get; set; }
}
