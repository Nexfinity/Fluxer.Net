#pragma warning disable IDE0130 // Namespace does not match folder structure
using Fluxer.Net.Rest;

namespace Fluxer.Net;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Http methods for <see cref="Channel"/> class. 
/// </summary>
public static class ThreadHelpers
{
    private static FluxerBaseClient ClientOf(IChannel channel) => ((Channel)channel).Client;

    /// <inheritdoc cref="FluxerApiClient.UpdateThreadAsync(ulong, UpdateThreadRequest, RequestOptions?)" />
    public static Task<ThreadChannel> ModifyAsync(this IThreadChannel channel, UpdateThreadRequest req, RequestOptions? options = null)
        => ClientOf(channel).Rest.UpdateThreadAsync(channel.Id, req, options);

    /// <inheritdoc cref="FluxerApiClient.UpdateThreadAsync(ulong, UpdateThreadRequest, RequestOptions?)" />
    public static Task<ThreadChannel> LockAsync(this IThreadChannel channel, RequestOptions? options = null)
        => ClientOf(channel).Rest.UpdateThreadAsync(channel.Id, new UpdateThreadRequest
        {
            IsLocked = true
        }, options);

    /// <inheritdoc cref="FluxerApiClient.UpdateThreadAsync(ulong, UpdateThreadRequest, RequestOptions?)" />
    public static Task<ThreadChannel> UnLockAsync(this IThreadChannel channel, RequestOptions? options = null)
        => ClientOf(channel).Rest.UpdateThreadAsync(channel.Id, new UpdateThreadRequest
        {
            IsLocked = false
        }, options);

    /// <inheritdoc cref="FluxerApiClient.UpdateThreadAsync(ulong, UpdateThreadRequest, RequestOptions?)" />
    public static Task<ThreadChannel> ArchiveAsync(this IThreadChannel channel, RequestOptions? options = null)
        => ClientOf(channel).Rest.UpdateThreadAsync(channel.Id, new UpdateThreadRequest
        {
            IsArchived = true
        }, options);

    /// <inheritdoc cref="FluxerApiClient.UpdateThreadAsync(ulong, UpdateThreadRequest, RequestOptions?)" />
    public static Task<ThreadChannel> OpenAsync(this IThreadChannel channel, RequestOptions? options = null)
        => ClientOf(channel).Rest.UpdateThreadAsync(channel.Id, new UpdateThreadRequest
        {
            IsArchived = false
        }, options);

    /// <inheritdoc cref="FluxerApiClient.JoinThreadAsync(ulong, RequestOptions?)" />
    public static Task JoinAsync(this IThreadChannel channel, RequestOptions? options = null)
        => ClientOf(channel).Rest.JoinThreadAsync(channel.Id, options);

    /// <inheritdoc cref="FluxerApiClient.LeaveThreadAsync(ulong, RequestOptions?)" />
    public static Task LeaveAsync(this IThreadChannel channel, RequestOptions? options = null)
        => ClientOf(channel).Rest.LeaveThreadAsync(channel.Id, options);

    /// <inheritdoc cref="FluxerApiClient.AddThreadMemberAsync(ulong, ulong, RequestOptions?)" />
    public static Task AddMemberAsync(this IThreadChannel channel, ulong userId, RequestOptions? options = null)
        => ClientOf(channel).Rest.AddThreadMemberAsync(channel.Id, userId, options);

    /// <inheritdoc cref="FluxerApiClient.AddThreadMemberAsync(ulong, ulong, RequestOptions?)" />
    public static Task AddMemberAsync(this IThreadChannel channel, GuildMember member, RequestOptions? options = null)
        => ClientOf(channel).Rest.AddThreadMemberAsync(channel.Id, member.Id, options);

    /// <inheritdoc cref="FluxerApiClient.RemoveThreadMemberAsync(ulong, ulong, RequestOptions?)" />
    public static Task RemoveMemberAsync(this IThreadChannel channel, ulong userId, RequestOptions? options = null)
        => ClientOf(channel).Rest.RemoveThreadMemberAsync(channel.Id, userId, options);

    /// <inheritdoc cref="FluxerApiClient.RemoveThreadMemberAsync(ulong, ulong, RequestOptions?)" />
    public static Task RemoveMemberAsync(this IThreadChannel channel, GuildMember member, RequestOptions? options = null)
        => ClientOf(channel).Rest.RemoveThreadMemberAsync(channel.Id, member.Id, options);
}
