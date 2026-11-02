namespace Fluxer.Net;

/// <summary>
/// Converts snowflake identifiers to and from timestamps.
/// </summary>
public static class SnowflakeUtils
{
    /// <summary>
    /// Gets the time a snowflake was created.
    /// </summary>
    /// <param name="value">The snowflake id.</param>
    /// <returns>The creation time, with a zero offset.</returns>
    public static DateTimeOffset FromSnowflake(ulong value)
        => DateTimeOffset.FromUnixTimeMilliseconds((long)((value >> 22) + 1420070400000UL));

    /// <summary>
    /// Creates a snowflake id for a point in time. The worker, process and sequence bits are set to zero.
    /// </summary>
    /// <param name="value">The time to encode.</param>
    /// <returns>The snowflake id for the time.</returns>
    public static ulong ToSnowflake(DateTimeOffset value)
        => ((ulong)value.ToUnixTimeMilliseconds() - 1420070400000UL) << 22;
}