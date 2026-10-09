namespace Fluxer.Net.Rest;

public class CreatePrivateThreadRequest : CreateThreadRequest
{
    public override GuildChannelType Type => GuildChannelType.PrivateThread;
}
