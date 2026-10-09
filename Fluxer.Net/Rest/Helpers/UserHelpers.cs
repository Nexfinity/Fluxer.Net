using Fluxer.Net.Rest;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Fluxer.Net;
#pragma warning restore IDE0130 // Namespace does not match folder structure

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
        DMChannel chan = await user.Client.Rest.CreateDMChannelAsync(new CreatePrivateChannelRequest
        {
            RecipientId = user.Id
        });

        return chan;
    }

    /// <inheritdoc cref="CreateGroupChannelAsync(CurrentUser, HashSet{ulong})" />
    public static async Task<GroupChannel> CreateGroupChannelAsync(this CurrentUser user, HashSet<User> users)
    {
        GroupChannel chan = await user.Client.Rest.CreateGroupChannelAsync(new CreatePrivateChannelRequest
        {
            Recipients = users.Select(x => x.Id).ToHashSet()
        });

        return chan;
    }

    /// <summary>
    /// Create a <see cref="GroupChannel"/>.
    /// </summary>
    public static async Task<GroupChannel> CreateGroupChannelAsync(this CurrentUser user, HashSet<ulong> userIds)
    {
        GroupChannel chan = await user.Client.Rest.CreateGroupChannelAsync(new CreatePrivateChannelRequest
        {
            Recipients = userIds
        });

        return chan;
    }
}
