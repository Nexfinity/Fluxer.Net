namespace Fluxer.Net;

public static class RoleHelpers
{
    public static Task DeleteAsync(this Role role)
        => role.Client.Rest.DeleteRoleAsync(role.GuildId, role.Id);

    public static Task ModifyAsync(this Role role, GuildRoleUpdateRequest request)
        => role.Client.Rest.UpdateRoleAsync(role.GuildId, role.Id, request);

    public static Task AddMemberAsync(this Role role, ulong userId)
        => role.Client.Rest.AddMemberRoleAsync(role.GuildId, userId, role.Id);

    public static Task AddMemberAsync(this Role role, GuildMember member)
        => role.Client.Rest.AddMemberRoleAsync(role.GuildId, member.UserId, role.Id);

    public static Task RemoveMemberAsync(this Role role, ulong userId)
        => role.Client.Rest.RemoveMemberRoleAsync(role.GuildId, userId, role.Id);

    public static Task RemoveMemberAsync(this Role role, GuildMember member)
        => role.Client.Rest.RemoveMemberRoleAsync(role.GuildId, member.UserId, role.Id);
}
