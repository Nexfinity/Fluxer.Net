namespace Fluxer.Net;

/// <inheritdoc />
public class Channel : Entity, IChannel
{
    /// <inheritdoc />
    public ulong Id { get; internal set; }

    /// <inheritdoc />
    public string Mention => $"<#{Id}>";

    /// <inheritdoc />
    public ulong? GuildId { get; internal set; }

    /// <inheritdoc />
    public ChannelType Type { get; internal set; }

    /// <inheritdoc />
    public string? Name { get; internal set; }

    /// <inheritdoc />
    public string? Topic { get; internal set; }

    /// <inheritdoc />
    public string? IconHash { get; internal set; }

    /// <inheritdoc />
    public string? Url { get; internal set; }

    /// <inheritdoc />
    public ulong? ParentId { get; internal set; }

    /// <inheritdoc />
    public int Position { get; internal set; }

    /// <inheritdoc />
    public ulong? OwnerId { get; internal set; }

    /// <inheritdoc />
    public HashSet<ulong>? RecipientIds { get; internal set; }

    /// <inheritdoc />
    public bool IsNsfw { get; internal set; }

    /// <inheritdoc />
    public int RateLimitPerUser { get; internal set; }

    /// <inheritdoc />
    public int? Bitrate { get; internal set; }

    /// <inheritdoc />
    public int? UserLimit { get; internal set; }

    /// <inheritdoc />
    public string? RtcRegion { get; internal set; }

    /// <inheritdoc />
    public ulong? LastMessageId { get; internal set; }

    /// <inheritdoc />
    public DateTime? LastPinAt { get; internal set; }

    /// <inheritdoc />
    public IEnumerable<PermissionOverwrite>? PermissionOverwrites { get; internal set; }

    /// <inheritdoc />
    public Dictionary<string, string>? Nicknames { get; internal set; }

    /// <inheritdoc />
    public bool IsSoftDeleted { get; internal set; }

    /// <inheritdoc />
    public DateTime? IndexedAt { get; internal set; }

    IEnumerable<IPermissionOverwrite>? IChannel.PermissionOverwrites => PermissionOverwrites;

    public bool IsTextable { get; internal set; }

    internal Channel(FluxerBaseClient client) : base(client)
    {

    }

    /// <summary>
    /// Create a Channel object from json.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="json"></param>
    /// <returns></returns>
    public static Channel Create(FluxerBaseClient client, ChannelJson json)
    {
        Channel data = null;

        switch (json.Type)
        {
            case ChannelType.GuildText:
                {
                    data = new TextChannel(client);
                    data.IsTextable = true;
                }
                break;
            case ChannelType.GuildVoice:
                {
                    data = new VoiceChannel(client);
                }
                break;
            case ChannelType.Dm:
                {
                    data = new DMChannel(client);
                    data.IsTextable = true;
                }
                break;
            case ChannelType.DmPersonalNotes:
                {
                    data = new SavedNotesChannel(client);
                    data.IsTextable = true;
                }
                break;
            case ChannelType.Group:
                {
                    data = new GroupChannel(client);
                    data.IsTextable = true;
                }
                break;
            case ChannelType.GuildCategory:
                {
                    data = new CategoryChannel(client);
                }
                break;
            case ChannelType.GuildLink:
                {
                    data = new LinkChannel(client);
                }
                break;
            default:
                {
                    if (data.GuildId.HasValue)
                        data = new GuildChannel(client);
                    else
                        data = new Channel(client);
                    data.IsTextable = true;
                }
                break;
        }
        data.Update(client, json);
        return data;
    }

    internal virtual void Update(FluxerBaseClient client, ChannelJson json)
    {
        Id = json.Id;
        GuildId = json.GuildId;
        Type = json.Type;
        Name = json.Name;
        Topic = json.Topic;
        IconHash = json.IconHash;
        Url = json.Url;
        ParentId = json.ParentId;
        Position = json.Position;
        OwnerId = json.OwnerId;
        RecipientIds = json.RecipientIds;
        IsNsfw = json.IsNsfw;
        RateLimitPerUser = json.RateLimitPerUser;
        Bitrate = json.Bitrate;
        UserLimit = json.UserLimit;
        RtcRegion = json.RtcRegion;
        LastMessageId = json.LastMessageId;
        LastPinAt = json.LastPinAt;
        if (json.PermissionOverwrites != null)
            PermissionOverwrites = json.PermissionOverwrites.Select(x => PermissionOverwrite.Create(client, x));
        Nicknames = json.Nicknames;
        IsSoftDeleted = json.IsSoftDeleted;
        IndexedAt = json.IndexedAt;
    }
}
