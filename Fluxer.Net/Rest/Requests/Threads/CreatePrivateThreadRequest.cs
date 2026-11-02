namespace Fluxer.Net.Rest;

/// <summary>
/// Request body for creating a private thread.
/// </summary>
public class CreatePrivateThreadRequest : CreateThreadRequest
{
    /// <summary>
    /// The channel type, always a private thread.
    /// </summary>
    public override GuildChannelType Type => GuildChannelType.PrivateThread;
}
