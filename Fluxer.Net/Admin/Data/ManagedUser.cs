namespace Fluxer.Net.Admin;

public class ManagedUser : CurrentUser
{
    internal ManagedUser(FluxerBaseClient client) : base(client)
    {

    }

    public static ManagedUser Create(FluxerAdminClient client, CurrentUserJson json)
    {
        ManagedUser data = new ManagedUser(client);
        data.Update(json);
        return data;
    }
}
