using Fluxer.Net.Rest;

namespace Fluxer.Net;

/// <summary>
/// Http methods for <see cref="GuildMember"/> class. 
/// </summary>
public static class MemberHelpers
{
    /// <inheritdoc cref="FluxerApiClient.BanMemberAsync(ulong, ulong, CreateGuildBanRequest)" />
    public static Task BanAsync(this GuildMember member, CreateGuildBanRequest request)
        => member.Client.Rest.BanMemberAsync(member.GuildId, member.Id, request);

    /// <inheritdoc cref="FluxerApiClient.KickMemberAsync(ulong, ulong)" />
    public static Task KickAsync(this GuildMember member)
        => member.Client.Rest.KickMemberAsync(member.GuildId, member.Id);

    /// <inheritdoc cref="FluxerApiClient.UpdateMemberAsync(ulong, ulong, GuildMemberJson)" />
    public static Task<GuildMember> ModifyAsync(this GuildMember member, GuildMemberJson request)
        => member.Client.Rest.UpdateMemberAsync(member.GuildId, member.Id, request);

    /// <inheritdoc cref="FluxerApiClient.AddMemberRoleAsync(ulong, ulong, ulong)" />
    public static Task AddRoleAsync(this GuildMember member, ulong roleId)
        => member.Client.Rest.AddMemberRoleAsync(member.GuildId, member.Id, roleId);

    /// <inheritdoc cref="FluxerApiClient.AddMemberRoleAsync(ulong, ulong, ulong)" />
    public static Task AddRoleAsync(this GuildMember member, Role role)
        => member.Client.Rest.AddMemberRoleAsync(member.GuildId, member.Id, role.Id);

    /// <inheritdoc cref="FluxerApiClient.RemoveMemberRoleAsync(ulong, ulong, ulong)" />
    public static Task RemoveRoleAsync(this GuildMember member, ulong roleId)
        => member.Client.Rest.RemoveMemberRoleAsync(member.GuildId, member.Id, roleId);

    /// <inheritdoc cref="FluxerApiClient.RemoveMemberRoleAsync(ulong, ulong, ulong)" />
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
    public static Task SetTimeoutAsync(this GuildMember member, DateTimeOffset? date)
        => member.Client.Rest.UpdateMemberAsync(member.GuildId, member.Id, new GuildMemberJson
        {
            CommunicationDisabledUntil = date
        });
}
