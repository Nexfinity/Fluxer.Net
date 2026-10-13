#pragma warning disable IDE0130 // Namespace does not match folder structure
using Fluxer.Net.Rest;

namespace Fluxer.Net;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Http methods for <see cref="Channel"/> class. 
/// </summary>
public static class ForumHelpers
{
    /// <inheritdoc cref="FluxerApiClient.CreatePostAsync(ulong, CreatePostRequest, RequestOptions?)" />
    public static Task<ThreadChannel> CreatePostAsync(this Channel channel, CreatePostRequest req, RequestOptions? options = null)
        => channel.Client.Rest.CreatePostAsync(channel.Id, req, options);

    /// <inheritdoc cref="FluxerApiClient.CreateForumTagAsync(ulong, CreateForumTagRequest, RequestOptions?)" />
    public static Task<ForumChannel> CreateTagAsync(this Channel channel, CreateForumTagRequest req, RequestOptions? options = null)
        => channel.Client.Rest.CreateForumTagAsync(channel.Id, req, options);

    /// <inheritdoc cref="FluxerApiClient.UpdateForumTagAsync(ulong, ulong, CreateForumTagRequest, RequestOptions?)" />
    public static Task<ForumChannel> ModifyTagAsync(this Channel channel, ulong tagId, CreateForumTagRequest req, RequestOptions? options = null)
        => channel.Client.Rest.UpdateForumTagAsync(channel.Id, tagId, req, options);

    /// <inheritdoc cref="FluxerApiClient.UpdateForumTagAsync(ulong, ulong, CreateForumTagRequest, RequestOptions?)" />
    public static Task<ForumChannel> ModifyAsync(this ForumTag tag, CreateForumTagRequest req, RequestOptions? options = null)
        => tag.Client.Rest.UpdateForumTagAsync(tag.ChannelId, tag.Id, req, options);

    /// <inheritdoc cref="FluxerApiClient.DeleteForumTagAsync(ulong, ulong, RequestOptions?)" />
    public static Task<ForumChannel> DeleteTagAsync(this Channel channel, ulong tagId, RequestOptions? options = null)
        => channel.Client.Rest.DeleteForumTagAsync(channel.Id, tagId, options);

    /// <inheritdoc cref="FluxerApiClient.DeleteForumTagAsync(ulong, ulong, RequestOptions?)" />
    public static Task<ForumChannel> DeleteAsync(this ForumTag tag, RequestOptions? options = null)
        => tag.Client.Rest.DeleteForumTagAsync(tag.ChannelId, tag.Id, options);


}
