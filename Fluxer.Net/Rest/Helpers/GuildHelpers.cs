namespace Fluxer.Net;

public static class GuildHelpers
{
    public static Task LeaveAsync(this Guild guild)
        => guild.Client.Rest.LeaveGuildAsync(guild.Id);

    public static Task<Guild> ModifyAsync(this Guild guild, GuildJson request)
        => guild.Client.Rest.UpdateGuildAsync(guild.Id, request);

    public static Task<GuildAuditLogListJson> SearchAuditLogsAsync(this Guild guild, GuildAuditLogListRequest request)
        => guild.Client.Rest.SearchAuditLogAsync(guild.Id, request);

    public static Task<IEnumerable<GuildBan>> GetBansAsync(this Guild guild)
        => guild.Client.Rest.GetBansAsync(guild.Id);

    public static Task UnbanUserAsync(this Guild guild, ulong userId)
        => guild.Client.Rest.UnbanMemberAsync(guild.Id, userId);

    public static Task UnbanUserAsync(this Guild guild, User user)
        => guild.Client.Rest.UnbanMemberAsync(guild.Id, user.Id);

    public static Task<IEnumerable<Channel>> GetChannelsAsync(this Guild guild)
        => guild.Client.Rest.GetChannelsAsync(guild.Id);

    public static Task<Channel> CreateChannelAsync(this Guild guild, CreateGuildChannelRequest request)
        => guild.Client.Rest.CreateGuildChannelAsync<ChannelJson>(guild.Id, request);

    public static Task<IEnumerable<GuildEmoji>> GetEmojisAsync(this Guild guild)
        => guild.Client.Rest.GetEmojisAsync(guild.Id);

    public static Task<GuildEmoji> CreateEmojiAsync(this Guild guild, CreateGuildEmojiRequest request)
        => guild.Client.Rest.CreateEmojiAsync(guild.Id, request);

    public static Task GetStickersAsync(this Guild guild)
        => guild.Client.Rest.GetStickersAsync(guild.Id);

    public static Task<GuildSticker> CreateStickerAsync(this Guild guild, CreateGuildStickerRequest request)
        => guild.Client.Rest.CreateStickerAsync(guild.Id, request);

    public static Task<IEnumerable<GuildMember>> GetMembersAsync(this Guild guild)
        => guild.Client.Rest.GetMembersAsync(guild.Id);

    public static Task<GuildMember> GetMemberAsync(this Guild guild, ulong userId)
        => guild.Client.Rest.GetMemberAsync(guild.Id, userId);

    public static Task<GuildMember> GetMemberAsync(this Guild guild, User user)
        => guild.Client.Rest.GetMemberAsync(guild.Id, user.Id);

    public static Task<Role> CreateRoleAsync(this Guild guild, CreateGuildRoleRequest request)
        => guild.Client.Rest.CreateRoleAsync(guild.Id, request);

    public static Task<GuildVanityUrl> GetVanityUrlAsync(this Guild guild)
        => guild.Client.Rest.GetGuildVanityUrlAsync(guild.Id);
}
