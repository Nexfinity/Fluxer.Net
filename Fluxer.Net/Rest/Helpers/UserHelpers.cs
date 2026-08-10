namespace Fluxer.Net;

/// <summary>
/// Http methods for <see cref="User"/> class. 
/// </summary>
public static class UserHelpers
{
    /// <summary>
    /// Create a <see cref="DMChannel"/>.
    /// </summary>
    public static async Task<DMChannel> GetOrCreateDMChannelAsync(this User user)
    {
        Channel chan = await user.Client.Rest.CreatePrivateChannelAsync(new CreatePrivateChannelRequest
        {
            RecipientId = user.Id
        });

        return (DMChannel)chan;
    }

    /// <inheritdoc cref="CreateGroupChannelAsync(CurrentUser, HashSet{ulong})" />
    public static async Task<GroupChannel> CreateGroupChannelAsync(this CurrentUser user, HashSet<User> users)
    {
        Channel chan = await user.Client.Rest.CreatePrivateChannelAsync(new CreatePrivateChannelRequest
        {
            Recipients = users.Select(x => x.Id).ToHashSet()
        });

        return (GroupChannel)chan;
    }

    /// <summary>
    /// Create a <see cref="GroupChannel"/>.
    /// </summary>
    public static async Task<GroupChannel> CreateGroupChannelAsync(this CurrentUser user, HashSet<ulong> userIds)
    {
        Channel chan = await user.Client.Rest.CreatePrivateChannelAsync(new CreatePrivateChannelRequest
        {
            Recipients = userIds
        });

        return (GroupChannel)chan;
    }
}
