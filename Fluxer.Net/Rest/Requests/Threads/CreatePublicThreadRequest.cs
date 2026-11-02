namespace Fluxer.Net.Rest;

/// <summary>
/// Request body for creating a public thread.
/// </summary>
public class CreatePublicThreadRequest : CreateThreadRequest
{
    /// <summary>
    /// The channel type, always a public thread.
    /// </summary>
    public override GuildChannelType Type => GuildChannelType.PublicThread;
}
