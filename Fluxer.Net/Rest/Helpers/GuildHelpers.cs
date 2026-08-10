using Fluxer.Net.Rest;

namespace Fluxer.Net;

/// <summary>
/// Http methods for <see cref="Guild"/> class. 
/// </summary>
public static class GuildHelpers
{
    /// <inheritdoc cref="ApiClient.LeaveGuildAsync(ulong)" />
    public static Task LeaveAsync(this Guild guild)
        => guild.Client.Rest.LeaveGuildAsync(guild.Id);

    /// <inheritdoc cref="ApiClient.UpdateGuildAsync(ulong, GuildJson)" />
    public static Task<Guild> ModifyAsync(this Guild guild, GuildJson request)
        => guild.Client.Rest.UpdateGuildAsync(guild.Id, request);

    /// <inheritdoc cref="ApiClient.SearchAuditLogAsync(ulong, GuildAuditLogListRequest)" />
    public static Task<GuildAuditLogListJson> SearchAuditLogsAsync(this Guild guild, GuildAuditLogListRequest request)
        => guild.Client.Rest.SearchAuditLogAsync(guild.Id, request);

    /// <inheritdoc cref="ApiClient.GetBansAsync(ulong)" />
    public static Task<IEnumerable<GuildBan>> GetBansAsync(this Guild guild)
        => guild.Client.Rest.GetBansAsync(guild.Id);

    /// <inheritdoc cref="ApiClient.UnbanMemberAsync(ulong, ulong)" />
    public static Task UnbanUserAsync(this Guild guild, ulong userId)
        => guild.Client.Rest.UnbanMemberAsync(guild.Id, userId);

    /// <inheritdoc cref="ApiClient.UnbanMemberAsync(ulong, ulong)" />
    public static Task UnbanUserAsync(this Guild guild, User user)
        => guild.Client.Rest.UnbanMemberAsync(guild.Id, user.Id);

    /// <inheritdoc cref="ApiClient.GetChannelsAsync(ulong)" />
    public static Task<IEnumerable<Channel>> GetChannelsAsync(this Guild guild)
        => guild.Client.Rest.GetChannelsAsync(guild.Id);

    /// <inheritdoc cref="ApiClient.CreateGuildChannelAsync(ulong, CreateGuildChannelRequest)" />
    public static Task<Channel> CreateChannelAsync(this Guild guild, CreateGuildChannelRequest request)
        => guild.Client.Rest.CreateGuildChannelAsync(guild.Id, request);

    /// <inheritdoc cref="ApiClient.GetEmojisAsync(ulong)" />
    public static Task<IEnumerable<GuildEmoji>> GetEmojisAsync(this Guild guild)
        => guild.Client.Rest.GetEmojisAsync(guild.Id);

    /// <inheritdoc cref="ApiClient.CreateEmojiAsync(ulong, CreateGuildEmojiRequest)" />
    public static Task<GuildEmoji> CreateEmojiAsync(this Guild guild, CreateGuildEmojiRequest request)
        => guild.Client.Rest.CreateEmojiAsync(guild.Id, request);

    /// <inheritdoc cref="ApiClient.GetStickersAsync(ulong)" />
    public static Task GetStickersAsync(this Guild guild)
        => guild.Client.Rest.GetStickersAsync(guild.Id);

    /// <inheritdoc cref="ApiClient.CreateStickerAsync(ulong, CreateGuildStickerRequest)" />
    public static Task<GuildSticker> CreateStickerAsync(this Guild guild, CreateGuildStickerRequest request)
        => guild.Client.Rest.CreateStickerAsync(guild.Id, request);

    /// <inheritdoc cref="ApiClient.GetMembersAsync(ulong, int, ulong?, RestClientQueryParams?)" />
    public static Task<IEnumerable<GuildMember>> GetMembersAsync(this Guild guild, int limit = 1000, ulong? afterId = null, RestClientQueryParams? queryParams = null)
        => guild.Client.Rest.GetMembersAsync(guild.Id, limit, afterId, queryParams);

    /// <inheritdoc cref="ApiClient.GetMemberAsync(ulong, ulong)" />
    public static Task<GuildMember> GetMemberAsync(this Guild guild, ulong userId)
        => guild.Client.Rest.GetMemberAsync(guild.Id, userId);

    /// <inheritdoc cref="ApiClient.GetMemberAsync(ulong, ulong)" />
    public static Task<GuildMember> GetMemberAsync(this Guild guild, User user)
        => guild.Client.Rest.GetMemberAsync(guild.Id, user.Id);

    /// <inheritdoc cref="ApiClient.CreateRoleAsync(ulong, CreateGuildRoleRequest)" />
    public static Task<Role> CreateRoleAsync(this Guild guild, CreateGuildRoleRequest request)
        => guild.Client.Rest.CreateRoleAsync(guild.Id, request);

    /// <inheritdoc cref="ApiClient.GetGuildVanityUrlAsync(ulong)" />
    public static Task<GuildVanityUrl> GetVanityUrlAsync(this Guild guild)
        => guild.Client.Rest.GetGuildVanityUrlAsync(guild.Id);

    /// <inheritdoc cref="ApiClient.GetGuildWebhooksAsync(ulong)" />
    public static Task<IEnumerable<Webhook>> GetWebhooksAsync(this Guild guild)
        => guild.Client.Rest.GetGuildWebhooksAsync(guild.Id);

    /// <inheritdoc cref="ApiClient.GetGuildInvitesAsync(ulong)" />
    public static Task<IEnumerable<Invite>> GetInvitesAsync(this Guild guild)
        => guild.Client.Rest.GetGuildInvitesAsync(guild.Id);
}
