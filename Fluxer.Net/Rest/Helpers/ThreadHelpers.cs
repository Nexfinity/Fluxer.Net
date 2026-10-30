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

    /// <summary>
    /// Replace the tags applied to the forum or media post.
    /// </summary>
    public static Task<ThreadChannel> SetTagsAsync(this IThreadChannel channel, IEnumerable<ulong> tagIds, RequestOptions? options = null)
        => ClientOf(channel).Rest.UpdateThreadAsync(channel.Id, new UpdateThreadRequest
        {
            AppliedTags = tagIds.ToList()
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

    /// <inheritdoc cref="FluxerApiClient.GetThreadMembersAsync(ulong, bool, int?, ulong?, RequestOptions?)" />
    public static Task<IEnumerable<ThreadMember>> GetUsersAsync(this IThreadChannel channel, bool withMember = false, int? limit = null, ulong? afterId = null, RequestOptions? options = null)
        => ClientOf(channel).Rest.GetThreadMembersAsync(channel.Id, withMember, limit, afterId, options);

    /// <inheritdoc cref="FluxerApiClient.GetThreadMemberAsync(ulong, ulong, bool, RequestOptions?)" />
    public static Task<ThreadMember> GetUserAsync(this IThreadChannel channel, ulong userId, bool withMember = false, RequestOptions? options = null)
        => ClientOf(channel).Rest.GetThreadMemberAsync(channel.Id, userId, withMember, options);

    /// <inheritdoc cref="FluxerApiClient.GetPublicArchivedThreadsAsync(ulong, int?, DateTimeOffset?, RequestOptions?)" />
    public static Task<ThreadList> GetPublicArchivedThreadsAsync(this Channel channel, int? limit = null, DateTimeOffset? before = null, RequestOptions? options = null)
        => channel.Client.Rest.GetPublicArchivedThreadsAsync(channel.Id, limit, before, options);

    /// <inheritdoc cref="FluxerApiClient.GetPrivateArchivedThreadsAsync(ulong, int?, DateTimeOffset?, RequestOptions?)" />
    public static Task<ThreadList> GetPrivateArchivedThreadsAsync(this Channel channel, int? limit = null, DateTimeOffset? before = null, RequestOptions? options = null)
        => channel.Client.Rest.GetPrivateArchivedThreadsAsync(channel.Id, limit, before, options);

    /// <inheritdoc cref="FluxerApiClient.GetJoinedPrivateArchivedThreadsAsync(ulong, int?, ulong?, RequestOptions?)" />
    public static Task<ThreadList> GetJoinedPrivateArchivedThreadsAsync(this Channel channel, int? limit = null, ulong? beforeId = null, RequestOptions? options = null)
        => channel.Client.Rest.GetJoinedPrivateArchivedThreadsAsync(channel.Id, limit, beforeId, options);

    /// <inheritdoc cref="FluxerApiClient.SearchThreadsAsync(ulong, ThreadSearchQuery, RequestOptions?)" />
    public static Task<ThreadList> SearchThreadsAsync(this Channel channel, ThreadSearchQuery query, RequestOptions? options = null)
        => channel.Client.Rest.SearchThreadsAsync(channel.Id, query, options);

    /// <inheritdoc cref="FluxerApiClient.GetActiveThreadsAsync(ulong, RequestOptions?)" />
    public static Task<ThreadList> GetActiveThreadsAsync(this Guild guild, RequestOptions? options = null)
        => guild.Client.Rest.GetActiveThreadsAsync(guild.Id, options);
}
