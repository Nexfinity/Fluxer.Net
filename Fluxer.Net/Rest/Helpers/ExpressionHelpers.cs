using Fluxer.Net.Rest;

namespace Fluxer.Net;

/// <summary>
/// Http methods for <see cref="GuildEmoji"/> and <see cref="GuildSticker"/> class. 
/// </summary>
public static class ExpressionHelpers
{
    /// <inheritdoc cref="FluxerApiClient.DeleteEmojiAsync(ulong, ulong, RequestOptions?)" />
    public static Task DeleteEmojiAsync(this Guild guild, GuildEmoji emoji, RequestOptions? options = null)
        => guild.Client.Rest.DeleteEmojiAsync(guild.Id, emoji.Id, options);

    /// <inheritdoc cref="FluxerApiClient.DeleteEmojiAsync(ulong, ulong, RequestOptions?)" />
    public static Task DeleteEmojiAsync(this Guild guild, ulong emojiId, RequestOptions? options = null)
        => guild.Client.Rest.DeleteEmojiAsync(guild.Id, emojiId, options);

    /// <inheritdoc cref="FluxerApiClient.DeleteEmojiAsync(ulong, ulong, RequestOptions?)" />
    public static Task DeleteEmojiAsync(this GuildEmoji emoji, RequestOptions? options = null)
        => emoji.Client.Rest.DeleteEmojiAsync(emoji.GuildId, emoji.Id, options);

    /// <inheritdoc cref="FluxerApiClient.UpdateEmojiAsync(ulong, ulong, UpdateGuildEmojiRequest, RequestOptions?)" />
    public static Task ModifyEmojiAsync(this Guild guild, GuildEmoji emoji, string name, RequestOptions? options = null)
        => guild.Client.Rest.UpdateEmojiAsync(guild.Id, emoji.Id, new UpdateGuildEmojiRequest
        {
            Name = name
        }, options);

    /// <inheritdoc cref="FluxerApiClient.UpdateEmojiAsync(ulong, ulong, UpdateGuildEmojiRequest, RequestOptions?)" />
    public static Task ModifyEmojiAsync(this Guild guild, ulong emojiId, string name, RequestOptions? options = null)
        => guild.Client.Rest.UpdateEmojiAsync(guild.Id, emojiId, new UpdateGuildEmojiRequest
        {
            Name = name
        }, options);

    /// <inheritdoc cref="FluxerApiClient.UpdateEmojiAsync(ulong, ulong, UpdateGuildEmojiRequest, RequestOptions?)" />
    public static Task ModifyAsync(this GuildEmoji emoji, string name, RequestOptions? options = null)
        => emoji.Client.Rest.UpdateEmojiAsync(emoji.GuildId, emoji.Id, new UpdateGuildEmojiRequest
        {
            Name = name
        }, options);

    /// <inheritdoc cref="FluxerApiClient.DeleteStickerAsync(ulong, ulong, RequestOptions?)" />
    public static Task DeleteStickerAsync(this Guild guild, GuildSticker sticker, RequestOptions? options = null)
        => guild.Client.Rest.DeleteStickerAsync(guild.Id, sticker.Id, options);

    /// <inheritdoc cref="FluxerApiClient.DeleteStickerAsync(ulong, ulong, RequestOptions?)" />
    public static Task DeleteStickerAsync(this Guild guild, ulong stickerId, RequestOptions? options = null)
        => guild.Client.Rest.DeleteStickerAsync(guild.Id, stickerId, options);

    /// <inheritdoc cref="FluxerApiClient.DeleteStickerAsync(ulong, ulong, RequestOptions?)" />
    public static Task DeleteAsync(this GuildSticker sticker, RequestOptions? options = null)
        => sticker.Client.Rest.DeleteStickerAsync(sticker.GuildId, sticker.Id, options);

    /// <inheritdoc cref="FluxerApiClient.UpdateStickerAsync(ulong, ulong, UpdateGuildStickerRequest, RequestOptions?)" />
    public static Task ModifyStickerAsync(this Guild guild, GuildSticker sticker, UpdateGuildStickerRequest request, RequestOptions? options = null)
        => guild.Client.Rest.UpdateStickerAsync(guild.Id, sticker.Id, request, options);

    /// <inheritdoc cref="FluxerApiClient.UpdateStickerAsync(ulong, ulong, UpdateGuildStickerRequest, RequestOptions?)" />
    public static Task ModifyStickerAsync(this Guild guild, ulong stickerId, UpdateGuildStickerRequest request, RequestOptions? options = null)
        => guild.Client.Rest.UpdateStickerAsync(guild.Id, stickerId, request, options);

    /// <inheritdoc cref="FluxerApiClient.UpdateStickerAsync(ulong, ulong, UpdateGuildStickerRequest, RequestOptions?)" />
    public static Task ModifyAsync(this GuildSticker sticker, UpdateGuildStickerRequest request, RequestOptions? options = null)
        => sticker.Client.Rest.UpdateStickerAsync(sticker.GuildId, sticker.Id, request, options);

    /// <inheritdoc cref="FluxerApiClient.CloneEmojiAsync(ulong, ulong, RequestOptions?)" />
    public static Task<GuildEmoji> CloneEmojiAsync(this Guild guild, ulong sourceEmojiId, RequestOptions? options = null)
        => guild.Client.Rest.CloneEmojiAsync(guild.Id, sourceEmojiId, options);

    /// <inheritdoc cref="FluxerApiClient.CloneStickerAsync(ulong, ulong, RequestOptions?)" />
    public static Task<GuildSticker> CloneStickerAsync(this Guild guild, ulong sourceStickerId, RequestOptions? options = null)
        => guild.Client.Rest.CloneStickerAsync(guild.Id, sourceStickerId, options);
}
