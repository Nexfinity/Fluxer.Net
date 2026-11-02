namespace Fluxer.Net.Rest;

/// <summary>
/// Query options for listing the pinned messages of a channel.
/// </summary>
public class ChannelPinsQuery
{
    /// <summary>
    /// Maximum number of pinned messages to return (1-50)
    /// </summary>
    public int? Limit { get; set; }

    /// <summary>
    /// Get pinned messages before this timestamp
    /// </summary>
    public DateTimeOffset? Before { get; set; }

    /// <summary>
    /// Builds the query string from the options that are set.
    /// </summary>
    /// <returns>The query string without a leading question mark.</returns>
    public string BuildQuery()
    {
        List<string> list = new List<string>(2);
        if (Limit.HasValue && Limit.Value > 0)
        {
            list.Add($"limit={Limit.Value}");
        }
        if (Before.HasValue)
        {
            list.Add("before=" + Before.Value.ToString("O"));
        }
        return string.Join("&", list);
    }
}
