namespace Fluxer.Net.Data.Requests;

/// <remarks>
/// <see href="https://github.com/fluxerapp/fluxer/blob/848269a4d4df7349acfc861ff926b17fe4c4a548/packages/schema/src/domains/message/MessageRequestSchemas.tsx#L314"/>
/// </remarks>
public class ChannelPinsQuery
{
    /// <summary>
    /// Maximum number of pinned messages to return (1-50)
    /// </summary>
    public int? Limit { get; set; }

    /// <summary>
    /// Get pinned messages before this timestamp
    /// </summary>
    public DateTime? Before { get; set; }

    public string BuildQuery()
    {
        var list = new List<string>(2);
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
