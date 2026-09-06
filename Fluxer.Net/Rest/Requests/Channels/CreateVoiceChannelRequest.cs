namespace Fluxer.Net.Rest;

public class CreateVoiceChannelRequest : CreateGuildChannelRequest
{
    public override GuildChannelType Type => GuildChannelType.GuildVoice;
}