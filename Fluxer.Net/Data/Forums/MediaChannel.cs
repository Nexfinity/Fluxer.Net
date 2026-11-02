namespace Fluxer.Net;

/// <summary>
/// A guild channel made of media posts.
/// </summary>
public class MediaChannel : ForumChannel, IMediaChannel
{
    internal MediaChannel(FluxerBaseClient client) : base(client)
    {

    }
}