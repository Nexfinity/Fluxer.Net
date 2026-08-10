namespace Fluxer.Net;

/// <summary>
/// Http methods for <see cref="Role"/> class. 
/// </summary>
public static class RoleHelpers
{
    /// <inheritdoc cref="ApiClient.DeleteRoleAsync(ulong, ulong)" />
    public static Task DeleteAsync(this Role role)
        => role.Client.Rest.DeleteRoleAsync(role.GuildId, role.Id);

    /// <inheritdoc cref="ApiClient.UpdateRoleAsync(ulong, ulong, UpdateGuildRoleRequest)" />
    public static Task ModifyAsync(this Role role, UpdateGuildRoleRequest request)
        => role.Client.Rest.UpdateRoleAsync(role.GuildId, role.Id, request);

    /// <inheritdoc cref="ApiClient.AddMemberRoleAsync(ulong, ulong, ulong)" />
    public static Task AddMemberAsync(this Role role, ulong userId)
        => role.Client.Rest.AddMemberRoleAsync(role.GuildId, userId, role.Id);

    /// <inheritdoc cref="ApiClient.AddMemberRoleAsync(ulong, ulong, ulong)" />
    public static Task AddMemberAsync(this Role role, GuildMember member)
        => role.Client.Rest.AddMemberRoleAsync(role.GuildId, member.Id, role.Id);

    /// <inheritdoc cref="ApiClient.RemoveMemberRoleAsync(ulong, ulong, ulong)" />
    public static Task RemoveMemberAsync(this Role role, ulong userId)
        => role.Client.Rest.RemoveMemberRoleAsync(role.GuildId, userId, role.Id);

    /// <inheritdoc cref="ApiClient.RemoveMemberRoleAsync(ulong, ulong, ulong)" />
    public static Task RemoveMemberAsync(this Role role, GuildMember member)
        => role.Client.Rest.RemoveMemberRoleAsync(role.GuildId, member.Id, role.Id);
}
