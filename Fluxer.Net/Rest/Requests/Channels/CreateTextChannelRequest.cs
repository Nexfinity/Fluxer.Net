namespace Fluxer.Net.Rest;

/// <summary>
/// Request body for creating a text channel in a guild.
/// </summary>
public class CreateTextChannelRequest : CreateGuildChannelRequest
{
    /// <summary>
    /// The channel type, always a text channel.
    /// </summary>
    public override GuildChannelType Type => GuildChannelType.GuildText;
}
