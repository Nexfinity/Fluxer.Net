namespace Fluxer.Net.Rest;

public class CreateNewsChannelRequest : CreateGuildChannelRequest
{
    public override GuildChannelType Type => GuildChannelType.GuildNews;
}