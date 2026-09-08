namespace Fluxer.Net;

/// <inheritdoc />
public class SavedMessage : Message, ISavedMessage
{
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
        base.Update(json);
    }
}