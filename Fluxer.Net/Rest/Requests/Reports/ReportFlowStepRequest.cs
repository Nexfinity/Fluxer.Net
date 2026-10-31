using Newtonsoft.Json;

namespace Fluxer.Net.Rest;

/// <summary>
/// The answer chosen on one screen of a report flow.
/// </summary>
public class ReportFlowStepRequest
{
    /// <summary>
    /// The screen that was answered.
    /// </summary>
    [JsonProperty("screen_id")]
    public string ScreenId { get; set; }

    /// <summary>
    /// The option that was chosen.
    /// </summary>
    [JsonProperty("option_id")]
    public string? OptionId { get; set; }

    /// <summary>
    /// The checklist items that were checked, up to 32.
    /// </summary>
    [JsonProperty("item_ids")]
    public IEnumerable<string>? ItemIds { get; set; }
}
