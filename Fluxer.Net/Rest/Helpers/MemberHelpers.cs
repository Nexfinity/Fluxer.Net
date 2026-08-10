namespace Fluxer.Net;

/// <summary>
/// Http methods for <see cref="GuildMember"/> class. 
/// </summary>
public static class MemberHelpers
{
    /// <inheritdoc cref="ApiClient.BanMemberAsync(ulong, ulong, CreateGuildBanRequest)" />
    public static Task BanAsync(this GuildMember member, CreateGuildBanRequest request)
        => member.Client.Rest.BanMemberAsync(member.GuildId, member.Id, request);

    /// <inheritdoc cref="ApiClient.KickMemberAsync(ulong, ulong)" />
    public static Task KickAsync(this GuildMember member)
        => member.Client.Rest.KickMemberAsync(member.GuildId, member.Id);

    /// <inheritdoc cref="ApiClient.UpdateMemberAsync(ulong, ulong, GuildMemberJson)" />
    public static Task<GuildMember> ModifyAsync(this GuildMember member, GuildMemberJson request)
        => member.Client.Rest.UpdateMemberAsync(member.GuildId, member.Id, request);

    /// <inheritdoc cref="ApiClient.AddMemberRoleAsync(ulong, ulong, ulong)" />
    public static Task AddRoleAsync(this GuildMember member, ulong roleId)
        => member.Client.Rest.AddMemberRoleAsync(member.GuildId, member.Id, roleId);

    /// <inheritdoc cref="ApiClient.AddMemberRoleAsync(ulong, ulong, ulong)" />
    public static Task AddRoleAsync(this GuildMember member, Role role)
        => member.Client.Rest.AddMemberRoleAsync(member.GuildId, member.Id, role.Id);

    /// <inheritdoc cref="ApiClient.RemoveMemberRoleAsync(ulong, ulong, ulong)" />
    public static Task RemoveRoleAsync(this GuildMember member, ulong roleId)
        => member.Client.Rest.RemoveMemberRoleAsync(member.GuildId, member.Id, roleId);

    /// <inheritdoc cref="ApiClient.RemoveMemberRoleAsync(ulong, ulong, ulong)" />
    public static Task RemoveRoleAsync(this GuildMember member, Role role)
        => member.Client.Rest.RemoveMemberRoleAsync(member.GuildId, member.Id, role.Id);

    /// <summary>
    /// Set a members nickname.
    /// </summary>
    /// <remarks>
    /// Requires either <see cref="GuildPermissions.ChangeNickname"/> for current user or <see cref="GuildPermissions.ManageNicknames"/>.
    /// </remarks>
    public static Task SetNicknameAsync(this GuildMember member, string? name)
        => member.Client.Rest.UpdateMemberAsync(member.GuildId, member.Id, new GuildMemberJson
        {
            Nickname = name
        });

    /// <summary>
    /// Set a members timeout.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="GuildPermissions.ModerateMembers"/>.</remarks>
    public static Task SetTimeoutAsync(this GuildMember member, DateTime? date)
        => member.Client.Rest.UpdateMemberAsync(member.GuildId, member.Id, new GuildMemberJson
        {
            CommunicationDisabledUntil = date
        });
}
