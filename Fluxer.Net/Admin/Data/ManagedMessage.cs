namespace Fluxer.Net.Admin;

public class ManagedMessage : Message
{
    internal ManagedMessage(FluxerBaseClient client) : base(client)
    {

    }

    public static ManagedMessage Create(FluxerAdminClient client, MessageJson json)
    {
        ManagedMessage data = new ManagedMessage(client);
        data.Update(json);
        return data;
    }
}
