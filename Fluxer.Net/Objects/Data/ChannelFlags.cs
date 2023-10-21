namespace Fluxer.Net.Objects.Data;

public enum ChannelFlags
{
    None = 0,
    Sensitive = 1 << 0,
    /// <summary>
    ///     NOTE: this flag will probably be removed.
    /// </summary>
    OwnerOnlyDeleteMe = 1 << 1,
    ResourceChannel = 1 << 2,
    ChannelDeleteLocked = 1 << 3,
}
