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

    IMessage[] IGlobalSearch.Messages => Messages;

    IChannel[] IGlobalSearch.Channels => Channels;
}
