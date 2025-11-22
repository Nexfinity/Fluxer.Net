# Fluxer.Net Voice Setup Guide

This guide will help you set up voice support for Fluxer.Net using the **Fluxer Voice Bridge**.

## Architecture Overview

```
┌─────────────────┐         WebSocket         ┌──────────────────┐         WebRTC         ┌──────────────┐
│                 │ ◄──────────────────────► │                  │ ◄────────────────────► │              │
│  Fluxer.Net     │   JSON commands/events   │  Voice Bridge    │   LiveKit Protocol     │   Fluxer     │
│  (C# Bot)       │                           │  (Node.js)       │                        │   Server     │
└─────────────────┘                           └──────────────────┘                        └──────────────┘
                                                      │
                                                      └──► Uses official livekit-client SDK
```

The Voice Bridge uses the **same LiveKit SDK** as the official Fluxer web/mobile apps, ensuring maximum compatibility and stability.

## Prerequisites

1. **.NET 7.0+** - Already installed if you're using Fluxer.Net
2. **Node.js v18+** - Required for the voice bridge
   - Download: https://nodejs.org/
   - Or install via package manager:
     - Windows: `winget install OpenJS.NodeJS`
     - macOS: `brew install node`
     - Linux: `sudo apt install nodejs npm`

## Installation Steps

### Step 1: Install Node.js Dependencies

```bash
cd FluxerVoiceBridge
npm install
```

This installs:
- `livekit-client` (v2.15.14) - Official LiveKit WebRTC SDK
- `ws` (v8.18.0) - WebSocket server for communication with .NET

### Step 2: Start the Voice Bridge

```bash
# Default configuration (port 8765)
npm start

# Or in development mode with auto-reload
npm run dev
```

You should see:
```
[Fluxer Voice Bridge] Ready and listening on port 8765
[Bridge] Waiting for connections from Fluxer.Net...
```

**Keep this terminal window open** - the bridge needs to stay running while your bot is connected to voice.

### Step 3: Test Voice Commands

In your Fluxer.Net bot, you can now use the voice commands:

```
/join  - Join your current voice channel
/leave - Leave voice channel
/mute  - Toggle microphone mute
/deaf  - Toggle deafen (mutes mic + disables audio)
```

## Usage Example

Here's a complete example of using the voice bridge in your bot:

```csharp
using Fluxer.Net.Voice;
using Serilog;

// When you receive VOICE_SERVER_UPDATE from the gateway
gateway.VoiceServerUpdate += async voiceData =>
{
    // Create voice bridge client
    var voiceClient = new VoiceBridgeClient(
        bridgeUrl: "ws://localhost:8765",
        guildId: voiceData.GuildId ?? 0,
        channelId: yourChannelId,
        userId: currentUserId,
        sessionId: voiceData.ConnectionId ?? "",
        logger: Log.Logger
    );

    // Set up events
    voiceClient.OnReady += () => Console.WriteLine("Voice ready!");
    voiceClient.OnParticipantJoined += (p) => Console.WriteLine($"{p.Identity} joined");
    voiceClient.OnSpeakingChanged += (speakers) => Console.WriteLine($"Speaking: {string.Join(", ", speakers)}");
    voiceClient.OnDisconnected += (reason) => Console.WriteLine($"Disconnected: {reason}");

    // Connect to voice
    await voiceClient.ConnectAsync(
        endpoint: voiceData.Endpoint!,
        token: voiceData.Token!
    );

    // Control audio
    await voiceClient.SetMuteAsync(true);   // Mute mic
    await voiceClient.SetDeafAsync(false);  // Enable audio

    // Later, disconnect
    await voiceClient.DisconnectAsync();
};
```

## Configuration

### Bridge Configuration

Environment variables for the voice bridge:

| Variable | Default | Description |
|----------|---------|-------------|
| `VOICE_BRIDGE_PORT` | `8765` | WebSocket server port |
| `DEBUG` | `false` | Enable verbose logging |

Example with custom port:
```bash
VOICE_BRIDGE_PORT=9000 npm start
```

Then update your .NET code:
```csharp
var voiceClient = new VoiceBridgeClient(
    bridgeUrl: "ws://localhost:9000",  // Updated port
    // ... other parameters
);
```

### Running Bridge as Background Service

**Windows (PowerShell):**
```powershell
# Start in background
Start-Process -NoNewWindow -FilePath "node" -ArgumentList "index.js"

# Stop (find PID first)
Get-Process -Name node | Stop-Process
```

**Linux/macOS:**
```bash
# Using nohup
nohup node index.js > bridge.log 2>&1 &

# Using PM2 (recommended for production)
npm install -g pm2
pm2 start index.js --name fluxer-voice-bridge
pm2 save
pm2 startup  # Auto-start on boot
```

**Docker:**
```dockerfile
FROM node:18-alpine
WORKDIR /app
COPY package*.json ./
RUN npm install
COPY index.js ./
EXPOSE 8765
CMD ["node", "index.js"]
```

Build and run:
```bash
docker build -t fluxer-voice-bridge .
docker run -d -p 8765:8765 --name voice-bridge fluxer-voice-bridge
```

## Troubleshooting

### Bridge Won't Start

**Problem:** `Error: Cannot find module 'livekit-client'`

**Solution:**
```bash
cd FluxerVoiceBridge
rm -rf node_modules package-lock.json
npm install
```

---

**Problem:** `Error: listen EADDRINUSE: address already in use :::8765`

**Solution:** Port 8765 is already in use. Either:
1. Stop the process using port 8765
2. Use a different port: `VOICE_BRIDGE_PORT=9000 npm start`

---

### Connection Issues from .NET

**Problem:** `Failed to connect to voice bridge`

**Checklist:**
1. ✅ Is the bridge running? Check for "Ready and listening on port 8765"
2. ✅ Is the URL correct? Should be `ws://localhost:8765` (not `wss://`)
3. ✅ Is your firewall blocking port 8765?
4. ✅ Are you using the correct port in VoiceBridgeClient constructor?

---

**Problem:** `WebSocket connection failed`

**Solution:** Test if the bridge is accessible:
```powershell
# Windows PowerShell
Test-NetConnection -ComputerName localhost -Port 8765

# Or use a WebSocket test tool
wscat -c ws://localhost:8765
```

---

### No Audio

**Problem:** Connected but can't hear anyone

**Checklist:**
1. ✅ Is deafen disabled? Check with `/deaf` command
2. ✅ Are other participants muted? Check LiveKit dashboard
3. ✅ Is your volume up? Check system audio settings
4. ✅ Check bridge logs for "Media device error" messages

---

**Problem:** Can't speak (no one hears you)

**Checklist:**
1. ✅ Is your microphone muted? Check with `/mute` command
2. ✅ Does the bridge have microphone permission?
3. ✅ Check browser console (if testing in web) for permission errors
4. ✅ Verify correct audio input device is selected

---

### High CPU/Memory Usage

**Issue:** Bridge using too much CPU or memory

**Solutions:**
1. **Limit connections** - Each voice connection uses resources
2. **Run on dedicated server** - Don't run bridge on resource-constrained machines
3. **Monitor logs** - Enable DEBUG mode and check for excessive reconnections
4. **Update Node.js** - Ensure you're using Node.js v18+ (better WebRTC performance)

---

## Advanced Topics

### Multiple Bots

You can run multiple Fluxer.Net bots with a single voice bridge. Each bot connects with a unique `sessionId`:

```csharp
// Bot 1
var bot1Voice = new VoiceBridgeClient(
    bridgeUrl: "ws://localhost:8765",
    sessionId: "bot1-connection-123",
    // ...
);

// Bot 2
var bot2Voice = new VoiceBridgeClient(
    bridgeUrl: "ws://localhost:8765",
    sessionId: "bot2-connection-456",
    // ...
);
```

### Distributed Setup

Run the bridge on a separate server:

```csharp
// On your .NET bot server, connect to remote bridge
var voiceClient = new VoiceBridgeClient(
    bridgeUrl: "ws://192.168.1.100:8765",  // Bridge server IP
    // ...
);
```

**Security Note:** If exposing the bridge over a network:
1. Use a reverse proxy (nginx, Caddy) with TLS (`wss://` instead of `ws://`)
2. Implement authentication (add API key to bridge)
3. Use firewall rules to restrict access

### Audio Processing

The bridge uses LiveKit's built-in audio processing. To customize:

```javascript
// In index.js, modify room creation:
const room = new Room({
    adaptiveStream: true,
    dynacast: true,

    // Custom audio settings
    audioCaptureDefaults: {
        echoCancellation: true,
        noiseSuppression: true,
        autoGainControl: true,
    }
});
```

### Monitoring

Add logging to track voice connection health:

```javascript
// In index.js, add after room creation:
room.on(RoomEvent.ConnectionQualityChanged, (quality, participant) => {
    console.log(`[Monitor] ${participant.identity}: ${quality}`);
    // Send to monitoring service (Datadog, Prometheus, etc.)
});
```

## FAQ

**Q: Do I need to keep the bridge running 24/7?**
A: Only when you want voice functionality. You can start/stop it as needed.

**Q: Can I run multiple bridges for redundancy?**
A: Yes, but you'll need different ports. Load balance on the .NET side.

**Q: Does this support video or screen share?**
A: Currently audio-only. Video support could be added to the bridge.

**Q: How many bots can connect to one bridge?**
A: Dozens easily. Limit depends on your server resources.

**Q: Is the bridge secure?**
A: For local use, yes. For network use, add TLS and authentication.

**Q: Why not implement LiveKit directly in C#?**
A: LiveKit has no official .NET client SDK. This bridge is the most reliable approach.

## Next Steps

- **Implement auto-join** - Make your bot automatically join voice when users join
- **Add music playback** - Stream audio files through the bridge
- **Voice activity detection** - React to who's speaking
- **Voice state management** - Track who's in which channel
- **Metrics dashboard** - Monitor connection quality and usage

## Getting Help

- **Bridge issues**: Check `FluxerVoiceBridge/README.md`
- **LiveKit docs**: https://docs.livekit.io/
- **Fluxer.Net issues**: GitHub issues page
- **WebRTC debugging**: Use `DEBUG=true npm start` for verbose logs

## Appendix: Technical Details

### WebSocket Protocol

The bridge uses a simple JSON protocol. See `FluxerVoiceBridge/README.md` for the complete message specification.

### Why This Approach Works

1. **Official SDK** - Uses the same `livekit-client` as Fluxer's web app
2. **Proven** - The SDK handles billions of WebRTC connections globally
3. **Maintained** - LiveKit team keeps the SDK up-to-date with protocol changes
4. **Simple** - The bridge is just ~300 lines of straightforward code
5. **Flexible** - Easy to extend with new features (video, screen share, etc.)

### Performance Characteristics

- **Latency**: ~50-150ms typical (depends on network and LiveKit server location)
- **CPU**: ~5-10% per active voice connection on modern hardware
- **Memory**: ~50-100MB for the bridge + ~20-50MB per connection
- **Bandwidth**: ~20-60 kbps per participant (audio only)

### Alternatives Considered

1. **Custom C# Implementation** ❌
   - Would require 100+ hours of development
   - Difficult to maintain as LiveKit evolves
   - No guarantee of compatibility

2. **Unity SDK Port** ❌
   - Heavily depends on Unity's audio/rendering systems
   - Not designed for standalone .NET

3. **Embedded JavaScript Engine** ❌
   - Complex setup with V8/Jint
   - Large dependency footprint
   - Audio capture still challenging

4. **Node.js Bridge** ✅ **CHOSEN**
   - Uses official SDK
   - Simple and maintainable
   - Proven reliable

---

*Happy voice chatting! 🎤*
