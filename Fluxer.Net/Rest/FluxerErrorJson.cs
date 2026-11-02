using Newtonsoft.Json;

namespace Fluxer.Net;

/// <summary>
/// The error body the Fluxer API returns for a failed request.
/// </summary>
public class FluxerErrorJson
{
    /// <summary>
    /// The error code, such as <c>ACCESS_DENIED</c>.
    /// </summary>
    [JsonProperty("code")]
    public string Code { get; set; }

    /// <summary>
    /// The human-readable error message.
    /// </summary>
    [JsonProperty("message")]
    public string Reason { get; set; }

    /// <summary>
    /// The errors for individual fields, or null when the response has none.
    /// </summary>
    [JsonProperty("errors")]
    public FluxerErrorItemJson[]? Errors { get; set; }
}

/// <summary>
/// A validation error for one field of a request.
/// </summary>
public class FluxerErrorItemJson
{
    /// <summary>
    /// The path of the field that failed validation.
    /// </summary>
    [JsonProperty("path")]
    public string Path { get; set; }

    /// <summary>
    /// The error code for the field.
    /// </summary>
    [JsonProperty("code")]
    public string Code { get; set; }

    /// <summary>
    /// The human-readable message for the field.
    /// </summary>
    [JsonProperty("message")]
    public string Reason { get; set; }
}