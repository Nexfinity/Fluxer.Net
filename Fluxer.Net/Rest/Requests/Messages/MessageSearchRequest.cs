using Newtonsoft.Json;

namespace Fluxer.Net.Rest;

public class MessageSearchRequest
{
    /// <summary>
    /// Number of results per page (1-25)
    /// </summary>
    [JsonProperty("hits_per_page")]
    public int HitsPerPage { get; set; } = 25;

    /// <summary>
    /// Page number for pagination
    /// </summary>
    [JsonProperty("page")]
    public int Page { get; set; } = 1;

    /// <summary>
    /// Maximum message ID to include in results
    /// </summary>
    [JsonProperty("max_id")]
    public ulong? MaxId { get; set; }

    /// <summary>
    /// Minimum message ID to include in results
    /// </summary>
    [JsonProperty("min_id")]
    public ulong? MinId { get; set; }

    /// <summary>
    /// Text content to search for
    /// </summary>
    [JsonProperty("content")]
    public string? Content { get; set; }

    /// <summary>
    /// Multiple content queries to search for
    /// </summary>
    [JsonProperty("Multiple content queries to search for")]
    public HashSet<string>? Contents { get; set; }

    /// <summary>
    /// Exact phrases that must appear contiguously in message content
    /// </summary>
    [JsonProperty("exact_phrases")]
    public HashSet<string>? ExactPhrases { get; set; }

    /// <summary>
    /// Channel IDs to search in
    /// </summary>
    [JsonProperty("channel_id")]
    public HashSet<ulong>? ChannelIds { get; set; }

    /// <summary>
    /// Channel IDs to exclude from search
    /// </summary>
    [JsonProperty("exclude_channel_id")]
    public HashSet<ulong>? ExcludeChannelIds { get; set; }

    /// <summary>
    /// Author types to filter by
    /// </summary>
    [JsonProperty("author_type")]
    public HashSet<string>? AuthorTypes { get; set; }

    /// <summary>
    /// Author types to exclude
    /// </summary>
    [JsonProperty("exclude_author_type")]
    public HashSet<string>? ExcludeAuthorTypes { get; set; }

    /// <summary>
    /// Author user IDs to filter by
    /// </summary>
    [JsonProperty("author_id")]
    public HashSet<ulong>? AuthorId { get; set; }

    /// <summary>
    /// Author user IDs to exclude
    /// </summary>
    [JsonProperty("exclude_author_id")]
    public HashSet<ulong>? ExcludeAuthorId { get; set; }

    /// <summary>
    /// User IDs that must be mentioned
    /// </summary>
    [JsonProperty("mentions")]
    public HashSet<ulong>? MentionUserIds { get; set; }

    /// <summary>
    /// User IDs that must not be mentioned
    /// </summary>
    [JsonProperty("exclude_mentions")]
    public HashSet<ulong>? ExcludeMentionUserIds { get; set; }

    /// <summary>
    /// Filter by whether message mentions everyone
    /// </summary>
    [JsonProperty("mention_everyone")]
    public bool? MentionEveryone { get; set; }

    /// <summary>
    /// Filter by pinned status
    /// </summary>
    [JsonProperty("pinned")]
    public bool? Pinned { get; set; }

    /// <summary>
    /// Content types the message must have
    /// </summary>
    [JsonProperty("has")]
    public HashSet<string>? HasContentType { get; set; }

    /// <summary>
    /// Content types the message must not have
    /// </summary>
    [JsonProperty("exclude_has")]
    public HashSet<string>? ExcludeContentType { get; set; }

    /// <summary>
    /// Embed types to filter by
    /// </summary>
    [JsonProperty("embed_type")]
    public HashSet<string>? EmbedType { get; set; }

    /// <summary>
    /// Embed types to exclude
    /// </summary>
    [JsonProperty("exclude_embed_type")]
    public HashSet<string>? ExcludeEmbedType { get; set; }

    /// <summary>
    /// Embed providers to filter by
    /// </summary>
    [JsonProperty("embed_provider")]
    public HashSet<string>? EmbedProvider { get; set; }

    /// <summary>
    /// Embed providers to exclude
    /// </summary>
    [JsonProperty("exclude_embed_provider")]
    public HashSet<string>? ExcludeEmbedProvider { get; set; }

    /// <summary>
    /// Link hostnames to filter by
    /// </summary>
    [JsonProperty("link_hostname")]
    public HashSet<string>? LinkHostnames { get; set; }

    /// <summary>
    /// Link hostnames to exclude
    /// </summary>
    [JsonProperty("exclude_link_hostname")]
    public HashSet<string>? ExcludeLinkHostnames { get; set; }

    /// <summary>
    /// Attachment filenames to filter by
    /// </summary>
    [JsonProperty("attachment_filename")]
    public HashSet<string>? AttachmentFilenames { get; set; }

    /// <summary>
    /// Attachment filenames to exclude
    /// </summary>
    [JsonProperty("exclude_attachment_filename")]
    public HashSet<string>? ExcludeAttachmentFilenames { get; set; }

    /// <summary>
    /// File extensions to filter by
    /// </summary>
    [JsonProperty("attachment_extension")]
    public HashSet<string>? AttachmentExtensions { get; set; }

    /// <summary>
    /// File extensions to exclude
    /// </summary>
    [JsonProperty("exclude_attachment_extension")]
    public HashSet<string>? ExcludeAttachmentExtensions { get; set; }

    /// <summary>
    /// Field to sort results by
    /// </summary>
    [JsonProperty("sort_by")]
    public string? SortBy { get; set; }

    /// <summary>
    /// Sort order for results
    /// </summary>
    [JsonProperty("sort_order")]
    public string? SortOrder { get; set; }

    /// <summary>
    /// Whether to include NSFW channel results
    /// </summary>
    [JsonProperty("include_nsfw")]
    public bool? IncludeNsfw { get; set; }

    [JsonProperty("scope")]
    public string? MessageSearchScope { get; set; }
}
