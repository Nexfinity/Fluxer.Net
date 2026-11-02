namespace Fluxer.Net;

/// <summary>
/// A group direct message channel between several users.
/// </summary>
public class GroupChannel : Channel, IGroupChannel
{
    internal GroupChannel(FluxerBaseClient client) : base(client)
    {

    }
}