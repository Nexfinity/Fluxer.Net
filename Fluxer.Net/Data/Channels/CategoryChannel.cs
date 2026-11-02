namespace Fluxer.Net;

/// <summary>
/// A channel category that groups other channels in a guild.
/// </summary>
public class CategoryChannel : Channel, ICategoryChannel
{
    internal CategoryChannel(FluxerBaseClient client) : base(client)
    {

    }
}