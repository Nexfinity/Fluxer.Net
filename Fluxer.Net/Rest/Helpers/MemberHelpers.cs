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
    [Obsolete("Every property of GuildMemberJson is sent, including its empty role list, so this replaces the member's roles and voice state. Use the UpdateGuildMemberRequest overload.")]
    public static Task<GuildMember> ModifyAsync(this GuildMember member, GuildMemberJson request, RequestOptions? options = null)
        => member.Client.Rest.UpdateMemberAsync(member.GuildId, member.Id, request, options);

    /// <inheritdoc cref="FluxerApiClient.UpdateMemberAsync(ulong, ulong, UpdateGuildMemberRequest, RequestOptions?)" />
    public static Task<GuildMember> ModifyAsync(this GuildMember member, UpdateGuildMemberRequest request, RequestOptions? options = null)
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
        => member.Client.Rest.UpdateMemberAsync(member.GuildId, member.Id, new UpdateGuildMemberRequest
        {
            Nickname = name
        }, options);

    /// <summary>
    /// Set a members timeout.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="GuildPermissions.ModerateMembers"/>.</remarks>
    public static Task SetTimeoutAsync(this GuildMember member, DateTimeOffset? date, RequestOptions? options = null)
        => member.Client.Rest.UpdateMemberAsync(member.GuildId, member.Id, new UpdateGuildMemberRequest
        {
            CommunicationDisabledUntil = date
        }, options);

    /// <summary>
    /// Time a member out for a length of time.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="GuildPermissions.ModerateMembers"/>.
    /// </remarks>
    public static Task SetTimeoutAsync(this GuildMember member, TimeSpan duration, string? reason = null, RequestOptions? options = null)
    {
        UpdateGuildMemberRequest request = new UpdateGuildMemberRequest
        {
            CommunicationDisabledUntil = DateTimeOffset.UtcNow.Add(duration)
        };
        if (!string.IsNullOrEmpty(reason))
            request.TimeoutReason = reason;
        return member.Client.Rest.UpdateMemberAsync(member.GuildId, member.Id, request, options);
    }

    /// <summary>
    /// End a member's timeout.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="GuildPermissions.ModerateMembers"/>.
    /// </remarks>
    public static Task RemoveTimeoutAsync(this GuildMember member, RequestOptions? options = null)
        => member.SetTimeoutAsync((DateTimeOffset?)null, options);

    /// <summary>
    /// Give a member several roles, one request each.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="GuildPermissions.ManageRoles"/>.
    /// </remarks>
    public static async Task AddRolesAsync(this GuildMember member, IEnumerable<ulong> roleIds, RequestOptions? options = null)
    {
        foreach (ulong roleId in roleIds.Distinct())
            await member.Client.Rest.AddMemberRoleAsync(member.GuildId, member.Id, roleId, options);
    }

    /// <inheritdoc cref="AddRolesAsync(GuildMember, IEnumerable{ulong}, RequestOptions?)" />
    public static Task AddRolesAsync(this GuildMember member, IEnumerable<Role> roles, RequestOptions? options = null)
        => member.AddRolesAsync(roles.Select(x => x.Id), options);

    /// <summary>
    /// Take several roles from a member, one request each.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="GuildPermissions.ManageRoles"/>.
    /// </remarks>
    public static async Task RemoveRolesAsync(this GuildMember member, IEnumerable<ulong> roleIds, RequestOptions? options = null)
    {
        foreach (ulong roleId in roleIds.Distinct())
            await member.Client.Rest.RemoveMemberRoleAsync(member.GuildId, member.Id, roleId, options);
    }

    /// <inheritdoc cref="RemoveRolesAsync(GuildMember, IEnumerable{ulong}, RequestOptions?)" />
    public static Task RemoveRolesAsync(this GuildMember member, IEnumerable<Role> roles, RequestOptions? options = null)
        => member.RemoveRolesAsync(roles.Select(x => x.Id), options);

    /// <summary>
    /// A link that opens the member's profile in the web app.
    /// </summary>
    public static string GetProfileUrl(this GuildMember member)
        => $"{member.Client.Config.WebUrl}/users/{member.Id}";
}
