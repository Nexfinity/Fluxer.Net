using Newtonsoft.Json;

namespace Fluxer.Net.Rest;

/// <summary>
/// Base request body for creating a channel in a guild.
/// </summary>
public abstract class CreateGuildChannelRequest
{
    /// <summary>
    /// The type of channel to create.
    /// </summary>
    [JsonProperty("type")]
    public abstract GuildChannelType Type { get; }

    /// <summary>
    /// The name of the channel.
    /// </summary>
    [JsonProperty("name")]
    public string Name { get; set; }

    /// <summary>
    /// The topic of the channel.
    /// </summary>
    [JsonProperty("topic")]
    public string? Topic { get; set; }

    /// <summary>
    /// The ID of the category to create the channel in.
    /// </summary>
    [JsonProperty("parent_id")]
    public ulong? ParentCategoryId { get; set; }

    /// <summary>
    /// Whether the channel is marked as NSFW.
    /// </summary>
    [JsonProperty("nsfw")]
    public bool Nsfw { get; set; }

    /// <summary>
    /// The slowmode delay in seconds between messages from one user.
    /// </summary>
    [JsonProperty("rate_limit_per_user")]
    public int? RatelimitPerUser { get; set; }

    /// <summary>
    /// The permission overwrites to apply to the channel.
    /// </summary>
    [JsonProperty("permission_overwrites")]
    public ChannelOverwriteRequest[]? PermissionOverwrites { get; set; }
}
