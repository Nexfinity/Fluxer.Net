# Self-hosted instances

Fluxer is open source, so a community can run its own instance. A bot built with Fluxer.Net can connect to such an instance by pointing the client at its API and gateway; everything else the library needs, such as the media host for avatars or the invite host for links, comes from the instance itself.

## Pointing the client at an instance

Set the API and gateway URLs before connecting:

```csharp
var config = new FluxerConfig().UseInstance("https://api.example.org", "wss://gateway.example.org");
var client = new FluxerClient(token, config);
```

`UseInstance` completes what is missing: the API URL gets the `/v{v}` version path and the gateway URL gets its `?v=1&encoding=json` query. You can also set `ApiBaseUrl` and `GatewayUrl` by hand.

## Discovered endpoints

Every instance publishes the URLs of its services: `api_public`, `gateway`, `media`, `static_cdn`, `invite`, `webapp`, `admin`, `gift` and a few more. Some send them in the READY event under `endpoints`, and every instance serves them at `/.well-known/fluxer`, which `LoginAsync` and `Rest.GetInstanceAsync` read without the gateway. With `FluxerConfig.DiscoverEndpoints` on, which is the default, the gateway applies READY's endpoints to `MediaUrl`, `StaticUrl`, `InviteUrl`, `WebUrl`, `AdminUrl` and `GiftUrl` before the Ready event, so avatar, icon, emoji, sticker, attachment and invite URLs produced by the entities point at the right hosts. The API and gateway URLs are never changed by discovery.

The raw list from READY is on the gateway as `client.Gateway.Endpoints`, keyed by name, for anything the library does not map itself (the upload relay, the marketing site).

Turn discovery off when you want the configured hosts to stay as they are:

```csharp
config.DiscoverEndpoints = false;
```

## Sign in with an instance

The ASP.NET Core OAuth handler defaults to the main instance. Point it at another one with:

```csharp
builder.Services.AddAuthentication()
    .AddFluxer(options =>
    {
        options.ClientId = "...";
        options.ClientSecret = "...";
        options.UseInstance("https://api.example.org");
    });
```

`UseInstance` sets the authorize, token and user information endpoints under that API.

## Things to know

- Entities build URLs from the client configuration they were created with, so a process that talks to several instances needs one `FluxerClient` per instance.
- `WebhookJson.GetAvatarUrl` has no client at hand and uses the main instance's media host; use `Webhook.GetAvatarUrl` from a client for instance-aware URLs.
- Instances may run different versions of Fluxer. Fluxer.Net targets the API version in `FluxerConfig.Version`; an instance that is behind or ahead may lack routes or fields the library expects, and the library makes no promise beyond the main instance's current version.
