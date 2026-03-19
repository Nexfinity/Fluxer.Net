namespace Fluxer.Net;

public class PartialApplication : Entity
{
    internal PartialApplication(BaseClient client) : base(client)
    {

    }

    public static PartialApplication Create(BaseClient client, PartialApplicationJson json)
    {
        return new PartialApplication(client);
    }

    internal void Update(PartialApplicationJson json)
    {

    }
}
