#pragma warning disable IDE0130 // Namespace does not match folder structure
using Fluxer.Net.Rest;

namespace Fluxer.Net;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Http methods for <see cref="ICategoryChannel"/> interface. 
/// </summary>
public static class CategoryHelpers
{
    private static FluxerBaseClient ClientOf(IChannel channel) => ((Channel)channel).Client;

    /// <inheritdoc cref="FluxerApiClient.CreateGuildChannelAsync(ulong, CreateGuildChannelRequest, RequestOptions?)" />
    public static Task<TextChannel> CreateTextChannelAsync(this ICategoryChannel category, CreateTextChannelRequest request, RequestOptions? options = null)
    {
        request.ParentCategoryId = category.Id;
        return ClientOf(category).Rest.CreateTextChannelAsync(category.GuildId.Value, request, options);
    }

    /// <inheritdoc cref="FluxerApiClient.CreateGuildChannelAsync(ulong, CreateGuildChannelRequest, RequestOptions?)" />
    public static Task<VoiceChannel> CreateVoiceChannelAsync(this ICategoryChannel category, CreateVoiceChannelRequest request, RequestOptions? options = null)
    {
        request.ParentCategoryId = category.Id;
        return ClientOf(category).Rest.CreateVoiceChannelAsync(category.GuildId.Value, request, options);
    }

    /// <inheritdoc cref="FluxerApiClient.CreateGuildChannelAsync(ulong, CreateGuildChannelRequest, RequestOptions?)" />
    public static Task<LinkChannel> CreateLinkChannelAsync(this ICategoryChannel category, CreateLinkChannelRequest request, RequestOptions? options = null)
    {
        request.ParentCategoryId = category.Id;
        return ClientOf(category).Rest.CreateLinkChannelAsync(category.GuildId.Value, request, options);
    }

    /// <inheritdoc cref="FluxerApiClient.CreateGuildChannelAsync(ulong, CreateGuildChannelRequest, RequestOptions?)" />
    public static Task<NewsChannel> CreateNewsChannelAsync(this ICategoryChannel category, CreateNewsChannelRequest request, RequestOptions? options = null)
    {
        request.ParentCategoryId = category.Id;
        return ClientOf(category).Rest.CreateNewsChannelAsync(category.GuildId.Value, request, options);
    }

    /// <inheritdoc cref="FluxerApiClient.CreateGuildChannelAsync(ulong, CreateGuildChannelRequest, RequestOptions?)" />
    public static Task<ForumChannel> CreateForumChannelAsync(this ICategoryChannel category, CreateForumChannelRequest request, RequestOptions? options = null)
    {
        request.ParentCategoryId = category.Id;
        return ClientOf(category).Rest.CreateForumChannelAsync(category.GuildId.Value, request, options);
    }

    /// <inheritdoc cref="FluxerApiClient.CreateGuildChannelAsync(ulong, CreateGuildChannelRequest, RequestOptions?)" />
    public static Task<MediaChannel> CreateMediaChannelAsync(this ICategoryChannel category, CreateMediaChannelRequest request, RequestOptions? options = null)
    {
        request.ParentCategoryId = category.Id;
        return ClientOf(category).Rest.CreateMediaChannelAsync(category.GuildId.Value, request, options);
    }
}