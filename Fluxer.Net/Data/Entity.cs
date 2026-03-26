namespace Fluxer.Net;

public abstract class Entity
{
    public Entity(FluxerBaseClient client)
    {
        Client = client;
    }

    internal FluxerBaseClient Client { get; set; }
}