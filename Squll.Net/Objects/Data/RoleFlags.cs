namespace Squll.Net.Objects.Data;

public enum RoleFlags
{
    None = 0,
    Hoiseted = 1 << 0,
    Managed = 1 << 1,
    SelfAssignable = 1 << 2,
    MfaRequired = 1 << 3
}
