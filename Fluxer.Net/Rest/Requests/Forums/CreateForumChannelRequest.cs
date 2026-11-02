namespace Fluxer.Net.Rest;

/// <summary>
/// Request body for creating a forum channel in a guild.
/// </summary>
public class CreateForumChannelRequest : CreateGuildChannelRequest
{
    /// <summary>
    /// The channel type, always a forum channel.
    /// </summary>
    public override GuildChannelType Type => GuildChannelType.GuildForum;

}