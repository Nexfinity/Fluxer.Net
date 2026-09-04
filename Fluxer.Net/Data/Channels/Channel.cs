namespace Fluxer.Net;

/// <inheritdoc />
public class Channel : Entity, IChannel
{
    /// <inheritdoc />
    public ulong Id { get; private set; }

    /// <inheritdoc />
    public DateTimeOffset CreatedAt => SnowflakeUtils.FromSnowflake(Id);

    /// <inheritdoc />
    public string Mention => $"<#{Id}>";

    /// <inheritdoc />
    public ulong? GuildId { get; internal set; }

    /// <inheritdoc />
    public ChannelType Type { get; private set; }

    /// <inheritdoc />
    public string? Name { get; private set; }

    /// <inheritdoc />
    public string? Topic { get; private set; }

    /// <inheritdoc />
    public string? IconHash { get; private set; }

    /// <inheritdoc />
    public string? Url { get; private set; }

    /// <inheritdoc />
    public ulong? ParentId { get; private set; }

    /// <inheritdoc />
    public int Position { get; private set; }

    /// <inheritdoc />
    public ulong? OwnerId { get; private set; }

    /// <inheritdoc />
    public HashSet<ulong>? RecipientIds { get; private set; }

    /// <inheritdoc />
    public bool IsNsfw { get; private set; }

    /// <inheritdoc />
    public int RateLimitPerUser { get; private set; }

    /// <inheritdoc />
    public int? Bitrate { get; private set; }

    /// <inheritdoc />
    public int? UserLimit { get; private set; }

    /// <inheritdoc />
    public string? RtcRegion { get; private set; }

    /// <inheritdoc />
    public ulong? LastMessageId { get; private set; }

    /// <inheritdoc />
    public DateTimeOffset? LastPinAt { get; private set; }

    /// <inheritdoc />
    public IEnumerable<PermissionOverwrite>? PermissionOverwrites { get; private set; }

    /// <inheritdoc />
    public Dictionary<string, string>? Nicknames { get; private set; }

    /// <inheritdoc />
    public bool IsSoftDeleted { get; private set; }

    /// <inheritdoc />
    public DateTimeOffset? IndexedAt { get; private set; }

    IEnumerable<IPermissionOverwrite>? IChannel.PermissionOverwrites => PermissionOverwrites;

    /// <inheritdoc/>
    public bool IsTextable => TextableTypes(Type);

    /// <summary>
    /// Channel types that you can send messages to.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    static public bool TextableTypes(ChannelType type)
    {
        switch (type)
        {
            case ChannelType.Dm:
            case ChannelType.DmPersonalNotes:
            case ChannelType.Group:
            case ChannelType.GuildForum:
            case ChannelType.GuildMedia:
            case ChannelType.GuildNews:
            case ChannelType.GuildStageVoice:
            case ChannelType.GuildText:
            case ChannelType.GuildVoice:
            case ChannelType.NewsThread:
            case ChannelType.PrivateThread:
            case ChannelType.PublicThread:
                return true;
        }

        return false;
    }

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
                }
                break;
            case ChannelType.DmPersonalNotes:
                {
                    data = new SavedNotesChannel(client);
                }
                break;
            case ChannelType.Group:
                {
                    data = new GroupChannel(client);
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
