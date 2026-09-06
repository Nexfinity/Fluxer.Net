namespace Fluxer.Net.Rest;

public class CreateLinkChannelRequest : CreateGuildChannelRequest
{
    public override GuildChannelType Type => GuildChannelType.GuildLink;
}
