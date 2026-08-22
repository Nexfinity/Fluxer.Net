namespace Fluxer.Net.Rest;

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
