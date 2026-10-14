#pragma warning disable IDE0130 // Namespace does not match folder structure
using Fluxer.Net.Rest;

namespace Fluxer.Net;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Http methods for <see cref="CategoryChannel"/> class. 
/// </summary>
public static class CategoryHelpers
{
    /// <inheritdoc cref="FluxerApiClient.CreateGuildChannelAsync(ulong, CreateGuildChannelRequest, RequestOptions?)" />
    public static Task<TextChannel> CreateTextChannelAsync(this CategoryChannel category, CreateTextChannelRequest request, RequestOptions? options = null)
    {
        request.ParentCategoryId = category.Id;
        return category.Client.Rest.CreateTextChannelAsync(category.GuildId.Value, request, options);
    }

    /// <inheritdoc cref="FluxerApiClient.CreateGuildChannelAsync(ulong, CreateGuildChannelRequest, RequestOptions?)" />
    public static Task<VoiceChannel> CreateVoiceChannelAsync(this CategoryChannel category, CreateVoiceChannelRequest request, RequestOptions? options = null)
    {
        request.ParentCategoryId = category.Id;
        return category.Client.Rest.CreateVoiceChannelAsync(category.GuildId.Value, request, options);
    }

    /// <inheritdoc cref="FluxerApiClient.CreateGuildChannelAsync(ulong, CreateGuildChannelRequest, RequestOptions?)" />
    public static Task<LinkChannel> CreateLinkChannelAsync(this CategoryChannel category, CreateLinkChannelRequest request, RequestOptions? options = null)
    {
        request.ParentCategoryId = category.Id;
        return category.Client.Rest.CreateLinkChannelAsync(category.GuildId.Value, request, options);
    }

    /// <inheritdoc cref="FluxerApiClient.CreateGuildChannelAsync(ulong, CreateGuildChannelRequest, RequestOptions?)" />
    public static Task<NewsChannel> CreateNewsChannelAsync(this CategoryChannel category, CreateNewsChannelRequest request, RequestOptions? options = null)
    {
        request.ParentCategoryId = category.Id;
        return category.Client.Rest.CreateNewsChannelAsync(category.GuildId.Value, request, options);
    }

    /// <inheritdoc cref="FluxerApiClient.CreateGuildChannelAsync(ulong, CreateGuildChannelRequest, RequestOptions?)" />
    public static Task<ForumChannel> CreateForumChannelAsync(this CategoryChannel category, CreateForumChannelRequest request, RequestOptions? options = null)
    {
        request.ParentCategoryId = category.Id;
        return category.Client.Rest.CreateForumChannelAsync(category.GuildId.Value, request, options);
    }

    /// <inheritdoc cref="FluxerApiClient.CreateGuildChannelAsync(ulong, CreateGuildChannelRequest, RequestOptions?)" />
    public static Task<MediaChannel> CreateMediaChannelAsync(this CategoryChannel category, CreateMediaChannelRequest request, RequestOptions? options = null)
    {
        request.ParentCategoryId = category.Id;
        return category.Client.Rest.CreateMediaChannelAsync(category.GuildId.Value, request, options);
    }
}