using System.Net;

namespace Fluxer.Net.Rest;

/// <summary>
/// Thrown when the Fluxer API answers a request with an error.
/// </summary>
[System.Serializable]
public class FluxerApiException : System.Exception
{
    public FluxerApiException(string message, string data) : base(message)
    {
        FluxerData = data;
    }

    /// <summary>
    /// Creates the exception from a response status and its parsed error body.
    /// </summary>
    public FluxerApiException(string message, string data, HttpStatusCode httpCode, FluxerErrorJson? error) : this(message, data)
    {
        HttpCode = httpCode;
        ErrorCode = error?.Code;
        Reason = error?.Reason;
        Errors = error?.Errors ?? Array.Empty<FluxerErrorItemJson>();
    }

    /// <summary>
    /// The raw response body.
    /// </summary>
    public string FluxerData { get; set; }

    /// <summary>
    /// The HTTP status of the response.
    /// </summary>
    public HttpStatusCode HttpCode { get; }

    /// <summary>
    /// Fluxer's error code.
    /// </summary>
    public string? ErrorCode { get; }

    /// <summary>
    /// Fluxer's explanation of the error.
    /// </summary>
    public string? Reason { get; }

    /// <summary>
    /// The field errors of a validation failure.
    /// </summary>
    public IReadOnlyList<FluxerErrorItemJson> Errors { get; } = Array.Empty<FluxerErrorItemJson>();
}
