namespace Fluxer.Net.Data;

public abstract class Entity
{
    internal Entity(FluxerClient client)
    {
        Client = client;
    }

    internal FluxerClient Client { get; }
}
