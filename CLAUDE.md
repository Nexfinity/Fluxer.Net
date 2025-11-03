# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Fluxer.Net is a .NET 7.0 class library for interacting with the Fluxer API and real-time Gateway. It implements a dual-channel architecture with separate REST API and WebSocket Gateway clients.

**Current Status**: Alpha (v0.4.0) - Built for early Fluxer ALPHA, currently unsupported with the public BETA API. Breaking changes are permitted on minor version bumps (ABI breaks allowed, API breaks are not).

## Building and Testing

### Build the library
```sh
dotnet build
```

### Build NuGet package with symbols
```sh
dotnet pack --include-symbols --include-source
```

### Run the example application
```sh
dotnet run --project Fluxer.Net.Example
```

### Restore dependencies
```sh
dotnet restore
```

## Architecture

### Dual-Channel Communication Pattern

The library separates synchronous and asynchronous communication:

1. **ApiClient** - REST HTTP client for synchronous operations
   - User management, authentication, communities, channels, messages
   - Token management
   - All CRUD operations

2. **GatewayClient** - WebSocket client for real-time events
   - Event-driven architecture using delegates
   - Stateful connection with sequence tracking for ordered event processing
   - Automatic heartbeat mechanism
   - Session-based reconnection with resume capability

### Key Abstractions

**IGatewayData** (Marker Interface)
- Marks classes that can be deserialized from gateway event payloads
- All types in `Gateway/Data/` implement this interface
- Enables polymorphic deserialization via `JsonDerivedTypeConverter<IGatewayData>`

**GatewayPacket** (Message Envelope)
- `OpCode: FluxerOpCode` - Operation type (Dispatch=0, Heartbeat=1, Identify=2, PresenceUpdate=3, VoiceStateUpdate=4, VoiceServerPing=5, Resume=6, Reconnect=7, RequestGuildMembers=8, InvalidSession=9, Hello=10, HeartbeatAck=11, CallConnect=13, GuildSubscriptions=14)
- `Data: IGatewayData` - Polymorphic payload automatically deserialized to concrete type
- `Sequence: int?` - Event ordering for resumption after disconnection
- `Dispatch: string?` - Event name (e.g., "MESSAGE_CREATE", "CHANNEL_UPDATE", "MESSAGE_REACTION_ADD")

**FluxerConfig** (Shared Configuration)
- Single configuration object passed to both ApiClient and GatewayClient
- Controls reconnect behavior, API endpoints, logging, event filtering, and presence

### Gateway Event Flow

```
WebSocket message received
  ↓
GatewayMessageHandler deserializes to GatewayPacket
  ↓
JsonDerivedTypeConverter identifies concrete IGatewayData type
  ↓
OpCode dispatch:
  - Dispatch (0) → HandleDispatch() → Parse event type → Invoke event delegates
  - Hello (7) → HandleHello() → Start background heartbeat task
  - HeartbeatAck (8) → Log acknowledgment
  - InvalidSession (6) / Reconnect (5) → ReEstablishGatewayConnection()
  ↓
Application event handlers execute
```

### Reconnection Strategy

Connection loss or reconnect opcode triggers `ReEstablishGatewayConnection()`:
1. Rate limiting via `ReconnectAttemptDelay` config
2. Resume packet sent with sequence number and session ID
3. Server resumes from last acknowledged sequence
4. No event loss during brief disconnections

## Project Structure

```
Fluxer.Net/
├── ApiClient.cs                    # REST API client
├── GatewayClient.cs                # WebSocket gateway client
├── FluxerConfig.cs                 # Shared configuration
├── Gateway/
│   ├── FluxerOpCode.cs            # Protocol operation codes
│   ├── GatewayPacket.cs           # Message envelope
│   ├── IGatewayData.cs            # Marker interface for polymorphic payloads
│   ├── HeartbeatPacketOfDoom.cs   # Special heartbeat container
│   └── Data/                       # Gateway event payload types (21 types)
├── Objects/
│   ├── User.cs, Community.cs, Channel.cs, Message.cs, Role.cs, etc.
│   └── Data/                       # Enums and flags
│       ├── Permissions.cs          # [Flags] enum with 39 permission bits
│       ├── Status.cs, ChannelType.cs, MessageType.cs, etc.
│       └── *Flags.cs               # Feature flag enumerations
└── Extensions/
    ├── JsonDerivedTypeConverter.cs # Polymorphic JSON deserialization
    └── FluxerApiException.cs       # Custom exception type
```

## Development Patterns

### Adding a New REST Endpoint

1. Create or update domain object in `Objects/` if needed
2. Add public async method to `ApiClient`
3. Use appropriate `MakeFluxerApiRequest*` overload based on request/response types:
   - `MakeFluxerApiRequestRS<TRequest, TResponse>()` - Request and response bodies
   - `MakeFluxerApiRequestR<TRequest>()` - Request body only, no response
   - `MakeFluxerApiRequestS<TResponse>()` - No request body, response only
   - `MakeFluxerApiRequest()` - No request or response body

Example:
```csharp
public async Task<Channel> PatchChannel(ulong channelId, Channel channel)
{
    return await MakeFluxerApiRequestRS<Channel, Channel>(
        HttpMethod.Patch,
        $"/channels/{channelId}",
        channel
    );
}
```

### Adding a New Gateway Event Handler

1. Create `XyzGatewayData : IGatewayData` class in `Gateway/Data/`
2. Add case in `GatewayClient.HandleDispatch()` switch statement matching the event name (from Constants.ts)
3. Add event delegate type (e.g., `public delegate void XyzEvent(XyzGatewayData data);`)
4. Add event field (e.g., `public event XyzEvent? Xyz;`)
5. Use pattern matching to validate cast before invoking:
   ```csharp
   if (p.Data is XyzGatewayData xyzData)
       Xyz?.Invoke(xyzData);
   else
       _logger.Warning("XYZ_EVENT received but data could not be cast to XyzGatewayData");
   ```

**Reference**: See `ExternalApiReferences/Constants.ts` for the complete list of `GatewayDispatchEvent` types supported by the Fluxer API.

**Event Naming**: All gateway events follow the official Fluxer API naming:
- Guild events: `GUILD_CREATE`, `GUILD_UPDATE`, `GUILD_DELETE`
- Guild members: `GUILD_MEMBER_ADD`, `GUILD_MEMBER_UPDATE`, `GUILD_MEMBER_REMOVE`
- Guild roles: `GUILD_ROLE_CREATE`, `GUILD_ROLE_UPDATE`, `GUILD_ROLE_DELETE`

### Configuration Extension

Add properties to `FluxerConfig` and reference as `_config.PropertyName` in clients. Common patterns:
- Endpoint URLs (use string templates with placeholders)
- Timing values (delays, timeouts)
- Feature flags (bool properties)
- Collection filters (lists of ignored events)

## Naming Conventions

**Important Terminology**:
- The official Fluxer API uses **"Guild"** terminology (matching Discord-like platforms)
- "Community" and "Squad" were legacy/internal names from early development
- All gateway events use `GUILD_*` naming from the official specification
- Domain objects may still reference `Community` in some legacy code paths

**File Naming**:
- `GuildGatewayData` - Guild information events
- `GuildMemberGatewayData` - Guild member events
- File `FluxerOpCode.cs` was renamed from `SqullOpCode.cs`

**Important**: The OpCode values were corrected in a recent update. The previous implementation had incorrect enum values that didn't match the official Fluxer API specification (e.g., Resume was 4 instead of 6, Hello was 7 instead of 10). See `GATEWAY_UPDATES.md` for details.

## Dependencies

- **Newtonsoft.Json** (13.0.3) - JSON serialization with custom converters
- **Serilog** (3.0.1) - Structured logging throughout clients
- **Serilog.Sinks.Console** (4.1.0) - Console logging output
- **Websocket.Client** (5.0) - WebSocket connectivity for gateway

## Git Workflow

- **main** - Release branch, tagged for NuGet releases
- **dev** - Development branch, target for pull requests
- **feature/X** - Feature branches that merge to dev

When creating PRs, target the `dev` branch.
