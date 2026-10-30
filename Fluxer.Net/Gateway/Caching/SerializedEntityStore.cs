using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Fluxer.Net.Gateway;

/// <summary>
/// A base for stores backed by a key-value database such as Redis. It serializes entries with <see cref="FluxerJson"/>,
/// keeps every key under a namespace and tracks the keys of each guild and channel. A backend provides six string operations.
/// </summary>
/// <remarks>
/// <para>Keys look like <c>{namespace}:e:{key}</c>, the index of a guild or channel is the set <c>{namespace}:s:{id}</c>,
/// all keys are in the set <c>{namespace}:all</c> and the scope sets are listed in <c>{namespace}:scopes</c>.</para>
/// <para>With an <see cref="Expiry"/>, entries are written with that time to live. Index sets can then hold keys that
/// already expired. Those read as misses and are removed the next time their scope or the store is cleared.</para>
/// <para>Override <see cref="GetManyAsync"/> and <see cref="RemoveManyAsync"/> when the backend can do them in one round trip.</para>
/// </remarks>
public abstract class SerializedEntityStore<TKey, TValue> : IEntityStore<TKey, TValue> where TKey : notnull where TValue : class
{
    /// <summary>
    /// Creates the store.
    /// </summary>
    /// <param name="keyNamespace">A prefix unique to this store, for example <c>mybot:messages</c>. Stores sharing a backend need different ones.</param>
    /// <param name="expiry">How long entries are kept, or null to keep them until removed.</param>
    protected SerializedEntityStore(string keyNamespace, TimeSpan? expiry = null)
    {
        if (string.IsNullOrWhiteSpace(keyNamespace))
            throw new ArgumentException("A key namespace is required.", nameof(keyNamespace));

        Namespace = keyNamespace;
        Expiry = expiry;
    }

    /// <summary>
    /// The prefix of every key this store writes.
    /// </summary>
    public string Namespace { get; }

    /// <summary>
    /// How long entries are kept, or null to keep them until removed.
    /// </summary>
    public TimeSpan? Expiry { get; }

    /// <summary>
    /// Reads a string, or null when the key doesn't exist.
    /// </summary>
    protected abstract ValueTask<string?> ReadAsync(string key, CancellationToken cancellationToken);

    /// <summary>
    /// Writes a string, replacing any value, with an optional time to live.
    /// </summary>
    protected abstract ValueTask WriteAsync(string key, string value, TimeSpan? expiry, CancellationToken cancellationToken);

    /// <summary>
    /// Deletes keys. Keys that don't exist are ignored.
    /// </summary>
    protected abstract ValueTask DeleteAsync(IReadOnlyCollection<string> keys, CancellationToken cancellationToken);

    /// <summary>
    /// Adds a member to a set.
    /// </summary>
    protected abstract ValueTask AddToSetAsync(string set, string member, CancellationToken cancellationToken);

    /// <summary>
    /// Removes a member from a set.
    /// </summary>
    protected abstract ValueTask RemoveFromSetAsync(string set, string member, CancellationToken cancellationToken);

    /// <summary>
    /// Reads every member of a set, or an empty collection when it doesn't exist.
    /// </summary>
    protected abstract ValueTask<IReadOnlyCollection<string>> ReadSetAsync(string set, CancellationToken cancellationToken);

    /// <summary>
    /// Turns a key into the text used in backend keys. The default is <see cref="object.ToString"/>.
    /// </summary>
    protected virtual string FormatKey(TKey key) => key.ToString()!;

    private string EntryKey(string key) => $"{Namespace}:e:{key}";

    private string ScopeSet(ulong id) => $"{Namespace}:s:{id}";

    private string AllSet => $"{Namespace}:all";

    private string ScopesSet => $"{Namespace}:scopes";

    private sealed class Envelope
    {
        [JsonProperty("g")]
        public ulong? GuildId { get; set; }

        [JsonProperty("c")]
        public ulong? ChannelId { get; set; }

        [JsonProperty("v")]
        public JToken? Value { get; set; }
    }

    /// <inheritdoc />
    public virtual async ValueTask<TValue?> GetAsync(TKey key, CancellationToken cancellationToken = default)
        => Read(await ReadAsync(EntryKey(FormatKey(key)), cancellationToken))?.Value?.ToObject<TValue>(FluxerClient._gatewaySerializer);

    /// <inheritdoc />
    public virtual async ValueTask SetAsync(TKey key, TValue value, CacheScope scope, CancellationToken cancellationToken = default)
    {
        string formatted = FormatKey(key);
        Envelope envelope = new Envelope
        {
            GuildId = scope.GuildId,
            ChannelId = scope.ChannelId,
            Value = JToken.FromObject(value, FluxerClient._gatewaySerializer)
        };
        await WriteAsync(EntryKey(formatted), FluxerJson.Serialize(envelope), Expiry, cancellationToken);
        await AddToSetAsync(AllSet, formatted, cancellationToken);
        foreach (ulong id in ScopeIds(envelope))
        {
            await AddToSetAsync(ScopeSet(id), formatted, cancellationToken);
            await AddToSetAsync(ScopesSet, id.ToString(), cancellationToken);
        }
    }

    /// <inheritdoc />
    public virtual async ValueTask<TValue?> RemoveAsync(TKey key, CancellationToken cancellationToken = default)
    {
        string formatted = FormatKey(key);
        Envelope? envelope = Read(await ReadAsync(EntryKey(formatted), cancellationToken));
        await DeleteAsync(new[] { EntryKey(formatted) }, cancellationToken);
        await RemoveFromSetAsync(AllSet, formatted, cancellationToken);
        if (envelope != null)
        {
            foreach (ulong id in ScopeIds(envelope))
                await RemoveFromSetAsync(ScopeSet(id), formatted, cancellationToken);
        }

        return envelope?.Value?.ToObject<TValue>(FluxerClient._gatewaySerializer);
    }

    /// <inheritdoc />
    public virtual async ValueTask<IReadOnlyDictionary<TKey, TValue>> GetManyAsync(IEnumerable<TKey> keys, CancellationToken cancellationToken = default)
    {
        Dictionary<TKey, TValue> found = new Dictionary<TKey, TValue>();
        foreach (TKey key in keys)
        {
            if (await GetAsync(key, cancellationToken) is { } value)
                found[key] = value;
        }

        return found;
    }

    /// <inheritdoc />
    public virtual async ValueTask<IReadOnlyDictionary<TKey, TValue>> RemoveManyAsync(IEnumerable<TKey> keys, CancellationToken cancellationToken = default)
    {
        Dictionary<TKey, TValue> removed = new Dictionary<TKey, TValue>();
        foreach (TKey key in keys)
        {
            if (await RemoveAsync(key, cancellationToken) is { } value)
                removed[key] = value;
        }

        return removed;
    }

    /// <inheritdoc />
    public virtual async ValueTask RemoveScopeAsync(ulong scopeId, CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<string> keys = await ReadSetAsync(ScopeSet(scopeId), cancellationToken);
        if (keys.Count > 0)
            await DeleteAsync(keys.Select(EntryKey).ToList(), cancellationToken);

        foreach (string key in keys)
            await RemoveFromSetAsync(AllSet, key, cancellationToken);

        await DeleteAsync(new[] { ScopeSet(scopeId) }, cancellationToken);
        await RemoveFromSetAsync(ScopesSet, scopeId.ToString(), cancellationToken);
    }

    /// <inheritdoc />
    public virtual async ValueTask ClearAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<string> keys = await ReadSetAsync(AllSet, cancellationToken);
        IReadOnlyCollection<string> scopes = await ReadSetAsync(ScopesSet, cancellationToken);
        List<string> delete = keys.Select(EntryKey).Concat(scopes.Select(x => $"{Namespace}:s:{x}")).ToList();
        delete.Add(AllSet);
        delete.Add(ScopesSet);
        await DeleteAsync(delete, cancellationToken);
    }

    /// <inheritdoc />
    public virtual ValueTask DisposeAsync() => default;

    private static Envelope? Read(string? json) => json == null ? null : FluxerJson.Deserialize<Envelope>(json);

    private static IEnumerable<ulong> ScopeIds(Envelope envelope)
    {
        if (envelope.GuildId is ulong guildId)
            yield return guildId;
        if (envelope.ChannelId is ulong channelId && channelId != envelope.GuildId)
            yield return channelId;
    }
}
