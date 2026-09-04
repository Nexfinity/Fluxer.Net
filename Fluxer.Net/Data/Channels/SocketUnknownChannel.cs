namespace Fluxer.Net;

public class SocketUnknownChannel : Channel
{
    internal SocketUnknownChannel(FluxerBaseClient client) : base(client)
    {

    }

    /// <summary>
    /// Create a SocketChannel object from json.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="json"></param>
    /// <param name="guildId"></param>
    /// <returns></returns>
    public static Channel Create(FluxerBaseClient client, ChannelJson json, ulong guildId)
    {
        Channel data = null;

        switch (json.Type)
        {
            case ChannelType.GuildText:
                {
                    data = new SocketTextChannel(client);
                }
                break;
            case ChannelType.GuildVoice:
                {
                    data = new SocketVoiceChannel(client);
                }
                break;
            case ChannelType.Dm:
                {
                    data = new SocketDMChannel(client);
                }
                break;
            case ChannelType.DmPersonalNotes:
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
                    data = new SocketCategoryChannel(client);
                }
                break;
            case ChannelType.GuildLink:
                {
                    data = new SocketLinkChannel(client);
                }
                break;
            default:
                {
                    if (data.GuildId.HasValue)
                        data = new SocketUnknownGuildChannel(client);
                    else
                        data = new SocketUnknownChannel(client);
                }
                break;
        }
        data.GuildId = guildId;
        data.Update(client, json);
        return data;
    }

    internal override void Update(FluxerBaseClient client, ChannelJson json)
    {
        base.Update(client, json);
    }
}