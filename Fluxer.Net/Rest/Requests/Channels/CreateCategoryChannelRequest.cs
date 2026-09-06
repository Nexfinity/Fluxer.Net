namespace Fluxer.Net.Rest;

public class CreateCategoryChannelRequest : CreateGuildChannelRequest
{
    public override GuildChannelType Type => GuildChannelType.GuildCategory;
}
