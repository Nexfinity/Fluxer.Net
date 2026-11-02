namespace Fluxer.Net;

/// <summary>
/// A guild text channel.
/// </summary>
public class TextChannel : Channel, ITextChannel
{
    /// <inheritdoc />
    public virtual ulong? CategoryId => ParentId;

    internal TextChannel(FluxerBaseClient client) : base(client)
    {

    }
}
