using Newtonsoft.Json;

namespace Fluxer.Net;

public class GlobalSearch : Entity
{
    [JsonProperty("messages")]
    public Message[] Messages { get; internal set; }

    [JsonProperty("channels")]
    public Channel[] Channels { get; internal set; }

    [JsonProperty("total")]
    public ulong Total { get; internal set; }

    [JsonProperty("hits_per_page")]
    public int HitsPerPage { get; internal set; }

    [JsonProperty("page")]
    public int Page { get; internal set; }

    internal GlobalSearch(FluxerBaseClient client) : base(client)
    {

    }

    /// <summary>
    /// Create a Instance object from json.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="json"></param>
    /// <returns></returns>
    public static GlobalSearch Create(FluxerBaseClient client, GlobalSearchJson json)
    {
        return new GlobalSearch(client)
        {
            Messages = json.Messages.Select(x => Message.Create(client, x)).ToArray(),
            Channels = json.Channels.Select(x => Channel.Create(client, x)).ToArray(),
            HitsPerPage = json.HitsPerPage,
            Page = json.Page,
            Total = json.Total
        };
    }
}