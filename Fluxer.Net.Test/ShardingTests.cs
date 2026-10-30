using Newtonsoft.Json.Linq;

namespace Fluxer.Net.Test;

public class ShardingTests
{
    private static JToken Identify(FluxerConfig? config = null)
    {
        FluxerClient client = new("Bot test", config);
        client.Gateway.Config = new FluxerGatewayConfig();
        return client.Gateway.CreateIdentifyPacket(new Dictionary<string, string> { { "os", "linux" } }).Data!;
    }

    [Test]
    public void IdentifyOmitsShardUnlessConfigured()
    {
        Assert.That(Identify()["shard"], Is.Null);
    }

    [Test]
    public void IdentifySendsTheConfiguredShardPair()
    {
        JToken data = Identify(new FluxerConfig { ShardId = 2, ShardCount = 8 });

        Assert.Multiple(() =>
        {
            Assert.That(data["shard"]!.ToString(Newtonsoft.Json.Formatting.None), Is.EqualTo("[2,8]"));
            Assert.That(data["token"]!.ToString(), Is.EqualTo("test"));
        });
    }

    [Test]
    public void InvalidShardConfigurationIsRejected()
    {
        Assert.Multiple(() =>
        {
            Assert.That(new FluxerConfig().GetShard(), Is.Null);
            Assert.Throws<InvalidOperationException>(() => new FluxerConfig { ShardId = 1 }.GetShard());
            Assert.Throws<InvalidOperationException>(() => new FluxerConfig { ShardCount = 2 }.GetShard());
            Assert.Throws<InvalidOperationException>(() => new FluxerConfig { ShardId = 2, ShardCount = 2 }.GetShard());
            Assert.Throws<InvalidOperationException>(() => new FluxerConfig { ShardId = -1, ShardCount = 2 }.GetShard());
            Assert.Throws<InvalidOperationException>(() => new FluxerConfig { ShardId = 0, ShardCount = 16385 }.GetShard());
            Assert.That(new FluxerConfig { ShardId = 0, ShardCount = 1 }.GetShard(), Is.EqualTo(new[] { 0, 1 }));
        });
    }

    [Test]
    public void ReadyReportsTheShardTheServerAssigned()
    {
        FluxerClient client = new("Bot test");
        string ready = "{\"op\":0,\"t\":\"READY\",\"s\":1,\"d\":{\"session_id\":\"abc\",\"shard\":[3,8],\"sessions\":[],\"rtc_regions\":[],\"guilds\":[],"
            + "\"user\":{\"id\":\"1\",\"username\":\"bot\",\"discriminator\":\"0\"}}}";

        client.Gateway.GatewayMessageHandler(ready);

        Assert.Multiple(() =>
        {
            Assert.That(client.Gateway.ShardId, Is.EqualTo(3));
            Assert.That(client.Gateway.ShardCount, Is.EqualTo(8));
        });
    }

    [Test]
    public async Task GatewayBotInfoIsRead()
    {
        RecordingHandler handler = new("{\"url\":\"wss://gw\",\"shards\":4,\"session_start_limit\":{\"total\":1000,\"remaining\":990,\"reset_after\":5,\"max_concurrency\":2}}");
        FluxerClient client = new("Bot test", RecordingHandler.Config(new HttpClient(handler)));

        GatewayBotJson? info = await client.Rest.GetGatewayBotAsync();

        Assert.Multiple(() =>
        {
            Assert.That(handler.Requests[0].Url, Does.EndWith("/gateway/bot"));
            Assert.That(info!.Shards, Is.EqualTo(4));
            Assert.That(info.SessionStartLimit!.MaxConcurrency, Is.EqualTo(2));
        });
    }
}
