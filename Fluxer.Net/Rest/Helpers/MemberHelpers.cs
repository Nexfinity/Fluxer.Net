using Fluxer.Net.Rest;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Fluxer.Net;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Http methods for <see cref="GuildMember"/> class. 
/// </summary>
public static class MemberHelpers
{
    /// <inheritdoc cref="FluxerApiClient.BanMemberAsync(ulong, ulong, string?, int?, int?, RequestOptions?)" />
    public static Task BanAsync(this GuildMember member, string? reason = null, int? deleteMessageSeconds = null, int? banDurationSeconds = null, RequestOptions? options = null)
        => member.Client.Rest.BanMemberAsync(member.GuildId, member.Id, reason, deleteMessageSeconds, banDurationSeconds, options);

    /// <inheritdoc cref="FluxerApiClient.KickMemberAsync(ulong, ulong, string, RequestOptions?)" />
    public static Task KickAsync(this GuildMember member, string? reason = null, RequestOptions? options = null)
        => member.Client.Rest.KickMemberAsync(member.GuildId, member.Id, reason, options);

    /// <inheritdoc cref="FluxerApiClient.UpdateMemberAsync(ulong, ulong, GuildMemberJson, RequestOptions?)" />
    public static Task<GuildMember> ModifyAsync(this GuildMember member, GuildMemberJson request, RequestOptions? options = null)
        => member.Client.Rest.UpdateMemberAsync(member.GuildId, member.Id, request, options);

    /// <inheritdoc cref="FluxerApiClient.AddMemberRoleAsync(ulong, ulong, ulong, RequestOptions?)" />
    public static Task AddRoleAsync(this GuildMember member, ulong roleId, RequestOptions? options = null)
        => member.Client.Rest.AddMemberRoleAsync(member.GuildId, member.Id, roleId, options);

    /// <inheritdoc cref="FluxerApiClient.AddMemberRoleAsync(ulong, ulong, ulong, RequestOptions?)" />
    public static Task AddRoleAsync(this GuildMember member, Role role, RequestOptions? options = null)
        => member.Client.Rest.AddMemberRoleAsync(member.GuildId, member.Id, role.Id, options);

    /// <inheritdoc cref="FluxerApiClient.RemoveMemberRoleAsync(ulong, ulong, ulong, RequestOptions?)" />
    public static Task RemoveRoleAsync(this GuildMember member, ulong roleId, RequestOptions? options = null)
        => member.Client.Rest.RemoveMemberRoleAsync(member.GuildId, member.Id, roleId, options);

    /// <inheritdoc cref="FluxerApiClient.RemoveMemberRoleAsync(ulong, ulong, ulong, RequestOptions?)" />
    public static Task RemoveRoleAsync(this GuildMember member, Role role, RequestOptions? options = null)
        => member.Client.Rest.RemoveMemberRoleAsync(member.GuildId, member.Id, role.Id, options);

    /// <summary>
    /// Set a members nickname.
    /// </summary>
    /// <remarks>
    /// Requires either <see cref="GuildPermissions.ChangeNickname"/> for current user or <see cref="GuildPermissions.ManageNicknames"/>.
    /// </remarks>
    public static Task SetNicknameAsync(this GuildMember member, string? name, RequestOptions? options = null)
        => member.Client.Rest.UpdateMemberAsync(member.GuildId, member.Id, new GuildMemberJson
        {
            Nickname = name
        }, options);

    /// <summary>
    /// Set a members timeout.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="GuildPermissions.ModerateMembers"/>.</remarks>
    public static Task SetTimeoutAsync(this GuildMember member, DateTimeOffset? date, RequestOptions? options = null)
        => member.Client.Rest.UpdateMemberAsync(member.GuildId, member.Id, new GuildMemberJson
        {
            CommunicationDisabledUntil = date
        }, options);
}
