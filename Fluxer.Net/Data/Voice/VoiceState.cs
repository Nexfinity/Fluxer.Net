namespace Fluxer.Net;

/// <inheritdoc />
public class VoiceState : Entity, IVoiceState
{
    /// <inheritdoc />
    public string SessionId { get; internal set; }

    /// <inheritdoc />
    public ulong UserId { get; internal set; }

    /// <inheritdoc />
    public ulong? ChannelId { get; internal set; }

    /// <inheritdoc />
    public string? ConnectionId { get; internal set; }

    /// <inheritdoc />
    public bool IsDeaf { get; internal set; }

    /// <inheritdoc />
    public bool IsMute { get; internal set; }

    /// <inheritdoc />
    public ulong? GuildId { get; internal set; }

    /// <inheritdoc />
    public bool IsMobile { get; internal set; }

    /// <inheritdoc />
    public bool IsSelfDeaf { get; internal set; }

    /// <inheritdoc />
    public bool IsSelfMute { get; internal set; }

    /// <inheritdoc />
    public bool IsSelfStream { get; internal set; }

    /// <inheritdoc />
    public bool IsSelfVideo { get; internal set; }

    /// <inheritdoc />
    public bool IsSuppressed { get; internal set; }

    /// <inheritdoc />
    public string[] ViewerStreamKeys { get; internal set; }

    /// <inheritdoc />
    public GuildMember Member { get; internal set; }

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
        data.Update(client, json);
        return data;
    }

    internal virtual void Update(FluxerBaseClient client, VoiceStateJson json)
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
        Member = GuildMember.Create(client, json.Member);
    }
}
