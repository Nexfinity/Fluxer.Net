namespace Fluxer.Net.Rest;

/// <summary>
/// The query parameters of a REST request.
/// </summary>
public class RestClientQueryParams
{
    private readonly Dictionary<string, string?> _params = new();

    /// <summary>
    /// Adds a query parameter. A null value adds nothing.
    /// </summary>
    /// <param name="key">The parameter name.</param>
    /// <param name="value">The parameter value, converted to a string.</param>
    /// <returns>This instance, for chaining.</returns>
    public RestClientQueryParams Add(string key, object? value)
    {
        if (value != null)
            _params[key] = value.ToString();
        return this;
    }

    /// <summary>
    /// Adds a query parameter when the condition is true and the value is not null.
    /// </summary>
    /// <param name="condition">Whether the parameter is added.</param>
    /// <param name="key">The parameter name.</param>
    /// <param name="value">The parameter value, converted to a string.</param>
    /// <returns>This instance, for chaining.</returns>
    public RestClientQueryParams AddIf(bool condition, string key, object? value)
    {
        if (condition && value != null)
            _params[key] = value.ToString();
        return this;
    }

    /// <summary>
    /// Gets the query parameters as a dictionary of names and values.
    /// </summary>
    public Dictionary<string, string?> ToDictionary() => _params;
}