using Fluxer.Net.Rest;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Fluxer.Net;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Http methods for <see cref="Guild"/> class. 
/// </summary>
public static class GuildHelpers
{
    /// <inheritdoc cref="FluxerApiClient.LeaveGuildAsync(ulong, RequestOptions?)" />
    public static Task LeaveAsync(this Guild guild, RequestOptions? options = null)
        => guild.Client.Rest.LeaveGuildAsync(guild.Id, options);

    /// <inheritdoc cref="FluxerApiClient.UpdateGuildAsync(ulong, GuildJson, RequestOptions?)" />
    [Obsolete("Every property of GuildJson is sent, so this also resets settings that were not meant to change. Use the UpdateGuildRequest overload.")]
    public static Task<Guild> ModifyAsync(this Guild guild, GuildJson request, RequestOptions? options = null)
        => guild.Client.Rest.UpdateGuildAsync(guild.Id, request, options);

    /// <inheritdoc cref="FluxerApiClient.UpdateGuildAsync(ulong, UpdateGuildRequest, RequestOptions?)" />
    public static Task<Guild> ModifyAsync(this Guild guild, UpdateGuildRequest request, RequestOptions? options = null)
        => guild.Client.Rest.UpdateGuildAsync(guild.Id, request, options);

    /// <inheritdoc cref="FluxerApiClient.UpdateChannelPositionsAsync(ulong, IEnumerable{ChannelPositionUpdateRequestItem}, RequestOptions?)" />
    public static Task UpdateChannelPositionsAsync(this Guild guild, IEnumerable<ChannelPositionUpdateRequestItem> list, RequestOptions? options = null)
        => guild.Client.Rest.UpdateChannelPositionsAsync(guild.Id, list, options);

    /// <inheritdoc cref="FluxerApiClient.GetAuditLogAsync(ulong, GuildAuditLogListRequest, RequestOptions?)" />
    public static Task<AuditLogListJson> GetAuditLogsAsync(this Guild guild, GuildAuditLogListRequest request, RequestOptions? options = null)
        => guild.Client.Rest.GetAuditLogAsync(guild.Id, request, options);

    /// <inheritdoc cref="FluxerApiClient.GetBansAsync(ulong, RequestOptions?)" />
    public static Task<IEnumerable<GuildBan>> GetBansAsync(this Guild guild, RequestOptions? options = null)
        => guild.Client.Rest.GetBansAsync(guild.Id, options);

    /// <inheritdoc cref="FluxerApiClient.BanMemberAsync(ulong, ulong, string?, int?, int?, RequestOptions?)" />
    public static Task BanUserAsync(this Guild guild, ulong userId, string? reason = null, int? deleteMessageSeconds = null, int? banDurationSeconds = null, RequestOptions? options = null)
        => guild.Client.Rest.BanMemberAsync(guild.Id, userId, reason, deleteMessageSeconds, banDurationSeconds, options);

    /// <inheritdoc cref="FluxerApiClient.BanMemberAsync(ulong, ulong, string?, int?, int?, RequestOptions?)" />
    public static Task BanUserAsync(this Guild guild, User user, string? reason = null, int? deleteMessageSeconds = null, int? banDurationSeconds = null, RequestOptions? options = null)
        => guild.Client.Rest.BanMemberAsync(guild.Id, user.Id, reason, deleteMessageSeconds, banDurationSeconds, options);

    /// <inheritdoc cref="FluxerApiClient.UnbanMemberAsync(ulong, ulong, RequestOptions?)" />
    public static Task UnbanUserAsync(this Guild guild, ulong userId, RequestOptions? options = null)
        => guild.Client.Rest.UnbanMemberAsync(guild.Id, userId, options);

    /// <inheritdoc cref="FluxerApiClient.UnbanMemberAsync(ulong, ulong, RequestOptions?)" />
    public static Task UnbanUserAsync(this Guild guild, User user, RequestOptions? options = null)
        => guild.Client.Rest.UnbanMemberAsync(guild.Id, user.Id, options);

    /// <inheritdoc cref="FluxerApiClient.KickMemberAsync(ulong, ulong, string?, RequestOptions?)" />
    public static Task KickUserAsync(this Guild guild, ulong userId, string? reason = null, RequestOptions? options = null)
        => guild.Client.Rest.KickMemberAsync(guild.Id, userId, reason, options);

    /// <inheritdoc cref="FluxerApiClient.KickMemberAsync(ulong, ulong, string?, RequestOptions?)" />
    public static Task KickUserAsync(this Guild guild, User user, string? reason = null, RequestOptions? options = null)
        => guild.Client.Rest.KickMemberAsync(guild.Id, user.Id, reason, options);

    /// <inheritdoc cref="FluxerApiClient.GetChannelsAsync(ulong, RequestOptions?)" />
    public static Task<IEnumerable<Channel>> GetChannelsAsync(this Guild guild, RequestOptions? options = null)
        => guild.Client.Rest.GetChannelsAsync(guild.Id, options);

    /// <inheritdoc cref="FluxerApiClient.CreateGuildChannelAsync(ulong, CreateGuildChannelRequest, RequestOptions?)" />
    public static Task<TextChannel> CreateTextChannelAsync(this Guild guild, CreateTextChannelRequest request, RequestOptions? options = null)
        => guild.Client.Rest.CreateTextChannelAsync(guild.Id, request, options);

    /// <inheritdoc cref="FluxerApiClient.CreateGuildChannelAsync(ulong, CreateGuildChannelRequest, RequestOptions?)" />
    public static Task<VoiceChannel> CreateVoiceChannelAsync(this Guild guild, CreateVoiceChannelRequest request, RequestOptions? options = null)
        => guild.Client.Rest.CreateVoiceChannelAsync(guild.Id, request, options);

    /// <inheritdoc cref="FluxerApiClient.CreateGuildChannelAsync(ulong, CreateGuildChannelRequest, RequestOptions?)" />
    public static Task<LinkChannel> CreateLinkChannelAsync(this Guild guild, CreateLinkChannelRequest request, RequestOptions? options = null)
        => guild.Client.Rest.CreateLinkChannelAsync(guild.Id, request, options);

    /// <inheritdoc cref="FluxerApiClient.CreateGuildChannelAsync(ulong, CreateGuildChannelRequest, RequestOptions?)" />
    public static Task<CategoryChannel> CreateCategoryChannelAsync(this Guild guild, CreateCategoryChannelRequest request, RequestOptions? options = null)
        => guild.Client.Rest.CreateCategoryChannelAsync(guild.Id, request, options);

    /// <inheritdoc cref="FluxerApiClient.CreateGuildChannelAsync(ulong, CreateGuildChannelRequest, RequestOptions?)" />
    public static Task<NewsChannel> CreateNewsChannelAsync(this Guild guild, CreateNewsChannelRequest request, RequestOptions? options = null)
        => guild.Client.Rest.CreateNewsChannelAsync(guild.Id, request, options);

    /// <inheritdoc cref="FluxerApiClient.CreateGuildChannelAsync(ulong, CreateGuildChannelRequest, RequestOptions?)" />
    public static Task<ForumChannel> CreateForumChannelAsync(this Guild guild, CreateForumChannelRequest request, RequestOptions? options = null)
        => guild.Client.Rest.CreateForumChannelAsync(guild.Id, request, options);

    /// <inheritdoc cref="FluxerApiClient.CreateGuildChannelAsync(ulong, CreateGuildChannelRequest, RequestOptions?)" />
    public static Task<MediaChannel> CreateMediaChannelAsync(this Guild guild, CreateMediaChannelRequest request, RequestOptions? options = null)
        => guild.Client.Rest.CreateMediaChannelAsync(guild.Id, request, options);

    /// <inheritdoc cref="FluxerApiClient.GetEmojisAsync(ulong, RequestOptions?)" />
    public static Task<IEnumerable<GuildEmoji>> GetEmojisAsync(this Guild guild, RequestOptions? options = null)
        => guild.Client.Rest.GetEmojisAsync(guild.Id, options);

    /// <inheritdoc cref="FluxerApiClient.CreateEmojiAsync(ulong, CreateGuildEmojiRequest, RequestOptions?)" />
    public static Task<GuildEmoji> CreateEmojiAsync(this Guild guild, CreateGuildEmojiRequest request, RequestOptions? options = null)
        => guild.Client.Rest.CreateEmojiAsync(guild.Id, request, options);

    /// <inheritdoc cref="FluxerApiClient.GetStickersAsync(ulong, RequestOptions?)" />
    public static Task GetStickersAsync(this Guild guild, RequestOptions? options = null)
        => guild.Client.Rest.GetStickersAsync(guild.Id, options);

    /// <inheritdoc cref="FluxerApiClient.CreateStickerAsync(ulong, CreateGuildStickerRequest, RequestOptions?)" />
    public static Task<GuildSticker> CreateStickerAsync(this Guild guild, CreateGuildStickerRequest request, RequestOptions? options = null)
        => guild.Client.Rest.CreateStickerAsync(guild.Id, request, options);

    /// <inheritdoc cref="FluxerApiClient.GetMembersAsync(ulong, int, ulong?, RestClientQueryParams?, RequestOptions?)" />
    public static Task<IEnumerable<GuildMember>> GetMembersAsync(this Guild guild, int limit = 1000, ulong? afterId = null, RestClientQueryParams? queryParams = null, RequestOptions? options = null)
        => guild.Client.Rest.GetMembersAsync(guild.Id, limit, afterId, queryParams, options);

    /// <inheritdoc cref="FluxerApiClient.GetMemberAsync(ulong, ulong, RequestOptions?)" />
    public static Task<GuildMember?> GetMemberAsync(this Guild guild, ulong userId, RequestOptions? options = null)
        => guild.Client.Rest.GetMemberAsync(guild.Id, userId, options);

    /// <inheritdoc cref="FluxerApiClient.GetMemberAsync(ulong, ulong, RequestOptions?)" />
    public static Task<GuildMember?> GetMemberAsync(this Guild guild, User user, RequestOptions? options = null)
        => guild.Client.Rest.GetMemberAsync(guild.Id, user.Id, options);

    /// <inheritdoc cref="FluxerApiClient.CreateRoleAsync(ulong, CreateGuildRoleRequest, RequestOptions?)" />
    public static Task<Role> CreateRoleAsync(this Guild guild, CreateGuildRoleRequest request, RequestOptions? options = null)
        => guild.Client.Rest.CreateRoleAsync(guild.Id, request, options);

    /// <inheritdoc cref="FluxerApiClient.GetGuildVanityUrlAsync(ulong, RequestOptions?)" />
    public static Task<GuildVanityUrl> GetVanityUrlAsync(this Guild guild, RequestOptions? options = null)
        => guild.Client.Rest.GetGuildVanityUrlAsync(guild.Id, options);

    /// <inheritdoc cref="FluxerApiClient.GetGuildWebhooksAsync(ulong, RequestOptions?)" />
    public static Task<Webhook?> GetWebhookAsync(this Guild guild, ulong webhookId, RequestOptions? options = null)
        => guild.Client.Rest.GetWebhookAsync(webhookId, options);

    /// <inheritdoc cref="FluxerApiClient.GetGuildWebhooksAsync(ulong, RequestOptions?)" />
    public static Task<IEnumerable<Webhook>> GetWebhooksAsync(this Guild guild, RequestOptions? options = null)
        => guild.Client.Rest.GetGuildWebhooksAsync(guild.Id, options);

    /// <inheritdoc cref="FluxerApiClient.GetGuildInvitesAsync(ulong, RequestOptions?)" />
    public static Task<IEnumerable<Invite>> GetInvitesAsync(this Guild guild, RequestOptions? options = null)
        => guild.Client.Rest.GetGuildInvitesAsync(guild.Id, options);

    /// <inheritdoc cref="FluxerApiClient.GetThreadChannelAsync(ulong, RequestOptions?)" />
    public static Task<ThreadChannel?> GetThreadChannelAsync(this Guild guild, ulong threadId, RequestOptions? options = null)
        => guild.Client.Rest.GetThreadChannelAsync(threadId, options);
}
