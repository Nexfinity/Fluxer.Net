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

    internal GuildEmoji(FluxerBaseClient client) : base(client)
    {

    }

    public static GuildEmoji Create(FluxerBaseClient client, GuildEmojiJson json, ulong guildId)
    {
        GuildEmoji data = new GuildEmoji(client);
        data.GuildId = guildId;
        data.Update(client, json);
        return data;
    }

    internal void Update(FluxerBaseClient client, GuildEmojiJson json)
    {
        base.Update(client, json);
        Creator = json.Creator;
    }
}
