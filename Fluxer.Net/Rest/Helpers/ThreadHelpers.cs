#pragma warning disable IDE0130 // Namespace does not match folder structure
using Fluxer.Net.Rest;

namespace Fluxer.Net;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Http methods for <see cref="Channel"/> class. 
/// </summary>
public static class ThreadHelpers
{
    /// <inheritdoc cref="FluxerApiClient.UpdateThreadAsync(ulong, UpdateThreadRequest, RequestOptions?)" />
    public static Task<ThreadChannel> ModifyAsync(this ThreadChannel channel, UpdateThreadRequest req, RequestOptions? options = null)
        => channel.Client.Rest.UpdateThreadAsync(channel.Id, req, options);

    /// <inheritdoc cref="FluxerApiClient.UpdateThreadAsync(ulong, UpdateThreadRequest, RequestOptions?)" />
    public static Task<ThreadChannel> LockAsync(this ThreadChannel channel, RequestOptions? options = null)
        => channel.Client.Rest.UpdateThreadAsync(channel.Id, new UpdateThreadRequest
        {
            IsLocked = true
        }, options);

    /// <inheritdoc cref="FluxerApiClient.UpdateThreadAsync(ulong, UpdateThreadRequest, RequestOptions?)" />
    public static Task<ThreadChannel> UnLockAsync(this ThreadChannel channel, RequestOptions? options = null)
        => channel.Client.Rest.UpdateThreadAsync(channel.Id, new UpdateThreadRequest
        {
            IsLocked = false
        }, options);

    /// <inheritdoc cref="FluxerApiClient.UpdateThreadAsync(ulong, UpdateThreadRequest, RequestOptions?)" />
    public static Task<ThreadChannel> ArchiveAsync(this ThreadChannel channel, RequestOptions? options = null)
        => channel.Client.Rest.UpdateThreadAsync(channel.Id, new UpdateThreadRequest
        {
            IsArchived = true
        }, options);

    /// <inheritdoc cref="FluxerApiClient.UpdateThreadAsync(ulong, UpdateThreadRequest, RequestOptions?)" />
    public static Task<ThreadChannel> OpenAsync(this ThreadChannel channel, RequestOptions? options = null)
        => channel.Client.Rest.UpdateThreadAsync(channel.Id, new UpdateThreadRequest
        {
            IsArchived = false
        }, options);

    /// <inheritdoc cref="FluxerApiClient.JoinThreadAsync(ulong, RequestOptions?)" />
    public static Task JoinAsync(this ThreadChannel channel, RequestOptions? options = null)
        => channel.Client.Rest.JoinThreadAsync(channel.Id, options);

    /// <inheritdoc cref="FluxerApiClient.LeaveThreadAsync(ulong, RequestOptions?)" />
    public static Task LeaveAsync(this ThreadChannel channel, RequestOptions? options = null)
        => channel.Client.Rest.LeaveThreadAsync(channel.Id, options);

    /// <inheritdoc cref="FluxerApiClient.AddThreadMemberAsync(ulong, ulong, RequestOptions?)" />
    public static Task AddMemberAsync(this ThreadChannel channel, ulong userId, RequestOptions? options = null)
        => channel.Client.Rest.AddThreadMemberAsync(channel.Id, userId, options);

    /// <inheritdoc cref="FluxerApiClient.AddThreadMemberAsync(ulong, ulong, RequestOptions?)" />
    public static Task AddMemberAsync(this ThreadChannel channel, GuildMember member, RequestOptions? options = null)
        => channel.Client.Rest.AddThreadMemberAsync(channel.Id, member.Id, options);

    /// <inheritdoc cref="FluxerApiClient.RemoveThreadMemberAsync(ulong, ulong, RequestOptions?)" />
    public static Task RemoveMemberAsync(this ThreadChannel channel, ulong userId, RequestOptions? options = null)
        => channel.Client.Rest.RemoveThreadMemberAsync(channel.Id, userId, options);

    /// <inheritdoc cref="FluxerApiClient.RemoveThreadMemberAsync(ulong, ulong, RequestOptions?)" />
    public static Task RemoveMemberAsync(this ThreadChannel channel, GuildMember member, RequestOptions? options = null)
        => channel.Client.Rest.RemoveThreadMemberAsync(channel.Id, member.Id, options);
}
