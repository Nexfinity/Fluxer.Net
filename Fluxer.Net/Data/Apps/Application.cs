namespace Fluxer.Net;

public class Application : PartialApplication
{
    internal Application(BaseClient client) : base(client)
    {

    }

    public static Application Create(BaseClient client, ApplicationJson json)
    {
        return new Application(client);
    }

    internal void Update(ApplicationJson json)
    {

    }
}
