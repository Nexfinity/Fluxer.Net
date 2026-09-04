namespace Fluxer.Net;

/// <inheritdoc />
public class VoiceState : Entity, IVoiceState
{
    /// <inheritdoc />
    public string SessionId { get; private set; }

    /// <inheritdoc />
    public ulong UserId { get; private set; }

    /// <inheritdoc />
    public ulong? ChannelId { get; private set; }

    /// <inheritdoc />
    public string? ConnectionId { get; private set; }

    /// <inheritdoc />
    public bool IsDeaf { get; private set; }

    /// <inheritdoc />
    public bool IsMute { get; private set; }

    /// <inheritdoc />
    public ulong? GuildId { get; private set; }

    /// <inheritdoc />
    public bool IsMobile { get; private set; }

    /// <inheritdoc />
    public bool IsSelfDeaf { get; private set; }

    /// <inheritdoc />
    public bool IsSelfMute { get; private set; }

    /// <inheritdoc />
    public bool IsSelfStream { get; private set; }

    /// <inheritdoc />
    public bool IsSelfVideo { get; private set; }

    /// <inheritdoc />
    public bool IsSuppressed { get; private set; }

    /// <inheritdoc />
    public string[] ViewerStreamKeys { get; private set; }

    /// <inheritdoc />
    public GuildMember Member { get; private set; }

    IGuildMember IVoiceState.Member => Member;

    internal VoiceState(FluxerBaseClient client) : base(client)
    {

    }

    /// <summary>
    /// Create a VoiceState object from json.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="json"></param>
    /// <returns></returns>
    public static VoiceState Create(FluxerBaseClient client, VoiceStateJson json)
    {
        VoiceState data = new VoiceState(client);
        data.Update(json);
        return data;
    }

    internal virtual void Update(VoiceStateJson json)
    {
        SessionId = json.SessionId;
        UserId = json.UserId;
        ChannelId = json.ChannelId;
        ConnectionId = json.ConnectionId;
        IsDeaf = json.IsDeaf;
        IsMute = json.IsMute;
        GuildId = json.GuildId;
        IsMobile = json.IsMobile;
        IsSelfDeaf = json.IsSelfDeaf;
        IsSelfMute = json.IsSelfMute;
        IsSelfStream = json.IsSelfStream;
        IsSelfVideo = json.IsSelfVideo;
        IsSuppressed = json.IsSuppressed;
        ViewerStreamKeys = json.ViewerStreamKeys;
        Member = GuildMember.Create(Client, json.Member);
    }
}
