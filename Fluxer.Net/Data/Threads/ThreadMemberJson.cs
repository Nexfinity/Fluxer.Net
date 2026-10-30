using Newtonsoft.Json;

namespace Fluxer.Net;

/// <inheritdoc />
public class ThreadMemberJson : IThreadMember
{
    /// <inheritdoc />
    [JsonProperty("id")]
    public ulong ThreadId { get; set; }

    /// <inheritdoc />
    [JsonProperty("user_id")]
    public ulong Id { get; set; }

    /// <inheritdoc />
    [JsonProperty("join_timestamp")]
    public DateTimeOffset JoinedAt { get; set; }

    /// <inheritdoc />
    [JsonProperty("flags")]
    public int Flags { get; set; }

    /// <summary>
    /// Whether the user has muted the thread. Only sent for the current user.
    /// </summary>
    [JsonProperty("muted")]
    public bool? Muted { get; set; }

    /// <inheritdoc />
    [JsonProperty("member")]
    public GuildMemberJson? Member { get; set; }

    IGuildMember? IThreadMember.Member => Member;
}
