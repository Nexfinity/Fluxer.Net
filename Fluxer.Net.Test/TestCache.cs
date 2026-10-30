using Fluxer.Net.Gateway;

namespace Fluxer.Net.Test;

internal static class TestCache
{
    public static void Use(FluxerClient client, int messagesPerChannel = 0, int users = 0, int presences = 0, int invites = 0)
        => client.Gateway.Cache = new GatewayCache(new FluxerCacheConfig
        {
            Messages = new CacheLimits { MaxPerChannel = messagesPerChannel },
            Users = new CacheLimits { MaxEntries = users },
            Presences = new CacheLimits { MaxEntries = presences },
            Invites = new CacheLimits { MaxEntries = invites }
        });

    public static MemoryEntityStore<ulong, MessageGatewayData> Messages(FluxerClient client)
        => Unwrap(client.Gateway.Cache.Messages);

    public static MemoryEntityStore<TKey, TValue> Unwrap<TKey, TValue>(IEntityStore<TKey, TValue> store) where TKey : notnull where TValue : class
        => (MemoryEntityStore<TKey, TValue>)((GuardedEntityStore<TKey, TValue>)store).Inner;
}
