namespace Fluxer.Net;

public static class PrivateChannelHelpers
{
    public static Task AddUserAsync(this GroupChannel channel, User user)
        => channel.Client.Rest.AddRecipientAsync(channel.Id, user.Id);

    public static Task AddUserAsync(this GroupChannel channel, ulong userId)
        => channel.Client.Rest.AddRecipientAsync(channel.Id, userId);

    public static Task RemoveUserAsync(this GroupChannel channel, User user)
        => channel.Client.Rest.RemoveRecipientAsync(channel.Id, user.Id);

    public static Task RemoveUserAsync(this GroupChannel channel, ulong userId)
        => channel.Client.Rest.RemoveRecipientAsync(channel.Id, userId);

    public static Task TransferOwnershipAsync(this GroupChannel channel, User user)
        => channel.Client.Rest.UpdateChannelAsync(channel.Id, new ChannelJson
        {
            OwnerId = user.Id
        });

    public static Task TransferOwnershipAsync(this GroupChannel channel, ulong userId)
        => channel.Client.Rest.UpdateChannelAsync(channel.Id, new ChannelJson
        {
            OwnerId = userId
        });

    public static Task EditNameAsync(this GroupChannel channel, string name)
         => channel.Client.Rest.UpdateChannelAsync(channel.Id, new ChannelJson
         {
             Name = name,
         });
}
