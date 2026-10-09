#pragma warning disable IDE0130 // Namespace does not match folder structure
using Fluxer.Net.Rest;

namespace Fluxer.Net;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Http methods for <see cref="Channel"/> class. 
/// </summary>
public static class ThreadHelpers
{
    /// <inheritdoc cref="FluxerApiClient.UpdateThreadAsync(ulong, UpdateThreadRequest)" />
    public static Task<ThreadChannel> ModifyAsync(this ThreadChannel channel, UpdateThreadRequest req)
        => channel.Client.Rest.UpdateThreadAsync(channel.Id, req);

    /// <inheritdoc cref="FluxerApiClient.UpdateThreadAsync(ulong, UpdateThreadRequest)" />
    public static Task<ThreadChannel> LockAsync(this ThreadChannel channel)
        => channel.Client.Rest.UpdateThreadAsync(channel.Id, new UpdateThreadRequest
        {
            IsLocked = true
        });

    /// <inheritdoc cref="FluxerApiClient.UpdateThreadAsync(ulong, UpdateThreadRequest)" />
    public static Task<ThreadChannel> UnLockAsync(this ThreadChannel channel)
        => channel.Client.Rest.UpdateThreadAsync(channel.Id, new UpdateThreadRequest
        {
            IsLocked = false
        });

    /// <inheritdoc cref="FluxerApiClient.UpdateThreadAsync(ulong, UpdateThreadRequest)" />
    public static Task<ThreadChannel> ArchiveAsync(this ThreadChannel channel)
        => channel.Client.Rest.UpdateThreadAsync(channel.Id, new UpdateThreadRequest
        {
            IsArchived = true
        });

    /// <inheritdoc cref="FluxerApiClient.UpdateThreadAsync(ulong, UpdateThreadRequest)" />
    public static Task<ThreadChannel> OpenAsync(this ThreadChannel channel)
        => channel.Client.Rest.UpdateThreadAsync(channel.Id, new UpdateThreadRequest
        {
            IsArchived = false
        });

    /// <inheritdoc cref="FluxerApiClient.JoinThreadAsync(ulong)" />
    public static Task JoinAsync(this ThreadChannel channel)
        => channel.Client.Rest.JoinThreadAsync(channel.Id);

    /// <inheritdoc cref="FluxerApiClient.LeaveThreadAsync(ulong)" />
    public static Task LeaveAsync(this ThreadChannel channel)
        => channel.Client.Rest.LeaveThreadAsync(channel.Id);

    /// <inheritdoc cref="FluxerApiClient.AddThreadMemberAsync(ulong, ulong)" />
    public static Task AddMemberAsync(this ThreadChannel channel, ulong userId)
        => channel.Client.Rest.AddThreadMemberAsync(channel.Id, userId);

    /// <inheritdoc cref="FluxerApiClient.AddThreadMemberAsync(ulong, ulong)" />
    public static Task AddMemberAsync(this ThreadChannel channel, GuildMember member)
        => channel.Client.Rest.AddThreadMemberAsync(channel.Id, member.Id);

    /// <inheritdoc cref="FluxerApiClient.RemoveThreadMemberAsync(ulong, ulong)" />
    public static Task RemoveMemberAsync(this ThreadChannel channel, ulong userId)
        => channel.Client.Rest.RemoveThreadMemberAsync(channel.Id, userId);

    /// <inheritdoc cref="FluxerApiClient.RemoveThreadMemberAsync(ulong, ulong)" />
    public static Task RemoveMemberAsync(this ThreadChannel channel, GuildMember member)
        => channel.Client.Rest.RemoveThreadMemberAsync(channel.Id, member.Id);
}
