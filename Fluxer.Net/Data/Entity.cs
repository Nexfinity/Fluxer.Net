namespace Fluxer.Net;

public abstract class Entity
{
    public Entity(BaseClient client)
    {
        Client = client;
    }

    internal BaseClient Client { get; set; }
}
