namespace Fluxer.Net;

public static class UserHelpers
{
    public static async Task<DMChannel> GetOrCreateDMChannelAsync(this User user)
    {
        var chan = await user.Client.Rest.CreatePrivateChannelAsync(new CreatePrivateChannelRequest
        {
            RecipientId = user.Id
        });

        return (DMChannel)chan;
    }

    public static async Task<GroupChannel> CreateGroupChannelAsync(this CurrentUser user, HashSet<User> users)
    {
        var chan = await user.Client.Rest.CreatePrivateChannelAsync(new CreatePrivateChannelRequest
        {
            Recipients = users.Select(x => x.Id).ToHashSet()
        });

        return (GroupChannel)chan;
    }

    public static async Task<GroupChannel> CreateGroupChannelAsync(this CurrentUser user, HashSet<ulong> userIds)
    {
        var chan = await user.Client.Rest.CreatePrivateChannelAsync(new CreatePrivateChannelRequest
        {
            Recipients = userIds
        });

        return (GroupChannel)chan;
    }
}
