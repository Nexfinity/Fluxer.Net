namespace Fluxer.Net;

/// <inheritdoc />
public class SavedMessage : Message, ISavedMessage
{
    internal SavedMessage(FluxerBaseClient client) : base(client)
    {

    }

    /// <summary>
    /// Creates a saved message from JSON.
    /// </summary>
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