namespace Fluxer.Net;

public class SocketUnknownChannel : Channel
{
    /// <summary>
    /// Permissions for the channel.
    /// </summary>
    public ChannelPermissions Permissions { get; internal set; }

    internal SocketUnknownChannel(FluxerBaseClient client) : base(client)
    {

    }

    public static Channel Create(FluxerBaseClient client, ChannelJson json, ulong guildId)
    {
        Channel data = null;

        switch (json.Type)
        {
            case ChannelType.GuildText:
                {
                    data = new SocketTextChannel(client);
                    data.IsTextable = true;
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
                    data.IsTextable = true;
                }
                break;
            case ChannelType.DmPersonalNotes:
                {
                    data = new SocketSavedNotesChannel(client);
                    data.IsTextable = true;
                }
                break;
            case ChannelType.GroupDm:
                {
                    data = new SocketGroupChannel(client);
                    data.IsTextable = true;
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
                    data.IsTextable = true;
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
        PermissionOverwriteJson? overwrite = json.PermissionOverwrites.FirstOrDefault(x => x.Id == Id);
        if (overwrite != null)
            Permissions = overwrite.Allow;
        else
            Permissions = new ChannelPermissions(0);
    }
}