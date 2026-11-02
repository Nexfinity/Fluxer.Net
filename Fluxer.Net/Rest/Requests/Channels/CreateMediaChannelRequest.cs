namespace Fluxer.Net.Rest;

/// <summary>
/// Request body for creating a media channel in a guild.
/// </summary>
public class CreateMediaChannelRequest : CreateGuildChannelRequest
{
    /// <summary>
    /// The channel type, always a media channel.
    /// </summary>
    public override GuildChannelType Type => GuildChannelType.GuildMedia;
}