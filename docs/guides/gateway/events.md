---
uid: Guides.Gateway.Events
title: Events
---

# Events

Subscribe to gateway events with `+=` and unsubscribe with `-=`. A handler can be a normal method or an `async` one:

```csharp
client.Gateway.MessageCreated += message => Console.WriteLine(message.Content);

client.Gateway.MemberJoined += async member =>
{
    await welcomer.GreetAsync(member);
};
```

Async handlers are awaited by the gateway. They never run as `async void`, there is no need for `Task.Run`, and an
exception thrown in a handler is caught and logged. A throwing handler doesn't stop the other handlers of the event.

## Order

By default events of one guild reach your handlers one at a time, in the order they arrived. While an `async` handler
for a guild is running, later events of that guild wait for it, up to `LaneHoldLimit` (2 seconds by default). Past
that they go ahead while the handler keeps running. Handlers that finish within the limit, which is nearly all of
them, are strictly ordered. Set `LaneHoldLimit` to null to always wait. Events of different guilds are handled at the
same time. DM and group channels are ordered per channel.

Set `Events.Mode` when connecting to choose otherwise:

```csharp
await client.Gateway.ConnectAsync(new FluxerGatewayConfig
{
    Events = new FluxerEventConfig { Mode = EventDispatchMode.OrderedPerGuild, Lanes = 32 }
});
```

| Mode | Behaviour |
| --- | --- |
| `OrderedPerGuild` | One event at a time per guild, guilds in parallel. The default. |
| `Sequential` | One event at a time overall. |
| `Concurrent` | Every event starts as soon as it arrives. No ordering. |

Guilds are spread over `Lanes` lanes, 32 by default. Guilds sharing a lane wait for each other. More lanes means
fewer guilds waiting behind a busy one.

## Waiting for a later event

Wait for replies, reactions and other events with the wait helpers. They let later events of the guild go ahead at
once and see every event that arrives after the one being handled:

```csharp
client.Gateway.MessageCreated += async message =>
{
    if (message.Content != "!quiz")
        return;

    SocketMessage? answer = await client.Gateway.WaitForReplyAsync(message, TimeSpan.FromSeconds(30));
    if (answer == null)
        return;
    // ...
};
```

| Helper | Waits for |
| --- | --- |
| `WaitForReplyAsync(message)` | The next message from the same author in the same channel |
| `WaitForMessageAsync(predicate)` | The next message matching the predicate |
| `WaitForReactionAsync(predicate)` | The next guild reaction matching the predicate |
| `AnyEvent.WaitAsync(predicate)` | The next time that event is raised with matching arguments |

Each returns null when the timeout passes first and throws when the cancellation token is cancelled.

A handler that waits some other way, for example on a signal of its own, holds up its guild for at most
`LaneHoldLimit`. Call `ReleaseEventLane` first to let later events go ahead at once. The same goes for long work, such
as a large download.

## Things to know

- **Threads.** Handlers for different guilds run at the same time on thread pool threads. State shared between guilds,
  such as a dictionary of settings, must be thread safe.
- **Live objects.** Guilds, channels, members and roles are updated as events arrive. A slow handler can see a newer
  state than the event it was given.
- **Slow handlers.** A handler running longer than `SlowHandlerWarning`, 3 seconds by default, is logged with its name.
  Handlers that waited with the helpers or `ReleaseEventLane` are left out.
- **Back-pressure.** When more than `MaxQueuedEvents` events (10,000 by default) are waiting for handlers, the gateway
  pauses applying new events until handlers catch up. Receiving, heartbeats and reconnects carry on meanwhile.
  `QueuedEvents` and `PendingEvents` show how many are waiting.
- **Shutdown.** `WaitForPendingEventsAsync` waits for received events and their handlers to finish.
- **Named delegates.** Handlers of the older named delegate types, such as `MessageCreatedEvent`, still subscribe.
  Events are properties of type `GatewayEvent<...>`. Code compiled against an earlier version must be rebuilt.
