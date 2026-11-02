namespace Fluxer.Net;

/// <summary>
/// A direct message channel between two users.
/// </summary>
public class DMChannel : Channel, IDMChannel
{
    internal DMChannel(FluxerBaseClient client) : base(client)
    {

    }
}
