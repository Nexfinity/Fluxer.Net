using Fluxer.Net.Rest;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Fluxer.Net;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Http methods for <see cref="DMChannel"/> and <see cref="GroupChannel"/> class. 
/// </summary>
public static class PrivateChannelHelpers
{
    /// <inheritdoc cref="FluxerApiClient.AddRecipientAsync(ulong, ulong, RequestOptions?)" />
    public static Task AddUserAsync(this GroupChannel channel, User user, RequestOptions? options = null)
        => channel.Client.Rest.AddRecipientAsync(channel.Id, user.Id, options);

    /// <inheritdoc cref="FluxerApiClient.AddRecipientAsync(ulong, ulong, RequestOptions?)" />
    public static Task AddUserAsync(this GroupChannel channel, ulong userId, RequestOptions? options = null)
        => channel.Client.Rest.AddRecipientAsync(channel.Id, userId, options);

    /// <inheritdoc cref="FluxerApiClient.RemoveRecipientAsync(ulong, ulong, RequestOptions?)" />
    public static Task RemoveUserAsync(this GroupChannel channel, User user, RequestOptions? options = null)
        => channel.Client.Rest.RemoveRecipientAsync(channel.Id, user.Id, options);

    /// <inheritdoc cref="FluxerApiClient.RemoveRecipientAsync(ulong, ulong, RequestOptions?)" />
    public static Task RemoveUserAsync(this GroupChannel channel, ulong userId, RequestOptions? options = null)
        => channel.Client.Rest.RemoveRecipientAsync(channel.Id, userId, options);

    /// <inheritdoc cref="TransferOwnershipAsync(GroupChannel, ulong, RequestOptions?)" />
    public static Task TransferOwnershipAsync(this GroupChannel channel, User user, RequestOptions? options = null)
        => channel.Client.Rest.UpdateChannelAsync(channel.Id, new ChannelJson
        {
            OwnerId = user.Id
        }, options);

    /// <summary>
    /// Change ownership of the group channel.
    /// </summary>
    /// /// <remarks>
    /// Requires group ownership.
    /// </remarks>
    public static Task TransferOwnershipAsync(this GroupChannel channel, ulong userId, RequestOptions? options = null)
        => channel.Client.Rest.UpdateChannelAsync(channel.Id, new ChannelJson
        {
            OwnerId = userId
        }, options);

    /// <summary>
    /// Edit the name of the group channel.
    /// </summary>
    /// <remarks>
    /// Requires group ownership.
    /// </remarks>
    public static Task EditNameAsync(this GroupChannel channel, string name, RequestOptions? options = null)
         => channel.Client.Rest.UpdateChannelAsync(channel.Id, new ChannelJson
         {
             Name = name,
         }, options);

    /// <summary>
    /// Close the DM channel.
    /// </summary>
    public static Task CloseAsync(this DMChannel channel, RequestOptions? options = null)
        => channel.Client.Rest.DeleteChannelAsync(channel.Id, options);
}
