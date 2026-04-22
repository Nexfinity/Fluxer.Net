namespace Fluxer.Net;

/// <inheritdoc />
public class GuildSticker : Sticker, IGuildSticker
{
    /// <inheritdoc />
    public ulong GuildId { get; internal set; }

    /// <inheritdoc />
    public string? Description { get; internal set; }

    /// <inheritdoc />
    public List<string>? Tags { get; internal set; }

    /// <inheritdoc />
    public User? Creator { get; internal set; }

    /// <inheritdoc />
    public string? GetStickerUrl(int size = 320)
    {
        return $"https://fluxerusercontent.com/stickers/{Id}.webp?size={size}";
    }

    IUser? IGuildSticker.Creator => Creator;

    internal GuildSticker(FluxerBaseClient client) : base(client)
    {

    }

    public static GuildSticker Create(FluxerBaseClient client, GuildStickerJson json, ulong guildId)
    {
        GuildSticker data = new GuildSticker(client);
        data.GuildId = guildId;
        data.Update(client, json);
        return data;
    }

    internal void Update(FluxerBaseClient client, GuildStickerJson json)
    {
        base.Update(client, json);
        Description = json.Description;
        Tags = json.Tags;
        if (json.Creator != null)
            Creator = User.Create(client, json.Creator);
    }
}
