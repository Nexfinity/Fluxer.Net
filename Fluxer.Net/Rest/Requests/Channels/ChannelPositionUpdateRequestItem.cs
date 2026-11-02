using Newtonsoft.Json;

namespace Fluxer.Net.Rest;

/// <summary>
/// One channel's new position within a guild's channel list.
/// </summary>
public class ChannelPositionUpdateRequestItem
{
    /// <summary>
    /// The ID of the channel to move.
    /// </summary>
    [JsonProperty("id")]
    public ulong Id { get; set; }

    /// <summary>
    /// Whether to lock the channel's permissions to its parent category.
    /// </summary>
    [JsonProperty("lock_permissions")]
    public bool? LockPermissions { get; set; }

    /// <summary>
    /// The ID of the category to move the channel into.
    /// </summary>
    [JsonProperty("parent_id")]
    public ulong? ParentId { get; set; }

    /// <summary>
    /// The new position of the channel relative to other channels.
    /// </summary>
    [JsonProperty("position")]
    public int? Position { get; set; }

    /// <summary>
    /// The ID of the channel to place this channel after.
    /// </summary>
    [JsonProperty("preceding_sibling_id")]
    public ulong? PrecedingSiblingId { get; set; }
}
