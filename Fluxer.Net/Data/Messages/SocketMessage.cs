using Fluxer.Net.Gateway;

namespace Fluxer.Net;

/// <summary>
/// A message received in a private channel.
/// </summary>
public class SocketPrivateMessage : SocketMessage
{
    /// <summary>
    /// The channel the message was sent in, as a cacheable value.
    /// </summary>
    public new Cacheable<Channel> Channel { get; private set; }

    internal SocketPrivateMessage(FluxerBaseClient client) : base(client)
    {

    }

    /// <summary>
    /// Creates a private message from gateway data.
    /// </summary>
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

/// <summary>
/// A message received through the gateway.
/// </summary>
public class SocketMessage : Message
{
    /// <summary>
    /// The channel the message was sent in.
    /// </summary>
    public Channel Channel { get; internal set; }

    /// <summary>
    /// The author of the message, or null when it is not sent.
    /// </summary>
    public new SocketUser? Author { get; internal set; }

    /// <summary>
    /// The guild the message was sent in, or null for a private message.
    /// </summary>
    public SocketGuild? Guild { get; private set; }

    /// <summary>
    /// The guild member who sent the message, or null when it is not known.
    /// </summary>
    public SocketGuildMember? Member { get; internal set; }

    /// <summary>
    /// Gets a link that opens the message in the web app.
    /// </summary>
    public string GetJumpUrl() => this.GetJumpUrl(Channel.GuildId);

    internal SocketMessage(FluxerBaseClient client) : base(client)
    {

    }

    /// <summary>
    /// Creates a message from gateway data, with its channel and guild.
    /// </summary>
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
    /// <param name="member">The member the copy is sent by.</param>
    /// <param name="content">The content of the copy.</param>
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
