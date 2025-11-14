# Quick Start - Fluxer Voice Bridge

## 1. Install Node.js

Download and install from: https://nodejs.org/ (v18 or later)

Verify installation:
```bash
node --version  # Should show v18.x.x or higher
npm --version   # Should show 9.x.x or higher
```

## 2. Install Dependencies

```bash
cd FluxerVoiceBridge
npm install
```

You should see:
```
added 2 packages
```

## 3. Start the Bridge

```bash
npm start
```

You should see:
```
[Fluxer Voice Bridge] Starting on port 8765...
[Fluxer Voice Bridge] Ready and listening on port 8765
[Bridge] Waiting for connections from Fluxer.Net...
```

✅ **Bridge is ready!** Keep this terminal window open.

## 4. Test Voice in Your Bot

In your Fluxer.Net bot:

1. Join a voice channel in Fluxer
2. Send the command: `/join`
3. Your bot should connect to voice!

## Commands

- `/join` - Join your current voice channel
- `/leave` - Leave voice
- `/mute` - Toggle microphone
- `/deaf` - Toggle deafen (mic + audio)

## Troubleshooting

### "npm: command not found"
→ Node.js is not installed. Download from https://nodejs.org/

### "Error: Cannot find module 'livekit-client'"
→ Run `npm install` in the FluxerVoiceBridge directory

### "Error: listen EADDRINUSE"
→ Port 8765 is already in use. Stop the other process or use:
```bash
VOICE_BRIDGE_PORT=9000 npm start
```

### Bot says "Voice bridge is not running"
→ Make sure you ran `npm start` and see the "Ready and listening" message

## Next Steps

- See `README.md` for detailed documentation
- See `../VOICE_SETUP.md` for production deployment
- Check logs in the terminal for connection status

That's it! 🎉
