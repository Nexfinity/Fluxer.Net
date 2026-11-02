namespace Fluxer.Net;

/// <summary>
/// A link channel within a guild.
/// </summary>
public class LinkChannel : Channel, INestedChannel
{
    /// <inheritdoc />
    public ulong? CategoryId => ParentId;

    internal LinkChannel(FluxerBaseClient client) : base(client)
    {

    }
}