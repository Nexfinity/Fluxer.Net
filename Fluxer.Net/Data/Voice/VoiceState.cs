namespace Fluxer.Net;

public class VoiceState : Entity, IVoiceState
{
    /// <inheritdoc />
    public string SessionId { get; set; }

    /// <inheritdoc />
    public ulong UserId { get; set; }

    /// <inheritdoc />
    public ulong? ChannelId { get; set; }

    /// <inheritdoc />
    public string? ConnectionId { get; set; }

    /// <inheritdoc />
    public bool IsDeaf { get; set; }

    /// <inheritdoc />
    public bool IsMute { get; set; }

    /// <inheritdoc />
    public ulong? GuildId { get; set; }

    /// <inheritdoc />
    public bool IsMobile { get; set; }

    /// <inheritdoc />
    public bool IsSelfDeaf { get; set; }

    /// <inheritdoc />
    public bool IsSelfMute { get; set; }

    /// <inheritdoc />
    public bool IsSelfStream { get; set; }

    /// <inheritdoc />
    public bool IsSelfVideo { get; set; }

    /// <inheritdoc />
    public string[] ViewerStreamKeys { get; set; }

    /// <inheritdoc />
    public GuildMember Member { get; set; }

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
        var data = new VoiceState(client);
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
        ViewerStreamKeys = json.ViewerStreamKeys;
        Member = GuildMember.Create(client, json.Member);
    }
}
