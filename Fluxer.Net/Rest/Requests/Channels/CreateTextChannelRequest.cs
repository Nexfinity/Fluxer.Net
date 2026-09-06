namespace Fluxer.Net.Rest;

public class CreateTextChannelRequest : CreateGuildChannelRequest
{
    public override GuildChannelType Type => GuildChannelType.GuildText;
}
