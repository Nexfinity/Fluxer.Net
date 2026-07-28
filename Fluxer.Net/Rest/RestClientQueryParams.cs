namespace Fluxer.Net.Rest;

/// <summary>
/// Query params for HTTP requests
/// </summary>
public class RestClientQueryParams
{
    private readonly Dictionary<string, string?> _params = new();

    /// <summary>
    /// Add query param
    /// </summary>
    /// <param name="key">Query param name (Ex.: Limit)</param>
    /// <param name="value">Query param value (Ex. 10)</param>
    /// <returns></returns>
    public RestClientQueryParams Add(string key, object? value)
    {
        if (value != null)
            _params[key] = value.ToString();
        return this;
    }

    /// <summary>
    /// Add query param
    /// </summary>
    /// <param name="key">Query param name (Ex.: Limit)</param>
    /// <param name="value">Query param value (Ex. 10)</param>
    /// <returns></returns>
    public RestClientQueryParams AddIf(bool condition, string key, object? value)
    {
        if (condition && value != null)
            _params[key] = value.ToString();
        return this;
    }

    /// <summary>
    /// Create dictionary of query params for Uri
    /// </summary>
    /// <returns></returns>
    public Dictionary<string, string?> ToDictionary() => _params;
}