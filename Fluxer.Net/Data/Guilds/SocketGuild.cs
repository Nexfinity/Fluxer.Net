using Fluxer.Net.Gateway;
using System.Collections.Concurrent;
using System.Collections.Immutable;

namespace Fluxer.Net;

/// <summary>
/// A guild cached from gateway events, with its members, channels and roles.
/// </summary>
public class SocketGuild : Guild
{
    internal TaskCompletionSource<bool>? _downloaderPromise;

    /// <summary>
    /// The bot's own member of the guild.
    /// </summary>
    public SocketGuildMember CurrentMember { get; private set; }

    /// <summary>
    /// The @everyone role of the guild from the cache, or null when it is not cached.
    /// </summary>
    public SocketRole EveryoneRole => Roles.GetValueOrDefault(Id);

    /// <summary>
    /// Whether the member list has been fully received from the gateway.
    /// </summary>
    public bool HasAllMembers { get; internal set; }

    /// <summary>
    /// Whether the guild is available. False while the gateway reports it as unavailable.
    /// </summary>
    public bool IsAvailable { get; internal set; }

    /// <summary>
    /// The bot's open voice connection in this guild, or null when it is not in voice or voice is not set up.
    /// </summary>
    public IAudioClient? AudioClient => Client.Voice?.GetAudioClient(Id);

    /// <summary>
    /// Every open voice connection the bot has in this guild.
    /// </summary>
    public IReadOnlyCollection<IAudioClient> AudioClients => Client.Voice?.GetAudioClients(Id) ?? [];

    /// <summary>
    /// The members of the guild in the cache, by user ID.
    /// </summary>
    public ConcurrentDictionary<ulong, SocketGuildMember> Members { get; private set; } = new ConcurrentDictionary<ulong, SocketGuildMember>();
    /// <summary>
    /// The channels of the guild in the cache, by channel ID.
    /// </summary>
    public ConcurrentDictionary<ulong, Channel> Channels { get; private set; } = new ConcurrentDictionary<ulong, Channel>();
    /// <summary>
    /// The roles of the guild in the cache, by role ID.
    /// </summary>
    public ConcurrentDictionary<ulong, SocketRole> Roles { get; private set; } = new ConcurrentDictionary<ulong, SocketRole>();
    /// <summary>
    /// The emojis of the guild in the cache, by emoji ID. Empty when the emoji cache is disabled.
    /// </summary>
    public ConcurrentDictionary<ulong, GuildEmoji> Emojis { get; private set; }
    /// <summary>
    /// The stickers of the guild in the cache, by sticker ID. Empty when the sticker cache is disabled.
    /// </summary>
    public ConcurrentDictionary<ulong, GuildSticker> Stickers { get; private set; }

    /// <summary>
    /// Requests the member list of the guild from the gateway.
    /// </summary>
    /// <returns>True when the members arrived within 30 seconds, otherwise false.</returns>
    public Task<bool> DownloadMembersAsync()
        => (Client as FluxerClient).Gateway.DownloadMembersAsync(this);

    /// <summary>
    /// Gets a member from the cache, or fetches it from the API when it is not cached.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <returns>The member, or null when it is not cached and the request does not succeed.</returns>
    public new async Task<SocketGuildMember?> GetMemberAsync(ulong userId)
    {
        if (Members.TryGetValue(userId, out SocketGuildMember member))
            return member;

        GuildMemberJson? json = await Client.Rest.SendRequestAsync<GuildMemberJson?>(HttpMethod.Get, $"/guilds/{Id}/members/{userId}", false);
        if (json == null)
            return null;

        return AddOrUpdateMember(json);
    }

    /// <summary>
    /// Gets the owner of the guild, from the cache or the API.
    /// </summary>
    public new Task<SocketGuildMember?> GetOwnerAsync()
        => GetMemberAsync(OwnerId);

    /// <summary>
    /// The system channel from the cache, or null when it is not set or not cached.
    /// </summary>
    public Channel? SystemChannel => GetChannel(SystemChannelId);

    /// <summary>
    /// The rules channel from the cache, or null when it is not set or not cached.
    /// </summary>
    public Channel? RulesChannel => GetChannel(RulesChannelId);

    /// <summary>
    /// Gets a role from the cache by its ID, or null when it is not cached.
    /// </summary>
    /// <param name="roleId">The ID of the role.</param>
    public SocketRole? GetRole(ulong roleId)
        => Roles.GetValueOrDefault(roleId);

    internal Channel? GetChannel(ulong? channelId)
        => channelId.HasValue ? Channels.GetValueOrDefault(channelId.Value) : null;

    /// <summary>
    /// Gets a channel from the cache by its ID, or null when it is not cached.
    /// </summary>
    /// <param name="channelId">The ID of the channel.</param>
    public Channel? GetChannel(ulong channelId)
        => Channels.GetValueOrDefault(channelId);

    /// <summary>
    /// Gets a text channel from the cache by its ID, or null when there is no text channel with that ID.
    /// </summary>
    /// <param name="channelId">The ID of the channel.</param>
    public TextChannel? GetTextChannel(ulong channelId)
        => Channels.GetValueOrDefault(channelId) as TextChannel;

    /// <summary>
    /// Gets a voice channel from the cache by its ID, or null when there is no voice channel with that ID.
    /// </summary>
    /// <param name="channelId">The ID of the channel.</param>
    public IVoiceChannel? GetVoiceChannel(ulong channelId)
        => Channels.GetValueOrDefault(channelId) as IVoiceChannel;

    /// <summary>
    /// Gets a category channel from the cache by its ID, or null when there is no category with that ID.
    /// </summary>
    /// <param name="channelId">The ID of the channel.</param>
    public CategoryChannel? GetCategoryChannel(ulong channelId)
        => Channels.GetValueOrDefault(channelId) as CategoryChannel;

    /// <summary>
    /// Gets a link channel from the cache by its ID, or null when there is no link channel with that ID.
    /// </summary>
    /// <param name="channelId">The ID of the channel.</param>
    public LinkChannel? GetLinkChannel(ulong channelId)
        => Channels.GetValueOrDefault(channelId) as LinkChannel;

    /// <summary>
    /// Gets a news channel from the cache by its ID, or null when there is no news channel with that ID.
    /// </summary>
    /// <param name="channelId">The ID of the channel.</param>
    public INewsChannel? GetNewsChannel(ulong channelId)
        => Channels.GetValueOrDefault(channelId) as INewsChannel;

    /// <summary>
    /// Gets a forum channel from the cache by its ID, or null when there is no forum channel with that ID.
    /// </summary>
    /// <param name="channelId">The ID of the channel.</param>
    public ForumChannel? GetForumChannel(ulong channelId)
        => Channels.GetValueOrDefault(channelId) as ForumChannel;

    /// <summary>
    /// Gets a media channel from the cache by its ID, or null when there is no media channel with that ID.
    /// </summary>
    /// <param name="channelId">The ID of the channel.</param>
    public MediaChannel? GetMediaChannel(ulong channelId)
        => Channels.GetValueOrDefault(channelId) as MediaChannel;

    /// <summary>
    /// Gets an emoji from the cache by its ID, or null when it is not cached.
    /// </summary>
    /// <param name="emojiId">The ID of the emoji.</param>
    public GuildEmoji? GetEmoji(ulong emojiId)
        => Emojis.GetValueOrDefault(emojiId);

    /// <summary>
    /// Gets a sticker from the cache by its ID, or null when it is not cached.
    /// </summary>
    /// <param name="stickerId">The ID of the sticker.</param>
    public GuildSticker? GetSticker(ulong stickerId)
        => Stickers.GetValueOrDefault(stickerId);

    /// <summary>
    /// The permissions of the guild's @everyone role.
    /// </summary>
    public GuildPermissions Permissions { get; internal set; }

    internal SocketGuild Clone() => MemberwiseClone() as SocketGuild;

    internal SocketGuild(FluxerBaseClient client) : base(client)
    {

    }

    /// <summary>
    /// Creates a cached guild from its gateway data.
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

        data.Update(json.Properties);
        data.OnlineCount = json.OnlineCount;
        data.MemberCount = json.MemberCount;
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