using Fluxer.Net.Gateway;

namespace Fluxer.Net;

public class SocketPrivateMessage : SocketMessage
{
    public new Cacheable<Channel> Channel { get; private set; }

    internal SocketPrivateMessage(FluxerBaseClient client) : base(client)
    {

    }

    /// <summary>
    /// Create a SocketMessage object from json.
    /// </summary>
    /// <returns><see cref="SocketMessage"/></returns>
    internal static SocketPrivateMessage Create(FluxerBaseClient client, MessageGatewayData json, Cacheable<Channel> channel)
    {
        SocketPrivateMessage data = new SocketPrivateMessage(client)
        {
            Channel = channel,
            Author = json.Author != null ? SocketUser.Create(client, json.Author) : null
        };

        data.Update(json);
        return data;
    }
}

public class SocketMessage : Message
{
    public Channel Channel { get; private set; }

    public new SocketUser? Author { get; internal set; }

    public SocketGuild? Guild { get; private set; }

    public SocketGuildMember? Member { get; internal set; }

    public string GetJumpUrl()
    {
        string target = Channel.GuildId.HasValue ? Channel.GuildId.Value.ToString() : "@me";
        return $"{Client.Config.WebUrl}/{target}/{ChannelId}/{Id}";
    }

    internal SocketMessage(FluxerBaseClient client) : base(client)
    {

    }

    /// <summary>
    /// Create a SocketMessage object from json.
    /// </summary>
    /// <returns><see cref="SocketMessage"/></returns>
    internal static SocketMessage Create(FluxerBaseClient client, MessageGatewayData json, Channel channel, SocketGuild guild)
    {
        SocketMessage data = new SocketMessage(client)
        {
            Channel = channel,
            Guild = guild,
            Author = json.Author != null ? SocketUser.Create(client, json.Author) : null
        };

        if (json.GuildId.HasValue && data.Author != null)
        {
            data.Member = data.Guild.Members.GetValueOrDefault(json.Author.Id);
            if (data.Member == null && json.Member != null)
            {
                json.Member.User = json.Author;
                data.Member = SocketGuildMember.Create(client, json.Member);
                data.Member.GuildId = json.GuildId.Value;
                data.Member.Guild = data.Guild;
            }
        }
        data.Update(json);
        return data;
    }
}
