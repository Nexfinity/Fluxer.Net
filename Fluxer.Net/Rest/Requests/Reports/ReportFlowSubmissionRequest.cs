using Newtonsoft.Json;

namespace Fluxer.Net.Rest;

/// <summary>
/// The answers of a finished report flow.
/// </summary>
public abstract class ReportFlowSubmissionRequest
{
    /// <summary>
    /// The revision hash of the flow that was answered.
    /// </summary>
    [JsonProperty("revision_hash")]
    public string RevisionHash { get; set; }

    /// <summary>
    /// The screens answered in order, between 1 and 16.
    /// </summary>
    [JsonProperty("steps")]
    public IEnumerable<ReportFlowStepRequest> Steps { get; set; }

    /// <summary>
    /// The language tag the flow was shown in.
    /// </summary>
    [JsonProperty("locale")]
    public string? Locale { get; set; }
}
