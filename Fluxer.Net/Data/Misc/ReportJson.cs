using Newtonsoft.Json;

namespace Fluxer.Net;

/// <summary>
/// A report that was filed.
/// </summary>
public class ReportJson
{
    /// <summary>
    /// The report id.
    /// </summary>
    [JsonProperty("report_id")]
    public ulong ReportId { get; set; }

    /// <summary>
    /// "pending" or "resolved".
    /// </summary>
    [JsonProperty("status")]
    public string Status { get; set; }

    /// <summary>
    /// When the report was filed.
    /// </summary>
    [JsonProperty("reported_at")]
    public DateTimeOffset ReportedAt { get; set; }
}

/// <summary>
/// A stored theme.
/// </summary>
public class ThemeJson
{
    /// <summary>
    /// The theme id.
    /// </summary>
    [JsonProperty("id")]
    public string Id { get; set; }
}
