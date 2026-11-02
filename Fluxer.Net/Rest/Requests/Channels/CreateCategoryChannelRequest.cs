namespace Fluxer.Net.Rest;

/// <summary>
/// Request body for creating a category channel in a guild.
/// </summary>
public class CreateCategoryChannelRequest : CreateGuildChannelRequest
{
    /// <summary>
    /// The channel type, always a category.
    /// </summary>
    public override GuildChannelType Type => GuildChannelType.GuildCategory;
}
