namespace Fluxer.Net;

/// <inheritdoc />
public class GuildSticker : Entity, IGuildSticker
{
    /// <inheritdoc />
    public ulong GuildId { get; internal set; }

    /// <inheritdoc />
    public ulong Id { get; internal set; }

    /// <inheritdoc />
    public string Name { get; internal set; }

    /// <inheritdoc />
    public string? Description { get; internal set; }

    /// <inheritdoc />
    public List<string>? Tags { get; internal set; }

    /// <inheritdoc />
    public bool IsAnimated { get; internal set; }

    /// <inheritdoc />
    public User? Creator { get; internal set; }

    internal GuildSticker(FluxerBaseClient client) : base(client)
    {

    }

    public static GuildSticker Create(FluxerBaseClient client, GuildStickerJson json, ulong guildId)
    {
        var data = new GuildSticker(client);
        data.GuildId = guildId;
        data.Update(client, json);
        return data;
    }

    internal void Update(FluxerBaseClient client, GuildStickerJson json)
    {
        Id = json.Id;
        Name = json.Name;
        Description = json.Description;
        Tags = json.Tags;
        IsAnimated = json.IsAnimated;
        Creator = json.Creator;
    }
}
