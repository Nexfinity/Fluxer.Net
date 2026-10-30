using Newtonsoft.Json;

namespace Fluxer.Net;

/// <inheritdoc />
public class GlobalSearchJson : IGlobalSearch
{
    /// <inheritdoc />
    [JsonProperty("messages")]
    public MessageJson[] Messages { get; set; }

    /// <inheritdoc />
    [JsonProperty("channels")]
    public ChannelJson[] Channels { get; set; }

    /// <inheritdoc />
    [JsonProperty("total")]
    public ulong Total { get; set; }

    /// <inheritdoc />
    [JsonProperty("hits_per_page")]
    public int HitsPerPage { get; set; }

    /// <inheritdoc />
    [JsonProperty("page")]
    public int Page { get; set; }

    /// <summary>
    /// Pass this as <c>Cursor</c> on the next search request to get the next page, or null on the last page.
    /// </summary>
    [JsonProperty("cursor")]
    public string[]? Cursor { get; set; }

    IMessage[] IGlobalSearch.Messages => Messages;

    IChannel[] IGlobalSearch.Channels => Channels;
}
