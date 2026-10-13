using Fluxer.Net.Rest;

namespace Fluxer.Net;

/// <summary>
/// Http methods for <see cref="GuildEmoji"/> and <see cref="GuildSticker"/> class. 
/// </summary>
public static class ExpressionHelpers
{
    /// <inheritdoc cref="FluxerApiClient.DeleteEmojiAsync(ulong, ulong, RequestOptions?)" />
    public static Task DeleteAsync(this GuildEmoji emoji, RequestOptions? options = null)
        => emoji.Client.Rest.DeleteEmojiAsync(emoji.GuildId, emoji.Id, options);

    /// <inheritdoc cref="FluxerApiClient.UpdateEmojiAsync(ulong, ulong, UpdateGuildEmojiRequest, RequestOptions?)" />
    public static Task ModifyAsync(this GuildEmoji emoji, string name, RequestOptions? options = null)
        => emoji.Client.Rest.UpdateEmojiAsync(emoji.GuildId, emoji.Id, new UpdateGuildEmojiRequest
        {
            Name = name
        }, options);

    /// <inheritdoc cref="FluxerApiClient.DeleteStickerAsync(ulong, ulong, RequestOptions?)" />
    public static Task DeleteAsync(this GuildSticker sticker, RequestOptions? options = null)
        => sticker.Client.Rest.DeleteStickerAsync(sticker.GuildId, sticker.Id, options);

    /// <inheritdoc cref="FluxerApiClient.UpdateStickerAsync(ulong, ulong, UpdateGuildStickerRequest, RequestOptions?)" />
    public static Task ModifyAsync(this GuildSticker sticker, UpdateGuildStickerRequest request, RequestOptions? options = null)
        => sticker.Client.Rest.UpdateStickerAsync(sticker.GuildId, sticker.Id, request, options);
}
