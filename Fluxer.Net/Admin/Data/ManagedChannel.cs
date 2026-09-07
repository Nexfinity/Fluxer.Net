namespace Fluxer.Net.Admin;

public class ManagedChannel : Channel
{
    internal ManagedChannel(FluxerBaseClient client) : base(client)
    {

    }

    public static ManagedChannel Create(FluxerAdminClient client, ChannelJson json)
    {
        ManagedChannel data = new ManagedChannel(client);
        data.Update(json);
        return data;
    }
}
