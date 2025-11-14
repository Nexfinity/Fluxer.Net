/**
 * Fluxer Voice Bridge
 *
 * A lightweight Node.js service that bridges Fluxer.Net with LiveKit voice servers.
 * Uses the official livekit-client SDK to handle all WebRTC complexity.
 */

import { Room, RoomEvent, Track } from 'livekit-client';
import { WebSocketServer, WebSocket } from 'ws';

const PORT = process.env.VOICE_BRIDGE_PORT || 8765;
const DEBUG = process.env.DEBUG === 'true';

// Active voice connections (keyed by connection ID)
const connections = new Map();

// WebSocket server for communication with Fluxer.Net
const wss = new WebSocketServer({ port: PORT });

console.log(`[Fluxer Voice Bridge] Starting on port ${PORT}...`);

wss.on('connection', (ws) => {
    console.log('[Bridge] Client connected from Fluxer.Net');

    ws.on('message', async (data) => {
        try {
            const message = JSON.parse(data.toString());
            await handleMessage(ws, message);
        } catch (error) {
            console.error('[Bridge] Error handling message:', error);
            sendError(ws, 'PARSE_ERROR', error.message);
        }
    });

    ws.on('close', () => {
        console.log('[Bridge] Client disconnected, cleaning up connections...');
        // Disconnect all rooms associated with this WebSocket
        for (const [connectionId, conn] of connections.entries()) {
            if (conn.ws === ws) {
                disconnectRoom(connectionId);
            }
        }
    });

    ws.on('error', (error) => {
        console.error('[Bridge] WebSocket error:', error);
    });
});

/**
 * Handle incoming messages from Fluxer.Net
 */
async function handleMessage(ws, message) {
    const { type, connectionId, data } = message;

    if (DEBUG) {
        console.log(`[Bridge] Received: ${type}`, { connectionId, data: data ? '...' : null });
    }

    switch (type) {
        case 'CONNECT':
            await handleConnect(ws, connectionId, data);
            break;

        case 'DISCONNECT':
            await handleDisconnect(connectionId);
            break;

        case 'SET_MUTE':
            await handleSetMute(connectionId, data.muted);
            break;

        case 'SET_DEAF':
            await handleSetDeaf(connectionId, data.deafened);
            break;

        case 'PING':
            sendMessage(ws, { type: 'PONG', connectionId });
            break;

        default:
            console.warn(`[Bridge] Unknown message type: ${type}`);
            sendError(ws, 'UNKNOWN_TYPE', `Unknown message type: ${type}`);
    }
}

/**
 * Connect to a LiveKit room
 */
async function handleConnect(ws, connectionId, data) {
    const { endpoint, token, guildId, channelId, userId } = data;

    console.log(`[Bridge] Connecting to LiveKit: ${endpoint}`);
    console.log(`  Guild: ${guildId}, Channel: ${channelId}, User: ${userId}`);

    // Disconnect existing connection if present
    if (connections.has(connectionId)) {
        await disconnectRoom(connectionId);
    }

    try {
        // Create LiveKit room
        const room = new Room({
            adaptiveStream: true,
            dynacast: true,
            // Disable automatic track subscription - we'll handle it manually
            autoSubscribe: false,
        });

        // Store connection info
        const conn = {
            ws,
            room,
            connectionId,
            guildId,
            channelId,
            userId,
            endpoint,
            connected: false,
            muted: false,
            deafened: false,
        };

        connections.set(connectionId, conn);

        // Set up room event handlers
        setupRoomEvents(conn);

        // Connect to LiveKit server
        await room.connect(endpoint, token, { autoSubscribe: false });

        conn.connected = true;
        console.log(`[Bridge] ✓ Connected to LiveKit room: ${room.name}`);

        // Send ready event to Fluxer.Net
        sendMessage(ws, {
            type: 'READY',
            connectionId,
            data: {
                roomName: room.name,
                participantCount: room.numParticipants,
            }
        });

    } catch (error) {
        console.error('[Bridge] Connection failed:', error);
        connections.delete(connectionId);
        sendError(ws, 'CONNECTION_FAILED', error.message, connectionId);
    }
}

/**
 * Set up LiveKit room event handlers
 */
function setupRoomEvents(conn) {
    const { room, ws, connectionId } = conn;

    // Connection state events
    room.on(RoomEvent.Connected, () => {
        console.log(`[Bridge] Room connected: ${room.name}`);
        sendMessage(ws, { type: 'CONNECTED', connectionId });
    });

    room.on(RoomEvent.Disconnected, (reason) => {
        console.log(`[Bridge] Room disconnected: ${reason}`);
        conn.connected = false;
        sendMessage(ws, {
            type: 'DISCONNECTED',
            connectionId,
            data: { reason }
        });
        connections.delete(connectionId);
    });

    room.on(RoomEvent.Reconnecting, () => {
        console.log('[Bridge] Room reconnecting...');
        sendMessage(ws, { type: 'RECONNECTING', connectionId });
    });

    room.on(RoomEvent.Reconnected, () => {
        console.log('[Bridge] Room reconnected');
        sendMessage(ws, { type: 'RECONNECTED', connectionId });
    });

    // Participant events
    room.on(RoomEvent.ParticipantConnected, (participant) => {
        console.log(`[Bridge] Participant joined: ${participant.identity}`);
        sendMessage(ws, {
            type: 'PARTICIPANT_JOINED',
            connectionId,
            data: serializeParticipant(participant)
        });
    });

    room.on(RoomEvent.ParticipantDisconnected, (participant) => {
        console.log(`[Bridge] Participant left: ${participant.identity}`);
        sendMessage(ws, {
            type: 'PARTICIPANT_LEFT',
            connectionId,
            data: { identity: participant.identity }
        });
    });

    // Track events
    room.on(RoomEvent.TrackSubscribed, (track, publication, participant) => {
        console.log(`[Bridge] Track subscribed: ${track.kind} from ${participant.identity}`);

        // For audio tracks, handle deafen state
        if (track.kind === Track.Kind.Audio && conn.deafened) {
            publication.setEnabled(false);
        }

        sendMessage(ws, {
            type: 'TRACK_SUBSCRIBED',
            connectionId,
            data: {
                trackSid: publication.trackSid,
                kind: track.kind,
                participant: participant.identity,
            }
        });
    });

    room.on(RoomEvent.TrackUnsubscribed, (track, publication, participant) => {
        console.log(`[Bridge] Track unsubscribed: ${track.kind} from ${participant.identity}`);
        sendMessage(ws, {
            type: 'TRACK_UNSUBSCRIBED',
            connectionId,
            data: {
                trackSid: publication.trackSid,
                participant: participant.identity,
            }
        });
    });

    // Speaking events
    room.on(RoomEvent.ActiveSpeakersChanged, (speakers) => {
        const speakerIdentities = speakers.map(p => p.identity);
        if (DEBUG) {
            console.log('[Bridge] Active speakers:', speakerIdentities);
        }
        sendMessage(ws, {
            type: 'SPEAKING_CHANGED',
            connectionId,
            data: { speakers: speakerIdentities }
        });
    });

    // Track published (for local participant)
    room.on(RoomEvent.LocalTrackPublished, (publication) => {
        console.log(`[Bridge] Local track published: ${publication.kind}`);
        sendMessage(ws, {
            type: 'LOCAL_TRACK_PUBLISHED',
            connectionId,
            data: {
                trackSid: publication.trackSid,
                kind: publication.kind,
            }
        });
    });

    // Connection quality
    room.on(RoomEvent.ConnectionQualityChanged, (quality, participant) => {
        if (DEBUG) {
            console.log(`[Bridge] Connection quality: ${quality} for ${participant.identity}`);
        }
        sendMessage(ws, {
            type: 'CONNECTION_QUALITY',
            connectionId,
            data: {
                participant: participant.identity,
                quality,
            }
        });
    });

    // Error handling
    room.on(RoomEvent.MediaDevicesError, (error) => {
        console.error('[Bridge] Media device error:', error);
        sendError(ws, 'MEDIA_DEVICE_ERROR', error.message, connectionId);
    });
}

/**
 * Handle disconnect request
 */
async function handleDisconnect(connectionId) {
    console.log(`[Bridge] Disconnecting: ${connectionId}`);
    await disconnectRoom(connectionId);
}

/**
 * Disconnect from a room
 */
async function disconnectRoom(connectionId) {
    const conn = connections.get(connectionId);
    if (!conn) return;

    try {
        if (conn.room) {
            conn.room.removeAllListeners();
            await conn.room.disconnect();
        }
    } catch (error) {
        console.error('[Bridge] Error disconnecting:', error);
    } finally {
        connections.delete(connectionId);
        console.log(`[Bridge] Disconnected: ${connectionId}`);
    }
}

/**
 * Handle mute state change
 */
async function handleSetMute(connectionId, muted) {
    const conn = connections.get(connectionId);
    if (!conn || !conn.room) {
        console.warn(`[Bridge] Cannot set mute: connection ${connectionId} not found`);
        return;
    }

    try {
        const { room } = conn;
        conn.muted = muted;

        if (muted) {
            // Mute local microphone
            await room.localParticipant.setMicrophoneEnabled(false);
            console.log(`[Bridge] Microphone muted for ${connectionId}`);
        } else {
            // Unmute local microphone
            await room.localParticipant.setMicrophoneEnabled(true);
            console.log(`[Bridge] Microphone unmuted for ${connectionId}`);
        }

        sendMessage(conn.ws, {
            type: 'MUTE_CHANGED',
            connectionId,
            data: { muted }
        });

    } catch (error) {
        console.error('[Bridge] Error setting mute:', error);
        sendError(conn.ws, 'MUTE_ERROR', error.message, connectionId);
    }
}

/**
 * Handle deafen state change
 */
async function handleSetDeaf(connectionId, deafened) {
    const conn = connections.get(connectionId);
    if (!conn || !conn.room) {
        console.warn(`[Bridge] Cannot set deaf: connection ${connectionId} not found`);
        return;
    }

    try {
        const { room } = conn;
        conn.deafened = deafened;

        // When deafened, also mute microphone and disable all audio subscriptions
        if (deafened) {
            await room.localParticipant.setMicrophoneEnabled(false);

            // Disable all remote audio tracks
            room.remoteParticipants.forEach(participant => {
                participant.audioTrackPublications.forEach(publication => {
                    publication.setEnabled(false);
                    publication.setSubscribed(false);
                });
            });

            console.log(`[Bridge] Deafened for ${connectionId}`);
        } else {
            // Re-enable audio subscriptions (but respect mute state for mic)
            room.remoteParticipants.forEach(participant => {
                participant.audioTrackPublications.forEach(publication => {
                    publication.setEnabled(true);
                    publication.setSubscribed(true);
                });
            });

            // Only unmute mic if not manually muted
            if (!conn.muted) {
                await room.localParticipant.setMicrophoneEnabled(true);
            }

            console.log(`[Bridge] Undeafened for ${connectionId}`);
        }

        sendMessage(conn.ws, {
            type: 'DEAF_CHANGED',
            connectionId,
            data: { deafened }
        });

    } catch (error) {
        console.error('[Bridge] Error setting deaf:', error);
        sendError(conn.ws, 'DEAF_ERROR', error.message, connectionId);
    }
}

/**
 * Serialize participant info for sending to Fluxer.Net
 */
function serializeParticipant(participant) {
    return {
        identity: participant.identity,
        sid: participant.sid,
        name: participant.name,
        metadata: participant.metadata,
        isSpeaking: participant.isSpeaking,
        connectionQuality: participant.connectionQuality,
        isMicrophoneEnabled: participant.isMicrophoneEnabled,
        isCameraEnabled: participant.isCameraEnabled,
        isScreenShareEnabled: participant.isScreenShareEnabled,
    };
}

/**
 * Send message to Fluxer.Net
 */
function sendMessage(ws, message) {
    if (ws.readyState === WebSocket.OPEN) {
        ws.send(JSON.stringify(message));
    }
}

/**
 * Send error to Fluxer.Net
 */
function sendError(ws, code, message, connectionId = null) {
    sendMessage(ws, {
        type: 'ERROR',
        connectionId,
        data: { code, message }
    });
}

console.log(`[Fluxer Voice Bridge] Ready and listening on port ${PORT}`);
console.log('[Bridge] Waiting for connections from Fluxer.Net...');
