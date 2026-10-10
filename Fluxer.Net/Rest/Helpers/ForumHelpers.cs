#pragma warning disable IDE0130 // Namespace does not match folder structure
using Fluxer.Net.Rest;

namespace Fluxer.Net;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Http methods for <see cref="Channel"/> class. 
/// </summary>
public static class ForumHelpers
{
    /// <inheritdoc cref="FluxerApiClient.CreatePostAsync(ulong, CreatePostRequest)" />
    public static Task<ThreadChannel> CreatePostAsync(this Channel channel, CreatePostRequest req)
        => channel.Client.Rest.CreatePostAsync(channel.Id, req);

    /// <inheritdoc cref="FluxerApiClient.CreateForumTagAsync(ulong, CreateForumTagRequest)" />
    public static Task<ForumChannel> CreateTagAsync(this Channel channel, CreateForumTagRequest req)
        => channel.Client.Rest.CreateForumTagAsync(channel.Id, req);

    /// <inheritdoc cref="FluxerApiClient.UpdateForumTagAsync(ulong, ulong, CreateForumTagRequest)" />
    public static Task<ForumChannel> ModifyTagAsync(this Channel channel, ulong tagId, CreateForumTagRequest req)
        => channel.Client.Rest.UpdateForumTagAsync(channel.Id, tagId, req);

    /// <inheritdoc cref="FluxerApiClient.UpdateForumTagAsync(ulong, ulong, CreateForumTagRequest)" />
    public static Task<ForumChannel> ModifyAsync(this ForumTag tag, CreateForumTagRequest req)
        => tag.Client.Rest.UpdateForumTagAsync(tag.ChannelId, tag.Id, req);

    /// <inheritdoc cref="FluxerApiClient.DeleteForumTagAsync(ulong, ulong)" />
    public static Task<ForumChannel> DeleteTagAsync(this Channel channel, ulong tagId)
        => channel.Client.Rest.DeleteForumTagAsync(channel.Id, tagId);

    /// <inheritdoc cref="FluxerApiClient.DeleteForumTagAsync(ulong, ulong)" />
    public static Task<ForumChannel> DeleteAsync(this ForumTag tag)
        => tag.Client.Rest.DeleteForumTagAsync(tag.ChannelId, tag.Id);


}
