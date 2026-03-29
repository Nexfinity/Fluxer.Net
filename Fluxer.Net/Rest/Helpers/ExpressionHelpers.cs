namespace Fluxer.Net;

public static class ExpressionHelpers
{
    public static Task DeleteAsync(this GuildEmoji emoji)
        => emoji.Client.Rest.DeleteEmojiAsync(emoji.GuildId, emoji.Id);

    public static Task ModifyAsync(this GuildEmoji emoji, string name)
        => emoji.Client.Rest.UpdateEmojiAsync(emoji.GuildId, emoji.Id, new UpdateGuildEmojiRequest
        {
            Name = name
        });

    public static Task DeleteAsync(this GuildSticker sticker)
        => sticker.Client.Rest.DeleteStickerAsync(sticker.GuildId, sticker.Id);

    public static Task ModifyAsync(this GuildSticker sticker, UpdateGuildStickerRequest request)
        => sticker.Client.Rest.UpdateStickerAsync(sticker.GuildId, sticker.Id, request);
}
