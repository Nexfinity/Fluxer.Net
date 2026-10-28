using Fluxer.Net.Voice;

namespace Fluxer.Net.Test;

public class VoiceTests
{
    [Test]
    public async Task PcmStreamSendsWholeFramesAndFlushesTheRest()
    {
        FakeAudioClient audio = new FakeAudioClient();
        const int frameBytes = 48000 / 50 * 2 * sizeof(short);

        await using (Stream stream = audio.CreatePcmStream())
        {
            await stream.WriteAsync(new byte[frameBytes + frameBytes / 2 + 2]);
            Assert.That(audio.Sent.Select(x => x.Length), Is.EqualTo(new[] { frameBytes / 2 }));
        }

        Assert.That(audio.Sent.Select(x => x.Length), Is.EqualTo(new[] { frameBytes / 2, frameBytes / 4 }));
    }

    [Test]
    public async Task VoiceChannelsConnectThroughTheClientsProvider()
    {
        TestClient client = new TestClient();
        IVoiceChannel channel = (IVoiceChannel)Channel.Create(client,
            new ChannelJson { Id = 2, GuildId = 1, Type = ChannelType.GuildVoice });

        Assert.ThrowsAsync<InvalidOperationException>(() => channel.ConnectAsync());

        FakeProvider provider = new FakeProvider();
        client.Voice = provider;
        IAudioClient audio = await channel.ConnectAsync();

        Assert.Multiple(() =>
        {
            Assert.That(audio.GuildId, Is.EqualTo(1));
            Assert.That(audio.ChannelId, Is.EqualTo(2));
            Assert.That(provider.Joined, Is.EqualTo((1UL, 2UL, true, false)));
        });
    }

    private sealed class TestClient : FluxerBaseClient
    {
    }

    private sealed class FakeProvider : IVoiceProvider
    {
        public (ulong, ulong, bool, bool)? Joined { get; private set; }

        public Task<IAudioClient> ConnectAsync(ulong guildId, ulong channelId, bool selfDeaf, bool selfMute,
            CancellationToken cancellationToken = default)
        {
            Joined = (guildId, channelId, selfDeaf, selfMute);
            return Task.FromResult<IAudioClient>(new FakeAudioClient { GuildId = guildId, ChannelId = channelId });
        }

        public Task DisconnectAsync(ulong guildId) => Task.CompletedTask;

        public IAudioClient? GetAudioClient(ulong guildId) => null;
    }

    private sealed class FakeAudioClient : IAudioClient
    {
        public List<short[]> Sent { get; } = [];

        public ulong GuildId { get; init; }

        public ulong ChannelId { get; init; }

        public bool IsConnected => true;

        public TimeSpan QueuedDuration => TimeSpan.Zero;

        public event Action<IAudioClient, Exception?>? Disconnected
        {
            add { }
            remove { }
        }

        public Stream CreatePcmStream() => new PcmStream(this);

        public Task SendAsync(short[] samples, CancellationToken cancellationToken = default)
        {
            Sent.Add(samples);
            return Task.CompletedTask;
        }

        public void ClearQueue()
        {
        }

        public Task DisconnectAsync() => Task.CompletedTask;

        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }
}
