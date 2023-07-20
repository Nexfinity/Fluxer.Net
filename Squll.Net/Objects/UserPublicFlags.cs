namespace Squll.Net.Objects;

public enum UserPublicFlags
{
    None = 0,
    Staff = 1 << 0,
    Tester = 1 << 1,
    Verified = 1 << 2,
    Deleted = 1 << 3,
    Sensitive = 1 << 4,
    AgeVerified = 1 << 5,
    OngoingIncident = 1 << 6,
}
