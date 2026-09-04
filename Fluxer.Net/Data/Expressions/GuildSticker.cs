namespace Fluxer.Net;

/// <inheritdoc />
public class GuildSticker : Sticker, IGuildSticker
{
    /// <summary>
    /// Guild id for this sticker.
    /// </summary>
    public ulong GuildId { get; private set; }

    /// <inheritdoc />
    public string? Description { get; private set; }

    /// <inheritdoc />
    public List<string>? Tags { get; private set; }

    /// <inheritdoc />
    public User? Creator { get; private set; }

    /// <inheritdoc />
    public string? GetStickerUrl(int size = 320)
    {
        return $"{Client.Config.MediaUrl}/stickers/{Id}.webp?size={size}";
    }

    IUser? IGuildSticker.Creator => Creator;

    internal GuildSticker(FluxerBaseClient client) : base(client)
    {

    }

    /// <summary>
    /// Create a GuildSticker object from json.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="json"></param>
    /// <param name="guildId"></param>
    /// <returns></returns>
    public static GuildSticker Create(FluxerBaseClient client, GuildStickerJson json, ulong guildId)
    {
        GuildSticker data = new GuildSticker(client)
        {
            GuildId = guildId
        };
        data.Update(json);
        return data;
    }

    internal void Update(GuildStickerJson json)
    {
        base.Update(json);
        Description = json.Description;
        Tags = json.Tags;
        if (json.Creator != null)
            Creator = User.Create(Client, json.Creator);
    }
}
