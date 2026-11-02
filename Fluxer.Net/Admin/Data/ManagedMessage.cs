namespace Fluxer.Net.Admin;

/// <summary>
/// A message read through a <see cref="FluxerAdminClient"/>.
/// </summary>
public class ManagedMessage : Message
{
    internal ManagedMessage(FluxerBaseClient client) : base(client)
    {

    }

    /// <summary>
    /// Creates a message from its API data.
    /// </summary>
    /// <param name="client">The admin client the message was read with.</param>
    /// <param name="json">The message data.</param>
    public static ManagedMessage Create(FluxerAdminClient client, MessageJson json)
    {
        ManagedMessage data = new ManagedMessage(client);
        data.Update(json);
        return data;
    }
}
