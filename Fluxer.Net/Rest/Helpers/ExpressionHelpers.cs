namespace Fluxer.Net;

/// <summary>
/// Http methods for <see cref="GuildEmoji"/> and <see cref="GuildSticker"/> class. 
/// </summary>
public static class ExpressionHelpers
{
    /// <inheritdoc cref="ApiClient.DeleteEmojiAsync(ulong, ulong)" />
    public static Task DeleteAsync(this GuildEmoji emoji)
        => emoji.Client.Rest.DeleteEmojiAsync(emoji.GuildId, emoji.Id);

    /// <inheritdoc cref="ApiClient.UpdateEmojiAsync(ulong, ulong, UpdateGuildEmojiRequest)" />
    public static Task ModifyAsync(this GuildEmoji emoji, string name)
        => emoji.Client.Rest.UpdateEmojiAsync(emoji.GuildId, emoji.Id, new UpdateGuildEmojiRequest
        {
            Name = name
        });

    /// <inheritdoc cref="ApiClient.DeleteStickerAsync(ulong, ulong)" />
    public static Task DeleteAsync(this GuildSticker sticker)
        => sticker.Client.Rest.DeleteStickerAsync(sticker.GuildId, sticker.Id);

    /// <inheritdoc cref="ApiClient.UpdateStickerAsync(ulong, ulong, UpdateGuildStickerRequest)" />
    public static Task ModifyAsync(this GuildSticker sticker, UpdateGuildStickerRequest request)
        => sticker.Client.Rest.UpdateStickerAsync(sticker.GuildId, sticker.Id, request);
}
