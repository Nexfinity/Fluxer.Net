---
uid: GetStarted.Events
title: Using Events
---

# Using Events
Using Fluxer.net you can listen for events such as message create, community member joins and member bans.

Here is an example of using a message create event, this will let you use `data` for the properties of the incoming message.

[!code-csharp[Program](../example/samples/events.cs)]