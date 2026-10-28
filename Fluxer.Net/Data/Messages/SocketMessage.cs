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
            Author = json.Author != null ? SocketUser.Create(client, json.Author) : null
        };

        ((SocketMessage)data).Channel = channel.Value;
        data.Channel = new Cacheable<Channel>(channel.Id, channel.Value, async () =>
        {
            Channel? downloaded = await channel.GetOrDownloadAsync();
            if (downloaded != null)
                ((SocketMessage)data).Channel = downloaded;
            return downloaded;
        });

        data.Update(json);
        return data;
    }
}

public class SocketMessage : Message
{
    public Channel Channel { get; internal set; }

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

    /// <summary>
    /// Creates a local copy of this message as if another member had sent it with other content, for example to run
    /// a command on that member's behalf. The copy only exists in memory and is never sent to Fluxer.
    /// </summary>
    /// <param name="member">Member the copy is sent by.</param>
    /// <param name="content">Content of the copy.</param>
    /// <returns><see cref="SocketMessage"/></returns>
    public SocketMessage CopyAs(SocketGuildMember member, string content)
    {
        SocketMessage copy = (SocketMessage)MemberwiseClone();
        SocketUser author = member.User as SocketUser ?? SocketUser.Create(Client, member.User.ToJson());
        copy.Author = author;
        ((Message)copy).Author = author;
        copy.Member = member;
        copy.Content = content;
        return copy;
    }
}
