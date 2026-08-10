using Newtonsoft.Json;

namespace Fluxer.Net;

/// <inheritdoc />
public class GuildEmoji : Emoji, IGuildEmoji
{
    /// <inheritdoc />
    public ulong GuildId { get; internal set; }

    /// <inheritdoc />
    [JsonProperty("user")]
    public User? Creator { get; set; }

    /// <inheritdoc />
    public string? GetEmojiUrl(int size = 160)
    {
        return $"{Client.Config.MediaUrl}/emojis/{Id}.webp?size={size}";
    }

    IUser? IGuildEmoji.Creator => Creator;

    internal GuildEmoji(FluxerBaseClient client) : base(client)
    {

    }

    /// <summary>
    /// Create a GuildEmoji object from json.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="json"></param>
    /// <param name="guildId"></param>
    /// <returns></returns>
    public static GuildEmoji Create(FluxerBaseClient client, GuildEmojiJson json, ulong guildId)
    {
        GuildEmoji data = new GuildEmoji(client)
        {
            GuildId = guildId
        };
        data.Update(client, json);
        return data;
    }

    internal void Update(FluxerBaseClient client, GuildEmojiJson json)
    {
        base.Update(client, json);
        if (json.Creator != null)
            Creator = User.Create(client, json.Creator);
    }
}
