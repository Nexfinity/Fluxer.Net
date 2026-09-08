namespace Fluxer.Net.Admin;

public class ManagedApplication : Application
{


    internal ManagedApplication(FluxerBaseClient client) : base(client)
    {

    }

    public static ManagedApplication Create(FluxerAdminClient client, ApplicationJson json)
    {
        ManagedApplication data = new ManagedApplication(client);
        data.Update(json);
        return data;
    }
}
