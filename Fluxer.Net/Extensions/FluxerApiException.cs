using System.Net;

namespace Fluxer.Net.Rest;

/// <summary>
/// Thrown when the Fluxer API answers a request with an error.
/// </summary>
[System.Serializable]
public class FluxerApiException : System.Exception
{
    /// <summary>
    /// Creates the exception from a message and the raw response body.
    /// </summary>
    /// <param name="message">The exception message.</param>
    /// <param name="data">The raw response body.</param>
    public FluxerApiException(string message, string data) : base(message)
    {
        FluxerData = data;
    }

    /// <summary>
    /// Creates the exception from a response status and its parsed error body.
    /// </summary>
    /// <param name="message">The exception message.</param>
    /// <param name="data">The raw response body.</param>
    /// <param name="httpCode">The HTTP status of the response.</param>
    /// <param name="error">The parsed error body, or null when the body could not be parsed.</param>
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
    /// The error code from the response body, or null when there is none.
    /// </summary>
    public string? ErrorCode { get; }

    /// <summary>
    /// The reason from the response body, or null when there is none.
    /// </summary>
    public string? Reason { get; }

    /// <summary>
    /// The field errors of a validation failure.
    /// </summary>
    public IReadOnlyList<FluxerErrorItemJson> Errors { get; } = Array.Empty<FluxerErrorItemJson>();
}
