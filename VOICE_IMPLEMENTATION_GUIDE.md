# LiveKit Voice Implementation Guide

## Current Status

The voice connection implementation successfully connects to the LiveKit WebSocket server but gets disconnected after ~10 seconds with `{"leave":{"reason":"CLIENT_INITIATED"}}`.

### What Works ✅
- Gateway OpCode 4 (VOICE_STATE_UPDATE) sending
- Receiving VOICE_SERVER_UPDATE event with endpoint, token, and connection_id
- WebSocket connection to LiveKit server
- Receiving binary protobuf messages from server

### What Doesn't Work ❌
- **Not sending Answer to server's Offer** - This is the critical issue
- Not handling ICE candidates properly
- Not parsing protobuf messages (only logging them)

## Root Cause Analysis

Looking at the logs from `BasicCommands.cs:125-240`:

```
[08:00:25] Server sends JoinResponse (983 bytes) - We receive but don't parse
[08:00:25] Server sends Offer (489 bytes) - We receive but DON'T SEND ANSWER
[08:00:25] Server sends ICE candidates (167, 186, 276, 279 bytes) - We receive but don't acknowledge
[08:00:35] Server kicks us out after 10 seconds with no response
```

### The Problem

LiveKit uses **Protocol Buffers** (binary format), not JSON. The current `VoiceClient.cs` implementation:

1. Receives binary messages (`VoiceClient.cs:147-186`)
2. Logs them but doesn't parse the protobuf
3. Marks connection as ready after first message (`VoiceClient.cs:169-184`)
4. **Never sends an Answer back to the server's Offer**

According to [LiveKit Client Protocol](https://docs.livekit.io/reference/internals/client-protocol/):

> "Server sends a JoinResponse, which includes room information... then initiates a subscriber PeerConnection and transmits an offer to the client."
>
> "The client must accept the subscriber connection and send back an answer."

## LiveKit Protocol Flow

### Expected Message Exchange

```
Client → Server: WebSocket connection with access_token
Server → Client: SignalResponse with JoinResponse (room info)
Server → Client: SignalResponse with Offer (SDP offer)
Client → Server: SignalRequest with Answer (SDP answer) ❌ WE DON'T DO THIS
Server → Client: SignalResponse with Trickle (ICE candidates)
Client → Server: SignalRequest with Trickle (our ICE candidates) ❌ WE DON'T DO THIS
... Connection established ...
```

### Protobuf Message Definitions

From [livekit/protocol](https://github.com/livekit/protocol/blob/main/protobufs/livekit_rtc.proto):

```protobuf
message SignalRequest {
  oneof message {
    SessionDescription offer = 1;
    SessionDescription answer = 2;  // ← We need to send this
    TrickleRequest trickle = 3;     // ← And this
    AddTrackRequest add_track = 4;
    MuteTrackRequest mute = 5;
    UpdateSubscription subscription = 6;
    // ... more fields
  }
}

message SignalResponse {
  oneof message {
    JoinResponse join = 1;
    SessionDescription answer = 2;
    SessionDescription offer = 3;   // ← Server sends this
    TrickleRequest trickle = 4;     // ← Server sends this
    // ... more fields
  }
}

message SessionDescription {
  string type = 1;  // "offer" | "answer" | "pranswer" | "rollback"
  string sdp = 2;   // SDP content
}
```

## Solution: Implement Protobuf Protocol

### Step 1: Add Google.Protobuf NuGet Package

```bash
cd Fluxer.Net
dotnet add package Google.Protobuf --version 3.28.3
```

### Step 2: Add LiveKit Protocol Definitions

Option A: **Use Livekit.Client NuGet package** (Recommended for simplicity)
```bash
dotnet add package Livekit.Client
```

Option B: **Generate from .proto files** (More control)
1. Download protobuf files from https://github.com/livekit/protocol/tree/main/protobufs
2. Install `protoc` compiler
3. Generate C# classes:
```bash
protoc --csharp_out=./Voice/Protocol livekit_rtc.proto livekit_models.proto
```

### Step 3: Update VoiceClient.cs Message Handling

Current code at `VoiceClient.cs:139-255` needs to be replaced with:

```csharp
private void HandleWebSocketMessage(ResponseMessage message)
{
    if (message.MessageType == WebSocketMessageType.Binary && message.Binary != null)
    {
        try
        {
            // Deserialize protobuf SignalResponse
            var signalResponse = SignalResponse.Parser.ParseFrom(message.Binary);

            if (signalResponse.MessageCase == SignalResponse.MessageOneofCase.Join)
            {
                HandleJoinResponse(signalResponse.Join);
            }
            else if (signalResponse.MessageCase == SignalResponse.MessageOneofCase.Offer)
            {
                // CRITICAL: Respond to offer with answer
                HandleOffer(signalResponse.Offer);
            }
            else if (signalResponse.MessageCase == SignalResponse.MessageOneofCase.Trickle)
            {
                HandleIceCandidate(signalResponse.Trickle);
            }
            // ... handle other message types
        }
        catch (Exception ex)
        {
            _logger?.Error(ex, "Failed to parse SignalResponse");
        }
    }
}
```

### Step 4: Implement Answer Generation

The critical missing piece - responding to the server's WebRTC offer:

```csharp
private async Task HandleOffer(SessionDescription offer)
{
    _logger?.Information("Received Offer from server");

    // This is where WebRTC peer connection comes in
    // You need to:
    // 1. Create RTCPeerConnection
    // 2. Set remote description (the offer)
    // 3. Create answer
    // 4. Set local description (the answer)
    // 5. Send answer back to server

    // Simplified example (requires WebRTC library like SIPSorcery):
    var peerConnection = new RTCPeerConnection();
    await peerConnection.setRemoteDescription(new RTCSessionDescriptionInit {
        type = RTCSdpType.offer,
        sdp = offer.Sdp
    });

    var answerInit = await peerConnection.createAnswer();
    await peerConnection.setLocalDescription(answerInit);

    // Send answer back to server via SignalRequest
    var signalRequest = new SignalRequest {
        Answer = new SessionDescription {
            Type = "answer",
            Sdp = answerInit.sdp
        }
    };

    var bytes = signalRequest.ToByteArray();
    _wsClient?.Send(bytes);

    _logger?.Information("Sent Answer to server");
}
```

### Step 5: Handle ICE Candidates

```csharp
private void HandleIceCandidate(TrickleRequest trickle)
{
    _logger?.Debug("Received ICE candidate from server");

    // Add to peer connection
    var candidate = new RTCIceCandidateInit {
        candidate = trickle.CandidateInit,
        sdpMid = trickle.Target.ToString(),
        sdpMLineIndex = (ushort)trickle.Target
    };

    peerConnection.addIceCandidate(candidate);
}
```

## WebRTC Library Requirement

**Critical**: The above solution requires a WebRTC library for .NET. Options:

### Option 1: SIPSorcery (Recommended)
```bash
dotnet add package SIPSorcery --version 6.1.0
```

Pros:
- Pure .NET implementation
- Well-maintained
- Good WebRTC support

Cons:
- Learning curve
- More code to write

### Option 2: Use Livekit.Client NuGet Package Directly

Instead of implementing VoiceClient from scratch, use the official client:

```csharp
using LiveKit;

var room = new Room();
await room.Connect(livekitUrl, token);

// Publish audio track
var audioTrack = await LocalAudioTrack.CreateAudioTrack();
await room.LocalParticipant.PublishTrack(audioTrack);
```

This would be the **simplest and most reliable approach**.

## Recommended Path Forward

### Immediate Solution (Use Official SDK)

Replace custom `VoiceClient.cs` with the official `Livekit.Client` package:

1. Install package: `dotnet add package Livekit.Client`
2. Replace VoiceClient implementation
3. Test connection

### Long-term Solution (Full Implementation)

If you want to keep custom implementation:

1. Add Google.Protobuf + SIPSorcery packages
2. Generate/copy LiveKit protocol definitions
3. Implement proper message handling with Answer/ICE exchange
4. Add WebRTC peer connection management
5. Implement audio streaming via RTP

## Files That Need Changes

1. **Fluxer.Net.csproj** - Add NuGet dependencies
2. **VoiceClient.cs** - Complete rewrite of message handling (lines 139-255)
3. **Voice/Protocol/** - Add protobuf generated classes (new directory)
4. **AudioPlayer.cs** - May need updates depending on WebRTC integration

## Testing Checklist

- [ ] Install required NuGet packages
- [ ] Generate/add protobuf definitions
- [ ] Implement Answer sending after Offer received
- [ ] Implement ICE candidate exchange
- [ ] Test connection stays alive > 10 seconds
- [ ] Verify OnReady fires after successful negotiation
- [ ] Test audio playback

## References

- [LiveKit Client Protocol](https://docs.livekit.io/reference/internals/client-protocol/)
- [LiveKit Protocol GitHub](https://github.com/livekit/protocol)
- [LiveKit JS SDK](https://github.com/livekit/client-sdk-js)
- [SIPSorcery Documentation](https://www.sipsorcery.com/)
- [Google.Protobuf NuGet](https://www.nuget.org/packages/Google.Protobuf)

## Summary

**The core issue**: We receive the server's WebRTC Offer but never send back an Answer, causing the server to disconnect us after 10 seconds.

**The solution**: Implement proper protobuf message parsing and WebRTC signaling, either by using the official Livekit.Client package or by implementing the protocol manually with Google.Protobuf + SIPSorcery.
