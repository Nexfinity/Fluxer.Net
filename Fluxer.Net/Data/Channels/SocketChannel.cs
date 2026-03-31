namespace Fluxer.Net;

public class SocketChannel : Channel
{
    /// <summary>
    /// Permissions for the channel.
    /// </summary>
    public ChannelPermissions Permissions { get; internal set; }

    internal SocketChannel(FluxerBaseClient client) : base(client)
    {

    }

    public static SocketChannel Create(FluxerBaseClient client, ChannelJson json, ulong guildId)
    {
        SocketChannel data = new SocketChannel(client);
        data.GuildId = guildId;
        data.Update(client, json);
        return data;
    }

    internal void Update(FluxerBaseClient client, ChannelJson json)
    {
        base.Update(client, json);
        PermissionOverwriteJson? overwrite = json.PermissionOverwrites.FirstOrDefault(x => x.Id == Id);
        if (overwrite != null)
            Permissions = new ChannelPermissions((Permissions)overwrite.Allow);
        else
            Permissions = new ChannelPermissions(0);
    }
}