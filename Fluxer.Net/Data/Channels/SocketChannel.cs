namespace Fluxer.Net;

/// <summary>
/// The factory for the cached channel types. No channel type derives from it.
/// </summary>
public abstract class SocketChannel : Channel
{
    internal SocketChannel(FluxerBaseClient client) : base(client)
    {

    }

    /// <summary>
    /// Creates a cached channel from JSON.
    /// </summary>
    internal static Channel Create(FluxerBaseClient client, ChannelJson json, SocketGuild? guild)
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
            case ChannelType.NewsThread:
            case ChannelType.PublicThread:
            case ChannelType.PrivateThread:
                {
                    data = new SocketThreadChannel(client)
                    {
                        Guild = guild,
                        ThreadType = ThreadChannel.GetThreadType(json.Type),
                        ParentChannel = json.ParentId.HasValue ? (client as FluxerClient)?.Gateway?.GetChannel(json.ParentId.Value) : null
                    };
                }
                break;
            case ChannelType.GuildNews:
                {
                    data = new SocketNewsChannel(client)
                    {
                        Guild = guild
                    };
                }
                break;
            case ChannelType.GuildVoice:
            case ChannelType.GuildStageVoice:
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
            case ChannelType.GuildForum:
                {
                    data = new SocketForumChannel(client)
                    {
                        Guild = guild
                    };
                }
                break;
            case ChannelType.GuildMedia:
                {
                    data = new SocketMediaChannel(client)
                    {
                        Guild = guild
                    };
                }
                break;
            default:
                {
                    data = new Channel(client);
                }
                break;
        }
        data.GuildId = guild?.Id;
        data.Update(json);
        return data;
    }
}