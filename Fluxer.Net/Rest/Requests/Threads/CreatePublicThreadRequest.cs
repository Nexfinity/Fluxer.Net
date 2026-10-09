namespace Fluxer.Net.Rest;

public class CreatePublicThreadRequest : CreateThreadRequest
{
    public override GuildChannelType Type => GuildChannelType.PublicThread;
}
