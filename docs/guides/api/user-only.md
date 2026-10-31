---
uid: Guides.API.UserOnly
title: User-only routes
---

# User-only routes

Fluxer lets user accounts run automated clients, and Fluxer.Net supports the whole API for them. Some routes, though,
are only served to a logged-in user: GIF search, relationships, notes, saved messages, account settings and the like. A
bot token calling one gets 403 `ACCESS_DENIED`, and no application setting changes that.

## In the library

- Methods on `FluxerApiClient` that need a user token carry `[UserTokenOnly]` and say so in their docs.
- A bot client calling one throws `UserTokenRequiredException` before anything is sent, with the method and route in
  the exception. The same happens for a user-only route you call yourself through `SendRequestAsync`.
- `client.Rest.IsBot` tells you which kind of token the client has.
- `UserOnlyRoutes.IsUserOnly(method, route)` checks a route, and `UserOnlyRoutes.All` lists every route as the server
  declares it.

```csharp
if (!client.Rest.IsBot)
    gifs = await client.Rest.SearchKlipyAsync("cat");
```

## Instances with other rules

The list mirrors the server's `DefaultUserOnly` guards at the time of the release. To send such calls anyway, for
example against an instance that allows them, turn the check off:

```csharp
FluxerClient client = new FluxerClient(token, new FluxerConfig { EnforceUserOnlyRoutes = false });
```

The server's answer then decides.
