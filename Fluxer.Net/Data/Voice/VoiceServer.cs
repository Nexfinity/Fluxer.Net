namespace Fluxer.Net;

/// <inheritdoc />
public class VoiceServer : Entity, IVoiceServer
{
    /// <inheritdoc />
    public string Token { get; private set; }

    /// <inheritdoc />
    public string Endpoint { get; private set; }

    /// <inheritdoc />
    public string ConnectionId { get; private set; }

    /// <inheritdoc />
    public ulong ChannelId { get; private set; }

    /// <inheritdoc />
    public ulong? GuildId { get; private set; }

    /// <inheritdoc />
    public string? E2EEKey { get; private set; }

    internal VoiceServer(FluxerBaseClient client) : base(client)
    {

    }

    /// <summary>
    /// Create a VoiceServer object from json.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="json"></param>
    /// <returns></returns>
    public static VoiceServer Create(FluxerBaseClient client, VoiceServerJson json)
    {
        VoiceServer data = new VoiceServer(client);
        data.Update(json);
        return data;
    }

    internal virtual void Update(VoiceServerJson json)
    {
        Token = json.Token;
        Endpoint = json.Endpoint;
        ConnectionId = json.ConnectionId;
        ChannelId = json.ChannelId;
        GuildId = json.GuildId;
        E2EEKey = json.E2EEKey;
    }
}
