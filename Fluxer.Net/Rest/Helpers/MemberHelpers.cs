namespace Fluxer.Net;

public static class MemberHelpers
{
    public static Task BanAsync(this GuildMember member, CreateGuildBanRequest request)
        => member.Client.Rest.BanMemberAsync(member.GuildId, member.Id, request);

    public static Task KickAsync(this GuildMember member)
        => member.Client.Rest.KickMemberAsync(member.GuildId, member.Id);

    public static Task<GuildMember> ModifyAsync(this GuildMember member, GuildMemberJson request)
        => member.Client.Rest.UpdateMemberAsync(member.GuildId, member.Id, request);

    public static Task AddRoleAsync(this GuildMember member, ulong roleId)
        => member.Client.Rest.AddMemberRoleAsync(member.GuildId, member.Id, roleId);

    public static Task AddRoleAsync(this GuildMember member, Role role)
        => member.Client.Rest.AddMemberRoleAsync(member.GuildId, member.Id, role.Id);

    public static Task RemoveRoleAsync(this GuildMember member, ulong roleId)
        => member.Client.Rest.RemoveMemberRoleAsync(member.GuildId, member.Id, roleId);

    public static Task RemoveRoleAsync(this GuildMember member, Role role)
        => member.Client.Rest.RemoveMemberRoleAsync(member.GuildId, member.Id, role.Id);

    public static Task SetNicknameAsync(this GuildMember member, string? name)
        => member.Client.Rest.UpdateMemberAsync(member.GuildId, member.Id, new GuildMemberJson
        {
            Nickname = name
        });

    public static Task SetTimeoutAsync(this GuildMember member, DateTime? date)
        => member.Client.Rest.UpdateMemberAsync(member.GuildId, member.Id, new GuildMemberJson
        {
            CommunicationDisabledUntil = date
        });
}
