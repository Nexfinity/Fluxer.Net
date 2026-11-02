namespace Fluxer.Net.Rest;

/// <summary>
/// Request body for creating a news channel in a guild.
/// </summary>
public class CreateNewsChannelRequest : CreateGuildChannelRequest
{
    /// <summary>
    /// The channel type, always a news channel.
    /// </summary>
    public override GuildChannelType Type => GuildChannelType.GuildNews;
}