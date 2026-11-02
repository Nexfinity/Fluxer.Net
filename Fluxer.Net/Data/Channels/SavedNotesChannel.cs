namespace Fluxer.Net;

/// <summary>
/// A personal notes channel for the current user.
/// </summary>
public class SavedNotesChannel : Channel, IMessageChannel
{
    internal SavedNotesChannel(FluxerBaseClient client) : base(client)
    {

    }
}
