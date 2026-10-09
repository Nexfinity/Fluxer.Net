namespace Fluxer.Net.Rest;

public class CreateMediaChannelRequest : CreateGuildChannelRequest
{
    public override GuildChannelType Type => GuildChannelType.GuildMedia;

}