using Fluxer.Net.Rest;

namespace Fluxer.Net;

/// <summary>
/// Http methods for <see cref="Role"/> class. 
/// </summary>
public static class RoleHelpers
{
    /// <inheritdoc cref="FluxerApiClient.DeleteRoleAsync(ulong, ulong, RequestOptions?)" />
    public static Task DeleteRoleAsync(this Guild guild, Role role, RequestOptions? options = null)
        => guild.Client.Rest.DeleteRoleAsync(guild.Id, role.Id, options);

    /// <inheritdoc cref="FluxerApiClient.DeleteRoleAsync(ulong, ulong, RequestOptions?)" />
    public static Task DeleteRoleAsync(this Guild guild, ulong roleId, RequestOptions? options = null)
        => guild.Client.Rest.DeleteRoleAsync(guild.Id, roleId, options);

    /// <inheritdoc cref="FluxerApiClient.DeleteRoleAsync(ulong, ulong, RequestOptions?)" />
    public static Task DeleteAsync(this Role role, RequestOptions? options = null)
        => role.Client.Rest.DeleteRoleAsync(role.GuildId, role.Id, options);

    /// <inheritdoc cref="FluxerApiClient.UpdateRoleAsync(ulong, ulong, UpdateGuildRoleRequest, RequestOptions?)" />
    public static Task ModifyRoleAsync(this Guild guild, Role role, UpdateGuildRoleRequest request, RequestOptions? options = null)
        => guild.Client.Rest.UpdateRoleAsync(guild.Id, role.Id, request, options);

    /// <inheritdoc cref="FluxerApiClient.UpdateRoleAsync(ulong, ulong, UpdateGuildRoleRequest, RequestOptions?)" />
    public static Task ModifyRoleAsync(this Guild guild, ulong roleId, UpdateGuildRoleRequest request, RequestOptions? options = null)
        => guild.Client.Rest.UpdateRoleAsync(guild.Id, roleId, request, options);

    /// <inheritdoc cref="FluxerApiClient.UpdateRoleAsync(ulong, ulong, UpdateGuildRoleRequest, RequestOptions?)" />
    public static Task ModifyAsync(this Role role, UpdateGuildRoleRequest request, RequestOptions? options = null)
        => role.Client.Rest.UpdateRoleAsync(role.GuildId, role.Id, request, options);

    /// <inheritdoc cref="FluxerApiClient.AddMemberRoleAsync(ulong, ulong, ulong, RequestOptions?)" />
    public static Task AddMemberAsync(this Role role, ulong userId, RequestOptions? options = null)
        => role.Client.Rest.AddMemberRoleAsync(role.GuildId, userId, role.Id, options);

    /// <inheritdoc cref="FluxerApiClient.AddMemberRoleAsync(ulong, ulong, ulong, RequestOptions?)" />
    public static Task AddMemberAsync(this Role role, GuildMember member, RequestOptions? options = null)
        => role.Client.Rest.AddMemberRoleAsync(role.GuildId, member.Id, role.Id, options);

    /// <inheritdoc cref="FluxerApiClient.RemoveMemberRoleAsync(ulong, ulong, ulong, RequestOptions?)" />
    public static Task RemoveMemberAsync(this Role role, ulong userId, RequestOptions? options = null)
        => role.Client.Rest.RemoveMemberRoleAsync(role.GuildId, userId, role.Id, options);

    /// <inheritdoc cref="FluxerApiClient.RemoveMemberRoleAsync(ulong, ulong, ulong, RequestOptions?)" />
    public static Task RemoveMemberAsync(this Role role, GuildMember member, RequestOptions? options = null)
        => role.Client.Rest.RemoveMemberRoleAsync(role.GuildId, member.Id, role.Id, options);
}
