# Gateway OpCode and Event Updates

This document summarizes the updates made to align Fluxer.Net's gateway implementation with the official Fluxer API specification from `ExternalApiReferences/Constants.ts`.

## OpCode Updates

### Updated `FluxerOpCode` Enum

The OpCode enum values were **completely incorrect** in the original implementation. They have been corrected to match the official Fluxer gateway specification:

```csharp
// BEFORE (INCORRECT)
public enum FluxerOpCode
{
    Dispatch = 0,
    Heartbeat = 1,
    Identify = 2,
    PresenceUpdate = 3,
    Resume = 4,          // Was actually opcode 6!
    Reconnect = 5,       // Was actually opcode 7!
    InvalidSession = 6,  // Was actually opcode 9!
    Hello = 7,           // Was actually opcode 10!
    HeartbeatAck = 8     // Was actually opcode 11!
}

// AFTER (CORRECT)
public enum FluxerOpCode
{
    Dispatch = 0,
    Heartbeat = 1,
    Identify = 2,
    PresenceUpdate = 3,
    VoiceStateUpdate = 4,        // NEW
    VoiceServerPing = 5,         // NEW
    Resume = 6,                  // CORRECTED from 4
    Reconnect = 7,               // CORRECTED from 5
    RequestGuildMembers = 8,     // NEW
    InvalidSession = 9,          // CORRECTED from 6
    Hello = 10,                  // CORRECTED from 7
    HeartbeatAck = 11,           // CORRECTED from 8
    CallConnect = 13,            // NEW
    GuildSubscriptions = 14      // NEW
}
```

**Impact**: This was a **critical bug** that would have caused the gateway to misinterpret server messages, likely causing connection failures, missed events, or incorrect reconnection behavior.

### Updated GatewayClient OpCode Handler

Added proper handling for all opcodes with appropriate logging:

- **VoiceStateUpdate** (4) - Voice channel state changes
- **VoiceServerPing** (5) - Voice server heartbeat
- **RequestGuildMembers** (8) - Guild member chunk requests
- **CallConnect** (13) - Voice call connection
- **GuildSubscriptions** (14) - Guild event subscription management

## New Gateway Events

### Created Gateway Data Types

Created 9 new gateway data classes in `Gateway/Data/`:

1. **MessageReactionGatewayData.cs** - Message reaction add/remove events
2. **MessageReactionRemoveEmojiGatewayData.cs** - Remove all reactions of specific emoji
3. **MessageBulkDeleteGatewayData.cs** - Bulk message deletion
4. **MessageAckGatewayData.cs** - Message acknowledgment
5. **ChannelPinsUpdateGatewayData.cs** - Channel pins update
6. **VoiceStateGatewayData.cs** - Voice state changes
7. **VoiceServerUpdateGatewayData.cs** - Voice server connection info
8. **WebhooksUpdateGatewayData.cs** - Webhook updates
9. **GuildBanGatewayData.cs** - Guild ban add/remove events

### Added Event Handlers

Added 14 new event delegates and handlers to `GatewayClient.cs`:

#### Message Reactions
- `MessageReactionAdd` - MESSAGE_REACTION_ADD
- `MessageReactionRemove` - MESSAGE_REACTION_REMOVE
- `MessageReactionRemoveAll` - MESSAGE_REACTION_REMOVE_ALL
- `MessageReactionRemoveEmoji` - MESSAGE_REACTION_REMOVE_EMOJI

#### Message Operations
- `MessageDeleteBulk` - MESSAGE_DELETE_BULK
- `MessageAck` - MESSAGE_ACK

#### Channel Updates
- `ChannelPinsUpdate` - CHANNEL_PINS_UPDATE

#### Voice Events
- `VoiceStateUpdate` - VOICE_STATE_UPDATE
- `VoiceServerUpdate` - VOICE_SERVER_UPDATE

#### Guild/Community Bans
- `GuildBanAdd` - GUILD_BAN_ADD
- `GuildBanRemove` - GUILD_BAN_REMOVE

#### Webhooks
- `WebhooksUpdate` - WEBHOOKS_UPDATE

### Current Guild Event Naming

The Fluxer API uses `GUILD_*` event naming (not COMMUNITY). All events have been properly named to match the specification from Constants.ts:

- `GUILD_CREATE` → GuildCreate
- `GUILD_UPDATE` → GuildUpdate
- `GUILD_DELETE` → GuildDelete
- `GUILD_MEMBER_ADD` → GuildMemberAdd
- `GUILD_MEMBER_UPDATE` → GuildMemberUpdate
- `GUILD_MEMBER_REMOVE` → GuildMemberRemove
- `GUILD_ROLE_CREATE` → GuildRoleCreate
- `GUILD_ROLE_UPDATE` → GuildRoleUpdate
- `GUILD_ROLE_DELETE` → GuildRoleDelete

**Note**: "Community" was a legacy/internal name used in early development. The official Fluxer API uses "Guild" terminology consistently with Discord-like platforms.

## Events from Constants.ts Not Yet Implemented

The following events from `Constants.ts` are **not yet implemented** as they require additional domain objects or user-specific data structures:

### User Settings & State
- `SESSIONS_REPLACE` - User session management
- `USER_PINNED_DMS_UPDATE` - Pinned DM changes
- `USER_SETTINGS_UPDATE` - User settings changes
- `USER_GUILD_SETTINGS_UPDATE` - Per-guild user settings
- `USER_NOTE_UPDATE` - User note updates

### Saved Content & Favorites
- `RECENT_MENTION_DELETE` - Recent mention deletion
- `SAVED_MESSAGE_CREATE` - Saved message added
- `SAVED_MESSAGE_DELETE` - Saved message removed
- `FAVORITE_MEME_CREATE` - Favorite meme added
- `FAVORITE_MEME_UPDATE` - Favorite meme updated
- `FAVORITE_MEME_DELETE` - Favorite meme removed

### Authentication & Sessions
- `AUTH_SESSION_CHANGE` - Authentication session changes

### Guild/Community Features
- `GUILD_EMOJIS_UPDATE` - Guild emoji list update
- `GUILD_STICKERS_UPDATE` - Guild sticker list update
- `GUILD_ROLE_UPDATE_BULK` - Bulk role updates

### Channel Advanced Features
- `CHANNEL_UPDATE_BULK` - Bulk channel updates
- `CHANNEL_PINS_ACK` - Pin acknowledgment
- `CHANNEL_RECIPIENT_ADD` - Group DM recipient added
- `CHANNEL_RECIPIENT_REMOVE` - Group DM recipient removed

### Relationships
- `RELATIONSHIP_ADD` - Friend/block added
- `RELATIONSHIP_UPDATE` - Friend/block updated
- `RELATIONSHIP_REMOVE` - Friend/block removed

**Reason for omission**: These events require corresponding domain objects (e.g., `Relationship`, `SavedMessage`, `FavoriteMeme`, `UserSettings`) that don't currently exist in the `Objects/` namespace. They should be implemented when those domain models are added.

## Usage Examples

### Subscribing to New Events

```csharp
var gateway = new GatewayClient(token, config);

// Guild events
gateway.GuildCreate += (data) =>
{
    Console.WriteLine($"Joined guild: {data.Name} (ID: {data.Id})");
};

gateway.GuildMemberAdd += (data) =>
{
    Console.WriteLine($"Member {data.User.Username} joined guild");
};

// Message reactions
gateway.MessageReactionAdd += (data) =>
{
    Console.WriteLine($"User {data.UserId} reacted with {data.Emoji.Name} to message {data.MessageId}");
};

gateway.MessageReactionRemove += (data) =>
{
    Console.WriteLine($"User {data.UserId} removed reaction {data.Emoji.Name} from message {data.MessageId}");
};

// Bulk message deletion
gateway.MessageDeleteBulk += (data) =>
{
    Console.WriteLine($"Bulk deleted {data.Ids.Count} messages in channel {data.ChannelId}");
};

// Voice state updates
gateway.VoiceStateUpdate += (data) =>
{
    if (data.ChannelId.HasValue)
        Console.WriteLine($"User {data.UserId} joined voice channel {data.ChannelId}");
    else
        Console.WriteLine($"User {data.UserId} left voice channel");
};

// Guild bans
gateway.GuildBanAdd += (data) =>
{
    Console.WriteLine($"User {data.User.Username} was banned from guild {data.GuildId}");
};

gateway.GuildBanRemove += (data) =>
{
    Console.WriteLine($"User {data.User.Username} was unbanned from guild {data.GuildId}");
};

// Channel pins
gateway.ChannelPinsUpdate += (data) =>
{
    Console.WriteLine($"Pins updated in channel {data.ChannelId}");
};

await gateway.ConnectAsync();
```

## Breaking Changes

### OpCode Value Changes

**CRITICAL**: The OpCode enum values changed significantly. Any code that:
- Hardcodes OpCode integer values
- Logs or stores OpCode values
- Implements custom gateway packet handling

**Must be updated** to use the new enum values. The old values will cause incorrect behavior.

### Example of Breaking Code

```csharp
// DON'T DO THIS - hardcoded values are now wrong!
if (packet.OpCode == (FluxerOpCode)7) // This was Hello, now it's Reconnect!
{
    // ...
}

// DO THIS instead - use the enum
if (packet.OpCode == FluxerOpCode.Hello)
{
    // ...
}
```

## Testing Recommendations

1. **Test OpCode handling** - Verify correct opcodes are sent/received
2. **Test reconnection** - Ensure Resume (opcode 6) and Reconnect (opcode 7) work correctly
3. **Test voice events** - If using voice features, test VoiceStateUpdate and VoiceServerUpdate
4. **Test reactions** - Verify reaction add/remove events fire correctly
5. **Test bulk operations** - Test bulk message deletion
6. **Test legacy events** - Verify GUILD_* events map to COMMUNITY_* handlers
7. **Test unknown events** - Verify graceful handling of unimplemented events

## Build Status

✅ **Build Successful** - All changes compile without errors or warnings.

## Next Steps

To fully implement the Fluxer gateway specification:

1. Create domain objects for user settings, relationships, saved messages, and favorite memes
2. Implement the missing gateway data types listed above
3. Add corresponding event handlers to GatewayClient
4. Update HandleDispatch to handle the new events
5. Add integration tests for all gateway events
