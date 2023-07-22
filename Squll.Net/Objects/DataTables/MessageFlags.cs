namespace Squll.Net.Objects;

public enum MessageFlags
{
    None = 0,
    Encrypted = 1 << 0,
    Ephemeral = 1 << 1,
    MentionEveryone = 1 << 2,
    Pinned = 1 << 3,
    SuppressEmbeds = 1 << 4,
    SuppressNotifications = 1 << 5,
}
