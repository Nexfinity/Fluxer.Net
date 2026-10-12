namespace Fluxer.Net;

/// <summary>
/// The type of webhook.
/// </summary>
public enum WebhookType
{
    /// <summary>
    /// Used by external services and bots.
    /// </summary>
    External = 1,

    /// <summary>
    /// Used for announcement/news channel followed messages.
    /// </summary>
    ChannelFollow = 2
}
