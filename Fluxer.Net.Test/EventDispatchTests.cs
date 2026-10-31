using System.Collections.Concurrent;
using Fluxer.Net.Gateway;

namespace Fluxer.Net.Test;

public class EventDispatchTests
{
    private const string Author = "{\"id\":\"1\",\"username\":\"a\"}";

    private static FluxerClient Create(FluxerEventConfig? events = null, params ulong[] guildIds)
    {
        FluxerClient client = new("Bot test");
        foreach (ulong guildId in guildIds.Length == 0 ? new ulong[] { 10 } : guildIds)
        {
            SocketGuild guild = new(client);
            guild.Update(new GuildJson { Id = guildId, Name = "Test" });
            client.Gateway.Guilds[guildId] = guild;
            client.Gateway.Channels[guildId + 1] = Channel.Create(client, new ChannelJson { Id = guildId + 1, GuildId = guildId, Type = ChannelType.GuildText });
        }

        if (events != null)
            client.Gateway.StartEventDispatch(events);
        return client;
    }

    private static void Send(FluxerClient client, ulong id, ulong guildId = 10, string content = "m")
        => client.Gateway.GatewayMessageHandler($"{{\"op\":0,\"t\":\"MESSAGE_CREATE\",\"s\":1,\"d\":{{\"id\":\"{id}\",\"channel_id\":\"{guildId + 1}\",\"guild_id\":\"{guildId}\",\"author\":{Author},\"content\":\"{content}\",\"timestamp\":\"2025-01-01T00:00:00Z\"}}}}");

    private static async Task<bool> Within(Task task, int milliseconds = 5000)
        => await Task.WhenAny(task, Task.Delay(milliseconds)) == task;

    [Test]
    public void HandlersOfTheNamedDelegateTypesStillSubscribeAndUnsubscribe()
    {
        FluxerClient client = Create();
        List<string> seen = new();
        FluxerGatewayClient.MessageCreatedEvent first = m => seen.Add("first " + m.Content);
        FluxerGatewayClient.MessageCreatedEvent both = first;
        both += m => seen.Add("second " + m.Content);

        client.Gateway.MessageCreated += both;
        Send(client, 1, content: "a");
        client.Gateway.MessageCreated -= both;
        Send(client, 2, content: "b");

        Assert.That(seen, Is.EquivalentTo(new[] { "first a", "second a" }));
        Assert.That(client.Gateway.MessageCreated.HandlerCount, Is.Zero);
    }

    [Test]
    public void AThrowingHandlerDoesNotStopTheOthers()
    {
        FluxerClient client = Create();
        int reached = 0;
        client.Gateway.MessageCreated += _ => throw new InvalidOperationException("boom");
        client.Gateway.MessageCreated += async _ =>
        {
            await Task.Yield();
            throw new InvalidOperationException("async boom");
        };
        client.Gateway.MessageCreated += _ => Interlocked.Increment(ref reached);

        Assert.DoesNotThrow(() => Send(client, 1));
        Assert.That(reached, Is.EqualTo(1));
    }

    [Test]
    public void EventsCanOnlyBeChangedWithPlusAndMinus()
    {
        FluxerClient client = Create();
        FluxerClient other = Create();

        Assert.Throws<InvalidOperationException>(() => client.Gateway.MessageCreated = other.Gateway.MessageCreated);
    }

    [Test]
    public async Task AsyncHandlersOfOneGuildRunInOrder()
    {
        FluxerClient client = Create(new FluxerEventConfig());
        ConcurrentQueue<string> log = new();
        client.Gateway.MessageCreated += async m =>
        {
            log.Enqueue("start " + m.Content);
            await Task.Delay(m.Content == "a" ? 100 : 0);
            log.Enqueue("end " + m.Content);
        };

        Send(client, 1, content: "a");
        Send(client, 2, content: "b");
        await client.Gateway.WaitForPendingEventsAsync();

        Assert.That(log, Is.EqualTo(new[] { "start a", "end a", "start b", "end b" }));
    }

    [Test]
    public async Task ABusyGuildDoesNotHoldUpOtherGuilds()
    {
        FluxerClient client = Create(new FluxerEventConfig(), 10, 20);
        TaskCompletionSource<bool> gate = new(TaskCreationOptions.RunContinuationsAsynchronously);
        TaskCompletionSource<bool> otherGuild = new(TaskCreationOptions.RunContinuationsAsynchronously);
        client.Gateway.MessageCreated += async m =>
        {
            if (m.Guild!.Id == 10)
                await gate.Task;
            else
                otherGuild.TrySetResult(true);
        };

        Send(client, 1, guildId: 10);
        Send(client, 2, guildId: 20);

        Assert.That(await Within(otherGuild.Task), Is.True);
        gate.SetResult(true);
        await client.Gateway.WaitForPendingEventsAsync();
    }

    [Test]
    public async Task SequentialModeHandlesOneEventAtATime()
    {
        FluxerClient client = Create(new FluxerEventConfig { Mode = EventDispatchMode.Sequential }, 10, 20);
        ConcurrentQueue<ulong> log = new();
        client.Gateway.MessageCreated += async m =>
        {
            await Task.Delay(m.Guild!.Id == 10 ? 100 : 0);
            log.Enqueue(m.Guild.Id);
        };

        Send(client, 1, guildId: 10);
        Send(client, 2, guildId: 20);
        await client.Gateway.WaitForPendingEventsAsync();

        Assert.That(log, Is.EqualTo(new ulong[] { 10, 20 }));
    }

    [Test]
    public async Task ConcurrentModeDoesNotWaitForEarlierEvents()
    {
        FluxerClient client = Create(new FluxerEventConfig { Mode = EventDispatchMode.Concurrent });
        TaskCompletionSource<bool> gate = new(TaskCreationOptions.RunContinuationsAsynchronously);
        TaskCompletionSource<bool> second = new(TaskCreationOptions.RunContinuationsAsynchronously);
        client.Gateway.MessageCreated += async m =>
        {
            if (m.Content == "a")
                await gate.Task;
            else
                second.TrySetResult(true);
        };

        Send(client, 1, content: "a");
        Send(client, 2, content: "b");

        Assert.That(await Within(second.Task), Is.True);
        gate.SetResult(true);
        await client.Gateway.WaitForPendingEventsAsync();
    }

    [Test]
    public async Task ReleasingTheLaneLetsAHandlerWaitForALaterEvent()
    {
        FluxerClient client = Create(new FluxerEventConfig());
        TaskCompletionSource<string> reply = new(TaskCreationOptions.RunContinuationsAsynchronously);
        TaskCompletionSource<string> answered = new(TaskCreationOptions.RunContinuationsAsynchronously);
        client.Gateway.MessageCreated += async m =>
        {
            if (m.Content != "question")
            {
                reply.TrySetResult(m.Content);
                return;
            }

            client.Gateway.ReleaseEventLane();
            answered.TrySetResult(await reply.Task);
        };

        Send(client, 1, content: "question");
        Send(client, 2, content: "answer");

        Assert.That(await Within(answered.Task), Is.True);
        Assert.That(answered.Task.Result, Is.EqualTo("answer"));
    }

    [Test]
    public async Task WaitingForAReplyNeedsNoRelease()
    {
        FluxerClient client = Create(new FluxerEventConfig { LaneHoldLimit = null });
        TaskCompletionSource<string?> answered = new(TaskCreationOptions.RunContinuationsAsynchronously);
        client.Gateway.MessageCreated += async m =>
        {
            if (m.Content == "question")
                answered.TrySetResult((await client.Gateway.WaitForReplyAsync(m, TimeSpan.FromSeconds(5)))?.Content);
        };

        Send(client, 1, content: "question");
        Send(client, 2, content: "answer");

        Assert.That(await Within(answered.Task), Is.True);
        Assert.That(answered.Task.Result, Is.EqualTo("answer"));
    }

    [Test]
    public async Task LaterEventsGoAheadAfterTheHoldLimit()
    {
        FluxerClient client = Create(new FluxerEventConfig { LaneHoldLimit = TimeSpan.FromMilliseconds(100) });
        TaskCompletionSource<bool> signal = new(TaskCreationOptions.RunContinuationsAsynchronously);
        TaskCompletionSource<bool> finished = new(TaskCreationOptions.RunContinuationsAsynchronously);
        client.Gateway.MessageCreated += async m =>
        {
            if (m.Content != "wait")
            {
                signal.TrySetResult(true);
                return;
            }

            await signal.Task;
            finished.TrySetResult(true);
        };

        Send(client, 1, content: "wait");
        Send(client, 2, content: "go");

        Assert.That(await Within(finished.Task, 2000), Is.True);
    }

    [Test]
    public async Task WaitingReturnsNothingAfterTheTimeoutAndStopsListening()
    {
        FluxerClient client = Create(new FluxerEventConfig());

        SocketMessage? message = await client.Gateway.WaitForMessageAsync(_ => true, TimeSpan.FromMilliseconds(50));
        (Channel, Cacheable<Message>, SocketReaction)? reaction = await client.Gateway.MessageReactionAdded.WaitAsync(timeout: TimeSpan.FromMilliseconds(10));

        Assert.That(message, Is.Null);
        Assert.That(reaction, Is.Null);
        Assert.That(client.Gateway.MessageCreated.HandlerCount, Is.Zero);
        Assert.That(client.Gateway.MessageReactionAdded.HandlerCount, Is.Zero);
    }

    [Test]
    public void CancellingAWaitThrows()
    {
        FluxerClient client = Create(new FluxerEventConfig());
        using CancellationTokenSource cancel = new(50);

        Assert.ThrowsAsync<TaskCanceledException>(() => client.Gateway.WaitForMessageAsync(_ => true, cancellationToken: cancel.Token));
        Assert.That(client.Gateway.MessageCreated.HandlerCount, Is.Zero);
    }

    [Test]
    public async Task WaitingPicksTheMatchingEvent()
    {
        FluxerClient client = Create();
        Task<SocketMessage?> waiting = client.Gateway.WaitForMessageAsync(m => m.Content == "b", TimeSpan.FromSeconds(5));

        Send(client, 1, content: "a");
        Send(client, 2, content: "b");

        Assert.That(await Within(waiting), Is.True);
        Assert.That(waiting.Result!.Content, Is.EqualTo("b"));
    }

    [Test]
    public async Task WaitingForPendingEventsWaitsForHandlersReleasedByTheHoldLimit()
    {
        FluxerClient client = Create(new FluxerEventConfig { LaneHoldLimit = TimeSpan.FromMilliseconds(20) });
        bool finished = false;
        client.Gateway.MessageCreated += async _ =>
        {
            await Task.Delay(300);
            finished = true;
        };

        Send(client, 1);
        await Task.Delay(100);
        await client.Gateway.WaitForPendingEventsAsync();

        Assert.That(finished, Is.True);
        Assert.That(client.Gateway.QueuedEvents, Is.Zero);
    }

    [Test]
    public async Task DisposingWakesWaitersAndParkedDispatch()
    {
        FluxerClient client = Create(new FluxerEventConfig { MaxQueuedEvents = 1 });
        TaskCompletionSource<bool> gate = new(TaskCreationOptions.RunContinuationsAsynchronously);
        client.Gateway.MessageCreated += async _ => await gate.Task;

        for (ulong id = 1; id <= 4; id++)
            Send(client, id);
        await Task.Delay(50);
        Task waiting = client.Gateway.WaitForPendingEventsAsync();

        client.Gateway.Dispose();

        Assert.That(await Within(waiting, 2000), Is.True);
        gate.SetResult(true);
    }

    [Test]
    public async Task WaitingForPendingEventsWaitsForHandlers()
    {
        FluxerClient client = Create(new FluxerEventConfig());
        int finished = 0;
        client.Gateway.MessageCreated += async _ =>
        {
            await Task.Delay(50);
            Interlocked.Increment(ref finished);
        };

        for (ulong id = 1; id <= 3; id++)
            Send(client, id);
        await client.Gateway.WaitForPendingEventsAsync();

        Assert.That(finished, Is.EqualTo(3));
        Assert.That(client.Gateway.QueuedEvents, Is.Zero);
    }

    [Test]
    public async Task BusyHandlersPauseHandlingWithoutHoldingUpReceiving()
    {
        FluxerClient client = Create(new FluxerEventConfig { MaxQueuedEvents = 1 });
        TaskCompletionSource<bool> gate = new(TaskCreationOptions.RunContinuationsAsynchronously);
        int handled = 0;
        client.Gateway.MessageCreated += async _ =>
        {
            await gate.Task;
            Interlocked.Increment(ref handled);
        };

        for (ulong id = 1; id <= 5; id++)
            Send(client, id);

        await Task.Delay(100);
        Assert.That(client.Gateway.QueuedEvents, Is.LessThanOrEqualTo(2));
        Assert.That(client.Gateway.PendingEvents, Is.GreaterThan(0));

        gate.SetResult(true);
        Assert.That(await Within(client.Gateway.WaitForPendingEventsAsync()), Is.True);
        Assert.That(handled, Is.EqualTo(5));
    }

    [Test]
    public void InvalidEventSettingsAreRejected()
    {
        Assert.Multiple(() =>
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Create(new FluxerEventConfig { Lanes = 0 }));
            Assert.Throws<ArgumentOutOfRangeException>(() => Create(new FluxerEventConfig { MaxQueuedEvents = 0 }));
            Assert.Throws<ArgumentOutOfRangeException>(() => Create(new FluxerEventConfig { SlowHandlerWarning = TimeSpan.Zero }));
            Assert.Throws<ArgumentOutOfRangeException>(() => Create(new FluxerEventConfig { LaneHoldLimit = TimeSpan.Zero }));
        });
    }
}
