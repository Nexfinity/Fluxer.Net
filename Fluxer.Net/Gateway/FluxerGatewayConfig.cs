using Fluxer.Net.Gateway;

namespace Fluxer.Net;

public class FluxerGatewayConfig
{
    /// <summary>
    /// List of gateway event dispatch types to ignore. Useful for filtering out high-volume events
    /// your application doesn't need (e.g., "PRESENCE_UPDATE", "TYPING_START"). Defaults to empty list.
    /// </summary>
    public List<string> IgnoredEvents { get; set; } = new();

    /// <summary>
    /// Initial presence data to send when connecting to the gateway. If null, no presence is sent.
    /// Allows setting your bot's or user's online status, activity, and other presence information.
    /// </summary>
    public PresenceUpdateGatewayData? Presence { get; set; } = null;

    /// <summary>
    /// Don't cache emojis and also disable emojis updated event.
    /// </summary>
    public bool DisableEmojiCache { get; set; }

    /// <summary>
    /// Don't cache stickers and also disable stickers updated event.
    /// </summary>
    public bool DisableStickerCache { get; set; }
}
