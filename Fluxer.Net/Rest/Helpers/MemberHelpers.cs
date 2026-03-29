namespace Fluxer.Net;

public static class MemberHelpers
{
    public static Task BanAsync(this GuildMember member, CreateGuildBanRequest request)
        => member.Client.Rest.BanMemberAsync(member.GuildId, member.UserId, request);

    public static Task KickAsync(this GuildMember member)
        => member.Client.Rest.KickMemberAsync(member.GuildId, member.UserId);

    public static Task<GuildMember> ModifyAsync(this GuildMember member, GuildMemberJson request)
        => member.Client.Rest.UpdateMemberAsync(member.GuildId, member.UserId, request);

    public static Task AddRoleAsync(this GuildMember member, ulong roleId)
        => member.Client.Rest.AddMemberRoleAsync(member.GuildId, member.UserId, roleId);

    public static Task AddRoleAsync(this GuildMember member, Role role)
        => member.Client.Rest.AddMemberRoleAsync(member.GuildId, member.UserId, role.Id);

    public static Task RemoveRoleAsync(this GuildMember member, ulong roleId)
        => member.Client.Rest.RemoveMemberRoleAsync(member.GuildId, member.UserId, roleId);

    public static Task RemoveRoleAsync(this GuildMember member, Role role)
        => member.Client.Rest.RemoveMemberRoleAsync(member.GuildId, member.UserId, role.Id);
}
