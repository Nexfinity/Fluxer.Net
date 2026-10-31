using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using Fluxer.Net.RateLimiting;
using Serilog;
using Serilog.Events;
using System.Net;
using System.Net.Http.Headers;
using System.Reflection;

namespace Fluxer.Net.Rest;

public class FluxerBaseApiClient
{
    internal FluxerBaseApiClient() { }

    internal string _token;
    internal FluxerConfig _config;

    /// <summary>
    /// Whether the client authenticates with a bot token. User tokens start with <c>flx_</c>.
    /// </summary>
    public bool IsBot => _token != null && _token.StartsWith("Bot ", StringComparison.OrdinalIgnoreCase);

    private string Authorization(HttpMethod method, string route)
    {
        if (IsBot && _config.EnforceUserOnlyRoutes && UserOnlyRoutes.IsUserOnly(method, route))
            throw new UserTokenRequiredException(method, route);

        return _token;
    }

    /// <summary>
    /// The HTTP client used to make requests. Can be shared or injected for connection pooling.
    /// </summary>
    public HttpClient HttpClient { get; internal set; }

    /// <summary>
    /// Holds requests back to stay inside Fluxer's rate limits and retries the ones answered with 429.
    /// </summary>
    internal RequestQueue RequestQueue { get; private set; }

    /// <summary>
    /// API limits for message length, attachment count and premium limits.
    /// </summary>
    public ApiLimits Limits { get; internal set; }

    internal ILogger _logger;

    public delegate void HttpErrorEvent(FluxerErrorJson error, HttpResponseMessage response);

    public event HttpErrorEvent HttpError;

    internal void Initialize()
    {
        HttpClient = _config.HttpClient ?? new(new HttpClientHandler
        {
            Proxy = _config.Proxy
        });
        RequestQueue = new RequestQueue(_config, _logger);
        _logger.Information("Initialized Fluxer.Net api client ({AssemblyVersion}) (API {ApiVersion})",
            Assembly.GetExecutingAssembly().GetName().Version,
            _config.Version);
    }

    private void SetOptions(HttpRequestMessage req, RequestOptions? options = null)
    {
        if (options == null)
            return;
        if (!string.IsNullOrEmpty(options.AuditLogReason))
            req.Headers.Add("X-Audit-Log-Reason", Uri.EscapeDataString(options.AuditLogReason));
    }

    /// <summary>
    /// Makes an HTTP request with both request and response bodies.
    /// </summary>
    /// <typeparam name="TResponse">The type to deserialize the response into.</typeparam>
    /// <typeparam name="TSend">The type of the request body.</typeparam>
    /// <param name="method">The HTTP method (GET, POST, PATCH, etc.).</param>
    /// <param name="route">The API route (e.g., "/channels/123/messages").</param>
    /// <param name="data">The request body data to serialize and send.</param>
    /// <param name="throwOnNonSuccess">Whether to throw an exception on non-2xx status codes.</param>
    /// <param name="authorize">Whether to include the Authorization header.</param>
    /// <param name="otherFormData"></param>
    /// <param name="options"></param>
    /// <returns>The deserialized response object.</returns>
    /// <exception cref="FluxerApiException">Thrown when <paramref name="throwOnNonSuccess"/> is true and the API returns a non-success status code.</exception>
    public async Task<TResponse> SendRequestAsync<TResponse, TSend>(HttpMethod method, string route, TSend data, bool throwOnNonSuccess = false, bool authorize = true,
        ICollection<KeyValuePair<string, (HttpContent content, string? filename)>>? otherFormData = null, RequestOptions? options = null)
    {
        string rawContent = JsonConvert.SerializeObject(data, FluxerClient._restSerializer);
        if (_logger.IsEnabled(LogEventLevel.Verbose))
            _logger.Verbose("Sending {Enums} to {Route}", LogRedaction.Redact(rawContent), LogRedaction.Redact(route));
        HttpRequestMessage req = new HttpRequestMessage()
        {
            Method = method,
            RequestUri = new(_config.RealApiBaseUrl + route)
        };
        SetOptions(req, options);

        if (otherFormData != null)
        {
            MultipartFormDataContent form = new MultipartFormDataContent
            {
                {
                    new StringContent(rawContent,
#if NET5_0_OR_GREATER
                    new MediaTypeHeaderValue("application/json")
#else
                    System.Text.Encoding.UTF8,
                    "application/json"
#endif
                    ),
                    "payload_json"
                }
            };
            foreach ((string key, (HttpContent content, string filename)) in otherFormData)
            {
                if (key == "payload_json")
                    continue;

                if (string.IsNullOrEmpty(filename?.Trim()))
                {
                    form.Add(content, key);
                }
                else
                {
                    form.Add(content, key, filename);
                }
            }
            req.Content = form;
        }
        else
        {
            req.Content = new StringContent(rawContent,
#if NET5_0_OR_GREATER
                    new MediaTypeHeaderValue("application/json")
#else
                    System.Text.Encoding.UTF8,
                    "application/json"
#endif
                );
        }
        if (!string.IsNullOrEmpty(_token) && authorize)
            req.Headers.Add("Authorization", Authorization(method, route));

        HttpResponseMessage result = await RequestQueue.SendAsync(HttpClient, req, route, options);

        if (_logger.IsEnabled(LogEventLevel.Debug))
            _logger.Debug("Made {Method} request to {Route}", method, LogRedaction.Redact(route));
        string resp = await result.Content.ReadAsStringAsync();
        if (_logger.IsEnabled(LogEventLevel.Verbose))
            _logger.Verbose("Received {Code}:{Result} from {Route}", result.StatusCode, LogRedaction.Redact(resp), LogRedaction.Redact(route));

        if (!result.IsSuccessStatusCode)
        {
            FluxerErrorJson? json = JsonConvert.DeserializeObject<FluxerErrorJson>(resp, FluxerClient._restSerializer);
            HttpError?.Invoke(json, result);

            if (throwOnNonSuccess)
            {
                if (json != null)
                    throw new FluxerApiException($"Fluxer returned an error code {result.StatusCode} with {json.Code} - {json.Reason}", resp, result.StatusCode, json);

                throw new FluxerApiException($"Fluxer returned an error code {result.StatusCode}", resp, result.StatusCode, null);
            }

            return default!;
        }

        return JsonConvert.DeserializeObject<TResponse>(resp, FluxerClient._restSerializer);
    }

    internal async Task<TResponse> InternalSendRequestFormAsync<TResponse>(HttpMethod method, string route, bool throwOnNonSuccess = false,
        Dictionary<string, string?>? formData = null, RequestOptions? options = null)
    {
        HttpRequestMessage req = new HttpRequestMessage()
        {
            Method = method,
            RequestUri = new(_config.RealApiBaseUrl + route)
        };
        SetOptions(req, options);

        if (formData != null)
        {
            MultipartFormDataContent form = new MultipartFormDataContent();
            foreach ((string key, string value) in formData)
            {
                form.Add(new StringContent(value), key);
            }
            req.Content = form;
        }

        HttpResponseMessage result = await RequestQueue.SendAsync(HttpClient, req, route, options);

        if (_logger.IsEnabled(LogEventLevel.Debug))
            _logger.Debug("Made {Method} request to {Route}", method, LogRedaction.Redact(route));
        string resp = await result.Content.ReadAsStringAsync();
        if (_logger.IsEnabled(LogEventLevel.Verbose))
            _logger.Verbose("Received {Code}:{Result} from {Route}", result.StatusCode, LogRedaction.Redact(resp), LogRedaction.Redact(route));

        if (!result.IsSuccessStatusCode)
        {
            FluxerErrorJson? json = JsonConvert.DeserializeObject<FluxerErrorJson>(resp, FluxerClient._restSerializer);
            HttpError?.Invoke(json, result);

            if (throwOnNonSuccess)
            {
                if (json != null)
                    throw new FluxerApiException($"Fluxer returned an error code {result.StatusCode} with {json.Code} - {json.Reason}", resp, result.StatusCode, json);

                throw new FluxerApiException($"Fluxer returned an error code {result.StatusCode}", resp, result.StatusCode, null);
            }

            return default!;
        }

        return JsonConvert.DeserializeObject<TResponse>(resp, FluxerClient._restSerializer);
    }

    /// <summary>
    /// Makes an HTTP request with a request body but no response body (returns status code only).
    /// </summary>
    /// <typeparam name="TSend">The type of the request body.</typeparam>
    /// <param name="method">The HTTP method (POST, PATCH, DELETE, etc.).</param>
    /// <param name="route">The API route (e.g., "/channels/123/typing").</param>
    /// <param name="data">The request body data to serialize and send.</param>
    /// <param name="throwOnNonSuccess">Whether to throw an exception on non-2xx status codes.</param>
    /// <param name="authorize">Whether to include the Authorization header.</param>
    /// <param name="options"></param>
    /// <returns>The HTTP status code of the response.</returns>
    /// <exception cref="FluxerApiException">Thrown when <paramref name="throwOnNonSuccess"/> is true and the API returns a non-success status code.</exception>
    public async Task<HttpStatusCode> SendRequestAsync<TSend>(HttpMethod method, string route, TSend data, bool throwOnNonSuccess = false,
        bool authorize = true, RequestOptions? options = null)
    {
        if (_logger.IsEnabled(LogEventLevel.Verbose))
            _logger.Verbose("Sending {Enums} to {Route}", LogRedaction.Redact(JsonConvert.SerializeObject(data, FluxerClient._restSerializer)), LogRedaction.Redact(route));
        HttpRequestMessage req = new HttpRequestMessage()
        {
            Method = method,
            Content = new StringContent(JsonConvert.SerializeObject(data, FluxerClient._restSerializer),
#if NET5_0_OR_GREATER
            new MediaTypeHeaderValue("application/json")
#else
            System.Text.Encoding.UTF8,
            "application/json"
#endif
            ),
            RequestUri = new(_config.RealApiBaseUrl + route)
        };
        SetOptions(req, options);

        if (!string.IsNullOrEmpty(_token) && authorize)
            req.Headers.Add("Authorization", Authorization(method, route));
        HttpResponseMessage result = await RequestQueue.SendAsync(HttpClient, req, route, options);

        if (_logger.IsEnabled(LogEventLevel.Debug))
            _logger.Debug("Made {Method} request to {Route}", method, LogRedaction.Redact(route));
        string resp = await result.Content.ReadAsStringAsync();
        if (_logger.IsEnabled(LogEventLevel.Verbose))
            _logger.Verbose("Received {Code}:{Result} from {Route}", result.StatusCode, LogRedaction.Redact(resp), LogRedaction.Redact(route));

        if (!result.IsSuccessStatusCode)
        {
            FluxerErrorJson? json = JsonConvert.DeserializeObject<FluxerErrorJson>(resp, FluxerClient._restSerializer);
            HttpError?.Invoke(json, result);

            if (throwOnNonSuccess)
            {
                if (json != null)
                    throw new FluxerApiException($"Fluxer returned an error code {result.StatusCode} with {json.Code} - {json.Reason}", resp, result.StatusCode, json);

                throw new FluxerApiException($"Fluxer returned an error code {result.StatusCode}", resp, result.StatusCode, null);
            }
        }

        return result.StatusCode;
    }

    /// <summary>
    /// Makes an HTTP request with no request body but expects a response body.
    /// </summary>
    /// <typeparam name="TResponse">The type to deserialize the response into.</typeparam>
    /// <param name="method">The HTTP method (typically GET).</param>
    /// <param name="route">The API route (e.g., "/users/@me").</param>
    /// <param name="throwOnNonSuccess">Whether to throw an exception on non-2xx status codes.</param>
    /// <param name="authorize">Whether to include the Authorization header.</param>
    /// <param name="options"></param>
    /// <returns>The deserialized response object.</returns>
    /// <exception cref="FluxerApiException">Thrown when <paramref name="throwOnNonSuccess"/> is true and the API returns a non-success status code.</exception>
    public Task<TResponse> SendRequestAsync<TResponse>(HttpMethod method, string route, bool throwOnNonSuccess = false,
        bool authorize = true, RequestOptions? options = null)
     => InternalSendRequestAsync<TResponse>(method, route, throwOnNonSuccess, authorize, null, options: options);

    internal async Task<TResponse> InternalSendRequestAsync<TResponse>(HttpMethod method, string route, bool throwOnNonSuccess, bool authorize, string accessToken = null, bool useConfigUrl = true, RequestOptions? options = null)
    {
        HttpRequestMessage req = new HttpRequestMessage()
        {
            Method = method,
            RequestUri = useConfigUrl ? new Uri(_config.RealApiBaseUrl + route) : new Uri(route)
        };
        SetOptions(req, options);

        if (!string.IsNullOrEmpty(accessToken))
            req.Headers.Add("Authorization", "Bearer " + accessToken);
        else if (!string.IsNullOrEmpty(_token) && authorize)
            req.Headers.Add("Authorization", Authorization(method, route));

        HttpResponseMessage result = await RequestQueue.SendAsync(HttpClient, req, route, options);

        if (_logger.IsEnabled(LogEventLevel.Debug))
            _logger.Debug("Made {Method} request to {Route}", method, LogRedaction.Redact(route));
        string resp = await result.Content.ReadAsStringAsync();
        if (_logger.IsEnabled(LogEventLevel.Verbose))
            _logger.Verbose("Received {Code}:{Result} from {Route}", result.StatusCode, LogRedaction.Redact(resp), LogRedaction.Redact(route));

        if (!result.IsSuccessStatusCode)
        {
            FluxerErrorJson? json = JsonConvert.DeserializeObject<FluxerErrorJson>(resp, FluxerClient._restSerializer);
            HttpError?.Invoke(json, result);

            if (throwOnNonSuccess)
            {
                if (json != null)
                    throw new FluxerApiException($"Fluxer returned an error code {result.StatusCode} with {json.Code} - {json.Reason}", resp, result.StatusCode, json);

                throw new FluxerApiException($"Fluxer returned an error code {result.StatusCode}", resp, result.StatusCode, null);
            }

            return default!;
        }

        return JsonConvert.DeserializeObject<TResponse>(resp, FluxerClient._restSerializer);
    }

    /// <summary>
    /// Makes an HTTP request with no request body but expects a response body.
    /// </summary>
    /// <typeparam name="TResponse">The type to deserialize the response into.</typeparam>
    /// <param name="method">The HTTP method (typically GET).</param>
    /// <param name="queryParams">The HTTP method query params like limit, after, etc.</param>
    /// <param name="route">The API route (e.g., "/users/@me").</param>
    /// <param name="throwOnNonSuccess">Whether to throw an exception on non-2xx status codes.</param>
    /// <param name="authorize">Whether to include the Authorization header.</param>
    /// <param name="options"></param>
    /// <returns>The deserialized response object.</returns>
    /// <exception cref="FluxerApiException">Thrown when <paramref name="throwOnNonSuccess"/> is true and the API returns a non-success status code.</exception>
    public Task<TResponse> SendRequestQueryParamsAsync<TResponse>(HttpMethod method, RestClientQueryParams queryParams, string route, bool throwOnNonSuccess = false,
        bool authorize = true, RequestOptions? options = null)
        => InternalSendRequestQueryParamsAsync<TResponse>(method, queryParams, route, throwOnNonSuccess, authorize, null, options: options);
    internal async Task<TResponse> InternalSendRequestQueryParamsAsync<TResponse>(
        HttpMethod method,
        RestClientQueryParams? queryParams,
        string route,
        bool throwOnNonSuccess,
        bool authorize,
        string accessToken, RequestOptions? options = null)
    {
        string uri = _config.RealApiBaseUrl + route;

        if (queryParams != null)
        {
            Dictionary<string, string?> query = queryParams.ToDictionary();

            uri = QueryHelpers.AddQueryString(uri, query);
        }

        HttpRequestMessage req = new()
        {
            Method = method,
            RequestUri = new Uri(uri)
        };
        SetOptions(req, options);

        if (!string.IsNullOrEmpty(accessToken))
            req.Headers.Add("Authorization", "Bearer " + accessToken);
        else if (!string.IsNullOrEmpty(_token) && authorize)
            req.Headers.Add("Authorization", Authorization(method, route));

        HttpResponseMessage result = await RequestQueue.SendAsync(HttpClient, req, route, options);

        if (_logger.IsEnabled(LogEventLevel.Debug))
            _logger.Debug("Made {Method} request to {Route}", method, LogRedaction.Redact(route));
        string resp = await result.Content.ReadAsStringAsync();
        if (_logger.IsEnabled(LogEventLevel.Verbose))
            _logger.Verbose("Received {Code}:{Result} from {Route}", result.StatusCode, LogRedaction.Redact(resp), LogRedaction.Redact(route));

        if (!result.IsSuccessStatusCode)
        {
            FluxerErrorJson? json = JsonConvert.DeserializeObject<FluxerErrorJson>(resp, FluxerClient._restSerializer);
            HttpError?.Invoke(json, result);

            if (throwOnNonSuccess)
            {
                if (json != null)
                    throw new FluxerApiException($"Fluxer returned an error code {result.StatusCode} with {json.Code} - {json.Reason}", resp, result.StatusCode, json);

                throw new FluxerApiException($"Fluxer returned an error code {result.StatusCode}", resp, result.StatusCode, null);
            }

            return default!;
        }

        return JsonConvert.DeserializeObject<TResponse>(resp, FluxerClient._restSerializer);
    }

    /// <summary>
    /// Makes an HTTP request with no request or response body (returns status code only).
    /// </summary>
    /// <param name="method">The HTTP method (DELETE, POST, etc.).</param>
    /// <param name="route">The API route (e.g., "/channels/123").</param>
    /// <param name="throwOnNonSuccess">Whether to throw an exception on non-2xx status codes.</param>
    /// <param name="authorize">Whether to include the Authorization header.</param>
    /// <param name="options"></param>
    /// <returns>The HTTP status code of the response.</returns>
    /// <exception cref="FluxerApiException">Thrown when <paramref name="throwOnNonSuccess"/> is true and the API returns a non-success status code.</exception>
    public async Task<HttpStatusCode> SendRequestRawAsync(HttpMethod method, string route, bool throwOnNonSuccess = false,
        bool authorize = true, RequestOptions? options = null)
    {
        HttpRequestMessage req = new HttpRequestMessage()
        {
            Method = method,
            RequestUri = new(_config.RealApiBaseUrl + route)
        };
        SetOptions(req, options);

        if (!string.IsNullOrEmpty(_token) && authorize)
            req.Headers.Add("Authorization", Authorization(method, route));
        HttpResponseMessage result = await RequestQueue.SendAsync(HttpClient, req, route, options);

        if (_logger.IsEnabled(LogEventLevel.Debug))
            _logger.Debug("Made {Method} request to {Route} with response code {Code}", method, LogRedaction.Redact(route), result.StatusCode);
        string resp = await result.Content.ReadAsStringAsync();
        if (_logger.IsEnabled(LogEventLevel.Verbose))
            _logger.Verbose("Received {Code}:{Result} from {Route}", result.StatusCode, LogRedaction.Redact(resp), LogRedaction.Redact(route));

        if (!result.IsSuccessStatusCode)
        {
            FluxerErrorJson? json = JsonConvert.DeserializeObject<FluxerErrorJson>(resp, FluxerClient._restSerializer);
            HttpError?.Invoke(json, result);

            if (throwOnNonSuccess)
            {
                if (json != null)
                    throw new FluxerApiException($"Fluxer returned an error code {result.StatusCode} with {json.Code} - {json.Reason}", resp, result.StatusCode, json);

                throw new FluxerApiException($"Fluxer returned an error code {result.StatusCode}", resp, result.StatusCode, null);
            }
        }

        return result.StatusCode;
    }
}
