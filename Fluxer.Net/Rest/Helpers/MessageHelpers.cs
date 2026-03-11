namespace Fluxer.Net;

public static class MessageHelpers
{
    public static Task AddReaction(this MessageBaseResponse message, string emoji)
        => message.Client.Rest.AddReactionAsync(message.ChannelId, message.Id, emoji);
}
