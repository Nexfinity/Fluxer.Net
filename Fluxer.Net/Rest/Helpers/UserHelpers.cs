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
    public static async Task<DMChannel> GetOrCreateDMChannelAsync(this User user, RequestOptions? options = null)
    {
        DMChannel chan = await user.Client.Rest.CreateDMChannelAsync(new CreatePrivateChannelRequest
        {
            RecipientId = user.Id
        }, options);

        return chan;
    }

    /// <inheritdoc cref="CreateGroupChannelAsync(CurrentUser, HashSet{ulong}, RequestOptions?)" />
    public static async Task<GroupChannel> CreateGroupChannelAsync(this CurrentUser user, HashSet<User> users, RequestOptions? options = null)
    {
        GroupChannel chan = await user.Client.Rest.CreateGroupChannelAsync(new CreatePrivateChannelRequest
        {
            Recipients = users.Select(x => x.Id).ToHashSet()
        }, options);

        return chan;
    }

    /// <summary>
    /// Create a <see cref="GroupChannel"/>.
    /// </summary>
    public static async Task<GroupChannel> CreateGroupChannelAsync(this CurrentUser user, HashSet<ulong> userIds, RequestOptions? options = null)
    {
        GroupChannel chan = await user.Client.Rest.CreateGroupChannelAsync(new CreatePrivateChannelRequest
        {
            Recipients = userIds
        }, options);

        return chan;
    }

    /// <inheritdoc cref="FluxerApiClient.GetUserProfileAsync(ulong, ulong?, bool, bool, RequestOptions?)" />
    public static Task<UserProfileResponse> GetProfileAsync(this User user, ulong? guildId = null, bool mutualFriends = false, bool mutualGuilds = false, RequestOptions? options = null)
        => user.Client.Rest.GetUserProfileAsync(user.Id, guildId, mutualFriends, mutualGuilds, options);
}
