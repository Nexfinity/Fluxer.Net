using System.Collections.Concurrent;

namespace Fluxer.Net.Gateway;

/// <summary>
/// Keeps the most recent guild messages of each channel for edit and delete events to hand over as they were.
/// </summary>
public sealed class MessageCache
{
    private readonly int _perChannel;
    private readonly ConcurrentDictionary<ulong, SocketMessage> _messages = new();
    private readonly ConcurrentDictionary<ulong, ChannelMessages> _channels = new();

    private sealed class ChannelMessages
    {
        public readonly LinkedList<ulong> Order = new LinkedList<ulong>();
        public readonly Dictionary<ulong, LinkedListNode<ulong>> Nodes = new Dictionary<ulong, LinkedListNode<ulong>>();
    }

    /// <summary>
    /// Creates a cache that keeps up to <paramref name="perChannel"/> messages per channel. Zero or less keeps nothing.
    /// </summary>
    public MessageCache(int perChannel)
    {
        _perChannel = perChannel;
    }

    /// <summary>
    /// Whether the cache keeps any messages.
    /// </summary>
    public bool IsEnabled => _perChannel > 0;

    /// <summary>
    /// The number of messages currently cached.
    /// </summary>
    public int Count => _messages.Count;

    /// <summary>
    /// Gets a cached message, or null when it isn't cached.
    /// </summary>
    public SocketMessage? Get(ulong messageId) => _messages.GetValueOrDefault(messageId);

    /// <summary>
    /// Gets the cached messages of a channel, oldest first.
    /// </summary>
    public IReadOnlyList<SocketMessage> GetChannel(ulong channelId)
    {
        if (!_channels.TryGetValue(channelId, out ChannelMessages? channel))
            return Array.Empty<SocketMessage>();

        List<ulong> ids;
        lock (channel)
            ids = channel.Order.ToList();

        return ids.Select(Get).Where(x => x != null).Cast<SocketMessage>().ToList();
    }

    /// <summary>
    /// Adds a message, dropping the channel's oldest one when the channel is full.
    /// </summary>
    public void Add(SocketMessage message)
    {
        if (!IsEnabled)
            return;

        ChannelMessages channel = _channels.GetOrAdd(message.ChannelId, _ => new ChannelMessages());
        lock (channel)
        {
            _messages[message.Id] = message;
            if (channel.Nodes.ContainsKey(message.Id))
                return;

            channel.Nodes[message.Id] = channel.Order.AddLast(message.Id);
            while (channel.Order.Count > _perChannel)
            {
                ulong oldest = channel.Order.First!.Value;
                channel.Order.RemoveFirst();
                channel.Nodes.Remove(oldest);
                _messages.TryRemove(oldest, out _);
            }
        }
    }

    /// <summary>
    /// Replaces a cached message with its edited version and returns the version from before, or null when it wasn't cached.
    /// </summary>
    public SocketMessage? Replace(SocketMessage message)
    {
        if (!IsEnabled || !_messages.TryGetValue(message.Id, out SocketMessage? before))
            return null;

        _messages[message.Id] = message;
        return before;
    }

    /// <summary>
    /// Removes a message and returns it, or null when it wasn't cached.
    /// </summary>
    public SocketMessage? Remove(ulong messageId)
    {
        if (!_messages.TryRemove(messageId, out SocketMessage? message))
            return null;

        if (_channels.TryGetValue(message.ChannelId, out ChannelMessages? channel))
        {
            lock (channel)
            {
                if (channel.Nodes.Remove(messageId, out LinkedListNode<ulong>? node))
                    channel.Order.Remove(node);
            }
        }

        return message;
    }

    /// <summary>
    /// Forgets every cached message of a channel or thread.
    /// </summary>
    public void RemoveChannel(ulong channelId)
    {
        if (!_channels.TryRemove(channelId, out ChannelMessages? channel))
            return;

        lock (channel)
        {
            foreach (ulong id in channel.Order)
                _messages.TryRemove(id, out _);
        }
    }

    /// <summary>
    /// Forgets every cached message of a guild.
    /// </summary>
    public void RemoveGuild(ulong guildId)
    {
        foreach (ulong channelId in _messages.Values.Where(x => x.Guild?.Id == guildId).Select(x => x.ChannelId).Distinct().ToList())
            RemoveChannel(channelId);
    }
}
