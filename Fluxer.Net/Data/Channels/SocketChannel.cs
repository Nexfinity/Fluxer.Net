namespace Fluxer.Net;

public class SocketChannel : Channel
{
    internal SocketChannel(FluxerBaseClient client) : base(client)
    {

    }

    /// <summary>
    /// Create a SocketChannel object from json.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="json"></param>
    /// <param name="guild"></param>
    /// <returns></returns>
    public static Channel Create(FluxerBaseClient client, ChannelJson json, SocketGuild? guild)
    {
        Channel data = null;

        switch (json.Type)
        {
            case ChannelType.GuildText:
                {
                    data = new SocketTextChannel(client)
                    {
                        Guild = guild
                    };
                }
                break;
            case ChannelType.GuildVoice:
                {
                    data = new SocketVoiceChannel(client)
                    {
                        Guild = guild
                    };
                }
                break;
            case ChannelType.DM:
                {
                    data = new SocketDMChannel(client);
                }
                break;
            case ChannelType.DMPersonalNotes:
                {
                    data = new SocketSavedNotesChannel(client);
                }
                break;
            case ChannelType.Group:
                {
                    data = new SocketGroupChannel(client);
                }
                break;
            case ChannelType.GuildCategory:
                {
                    data = new SocketCategoryChannel(client)
                    {
                        Guild = guild
                    };
                }
                break;
            case ChannelType.GuildLink:
                {
                    data = new SocketLinkChannel(client)
                    {
                        Guild = guild
                    };
                }
                break;
            default:
                {
                    if (data.GuildId.HasValue)
                        data = new SocketGuildChannel(client)
                        {
                            Guild = guild
                        };
                    else
                        data = new SocketChannel(client);
                }
                break;
        }
        data.GuildId = guild?.Id;
        data.Update(json);
        return data;
    }
}