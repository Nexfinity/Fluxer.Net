using Fluxer.Net.Gateway;

namespace Fluxer.Net.Commands
{
    /// <summary> The context of a command which may contain the client, user, guild, channel, and message. </summary>
    public class CommandContext : ICommandContext
    {
        /// <inheritdoc/>
        public FluxerClient Client { get; }

        /// <inheritdoc/>
        public ApiClient Rest { get; }

        /// <inheritdoc/>
        public GatewayClient Gateway { get; }

        /// <inheritdoc/>
        public SocketGuild Guild { get; }

        /// <inheritdoc/>
        public Channel Channel { get; }

        /// <inheritdoc/>
        public SocketUser User { get; }

        /// <inheritdoc/>
        public SocketGuildMember? Member { get; }

        /// <inheritdoc/>
        public SocketMessage Message { get; }

        /// <summary> Indicates whether the channel that the command is executed in is a private channel. </summary>
        public bool IsPrivate => Channel.Type == ChannelType.Dm;

        IGuild ICommandContext.Guild => Guild;

        IChannel ICommandContext.Channel => Channel;

        IUser ICommandContext.User => User;

        IGuildMember? ICommandContext.Member => Member;

        IMessage ICommandContext.Message => Message;

        /// <summary>
        ///     Initializes a new <see cref="CommandContext" /> class with the provided client and message.
        /// </summary>
        /// <param name="client">The underlying client.</param>
        /// <param name="msg">The underlying message.</param>
        public CommandContext(FluxerClient client, MessageGatewayData message)
        {
            Client = client;
            Rest = client.Rest;
            Gateway = client.Gateway;
            Message = SocketMessage.Create(client, message);
            Channel = client.Gateway.GetChannel(message.ChannelId);
            User = SocketUser.Create(client, message.Author);
            if (message.GuildId.HasValue)
            {
                Guild = client.Gateway.GetGuild(message.GuildId.Value);
                Member = Guild.GetMember(User.Id);
                if (Member == null)
                {
                    message.Member.User = message.Author;
                    Guild.AddOrUpdateMember(Client, message.Member);
                    Member = Guild.GetMember(User.Id);
                }

                if (Member.Guild == null)
                    Member.Guild = Guild;
            }
        }
    }
}
