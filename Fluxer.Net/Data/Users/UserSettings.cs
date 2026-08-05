namespace Fluxer.Net;

/// <inheritdoc />
public class UserSettings : Entity, IUserSettings
{
    /// <inheritdoc />
    public ulong UserId { get; set; }

    /// <inheritdoc />
    public string Locale { get; set; }

    /// <inheritdoc />
    public string Theme { get; set; }

    /// <inheritdoc />
    public string Status { get; set; }

    /// <inheritdoc />
    public UserCustomStatusJson? CustomStatus { get; set; }

    /// <inheritdoc />
    public bool DeveloperMode { get; set; }

    /// <inheritdoc />
    public bool CompactMessageDisplay { get; set; }

    /// <inheritdoc />
    public bool AnimateEmoji { get; set; }

    /// <inheritdoc />
    public int AnimateStickers { get; set; }

    /// <inheritdoc />
    public bool GifAutoPlay { get; set; }

    /// <inheritdoc />
    public bool RenderEmbeds { get; set; }

    /// <inheritdoc />
    public bool RenderReactions { get; set; }

    /// <inheritdoc />
    public int RenderSpoilers { get; set; }

    /// <inheritdoc />
    public bool InlineAttachmentMedia { get; set; }

    /// <inheritdoc />
    public bool InlineEmbedMedia { get; set; }

    /// <inheritdoc />
    public int ExplicitContentFilter { get; set; }

    /// <inheritdoc />
    public int FriendSourceFlags { get; set; }

    /// <inheritdoc />
    public int IncomingCallFlags { get; set; }

    /// <inheritdoc />
    public int GroupDmAddPermissionFlags { get; set; }

    /// <inheritdoc />
    public bool DefaultGuildsRestricted { get; set; }

    /// <inheritdoc />
    public List<ulong>? RestrictedGuilds { get; set; }

    /// <inheritdoc />
    public List<ulong>? GuildPositions { get; set; }

    /// <inheritdoc />
    public List<UserGuildFolderJson>? GuildFolders { get; set; }

    /// <inheritdoc />
    public int AfkTimeout { get; set; }

    /// <inheritdoc />
    public int TimeFormat { get; set; }

    internal UserSettings(FluxerBaseClient client) : base(client)
    {

    }

    /// <summary>
    /// Create a UserSettings object from json.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="json"></param>
    /// <returns></returns>
    public static UserSettings Create(FluxerBaseClient client, UserSettingsJson json)
    {
        UserSettings data = new UserSettings(client);
        data.Update(client, json);
        return data;
    }

    internal void Update(FluxerBaseClient client, UserSettingsJson json)
    {
        UserId = json.UserId;
        Locale = json.Locale;
        Theme = json.Theme;
        Status = json.Status;
        CustomStatus = json.CustomStatus;
        DeveloperMode = json.DeveloperMode;
        CompactMessageDisplay = json.CompactMessageDisplay;
        AnimateEmoji = json.AnimateEmoji;
        AnimateStickers = json.AnimateStickers;
        GifAutoPlay = json.GifAutoPlay;
        RenderEmbeds = json.RenderEmbeds;
        RenderReactions = json.RenderReactions;
        RenderSpoilers = json.RenderSpoilers;
        InlineAttachmentMedia = json.InlineAttachmentMedia;
        InlineEmbedMedia = json.InlineEmbedMedia;
        ExplicitContentFilter = json.ExplicitContentFilter;
        FriendSourceFlags = json.FriendSourceFlags;
        IncomingCallFlags = json.IncomingCallFlags;
        GroupDmAddPermissionFlags = json.GroupDmAddPermissionFlags;
        DefaultGuildsRestricted = json.DefaultGuildsRestricted;
        RestrictedGuilds = json.RestrictedGuilds;
        GuildPositions = json.GuildPositions;
        GuildFolders = json.GuildFolders;
        AfkTimeout = json.AfkTimeout;
        TimeFormat = json.TimeFormat;
    }
}
