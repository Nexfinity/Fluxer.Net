namespace Fluxer.Net.Rest;

public class CreateForumChannelRequest : CreateGuildChannelRequest
{
    public override GuildChannelType Type => GuildChannelType.GuildForum;

}