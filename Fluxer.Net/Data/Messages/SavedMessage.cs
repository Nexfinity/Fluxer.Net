namespace Fluxer.Net;

/// <inheritdoc />
public class SavedMessage : Entity, ISavedMessage
{
    /// <inheritdoc />
    public ulong Id { get; private set; }

    /// <inheritdoc />
    public DateTimeOffset CreatedAt => SnowflakeUtils.FromSnowflake(Id);

    /// <inheritdoc />
    public ulong UserId { get; private set; }

    /// <inheritdoc />
    public ulong ChannelId { get; private set; }

    /// <inheritdoc />
    public DateTimeOffset SavedAt { get; private set; }

    internal SavedMessage(FluxerBaseClient client) : base(client)
    {

    }

    /// <summary>
    /// Create a Guild object from json.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="json"></param>
    /// <returns></returns>
    public static SavedMessage Create(FluxerBaseClient client, SavedMessageJson json)
    {
        SavedMessage data = new SavedMessage(client);
        data.Update(json);
        return data;
    }

    internal virtual void Update(SavedMessageJson json)
    {
        Id = json.Id;
        UserId = json.UserId;
        ChannelId = json.ChannelId;
        SavedAt = json.SavedAt;
    }
}