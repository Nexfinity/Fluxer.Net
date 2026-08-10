namespace Fluxer.Net;

/// <summary>
/// Http methods for <see cref="DMChannel"/> and <see cref="GroupChannel"/> class. 
/// </summary>
public static class PrivateChannelHelpers
{
    /// <inheritdoc cref="ApiClient.AddRecipientAsync(ulong, ulong)" />
    public static Task AddUserAsync(this GroupChannel channel, User user)
        => channel.Client.Rest.AddRecipientAsync(channel.Id, user.Id);

    /// <inheritdoc cref="ApiClient.AddRecipientAsync(ulong, ulong)" />
    public static Task AddUserAsync(this GroupChannel channel, ulong userId)
        => channel.Client.Rest.AddRecipientAsync(channel.Id, userId);

    /// <inheritdoc cref="ApiClient.RemoveRecipientAsync(ulong, ulong)" />
    public static Task RemoveUserAsync(this GroupChannel channel, User user)
        => channel.Client.Rest.RemoveRecipientAsync(channel.Id, user.Id);

    /// <inheritdoc cref="ApiClient.RemoveRecipientAsync(ulong, ulong)" />
    public static Task RemoveUserAsync(this GroupChannel channel, ulong userId)
        => channel.Client.Rest.RemoveRecipientAsync(channel.Id, userId);

    /// <inheritdoc cref="TransferOwnershipAsync(GroupChannel, ulong)" />
    public static Task TransferOwnershipAsync(this GroupChannel channel, User user)
        => channel.Client.Rest.UpdateChannelAsync(channel.Id, new ChannelJson
        {
            OwnerId = user.Id
        });

    /// <summary>
    /// Change ownership of the group channel.
    /// </summary>
    /// /// <remarks>
    /// Requires group ownership.
    /// </remarks>
    public static Task TransferOwnershipAsync(this GroupChannel channel, ulong userId)
        => channel.Client.Rest.UpdateChannelAsync(channel.Id, new ChannelJson
        {
            OwnerId = userId
        });

    /// <summary>
    /// Edit the name of the group channel.
    /// </summary>
    /// <remarks>
    /// Requires group ownership.
    /// </remarks>
    public static Task EditNameAsync(this GroupChannel channel, string name)
         => channel.Client.Rest.UpdateChannelAsync(channel.Id, new ChannelJson
         {
             Name = name,
         });

    /// <summary>
    /// Close the DM channel.
    /// </summary>
    public static Task CloseAsync(this DMChannel channel)
        => channel.Client.Rest.DeleteChannelAsync(channel.Id);
}
