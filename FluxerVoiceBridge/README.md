# Fluxer Voice Bridge

A lightweight Node.js service that bridges **Fluxer.Net** with **LiveKit** voice servers. This bridge uses the official `livekit-client` JavaScript SDK to handle all WebRTC complexity, providing a stable and reliable voice connection implementation.

## Why a Bridge?

LiveKit does not provide an official .NET client SDK. Instead of implementing the complex LiveKit WebRTC protocol from scratch in C#, this bridge:

- ✅ Uses the **official LiveKit JavaScript SDK** (same as the official Fluxer web app)
- ✅ Handles all WebRTC, protobuf, SDP, and ICE complexity automatically
- ✅ Provides a simple WebSocket API for .NET to communicate with
- ✅ Is **~300 lines of code** vs. thousands needed for a custom implementation
- ✅ Stays up-to-date with LiveKit protocol changes automatically

## Architecture

```
┌─────────────┐         WebSocket         ┌──────────────┐         WebRTC/LiveKit         ┌──────────────┐
│             │ ◄──────────────────────► │              │ ◄────────────────────────────► │              │
│ Fluxer.Net  │   JSON commands/events   │ Voice Bridge │   Official livekit-client SDK  │ LiveKit      │
│   (C#)      │                           │  (Node.js)   │                                │   Server     │
└─────────────┘                           └──────────────┘                                └──────────────┘
```

## Installation

1. **Install Node.js** (v18 or later):
   - Download from https://nodejs.org/
   - Or use a package manager (e.g., `winget install OpenJS.NodeJS`)

2. **Install dependencies**:
   ```bash
   cd FluxerVoiceBridge
   npm install
   ```

## Usage

### Starting the Bridge

```bash
# Default port (8765)
npm start

# Custom port
VOICE_BRIDGE_PORT=9000 npm start

# Debug mode (verbose logging)
DEBUG=true npm start

# Development mode with auto-reload
npm run dev
```

The bridge will listen on `ws://localhost:8765` by default.

### Running as a Background Service

**Windows (PowerShell):**
```powershell
Start-Process -NoNewWindow -FilePath "node" -ArgumentList "index.js"
```

**Linux/macOS:**
```bash
# Using nohup
nohup node index.js &

# Or using PM2 (recommended for production)
npm install -g pm2
pm2 start index.js --name fluxer-voice-bridge
pm2 save
pm2 startup  # Enable auto-start on boot
```

### Using from Fluxer.Net

```csharp
using Fluxer.Net.Voice;

// When you receive a VOICE_SERVER_UPDATE event from the gateway:
var bridgeClient = new VoiceBridgeClient(
    bridgeUrl: "ws://localhost:8765",
    guildId: voiceServerUpdate.GuildId,
    channelId: voiceServerUpdate.ChannelId,
    userId: currentUser.Id,
    sessionId: voiceServerUpdate.ConnectionId,
    logger: Log.Logger
);

// Set up event handlers
bridgeClient.OnReady += () => Console.WriteLine("Voice connected!");
bridgeClient.OnParticipantJoined += (p) => Console.WriteLine($"{p.Identity} joined");
bridgeClient.OnSpeakingChanged += (speakers) => Console.WriteLine($"Speaking: {string.Join(", ", speakers)}");

// Connect using endpoint and token from VOICE_SERVER_UPDATE
await bridgeClient.ConnectAsync(
    endpoint: voiceServerUpdate.Endpoint,
    token: voiceServerUpdate.Token
);

// Control mute/deaf
await bridgeClient.SetMuteAsync(true);
await bridgeClient.SetDeafAsync(false);

// Disconnect
await bridgeClient.DisconnectAsync();
```

## WebSocket Protocol

The bridge exposes a simple JSON-based WebSocket protocol.

### Messages from .NET → Bridge

**CONNECT** - Join a voice channel:
```json
{
  "type": "CONNECT",
  "connectionId": "unique-connection-id",
  "data": {
    "endpoint": "wss://voice.fluxer.example.com",
    "token": "eyJhbGci...",
    "guildId": "123456789",
    "channelId": "987654321",
    "userId": "555555555"
  }
}
```

**DISCONNECT** - Leave voice channel:
```json
{
  "type": "DISCONNECT",
  "connectionId": "unique-connection-id"
}
```

**SET_MUTE** - Mute/unmute microphone:
```json
{
  "type": "SET_MUTE",
  "connectionId": "unique-connection-id",
  "data": { "muted": true }
}
```

**SET_DEAF** - Deafen/undeafen (mutes mic + disables audio output):
```json
{
  "type": "SET_DEAF",
  "connectionId": "unique-connection-id",
  "data": { "deafened": true }
}
```

**PING** - Keep-alive:
```json
{
  "type": "PING",
  "connectionId": "unique-connection-id"
}
```

### Messages from Bridge → .NET

**READY** - Voice connection established and ready:
```json
{
  "type": "READY",
  "connectionId": "unique-connection-id",
  "data": {
    "roomName": "guild_123_channel_456",
    "participantCount": 5
  }
}
```

**PARTICIPANT_JOINED** - Someone joined the channel:
```json
{
  "type": "PARTICIPANT_JOINED",
  "connectionId": "unique-connection-id",
  "data": {
    "identity": "user_123456_abc",
    "sid": "PA_xyz",
    "isSpeaking": false,
    "isMicrophoneEnabled": true,
    "connectionQuality": "excellent"
  }
}
```

**SPEAKING_CHANGED** - Active speakers changed:
```json
{
  "type": "SPEAKING_CHANGED",
  "connectionId": "unique-connection-id",
  "data": {
    "speakers": ["user_123_abc", "user_456_def"]
  }
}
```

**DISCONNECTED** - Voice connection lost:
```json
{
  "type": "DISCONNECTED",
  "connectionId": "unique-connection-id",
  "data": {
    "reason": "Connection closed"
  }
}
```

**ERROR** - An error occurred:
```json
{
  "type": "ERROR",
  "connectionId": "unique-connection-id",
  "data": {
    "code": "CONNECTION_FAILED",
    "message": "Failed to connect to LiveKit server"
  }
}
```

## Environment Variables

| Variable | Default | Description |
|----------|---------|-------------|
| `VOICE_BRIDGE_PORT` | `8765` | WebSocket server port |
| `DEBUG` | `false` | Enable verbose logging |

## Troubleshooting

### Bridge won't start

- Make sure Node.js v18+ is installed: `node --version`
- Check if port 8765 is already in use
- Run `npm install` to ensure dependencies are installed

### Connection fails from .NET

- Verify the bridge is running: check for "Ready and listening" message
- Check firewall isn't blocking port 8765
- Ensure WebSocket URL is correct: `ws://localhost:8765` (not `wss://`)

### No audio

- Check browser console (if testing in web) for media device permissions
- Verify LiveKit server endpoint and token are valid
- Check bridge logs for "Media device error" messages

### High CPU usage

- LiveKit uses WebRTC which can be CPU-intensive
- Multiple connections will use more CPU
- Consider running on a dedicated machine for production

## Development

The bridge is intentionally kept simple and minimal. Key files:

- `index.js` - Main bridge server (~300 lines)
- `package.json` - Dependencies and scripts

To add new features:
1. Add handler in `handleMessage()` for new message types
2. Add LiveKit event listeners in `setupRoomEvents()`
3. Update the protocol documentation above

## License

Same as Fluxer.Net (check parent project for license details).

## Support

For issues related to:
- **The bridge itself**: File an issue in Fluxer.Net repository
- **LiveKit SDK**: See https://docs.livekit.io/
- **Fluxer API**: See official Fluxer documentation
