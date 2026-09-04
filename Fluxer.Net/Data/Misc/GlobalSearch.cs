namespace Fluxer.Net;

/// <inheritdoc />
public class GlobalSearch : Entity, IGlobalSearch
{
    /// <inheritdoc />
    public Message[] Messages { get; private set; }

    /// <inheritdoc />
    public Channel[] Channels { get; private set; }

    /// <inheritdoc />
    public ulong Total { get; private set; }

    /// <inheritdoc />
    public int HitsPerPage { get; private set; }

    /// <inheritdoc />
    public int Page { get; private set; }

    IMessage[] IGlobalSearch.Messages => Messages;

    IChannel[] IGlobalSearch.Channels => Channels;

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
            Messages = json.Messages != null ? json.Messages.Select(x => Message.Create(client, x)).ToArray() : Array.Empty<Message>(),
            Channels = json.Channels != null ? json.Channels.Select(x => Channel.Create(client, x)).ToArray() : Array.Empty<Channel>(),
            HitsPerPage = json.HitsPerPage,
            Page = json.Page,
            Total = json.Total
        };
    }
}