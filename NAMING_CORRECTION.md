# Naming Correction: Community → Guild

This document describes the naming correction applied to align with the official Fluxer API specification.

## Context

The codebase incorrectly used "Community" terminology, which was a legacy/internal name from early development. The official Fluxer API uses **"Guild"** terminology (consistent with Discord-like platforms), as documented in `ExternalApiReferences/Constants.ts`.

## Changes Made

### Gateway Data Types Renamed

| Old Name (Incorrect) | New Name (Correct) | File |
|---------------------|-------------------|------|
| `CommunityGatewayData` | `GuildGatewayData` | `Gateway/Data/SquadGatewayData.cs` → Uses `GuildGatewayData` class |
| `CommunityMemberGatewayData` | `GuildMemberGatewayData` | `Gateway/Data/SquadMemberGatewayData.cs` → Uses `GuildMemberGatewayData` class |

### Event Delegates Renamed

All Community-related event delegates in `GatewayClient.cs` were renamed to use Guild terminology:

#### Guild Events
| Old Name | New Name |
|----------|----------|
| `CommunityCreateEvent` | `GuildCreateEvent` |
| `CommunityUpdateEvent` | `GuildUpdateEvent` |
| `CommunityDeleteEvent` | `GuildDeleteEvent` |

#### Guild Member Events
| Old Name | New Name |
|----------|----------|
| `CommunityMemberCreateEvent` | `GuildMemberAddEvent` |
| `CommunityMemberUpdateEvent` | `GuildMemberUpdateEvent` |
| `CommunityMemberDeleteEvent` | `GuildMemberRemoveEvent` |

**Note**: Member events also corrected to use Add/Remove instead of Create/Delete to match the API event names (`GUILD_MEMBER_ADD`, `GUILD_MEMBER_REMOVE`).

#### Guild Role Events
| Old Name | New Name |
|----------|----------|
| `RoleCreateEvent` | `GuildRoleCreateEvent` |
| `RoleUpdateEvent` | `GuildRoleUpdateEvent` |
| `RoleDeleteEvent` | `GuildRoleDeleteEvent` |

**Note**: Added "Guild" prefix to role events for clarity and consistency with API event names (`GUILD_ROLE_CREATE`, etc.).

### Event Fields Renamed

All corresponding event fields updated:

```csharp
// Old (Incorrect)
public event CommunityCreateEvent CommunityCreate;
public event CommunityMemberCreateEvent CommunityMemberCreate;
public event RoleCreateEvent RoleCreate;

// New (Correct)
public event GuildCreateEvent GuildCreate;
public event GuildMemberAddEvent GuildMemberAdd;
public event GuildRoleCreateEvent GuildRoleCreate;
```

### HandleDispatch Event Invocations Updated

All event invocations in `HandleDispatch()` updated to use the correct event names:

```csharp
// GUILD_CREATE event
case "GUILD_CREATE":
    if (p.Data is GuildGatewayData guildCreateData)
        GuildCreate?.Invoke(guildCreateData);  // Changed from CommunityCreate
    // ...

// GUILD_MEMBER_ADD event
case "GUILD_MEMBER_ADD":
    if (p.Data is GuildMemberGatewayData guildMemberAddData)
        GuildMemberAdd?.Invoke(guildMemberAddData);  // Changed from CommunityMemberCreate
    // ...

// GUILD_ROLE_CREATE event
case "GUILD_ROLE_CREATE":
    if (p.Data is RoleGatewayData guildRoleCreateData)
        GuildRoleCreate?.Invoke(guildRoleCreateData);  // Changed from RoleCreate
    // ...
```

### Removed Incorrect Documentation

Removed misleading documentation that suggested:
- GUILD_* events were "legacy" (they are actually current)
- COMMUNITY_* events were "current" (they never existed in the official API)
- Backward compatibility mapping from GUILD_* to Community handlers (unnecessary)

## API Alignment

All naming now correctly matches `ExternalApiReferences/Constants.ts`:

```typescript
export type GatewayDispatchEvent =
    | 'GUILD_CREATE'
    | 'GUILD_UPDATE'
    | 'GUILD_DELETE'
    | 'GUILD_MEMBER_ADD'
    | 'GUILD_MEMBER_UPDATE'
    | 'GUILD_MEMBER_REMOVE'
    | 'GUILD_ROLE_CREATE'
    | 'GUILD_ROLE_UPDATE'
    | 'GUILD_ROLE_DELETE'
    // ... other events
```

## Breaking Changes

### For Library Consumers

If you were using the library before this change, you need to update your event subscriptions:

```csharp
// OLD CODE (No longer works)
gateway.CommunityCreate += (data) => { /* ... */ };
gateway.CommunityMemberCreate += (data) => { /* ... */ };
gateway.RoleCreate += (data) => { /* ... */ };

// NEW CODE (Correct)
gateway.GuildCreate += (data) => { /* ... */ };
gateway.GuildMemberAdd += (data) => { /* ... */ };
gateway.GuildRoleCreate += (data) => { /* ... */ };
```

### Member Event Naming Change

Member events changed from Create/Delete to Add/Remove:

```csharp
// OLD
gateway.CommunityMemberCreate += ...
gateway.CommunityMemberDelete += ...

// NEW
gateway.GuildMemberAdd += ...
gateway.GuildMemberRemove += ...
```

## Remaining Legacy References

Some domain objects in the `Objects/` namespace may still use "Community" terminology:

- `Community` class (domain model)
- `CommunityProperties` class
- `CommunityMember` class

These can be renamed in a future update if desired, but they don't affect the gateway event naming which is now correct.

## Verification

✅ **Build Status**: All changes compile successfully with no errors or warnings.

✅ **Event Coverage**: All official Fluxer API guild-related events are properly implemented:
- GUILD_CREATE, GUILD_UPDATE, GUILD_DELETE
- GUILD_MEMBER_ADD, GUILD_MEMBER_UPDATE, GUILD_MEMBER_REMOVE
- GUILD_ROLE_CREATE, GUILD_ROLE_UPDATE, GUILD_ROLE_DELETE
- GUILD_BAN_ADD, GUILD_BAN_REMOVE

✅ **Documentation Updated**:
- `GATEWAY_UPDATES.md` corrected
- `CLAUDE.md` updated with accurate terminology

## Summary

The library now correctly uses **"Guild"** terminology throughout, matching the official Fluxer API specification. This eliminates confusion and ensures developers using this library work with the same terminology used in the official API documentation.
