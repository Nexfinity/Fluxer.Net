using Newtonsoft.Json;

namespace Fluxer.Net;

/// <summary>
/// A message that mentioned the user, as the API sends it.
/// </summary>
public class RecentMentionJson
{
    /// <summary>
    /// The id of the mentioned user.
    /// </summary>
    [JsonProperty("user_id")]
    public ulong UserId { get; set; }

    /// <summary>
    /// The id of the channel the message was sent in.
    /// </summary>
    [JsonProperty("channel_id")]
    public ulong ChannelId { get; set; }

    /// <summary>
    /// The id of the message.
    /// </summary>
    [JsonProperty("message_id")]
    public ulong MessageId { get; set; }

    /// <summary>
    /// The id of the guild the message was sent in.
    /// </summary>
    [JsonProperty("guild_id")]
    public ulong GuildId { get; set; }

    /// <summary>
    /// Whether the message mentioned everyone.
    /// </summary>
    [JsonProperty("is_everyone")]
    public bool IsEveryone { get; set; }

    /// <summary>
    /// Whether the message mentioned a role.
    /// </summary>
    [JsonProperty("is_role")]
    public bool IsRole { get; set; }
}
