using Fluxer.Net.Gateway;
using System.Collections.Concurrent;
using System.Collections.Immutable;

namespace Fluxer.Net;

/// <summary>
/// Cached guild from the gateway.
/// </summary>
public class SocketGuild : Guild
{
    internal TaskCompletionSource<bool>? _downloaderPromise;

    /// <summary>
    /// Cached current logged in member for the guild.
    /// </summary>
    public SocketGuildMember CurrentMember { get; private set; }

    public SocketRole EveryoneRole => Roles.GetValueOrDefault(Id);

    public bool HasAllMembers { get; internal set; }

    public bool IsAvailable { get; internal set; }

    /// <summary>
    /// The bot's open voice connection in this guild, or null when it is not in voice or voice is not set up.
    /// </summary>
    public IAudioClient? AudioClient => Client.Voice?.GetAudioClient(Id);

    public ConcurrentDictionary<ulong, SocketGuildMember> Members { get; private set; } = new ConcurrentDictionary<ulong, SocketGuildMember>();
    public ConcurrentDictionary<ulong, Channel> Channels { get; private set; } = new ConcurrentDictionary<ulong, Channel>();
    public ConcurrentDictionary<ulong, SocketRole> Roles { get; private set; } = new ConcurrentDictionary<ulong, SocketRole>();
    public ConcurrentDictionary<ulong, GuildEmoji> Emojis { get; private set; }
    public ConcurrentDictionary<ulong, GuildSticker> Stickers { get; private set; }

    public Task<bool> DownloadMembersAsync()
        => (Client as FluxerClient).Gateway.DownloadMembersAsync(this);

    public new async Task<SocketGuildMember?> GetMemberAsync(ulong userId)
    {
        if (Members.TryGetValue(userId, out SocketGuildMember member))
            return member;

        GuildMemberJson? json = await Client.Rest.SendRequestAsync<GuildMemberJson?>(HttpMethod.Get, $"/guilds/{Id}/members/{userId}", false);
        if (json == null)
            return null;

        return AddOrUpdateMember(json);
    }

    public new Task<SocketGuildMember?> GetOwnerAsync()
        => GetMemberAsync(OwnerId);

    public Channel? SystemChannel => GetChannel(SystemChannelId);

    public Channel? RulesChannel => GetChannel(RulesChannelId);

    public SocketRole? GetRole(ulong roleId)
        => Roles.GetValueOrDefault(roleId);

    internal Channel? GetChannel(ulong? channelId)
        => channelId.HasValue ? Channels.GetValueOrDefault(channelId.Value) : null;

    public Channel? GetChannel(ulong channelId)
        => Channels.GetValueOrDefault(channelId);

    public TextChannel? GetTextChannel(ulong channelId)
        => Channels.GetValueOrDefault(channelId) as TextChannel;

    public IVoiceChannel? GetVoiceChannel(ulong channelId)
        => Channels.GetValueOrDefault(channelId) as IVoiceChannel;

    public CategoryChannel? GetCategoryChannel(ulong channelId)
        => Channels.GetValueOrDefault(channelId) as CategoryChannel;

    public LinkChannel? GetLinkChannel(ulong channelId)
        => Channels.GetValueOrDefault(channelId) as LinkChannel;

    public INewsChannel? GetNewsChannel(ulong channelId)
        => Channels.GetValueOrDefault(channelId) as INewsChannel;

    public ForumChannel? GetForumChannel(ulong channelId)
        => Channels.GetValueOrDefault(channelId) as ForumChannel;

    public MediaChannel? GetMediaChannel(ulong channelId)
        => Channels.GetValueOrDefault(channelId) as MediaChannel;

    public GuildEmoji? GetEmoji(ulong emojiId)
        => Emojis.GetValueOrDefault(emojiId);

    public GuildSticker? GetSticker(ulong stickerId)
        => Stickers.GetValueOrDefault(stickerId);

    /// <summary>
    /// Permissions for the guild.
    /// </summary>
    public GuildPermissions Permissions { get; internal set; }

    internal SocketGuild Clone() => MemberwiseClone() as SocketGuild;

    internal SocketGuild(FluxerBaseClient client) : base(client)
    {

    }

    /// <summary>
    /// Create a SocketGuild object from json.
    /// </summary>
    /// <returns><see cref="SocketGuild"/></returns>
    internal static SocketGuild Create(FluxerBaseClient client, GuildGatewayData json, SocketGuildMember currentMember)
    {
        SocketGuild data = new SocketGuild(client)
        {
            CurrentMember = currentMember,
            IsAvailable = !(json.IsUnavailable.HasValue && json.IsUnavailable.Value)
        };
        if (json.Emojis != null && !(client as FluxerClient).Gateway.Config.DisableEmojiCache)
            data.Emojis = new ConcurrentDictionary<ulong, GuildEmoji>(json.Emojis.ToDictionary(x => x.Id.Value, x => GuildEmoji.Create(client, x, data.Id)));
        else
            data.Emojis = new ConcurrentDictionary<ulong, GuildEmoji>();

        if (json.Stickers != null && !(client as FluxerClient).Gateway.Config.DisableStickerCache)
            data.Stickers = new ConcurrentDictionary<ulong, GuildSticker>(json.Stickers.ToDictionary(x => x.Id, x => GuildSticker.Create(client, x, data.Id)));
        else
            data.Stickers = new ConcurrentDictionary<ulong, GuildSticker>();

        data.Members.TryAdd(currentMember.Id, currentMember);
        data.CurrentMember.GuildId = data.Id;
        data.CurrentMember.Guild = data;

        // Null count data on socket guild.
        data.OnlineCount = null;
        data.MemberCount = null;
        data.Update(json.Properties);
        return data;
    }

    internal void UpdatePermissions(SocketRole role)
    {
        Permissions = role.Permissions;
    }

    internal SocketGuildMember AddOrUpdateMember(GuildMemberJson json)
    {
        if (Members.TryGetValue(json.Id, out SocketGuildMember member))
        {
            member.Update(json);
            return member;
        }
        else
        {
            member = SocketGuildMember.Create(Client, json);
            member.GuildId = Id;
            member.Guild = this;
            Members.TryAdd(member.Id, member);
            return member;
        }
    }
}