# LiveKit Voice Implementation - Current Status

## Summary

Attempted to implement Option 1 (use official LiveKit .NET client SDK), but discovered that **no official client SDK exists** for .NET. The `Livekit.Client` NuGet package is actually a **server SDK** for managing rooms, not for joining them.

## What We've Accomplished

### ✅ Installed Dependencies
- **Google.Protobuf 3.32.1** - For parsing LiveKit protocol messages
- **SIPSorcery 6.2.0** - For WebRTC peer connection handling
- Removed unnecessary `Livekit.Client` and `Livekit.Server.Sdk.Dotnet` packages
- Project builds successfully with 0 errors

### ✅ Root Cause Identified
From logs in `BasicCommands.cs:125-240`:
- LiveKit server sends JoinResponse, Offer, and ICE candidates via Protocol Buffers
- Current VoiceClient receives but doesn't parse protobuf messages
- **Critical Issue**: We never send an Answer back to the server's Offer
- Server disconnects after 10 seconds due to no response

### ✅ Documentation Created
- `VOICE_IMPLEMENTATION_GUIDE.md` - Complete guide with protocol flow and examples
- `IMPLEMENTATION_STATUS.md` (this file) - Current status and next steps

## What Still Needs to Be Done

This is a **complex, multi-step implementation** that requires significant WebRTC expertise:

### Step 1: Add LiveKit Protocol Definitions

You need to either:

**Option A: Manual generation** (recommended for learning)
1. Download .proto files from https://github.com/livekit/protocol/tree/main/protobufs:
   - `livekit_rtc.proto` (main SignalRequest/SignalResponse messages)
   - `livekit_models.proto` (supporting types)
2. Install `protoc` compiler: https://grpc.io/docs/protoc-installation/
3. Generate C# classes:
   ```bash
   cd Fluxer.Net/Voice/Protocol
   protoc --csharp_out=. path/to/livekit_rtc.proto path/to/livekit_models.proto
   ```

**Option B: Copy from LiveKit Go SDK**
- The protobufs are already compiled in other LiveKit SDKs
- Could extract and adapt them

### Step 2: Rewrite VoiceClient Message Handling

Replace `VoiceClient.cs:139-255` with proper protobuf parsing:

```csharp
using Livekit;  // Generated from .proto files
using Google.Protobuf;

private void HandleWebSocketMessage(ResponseMessage message)
{
    if (message.MessageType == WebSocketMessageType.Binary && message.Binary != null)
    {
        // Deserialize SignalResponse
        var signalResponse = SignalResponse.Parser.ParseFrom(message.Binary);

        switch (signalResponse.MessageCase)
        {
            case SignalResponse.MessageOneofCase.Join:
                HandleJoinResponse(signalResponse.Join);
                break;
            case SignalResponse.MessageOneofCase.Offer:
                await HandleOffer(signalResponse.Offer);  // CRITICAL!
                break;
            case SignalResponse.MessageOneofCase.Trickle:
                HandleIceCandidate(signalResponse.Trickle);
                break;
        }
    }
}
```

### Step 3: Implement WebRTC Peer Connection

This is the **most complex part**. Using SIPSorcery:

```csharp
using SIPSorcery.Net;

private RTCPeerConnection _peerConnection;

private async Task HandleOffer(SessionDescription offer)
{
    // Create peer connection
    _peerConnection = new RTCPeerConnection();

    // Set up ICE candidate handler
    _peerConnection.onicecandidate += (candidate) =>
    {
        if (candidate != null)
        {
            SendIceCandidate(candidate);
        }
    };

    // Set remote description (the offer)
    var rtcOffer = new RTCSessionDescriptionInit
    {
        type = RTCSdpType.offer,
        sdp = offer.Sdp
    };
    await _peerConnection.setRemoteDescription(rtcOffer);

    // Create answer
    var answerInit = await _peerConnection.createAnswer(null);
    await _peerConnection.setLocalDescription(answerInit);

    // Send answer back to server
    var signalRequest = new SignalRequest
    {
        Answer = new SessionDescription
        {
            Type = "answer",
            Sdp = answerInit.sdp
        }
    };

    var bytes = signalRequest.ToByteArray();
    _wsClient?.Send(bytes);

    _logger?.Information("Sent Answer to LiveKit server");
}
```

### Step 4: Handle ICE Candidates

```csharp
private void HandleIceCandidate(TrickleRequest trickle)
{
    var candidate = new RTCIceCandidateInit
    {
        candidate = trickle.CandidateInit,
        sdpMid = trickle.Target.ToString(),
        sdpMLineIndex = (ushort)trickle.Target
    };

    _peerConnection?.addIceCandidate(candidate);
}

private void SendIceCandidate(RTCIceCandidate candidate)
{
    var signalRequest = new SignalRequest
    {
        Trickle = new TrickleRequest
        {
            CandidateInit = candidate.candidate,
            Target = SignalTarget.Publisher  // or Subscriber
        }
    };

    var bytes = signalRequest.ToByteArray();
    _wsClient?.Send(bytes);
}
```

### Step 5: Implement Audio Streaming

Once WebRTC connection is established, audio needs to be sent via RTP:

```csharp
// Add audio track to peer connection
var audioTrack = new MediaStreamTrack(
    SDPMediaTypesEnum.audio,
    false,
    new List<SDPAudioVideoMediaFormat> { new SDPAudioVideoMediaFormat(SDPWellKnownMediaFormatsEnum.PCMU) },
    MediaStreamStatusEnum.SendOnly
);

_peerConnection.addTrack(audioTrack);

// Send audio data
public async Task SendOpusFrame(byte[] opusData)
{
    // Convert Opus to RTP packet
    // Send via peer connection
}
```

## Estimated Effort

- **Step 1** (Proto generation): 1-2 hours
- **Step 2** (Message handling): 2-3 hours
- **Step 3** (WebRTC peer connection): 4-8 hours (complex!)
- **Step 4** (ICE handling): 1-2 hours
- **Step 5** (Audio streaming): 3-5 hours
- **Testing & debugging**: 4-8 hours

**Total: 15-28 hours of development work**

## Alternative Approaches

### Option A: Use JavaScript SDK via JSInterop
If this is for a Blazor app, you could use the official LiveKit JS SDK via JavaScript interop. This would be much faster.

### Option B: Wait for Official .NET SDK
Check if LiveKit has plans for an official .NET client SDK. The server SDK exists, so client SDK might be in development.

### Option C: Use Different Voice Solution
Consider if Fluxer supports other voice protocols that have better .NET support.

### Option D: Community Contribution
This could be a great open-source contribution! Building a LiveKit .NET client SDK would benefit the entire .NET + LiveKit community.

## Files Modified

- `Fluxer.Net.csproj` - Added Google.Protobuf and SIPSorcery dependencies
- `VoiceStateUpdatePayload.cs` - Added all required fields (completed)
- `VoiceStateManager.cs` - Modified `IsVoiceDataReady()` (completed)
- `BasicCommands.cs` - Updated to use ConnectionId as sessionId (completed)

## Files That Need Changes

- `VoiceClient.cs` - Complete rewrite of message handling (lines 139-255)
- `Voice/Protocol/*.cs` - New protobuf generated classes (need to create)
- `AudioPlayer.cs` - May need updates for WebRTC integration

## References

- [LiveKit Protocol](https://github.com/livekit/protocol)
- [LiveKit Client Protocol Docs](https://docs.livekit.io/reference/internals/client-protocol/)
- [SIPSorcery Documentation](https://www.sipsorcery.com/)
- [Protocol Buffers C#](https://protobuf.dev/getting-started/csharptutorial/)

## Recommendation

Given the complexity, I recommend:

1. **If you have WebRTC experience**: Proceed with manual implementation using the guide
2. **If you're new to WebRTC**: Consider alternatives (JSInterop, different voice solution)
3. **If you want to learn**: This is an excellent learning opportunity, but budget 20-30 hours

The foundation is in place (dependencies installed, root cause identified, documentation written). The next step requires WebRTC expertise and significant development time.
