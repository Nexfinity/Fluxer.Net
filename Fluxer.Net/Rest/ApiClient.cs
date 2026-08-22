using Fluxer.Net.Extensions;
using Fluxer.Net.Gateway;
using Fluxer.Net.OAuth;
using Fluxer.Net.RateLimiting;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using Serilog;
using System.Net;
using System.Net.Http.Headers;
using System.Reflection;

namespace Fluxer.Net.Rest;

/// <summary>
/// REST API client for the Fluxer platform. Provides methods for all Fluxer API endpoints
/// including authentication, channels, guilds, users, messages, and more.
/// </summary>
/// <remarks>
/// This client handles HTTP requests to the Fluxer API with automatic rate limiting,
/// JSON serialization, and error handling. It supports both synchronous operations via
/// REST and can be paired with <see cref="FluxerGatewayClient"/> for real-time events.
/// </remarks>
public class FluxerApiClient
{
    #region Declares
    private readonly string _token;
    private readonly FluxerConfig _config;
    private readonly FluxerBaseClient _client;
    private readonly bool _isWebhook;

    /// <summary>
    /// The HTTP client used to make requests. Can be shared or injected for connection pooling.
    /// </summary>
    public HttpClient HttpClient { get; set; }

    /// <summary>
    /// Manages client-side rate limiting using sliding window algorithm.
    /// Prevents exceeding Fluxer API rate limits by automatically waiting when necessary.
    /// </summary>
    public RateLimitManager RateLimitManager { get; set; }

    /// <summary>
    /// API limits for message length, attachment count and premium limits.
    /// </summary>
    public ApiLimits Limits { get; set; }

#pragma warning disable CS0169
    private readonly ILogger _logger;
#pragma warning restore CS0169
    #endregion

    #region Meta
    /// <summary>
    /// Initializes a new instance of the <see cref="FluxerApiClient"/> class.
    /// </summary>
    /// <remarks>
    /// The client is automatically configured with:
    /// <list type="bullet">
    /// <item>Rate limiting enabled by default (configurable via <see cref="FluxerConfig.EnableRateLimiting"/>)</item>
    /// <item>Serilog logger for request/response tracking</item>
    /// <item>HTTP client for connection pooling</item>
    /// </list>
    /// </remarks>
    internal FluxerApiClient(FluxerClient client)
    {
        _client = client;
        _token = client.Token;
        _config = client.Config;
        _logger = client.Config.RestSerilog;
        Initialize();
    }

    internal FluxerApiClient(FluxerWebhookClient webhook)
    {
        _isWebhook = true;
        _client = webhook;
        _token = webhook.Token;
        _config = webhook.Config;
        _logger = webhook.Config.RestSerilog;
        Initialize();
    }

    internal FluxerApiClient(FluxerOAuthClient oauth)
    {
        _client = oauth;
        _config = oauth.Config;
        _logger = oauth.Config.RestSerilog;
        //HttpClient.DefaultRequestHeaders.Add("Client-Id", oauth.ClientId.ToString());
        //HttpClient.DefaultRequestHeaders.Add("Client-Secret", oauth.ClientSecret);
        Initialize();
    }

    private void Initialize()
    {
        HttpClient = _config.HttpClient ?? new();
        RateLimitManager = new RateLimitManager(_config.EnableRateLimiting);

        _logger.Information("Initialized Fluxer.Net api client ({AssemblyVersion}) (API {ApiVersion}) with rate limiting {RateLimitEnabled}",
            Assembly.GetExecutingAssembly().GetName().Version,
            _config.Version,
            _config.EnableRateLimiting ? "enabled" : "disabled");
    }

    /// <summary>
    /// Helper method to wait for rate limiting before making a request.
    /// </summary>
    /// <param name="config">The rate limit configuration for this request.</param>
    /// <param name="channelId">Optional channel ID for channel-specific rate limits.</param>
    /// <param name="guildId">Optional guild ID for guild-specific rate limits.</param>
    /// <param name="userId">Optional user ID for user-specific rate limits.</param>
    /// <param name="webhookId">Optional webhook ID for webhook-specific rate limits.</param>
    /// <param name="inviteCode">Optional invite code for invite-specific rate limits.</param>
    private async Task WaitForRateLimitAsync(RateLimitConfig config, ulong? channelId = null, ulong? guildId = null, ulong? userId = null, ulong? webhookId = null, string inviteCode = null)
    {
        if (!_config.EnableRateLimiting)
            return;

        RateLimitBucket bucket = RateLimitManager.GetBucket(config, channelId, guildId, userId, webhookId, inviteCode);
        await RateLimitManager.WaitForRateLimitAsync(bucket);
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
    /// <returns>The deserialized response object.</returns>
    /// <exception cref="FluxerApiException">Thrown when <paramref name="throwOnNonSuccess"/> is true and the API returns a non-success status code.</exception>
    public async Task<TResponse> SendRequestAsync<TResponse, TSend>(HttpMethod method, string route, TSend data, bool throwOnNonSuccess = false, bool authorize = true,
        ICollection<KeyValuePair<string, (HttpContent content, string? filename)>>? otherFormData = null)
    {
        string rawContent = JsonConvert.SerializeObject(data, FluxerClient._restSerializer);
        _logger.Verbose("Sending {@Enums} to {Route}", rawContent, route);
        HttpRequestMessage req = new HttpRequestMessage()
        {
            Method = method,
            RequestUri = new(_config.RealApiBaseUrl + route)
        };


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
                if (key == "payload_json") continue;
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
            req.Headers.Add("Authorization", _token);

        HttpResponseMessage result = await HttpClient.SendAsync(req);

        _logger.Debug("Made {Method} request to {Route}", method, route);
        string resp = await result.Content.ReadAsStringAsync();
        _logger.Verbose("Received {Code}:{Result} from {Route}", result.StatusCode, resp, route);

        if (throwOnNonSuccess && !result.IsSuccessStatusCode)
            throw new FluxerApiException($"Fluxer returned a non-success code {result.StatusCode}", resp);

        return JsonConvert.DeserializeObject<TResponse>(resp);
    }

    internal async Task<TResponse> InternalSendRequestFormAsync<TResponse>(HttpMethod method, string route, bool throwOnNonSuccess = false,
        Dictionary<string, string?>? formData = null)
    {
        HttpRequestMessage req = new HttpRequestMessage()
        {
            Method = method,
            RequestUri = new(_config.RealApiBaseUrl + route)
        };


        if (formData != null)
        {
            MultipartFormDataContent form = new MultipartFormDataContent();
            foreach ((string key, string value) in formData)
            {
                form.Add(new StringContent(value), key);
            }
            req.Content = form;
        }

        HttpResponseMessage result = await HttpClient.SendAsync(req);

        _logger.Debug("Made {Method} request to {Route}", method, route);
        string resp = await result.Content.ReadAsStringAsync();
        _logger.Verbose("Received {Code}:{Result} from {Route}", result.StatusCode, resp, route);

        if (throwOnNonSuccess && !result.IsSuccessStatusCode)
            throw new FluxerApiException($"Fluxer returned a non-success code {result.StatusCode}", resp);

        return JsonConvert.DeserializeObject<TResponse>(resp);
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
    /// <returns>The HTTP status code of the response.</returns>
    /// <exception cref="FluxerApiException">Thrown when <paramref name="throwOnNonSuccess"/> is true and the API returns a non-success status code.</exception>
    public async Task<HttpStatusCode> SendRequestAsync<TSend>(HttpMethod method, string route, TSend data, bool throwOnNonSuccess = false, bool authorize = true)
    {
        _logger.Verbose("Sending {@Enums} to {Route}", data, route);
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
        if (!string.IsNullOrEmpty(_token) && authorize)
            req.Headers.Add("Authorization", _token);
        HttpResponseMessage result = await HttpClient.SendAsync(req);

        _logger.Debug("Made {Method} request to {Route}", method, route);
        string resp = await result.Content.ReadAsStringAsync();
        _logger.Verbose("Received {Code}:{Result} from {Route}", result.StatusCode, resp, route);

        if (throwOnNonSuccess && !result.IsSuccessStatusCode)
            throw new FluxerApiException($"Fluxer returned a non-success code {result.StatusCode}", resp);

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
    /// <returns>The deserialized response object.</returns>
    /// <exception cref="FluxerApiException">Thrown when <paramref name="throwOnNonSuccess"/> is true and the API returns a non-success status code.</exception>
    public Task<TResponse> SendRequestAsync<TResponse>(HttpMethod method, string route, bool throwOnNonSuccess = false, bool authorize = true)
     => InternalSendRequestAsync<TResponse>(method, route, throwOnNonSuccess, authorize, null);

    internal async Task<TResponse> InternalSendRequestAsync<TResponse>(HttpMethod method, string route, bool throwOnNonSuccess, bool authorize, string accessToken = null, bool useConfigUrl = true)
    {
        HttpRequestMessage req = new HttpRequestMessage()
        {
            Method = method,
            RequestUri = useConfigUrl ? new Uri(_config.RealApiBaseUrl + route) : new Uri(route)
        };

        if (!string.IsNullOrEmpty(accessToken))
            req.Headers.Add("Authorization", "Bearer " + accessToken);
        else if (!string.IsNullOrEmpty(_token) && authorize)
            req.Headers.Add("Authorization", _token);

        HttpResponseMessage result = await HttpClient.SendAsync(req);

        _logger.Debug("Made {Method} request to {Route}", method, route);
        string resp = await result.Content.ReadAsStringAsync();
        _logger.Verbose("Received {Code}:{Result} from {Route}", result.StatusCode, resp, route);

        if (throwOnNonSuccess && !result.IsSuccessStatusCode)
            throw new FluxerApiException($"Fluxer returned a non-success code {result.StatusCode}", resp);

        return JsonConvert.DeserializeObject<TResponse>(resp);
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
    /// <returns>The deserialized response object.</returns>
    /// <exception cref="FluxerApiException">Thrown when <paramref name="throwOnNonSuccess"/> is true and the API returns a non-success status code.</exception>
    public Task<TResponse> SendRequestQueryParamsAsync<TResponse>(HttpMethod method, RestClientQueryParams queryParams, string route, bool throwOnNonSuccess = false, bool authorize = true)
        => InternalSendRequestQueryParamsAsync<TResponse>(method, queryParams, route, throwOnNonSuccess, authorize, null);
    internal async Task<TResponse> InternalSendRequestQueryParamsAsync<TResponse>(
        HttpMethod method,
        RestClientQueryParams? queryParams,
        string route,
        bool throwOnNonSuccess,
        bool authorize,
        string accessToken)
    {
        string uri = _config.RealApiBaseUrl + route;

        if (queryParams != null)
        {
            var query = queryParams.ToDictionary();

            uri = QueryHelpers.AddQueryString(uri, query);
        }

        HttpRequestMessage req = new()
        {
            Method = method,
            RequestUri = new Uri(uri)
        };


        if (!string.IsNullOrEmpty(accessToken))
            req.Headers.Add("Authorization", "Bearer " + accessToken);
        else if (!string.IsNullOrEmpty(_token) && authorize)
            req.Headers.Add("Authorization", _token);

        HttpResponseMessage result = await HttpClient.SendAsync(req);

        _logger.Debug("Made {Method} request to {Route}", method, route);
        string resp = await result.Content.ReadAsStringAsync();
        _logger.Verbose("Received {Code}:{Result} from {Route}", result.StatusCode, resp, route);

        if (throwOnNonSuccess && !result.IsSuccessStatusCode)
            throw new FluxerApiException($"Fluxer returned a non-success code {result.StatusCode}", resp);

        return JsonConvert.DeserializeObject<TResponse>(resp);
    }

    /// <summary>
    /// Makes an HTTP request with no request or response body (returns status code only).
    /// </summary>
    /// <param name="method">The HTTP method (DELETE, POST, etc.).</param>
    /// <param name="route">The API route (e.g., "/channels/123").</param>
    /// <param name="throwOnNonSuccess">Whether to throw an exception on non-2xx status codes.</param>
    /// <param name="authorize">Whether to include the Authorization header.</param>
    /// <returns>The HTTP status code of the response.</returns>
    /// <exception cref="FluxerApiException">Thrown when <paramref name="throwOnNonSuccess"/> is true and the API returns a non-success status code.</exception>
    public async Task<HttpStatusCode> SendRequestRawAsync(HttpMethod method, string route, bool throwOnNonSuccess = false, bool authorize = true)
    {
        HttpRequestMessage req = new HttpRequestMessage()
        {
            Method = method,
            RequestUri = new(_config.RealApiBaseUrl + route)
        };
        if (!string.IsNullOrEmpty(_token) && authorize)
            req.Headers.Add("Authorization", _token);
        HttpResponseMessage result = await HttpClient.SendAsync(req);

        _logger.Debug("Made {Method} request to {Route} with response code {Code}", method, route, result.StatusCode);
        if (throwOnNonSuccess && !result.IsSuccessStatusCode)
            throw new FluxerApiException($"Fluxer returned a non-success code {result.StatusCode}", await result.Content.ReadAsStringAsync());

        return result.StatusCode;
    }
    #endregion

    /// <summary>
    /// Get info and config for the Fluxer instance, useful for self-hosted.
    /// </summary>
    /// <returns></returns>
    public async Task<Instance?> GetInstanceAsync()
    {
        InstanceJson? json = await SendRequestAsync<InstanceJson>(HttpMethod.Get, "/.well-known/fluxer", false);
        if (json == null)
            return null;

        return Instance.Create(_client, json);
    }

    #region Expressions API
    /// <summary>
    /// Get a custom emoji.
    /// </summary>
    /// <param name="emojiId"></param>
    /// <returns></returns>
    public async Task<Emoji?> GetEmojiAsync(ulong emojiId)
    {
        EmojiJson? json = await SendRequestAsync<EmojiJson>(HttpMethod.Get, $"/emojis/{emojiId}/metadata", false);
        if (json == null)
            return null;

        return Emoji.Create(_client, json);
    }

    /// <summary>
    /// Get a custom sticker.
    /// </summary>
    /// <param name="stickerId"></param>
    /// <returns></returns>
    public async Task<Sticker?> GetStickerAsync(ulong stickerId)
    {
        StickerJson? json = await SendRequestAsync<StickerJson>(HttpMethod.Get, $"/stickers/{stickerId}/metadata", false);
        if (json == null)
            return null;

        return Sticker.Create(_client, json);
    }

    #endregion

    #region Auth API
    /// <summary>
    /// Login with a user account.
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public async Task<Login> LoginAsync(LoginRequestJson data)
    {
        LoginJson json = await SendRequestAsync<LoginJson, LoginRequestJson>(HttpMethod.Post, "/auth/login", data, false);
        return Login.Create(_client, json);
    }

    /// <summary>
    /// Create a user account
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <param name="data"></param>
    /// <returns></returns>
    public async Task RegisterAsync<TRequest>(TRequest data)
        => await SendRequestAsync<TRequest>(HttpMethod.Post, "/auth/register", data, true, false);

    /// <summary>
    /// Login with a user account that has MFA.
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    /// <param name="data"></param>
    /// <returns></returns>
    public async Task LoginMfaTotpAsync<TRequest, TResponse>(TRequest data)
        => await SendRequestAsync<TResponse, TRequest>(HttpMethod.Post, "/auth/login/mfa/totp", data, true, false);

    /// <summary>
    /// Send a SMS code.
    /// </summary>
    /// <returns></returns>
    public async Task SendMfaSmsCodeAsync()
        => await SendRequestRawAsync(HttpMethod.Post, "/auth/login/mfa/sms/send", true, false);

    /// <summary>
    /// Login with a SMS code.
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    /// <param name="data"></param>
    /// <returns></returns>
    public async Task LoginMfaSmsAsync<TRequest, TResponse>(TRequest data)
        => await SendRequestAsync<TResponse, TRequest>(HttpMethod.Post, "/auth/login/mfa/sms", data, true, false);

    /// <summary>
    /// Logout of the current session/auth.
    /// </summary>
    /// <returns></returns>
    public async Task LogoutAsync()
        => await SendRequestRawAsync(HttpMethod.Post, "/auth/logout", true);

    /// <summary>
    /// Verify the user account email.
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <param name="data"></param>
    /// <returns></returns>
    public async Task VerifyEmailAsync<TRequest>(TRequest data)
        => await SendRequestAsync<TRequest>(HttpMethod.Post, "/auth/verify", data, true, false);

    /// <summary>
    /// Send an email verification for the user account.
    /// </summary>
    /// <returns></returns>
    public async Task ResendVerificationEmailAsync()
        => await SendRequestRawAsync(HttpMethod.Post, "/auth/verify/resend", true);

    /// <summary>
    /// Request a password reset for the user account.
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <param name="data"></param>
    /// <returns></returns>
    public async Task ForgotPasswordAsync<TRequest>(TRequest data)
        => await SendRequestAsync<TRequest>(HttpMethod.Post, "/auth/forgot", data, true, false);

    /// <summary>
    /// Reset the password for the user account.
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <param name="data"></param>
    /// <returns></returns>
    public async Task ResetPasswordAsync<TRequest>(TRequest data)
        => await SendRequestAsync<TRequest>(HttpMethod.Post, "/auth/reset", data, true, false);

    /// <summary>
    /// Get all sessions for the user account.
    /// </summary>
    /// <param name="queryParams"></param>
    /// <returns></returns>
    public async Task<IEnumerable<AuthSession>> GetSessionsAsync(RestClientQueryParams? queryParams)
    {
        IEnumerable<AuthSessionJson> json = await SendRequestAsync<IEnumerable<AuthSessionJson>>(HttpMethod.Get, "/auth/sessions", true);
        return json.Select(x => AuthSession.Create(_client, x));
    }

    /// <summary>
    /// Logout a specific session for the user account.
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <param name="data"></param>
    /// <returns></returns>
    public async Task LogoutSessionsAsync<TRequest>(TRequest data)
        => await SendRequestAsync<TRequest>(HttpMethod.Post, "/auth/sessions/logout", data, true);

    /// <summary>
    /// Authorize and IP for the user account.
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <param name="data"></param>
    /// <returns></returns>
    public async Task PostAuthAuthorizeIpAsync<TRequest>(TRequest data)
        => await SendRequestAsync<TRequest>(HttpMethod.Post, "/auth/authorize-ip", data, true);

    /// <summary>
    /// Setup webauth for the user account.
    /// </summary>
    /// <typeparam name="TResponse"></typeparam>
    /// <returns></returns>
    public async Task<TResponse> PostAuthWebauthnAuthenticationOptionsAsync<TResponse>()
        => await SendRequestAsync<TResponse>(HttpMethod.Post, "/auth/webauthn/authentication-options", true, false);

    /// <summary>
    /// Setup webauth for the user account.
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    /// <param name="data"></param>
    /// <returns></returns>
    public async Task<TResponse> PostAuthWebauthnAuthenticateAsync<TRequest, TResponse>(TRequest data)
        => await SendRequestAsync<TResponse, TRequest>(HttpMethod.Post, "/auth/webauthn/authenticate", data, true, false);

    /// <summary>
    /// Setup webauth for the user account.
    /// </summary>
    /// <typeparam name="TResponse"></typeparam>
    /// <returns></returns>
    public async Task<TResponse> PostAuthLoginMfaWebauthnAuthenticationOptionsAsync<TResponse>()
        => await SendRequestAsync<TResponse>(HttpMethod.Post, "/auth/login/mfa/webauthn/authentication-options", true, false);

    /// <summary>
    /// Setup webauth for the user account.
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    /// <param name="data"></param>
    /// <returns></returns>
    public async Task<TResponse> PostAuthLoginMfaWebauthnAsync<TRequest, TResponse>(TRequest data)
        => await SendRequestAsync<TResponse, TRequest>(HttpMethod.Post, "/auth/login/mfa/webauthn", data, true, false);

    #endregion

    #region Channels API
    /// <summary>
    /// Create a DM or Group channel.
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    public async Task<Channel> CreatePrivateChannelAsync(CreatePrivateChannelRequest req)
    {
        ChannelJson json = await SendRequestAsync<ChannelJson, CreatePrivateChannelRequest>(HttpMethod.Post, $"/users/@me/channels", req, true);
        return Channel.Create(_client, json);
    }

    /// <summary>
    /// Get a channel.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="ChannelPermissions.ViewChannel"/> for guild channels.
    /// </remarks>
    /// <param name="channelId"></param>
    /// <returns></returns>
    public async Task<Channel> GetChannelAsync(ulong channelId)
    {
        ChannelJson json = await SendRequestAsync<ChannelJson>(HttpMethod.Get, $"/channels/{channelId}", true);
        return Channel.Create(_client, json);
    }

    /// <summary>
    /// Get voice regions for a channel.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="ChannelPermissions.ViewChannel"/> for guild channels.
    /// </remarks>
    /// <param name="channelId"></param>
    /// <returns></returns>
    public async Task<IEnumerable<RtcRegion>> GetChannelRtcRegionsAsync(ulong channelId)
    {
        IEnumerable<RtcRegionJson> json = await SendRequestAsync<IEnumerable<RtcRegionJson>>(HttpMethod.Get, $"/channels/{channelId}/rtc-regions", true);
        return json.Select(x => RtcRegion.Create(_client, x));
    }

    /// <summary>
    /// Modify a channel.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="ChannelPermissions.ManageChannels"/> for guild channels.
    /// </remarks>
    /// <param name="channelId"></param>
    /// <param name="channel"></param>
    /// <returns></returns>
    public async Task<Channel> UpdateChannelAsync(ulong channelId, ChannelJson channel)
    {
        ChannelJson json = await SendRequestAsync<ChannelJson, ChannelJson>(HttpMethod.Patch, $"/channels/{channelId}", channel, true);
        return Channel.Create(_client, json);
    }

    /// <summary>
    /// Delete a channel.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="ChannelPermissions.ManageChannels"/> for guild channels.
    /// </remarks>
    /// <param name="channelId"></param>
    /// <returns></returns>
    public async Task DeleteChannelAsync(ulong channelId)
        => await SendRequestRawAsync(HttpMethod.Delete, $"/channels/{channelId}", true);

    /// <summary>
    /// Clear message unreads for a channel.
    /// </summary>
    /// <param name="channelId"></param>
    /// <returns></returns>
    public async Task ClearMessageAcknowledgementAsync(ulong channelId)
        => await SendRequestRawAsync(HttpMethod.Delete, $"/channels/{channelId}/messages/ack", true);

    /// <summary>
    /// Get messages in a channel.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="ChannelPermissions.ReadMessageHistory"/> for guild channels.
    /// </remarks>
    /// <param name="channelId"></param>
    /// <param name="limit"></param>
    /// <param name="beforeId"></param>
    /// <param name="afterId"></param>
    /// <param name="aroundId"></param>
    /// <param name="queryParams"></param>
    /// <returns></returns>
    public async Task<IEnumerable<Message>> GetMessagesAsync(ulong channelId, int limit = 100, ulong? beforeId = null, ulong? afterId = null, ulong? aroundId = null, RestClientQueryParams? queryParams = null)
    {
        queryParams ??= new RestClientQueryParams()
            .Add(QueryParams.Limit, limit)
            .AddIf(beforeId != null, QueryParams.Before, beforeId)
            .AddIf(afterId != null, QueryParams.After, afterId)
            .AddIf(aroundId != null, QueryParams.Around, aroundId);

        IEnumerable<MessageJson> json = await SendRequestQueryParamsAsync<IEnumerable<MessageJson>>(HttpMethod.Get, queryParams, $"/channels/{channelId}/messages", true);
        return json.Select(x => Message.Create(_client, x));
    }

    /// <summary>
    /// Get a message in a channel.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="ChannelPermissions.ViewChannel"/> and <see cref="ChannelPermissions.ReadMessageHistory"/> for guild channels.
    /// </remarks>
    /// <param name="channelId"></param>
    /// <param name="messageId"></param>
    /// <returns></returns>
    public async Task<Message> GetMessageAsync(ulong channelId, ulong messageId)
    {
        MessageJson json = await SendRequestAsync<MessageJson>(HttpMethod.Get, $"/channels/{channelId}/messages/{messageId}", true);
        return Message.Create(_client, json);
    }

    /// <summary>
    /// Search messages in a channel.
    /// </summary>
    /// <remarks>
    /// Not implemented or tested properly.
    /// </remarks>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    /// <param name="channelId"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public async Task<TResponse> SearchChannelAsync<TRequest, TResponse>(ulong channelId, TRequest data)
        => await SendRequestAsync<TResponse, TRequest>(HttpMethod.Post, $"/channels/{channelId}/search", data, true);

    /// <summary>
    /// Send a message in the channel.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="ChannelPermissions.ViewChannel"/> and <see cref="ChannelPermissions.SendMessages"/> for guild channels.
    /// </remarks>
    /// <param name="channelId"></param>
    /// <param name="content"></param>
    /// <param name="embeds"></param>
    /// <param name="reference"></param>
    /// <param name="allowedMentions"></param>
    /// <param name="flags"></param>
    /// <param name="nonce"></param>
    /// <param name="favoruteMemeId"></param>
    /// <param name="tts"></param>
    /// <param name="stickerIds"></param>
    /// <param name="attachments"></param>
    /// <returns></returns>
    public async Task<Message> SendMessageAsync(ulong channelId, string? content = null, List<EmbedRequest>? embeds = null,
        MessageReferenceRequest? reference = null, AllowedMentionsRequest? allowedMentions = null, MessageFlag flags = MessageFlag.None,
        string? nonce = null, ulong? favoruteMemeId = null, bool? tts = null, List<ulong>? stickerIds = null, List<AttachmentRequest>? attachments = null)
    {
        MessageRequest req = new MessageRequest
        {
            Content = content,
            Embeds = embeds?.ToArray(),
            MessageReference = reference,
            AllowedMentions = allowedMentions,
            Flags = flags,
            Nonce = nonce,
            FavoriteMemeId = favoruteMemeId,
            IsTTS = tts,
            StickerIds = stickerIds,
        };

        if ((attachments?.Count ?? 0) > 0)
        {
            List<KeyValuePair<string, (HttpContent content, string? filename)>> form = new List<KeyValuePair<string, (HttpContent content, string? filename)>>();
            for (int i = 0; i < attachments.Count; i++)
            {
                attachments[i].Id = (ulong)i;
                form.Add(new KeyValuePair<string, (HttpContent content, string? filename)>($"file[{i}]", (new StreamContent(attachments[i].Stream), attachments[i].Filename)));
            }
            req.Attachments = attachments.Select(x => x.ToJson()).ToList();
        }

        MessageJson json = await SendRequestAsync<MessageJson, MessageRequest>(HttpMethod.Post,
            _isWebhook ? $"/webhooks/{channelId}/{_token}?wait=true" : $"/channels/{channelId}/messages",
            req, true);
        return Message.Create(_client, json);
    }

    /// <summary>
    /// Modify your message in a channel
    /// </summary>
    /// <remarks>
    /// Requires <see cref="ChannelPermissions.ViewChannel"/> and <see cref="ChannelPermissions.ReadMessageHistory"/> in a guild channel.
    /// </remarks>
    /// <param name="channelId"></param>
    /// <param name="messageId"></param>
    /// <param name="content"></param>
    /// <param name="embeds"></param>
    /// <param name="reference"></param>
    /// <param name="allowedMentions"></param>
    /// <param name="flags"></param>
    /// <param name="nonce"></param>
    /// <param name="favoruteMemeId"></param>
    /// <param name="stickerIds"></param>
    /// <param name="attachments"></param>
    /// <returns></returns>
    public async Task<Message> EditMessageAsync(ulong channelId, ulong messageId, string? content = null, List<EmbedRequest>? embeds = null,
        MessageReferenceRequest? reference = null, AllowedMentionsRequest? allowedMentions = null, MessageFlag flags = MessageFlag.None,
        string? nonce = null, ulong? favoruteMemeId = null, List<ulong>? stickerIds = null, List<AttachmentRequest>? attachments = null)
    {
        MessageRequest req = new MessageRequest
        {
            Content = content,
            Embeds = embeds?.ToArray(),
            MessageReference = reference,
            AllowedMentions = allowedMentions,
            Flags = flags,
            Nonce = nonce,
            FavoriteMemeId = favoruteMemeId,
            StickerIds = stickerIds,
        };

        if ((attachments?.Count ?? 0) > 0)
        {
            List<KeyValuePair<string, (HttpContent content, string? filename)>> form = new List<KeyValuePair<string, (HttpContent content, string? filename)>>();
            for (int i = 0; i < attachments.Count; i++)
            {
                attachments[i].Id = (ulong)i;
                form.Add(new KeyValuePair<string, (HttpContent content, string? filename)>($"file[{i}]", (new StreamContent(attachments[i].Stream), attachments[i].Filename)));
            }
            req.Attachments = attachments.Select(x => x.ToJson()).ToList();
        }

        MessageJson json = await SendRequestAsync<MessageJson, MessageRequest>(HttpMethod.Patch, $"/channels/{channelId}/messages/{messageId}", req, true);
        return Message.Create(_client, json);
    }

    /// <summary>
    /// Delete a message in a channel.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="ChannelPermissions.ViewChannel"/> and <see cref="ChannelPermissions.ReadMessageHistory"/> in a guild channel.
    /// <br />
    /// <see cref="ChannelPermissions.ManageMessages"/> for other user message.
    /// </remarks>
    /// <param name="channelId"></param>
    /// <param name="messageId"></param>
    /// <returns></returns>
    public async Task DeleteMessageAsync(ulong channelId, ulong messageId)
        => await SendRequestRawAsync(HttpMethod.Delete, $"/channels/{channelId}/messages/{messageId}", true);

    /// <summary>
    /// Delete a attachment on a message.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="ChannelPermissions.ViewChannel"/> and <see cref="ChannelPermissions.ReadMessageHistory"/> in a guild channel.
    /// </remarks>
    /// <param name="channelId"></param>
    /// <param name="messageId"></param>
    /// <param name="attachmentId"></param>
    /// <returns></returns>
    public async Task DeleteMessageAttachmentAsync(ulong channelId, ulong messageId, ulong attachmentId)
        => await SendRequestRawAsync(HttpMethod.Delete, $"/channels/{channelId}/messages/{messageId}/attachments/{attachmentId}", true);

    /// <summary>
    /// Delete many messages in a channel.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="ChannelPermissions.ViewChannel"/>, <see cref="ChannelPermissions.ReadMessageHistory"/> and <see cref="ChannelPermissions.ManageMessages"/> in a guild channel.
    /// </remarks>
    /// <param name="channelId"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public async Task BulkDeleteMessagesAsync(ulong channelId, BulkDeleteMessagesRequest data)
        => await SendRequestAsync(HttpMethod.Post, $"/channels/{channelId}/messages/bulk-delete", data, true);

    /// <summary>
    /// Send a user typing status for the channel.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="ChannelPermissions.ViewChannel"/> in a guild channel.
    /// </remarks>
    /// <param name="channelId"></param>
    /// <returns></returns>
    public async Task TriggerTypingIndicatorAsync(ulong channelId)
        => await SendRequestRawAsync(HttpMethod.Post, $"/channels/{channelId}/typing", true);

    /// <summary>
    /// Acknowledge a message has been read for the current user.
    /// </summary>
    /// <remarks>
    /// User accounts only.
    /// </remarks>
    /// <param name="channelId"></param>
    /// <param name="messageId"></param>
    /// <param name="details"></param>
    /// <returns></returns>
    public async Task AcknowledgeMessageAsync(ulong channelId, ulong messageId, MessageAckJson details)
        => await SendRequestAsync<MessageAckJson>(HttpMethod.Post, $"/channels/{channelId}/messages/{messageId}/ack", details, true);

    /// <summary>
    /// Get pinned messages in a channel.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="ChannelPermissions.ViewChannel"/> and <see cref="ChannelPermissions.ReadMessageHistory"/> in a guild channel.
    /// </remarks>
    /// <param name="channelId"></param>
    /// <param name="query"></param>
    /// <returns></returns>
    public async Task<ChannelPins> GetPinnedMessagesAsync(ulong channelId, ChannelPinsQuery? query = null)
    {
        ChannelPinsJson json = await SendRequestAsync<ChannelPinsJson>(HttpMethod.Get, $"/channels/{channelId}/pins?{query?.BuildQuery() ?? string.Empty}", true);
        return ChannelPins.Create(_client, json);
    }

    /// <summary>
    /// Pin a message in a channel.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="ChannelPermissions.ViewChannel"/>, <see cref="ChannelPermissions.ReadMessageHistory"/> and <see cref="ChannelPermissions.PinMessages"/> in a guild channel.
    /// </remarks>
    /// <param name="channelId"></param>
    /// <param name="messageId"></param>
    /// <returns></returns>
    public async Task PinMessageAsync(ulong channelId, ulong messageId)
        => await SendRequestRawAsync(HttpMethod.Put, $"/channels/{channelId}/pins/{messageId}", true);

    /// <summary>
    /// Remove a pinned message in a channel.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="ChannelPermissions.ViewChannel"/>, <see cref="ChannelPermissions.ReadMessageHistory"/> and <see cref="ChannelPermissions.PinMessages"/> in a guild channel.
    /// </remarks>
    /// <param name="channelId"></param>
    /// <param name="messageId"></param>
    /// <returns></returns>
    public async Task UnpinMessageAsync(ulong channelId, ulong messageId)
        => await SendRequestRawAsync(HttpMethod.Delete, $"/channels/{channelId}/pins/{messageId}", true);

    /// <summary>
    /// Get reaction users for a certain emoji on a message.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="ChannelPermissions.ViewChannel"/> and <see cref="ChannelPermissions.ReadMessageHistory"/> in a guild channel.
    /// </remarks>
    /// <param name="channelId"></param>
    /// <param name="messageId"></param>
    /// <param name="emoji"></param>
    /// <returns></returns>
    public async Task<IEnumerable<User>> GetReactionsForEmojiAsync(ulong channelId, ulong messageId, string emoji)
    {
        IEnumerable<UserJson> json = await SendRequestAsync<IEnumerable<UserJson>>(HttpMethod.Get, $"/channels/{channelId}/messages/{messageId}/reactions/{emoji}", true);
        return json.Select(x => User.Create(_client, x));
    }

    /// <summary>
    /// Add a reaction to a message.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="ChannelPermissions.ViewChannel"/> and <see cref="ChannelPermissions.AddReactions"/> in a guild channel.
    /// </remarks>
    /// <param name="channelId"></param>
    /// <param name="messageId"></param>
    /// <param name="emoji"></param>
    /// <returns></returns>
    public async Task AddReactionAsync(ulong channelId, ulong messageId, string emoji)
        => await SendRequestRawAsync(HttpMethod.Put, $"/channels/{channelId}/messages/{messageId}/reactions/{emoji}/@me", true);

    /// <summary>
    /// Remove a reaction on a message.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="ChannelPermissions.ViewChannel"/> in a guild channel.
    /// </remarks>
    /// <param name="channelId"></param>
    /// <param name="messageId"></param>
    /// <param name="emoji"></param>
    /// <returns></returns>
    public async Task RemoveOwnReactionAsync(ulong channelId, ulong messageId, string emoji)
        => await SendRequestRawAsync(HttpMethod.Delete, $"/channels/{channelId}/messages/{messageId}/reactions/{emoji}/@me", true);

    /// <summary>
    /// Remove a user's reaction on a message.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="ChannelPermissions.ViewChannel"/>, <see cref="ChannelPermissions.ReadMessageHistory"/> and <see cref="ChannelPermissions.ManageMessages"/> in a guild channel.
    /// </remarks>
    /// <param name="channelId"></param>
    /// <param name="messageId"></param>
    /// <param name="emoji"></param>
    /// <param name="targetId"></param>
    /// <returns></returns>
    public async Task RemoveUserReactionAsync(ulong channelId, ulong messageId, string emoji, ulong targetId)
        => await SendRequestRawAsync(HttpMethod.Delete, $"/channels/{channelId}/messages/{messageId}/reactions/{emoji}/{targetId}", true);

    /// <summary>
    /// Remove all reactions for a specific emoji on a message.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="ChannelPermissions.ViewChannel"/>, <see cref="ChannelPermissions.ReadMessageHistory"/> and <see cref="ChannelPermissions.ManageMessages"/> in a guild channel.
    /// </remarks>
    /// <param name="channelId"></param>
    /// <param name="messageId"></param>
    /// <param name="emoji"></param>
    /// <returns></returns>
    public async Task RemoveAllReactionsForEmojiAsync(ulong channelId, ulong messageId, string emoji)
        => await SendRequestRawAsync(HttpMethod.Delete, $"/channels/{channelId}/messages/{messageId}/reactions/{emoji}", true);

    /// <summary>
    /// Remove all reactions on a message.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="ChannelPermissions.ViewChannel"/>, <see cref="ChannelPermissions.ReadMessageHistory"/> and <see cref="ChannelPermissions.ManageMessages"/> in a guild channel.
    /// </remarks>
    /// <param name="channelId"></param>
    /// <param name="messageId"></param>
    /// <returns></returns>
    public async Task RemoveAllReactionsAsync(ulong channelId, ulong messageId)
        => await SendRequestRawAsync(HttpMethod.Delete, $"/channels/{channelId}/messages/{messageId}/reactions", true);

    /// <summary>
    /// Upload multiple attachments to a channel.
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    /// <param name="channelId"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public async Task<TResponse> UploadAttachmentsAsync<TRequest, TResponse>(ulong channelId, TRequest data)
        => await SendRequestAsync<TResponse, TRequest>(HttpMethod.Post, $"/channels/{channelId}/attachments", data, true);

    /// <summary>
    /// Add a user to a group channel.
    /// </summary>
    /// <remarks>
    /// User account only.
    /// </remarks>
    /// <param name="channelId"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task AddRecipientAsync(ulong channelId, ulong userId)
        => await SendRequestRawAsync(HttpMethod.Put, $"/channels/{channelId}/recipients/{userId}", true);

    /// <summary>
    /// Remove a user from the group channel.
    /// </summary>
    /// <remarks>
    /// User account only.
    /// </remarks>
    /// <param name="channelId"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task RemoveRecipientAsync(ulong channelId, ulong userId)
        => await SendRequestRawAsync(HttpMethod.Delete, $"/channels/{channelId}/recipients/{userId}", true);

    /// <summary>
    /// Get if you can call a voice channel/group/dm.
    /// </summary>
    /// <param name="channelId"></param>
    /// <returns></returns>
    public async Task<CallEligibility> GetVoiceEligibilityAsync(ulong channelId)
    {
        CallEligibilityJson json = await SendRequestAsync<CallEligibilityJson>(HttpMethod.Get, $"/channels/{channelId}/call", true);
        return CallEligibility.Create(_client, json);
    }

    /// <summary>
    /// Update the voice region for a voice channel.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="ChannelPermissions.ViewChannel"/> and <see cref="ChannelPermissions.UpdateRtcRegion"/> in a guild channel.
    /// </remarks>
    /// <param name="channelId"></param>
    /// <param name="region"></param>
    /// <returns></returns>
    public async Task UpdateVoiceRegionAsync(ulong channelId, string? region)
        => await SendRequestAsync(HttpMethod.Patch, $"/channels/{channelId}/call", new UpdateVoiceRegionRequest
        {
            Region = region
        }, true);

    /// <summary>
    /// Voice call users.
    /// </summary>
    /// <remarks>
    /// User account only.
    /// </remarks>
    /// <param name="channelId"></param>
    /// <param name="recipients"></param>
    /// <returns></returns>
    public async Task RingCallAsync(ulong channelId, ulong[] recipients)
        => await SendRequestAsync(HttpMethod.Post, $"/channels/{channelId}/call/ring", new VoiceRingRequest
        {
            Recipients = recipients
        }, true);

    /// <summary>
    /// Stop voice calling user.
    /// </summary>
    /// <remarks>
    /// User account only.
    /// </remarks>
    /// <param name="channelId"></param>
    /// <returns></returns>
    public async Task StopRingingAsync(ulong channelId)
        => await SendRequestRawAsync(HttpMethod.Post, $"/channels/{channelId}/call/stop-ringing", true);

    /// <summary>
    /// Get invites for a guild channel.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="ChannelPermissions.ViewChannel"/> and <see cref="ChannelPermissions.ManageChannels"/>.
    /// </remarks>
    /// <param name="channelId"></param>
    /// <returns></returns>
    public async Task<IEnumerable<Invite>> GetChannelInvitesAsync(ulong channelId)
    {
        IEnumerable<InviteJson> json = await SendRequestAsync<IEnumerable<InviteJson>>(HttpMethod.Get, $"/channels/{channelId}/invites", true);
        return json.Select(x => Invite.Create(_client, x));
    }

    /// <summary>
    /// Create invite for the guild channel.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="ChannelPermissions.ViewChannel"/> and <see cref="ChannelPermissions.CreateInstantInvite"/>.
    /// </remarks>
    /// <param name="channelId"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public async Task<Invite> CreateInviteAsync(ulong channelId, CreateInviteRequest data)
    {
        InviteJson json = await SendRequestAsync<InviteJson, CreateInviteRequest>(HttpMethod.Post, $"/channels/{channelId}/invites", data, true);
        return Invite.Create(_client, json);
    }

    /// <summary>
    /// Get webhooks for a guild channel.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="ChannelPermissions.ViewChannel"/> and <see cref="ChannelPermissions.ManageWebhooks"/>.
    /// </remarks>
    /// <param name="channelId"></param>
    /// <returns></returns>
    public async Task<IEnumerable<Webhook>> GetChannelWebhooksAsync(ulong channelId)
    {
        IEnumerable<WebhookJson> json = await SendRequestAsync<IEnumerable<WebhookJson>>(HttpMethod.Get, $"/channels/{channelId}/webhooks", true);
        return json.Select(x => Webhook.Create(_client, x));
    }

    /// <summary>
    /// Create a webhook for the guild channel.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="ChannelPermissions.ViewChannel"/> and <see cref="ChannelPermissions.ManageWebhooks"/>.
    /// </remarks>
    /// <param name="channelId"></param>
    /// <param name="name"></param>
    /// <param name="avatar"></param>
    /// <returns></returns>
    public async Task<Webhook> CreateWebhookAsync(ulong channelId, string name, string? avatar = null)
    {
        WebhookJson json = await SendRequestAsync<WebhookJson, CreateWebhookRequest>(HttpMethod.Post, $"/channels/{channelId}/webhooks", new CreateWebhookRequest
        {
            Name = name,
            Avatar = avatar
        }, true);
        return Webhook.Create(_client, json);
    }

    #endregion

    #region Attachments API
    /// <summary>
    /// Delete an attachment.
    /// </summary>
    /// <param name="uploadFilename"></param>
    /// <returns></returns>
    public async Task DeleteAttachmentAsync(string uploadFilename)
        => await SendRequestRawAsync(HttpMethod.Delete, $"/attachments/{uploadFilename}", true);

    #endregion

    #region Favorite Gifs API
    /// <summary>
    /// Get all your favorite gifs.
    /// </summary>
    /// <returns></returns>
    public async Task<IEnumerable<FavoriteGif>> GetCurrentUserFavoriteGifsAsync()
    {
        IEnumerable<FavoriteGifJson> json = await SendRequestAsync<IEnumerable<FavoriteGifJson>>(HttpMethod.Get, "/users/@me/memes", true);
        return json.Select(x => FavoriteGif.Create(_client, x));
    }

    /// <summary>
    /// Add a favorite gif.
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <param name="data"></param>
    /// <returns></returns>
    public async Task<FavoriteGif> PostCurrentUserFavoriteGifAsync<TRequest>(TRequest data)
    {
        FavoriteGifJson json = await SendRequestAsync<FavoriteGifJson, TRequest>(HttpMethod.Post, "/users/@me/memes", data, true);
        return FavoriteGif.Create(_client, json);
    }

    /// <summary>
    /// Get your favorite gif.
    /// </summary>
    /// <param name="memeId"></param>
    /// <returns></returns>
    public async Task<FavoriteGif> GetCurrentUserFavoriteGifAsync(ulong memeId)
    {
        FavoriteGifJson json = await SendRequestAsync<FavoriteGifJson>(HttpMethod.Get, $"/users/@me/memes/{memeId}", true);
        return FavoriteGif.Create(_client, json);
    }

    /// <summary>
    /// Update your favorite gif.
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <param name="memeId"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public async Task<FavoriteGif> PatchCurrentUserFavoriteGifAsync<TRequest>(ulong memeId, TRequest data)
    {
        FavoriteGifJson json = await SendRequestAsync<FavoriteGifJson, TRequest>(HttpMethod.Patch, $"/users/@me/memes/{memeId}", data, true);
        return FavoriteGif.Create(_client, json);
    }

    /// <summary>
    /// Delete your favorite gif.
    /// </summary>
    /// <param name="memeId"></param>
    /// <returns></returns>
    public async Task DeleteCurrentUserFavoriteGifAsync(ulong memeId)
        => await SendRequestRawAsync(HttpMethod.Delete, $"/users/@me/memes/{memeId}", true);

    #endregion

    #region Invites API
    /// <summary>
    /// Get invite info from invite code.
    /// </summary>
    /// <param name="inviteCode"></param>
    /// <returns></returns>
    public async Task<PartialInvite> GetInviteAsync(string inviteCode)
    {
        PartialInviteJson json = await SendRequestAsync<PartialInviteJson>(HttpMethod.Get, $"/invites/{inviteCode}", true);
        return PartialInvite.Create(_client, json);
    }

    /// <summary>
    /// Join a guild by invite code.
    /// </summary>
    /// <remarks>
    /// User account only.
    /// </remarks>
    /// <param name="inviteCode"></param>
    /// <returns></returns>
    public async Task<PartialInvite> JoinGuildAsync(string inviteCode)
    {
        PartialInviteJson json = await SendRequestAsync<PartialInviteJson>(HttpMethod.Post, $"/invites/{inviteCode}", true);
        return PartialInvite.Create(_client, json);
    }

    /// <summary>
    /// Delete an invite.
    /// </summary>
    /// Requires <see cref="ChannelPermissions.ViewChannel"/> and <see cref="ChannelPermissions.ManageChannels"/>.
    /// <param name="inviteCode"></param>
    /// <returns></returns>
    public async Task DeleteInviteAsync(string inviteCode)
        => await SendRequestRawAsync(HttpMethod.Delete, $"/invites/{inviteCode}", true);

    #endregion

    #region Read States API
    /// <summary>
    /// Clear read message stats for guilds or channels.
    /// </summary>
    /// <remarks>
    /// User account only.
    /// </remarks>
    /// <typeparam name="TRequest"></typeparam>
    /// <param name="data"></param>
    /// <returns></returns>
    public async Task PostReadStatesAckBulkAsync<TRequest>(TRequest data)
        => await SendRequestAsync<TRequest>(HttpMethod.Post, "/read-states/ack-bulk", data, true);

    #endregion

    #region Guilds API
    /// <summary>
    /// Create a guild.
    /// </summary>
    /// <remarks>
    /// User account only.
    /// </remarks>
    /// <param name="data"></param>
    /// <returns></returns>
    public async Task<Guild> CreateGuildAsync(CreateGuildRequest data)
    {
        GuildJson json = await SendRequestAsync<GuildJson, CreateGuildRequest>(HttpMethod.Post, "/guilds", data, true);
        return Guild.Create(_client, json);
    }

    /// <summary>
    /// Get all guilds for the current user.
    /// </summary>
    /// <returns></returns>
    public async Task<IEnumerable<Guild>> GetCurrentUserGuildsAsync()
    {
        IEnumerable<GuildJson> json = await SendRequestAsync<IEnumerable<GuildJson>>(HttpMethod.Get, "/users/@me/guilds", true);
        return json.Select(x => Guild.Create(_client, x));
    }

    /// <summary>
    /// Leave a guild for the current user.
    /// </summary>
    /// <param name="guildId"></param>
    /// <returns></returns>
    public async Task LeaveGuildAsync(ulong guildId)
        => await SendRequestRawAsync(HttpMethod.Delete, $"/users/@me/guilds/{guildId}", true);

    /// <summary>
    /// Get a guild that the current user is in.
    /// </summary>
    /// <param name="guildId"></param>
    /// <returns></returns>
    public async Task<Guild> GetGuildAsync(ulong guildId)
    {
        GuildJson json = await SendRequestAsync<GuildJson>(HttpMethod.Get, $"/guilds/{guildId}", true);
        return Guild.Create(_client, json);
    }

    /// <summary>
    /// Update a guild that you manage.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="GuildPermissions.ManageGuild"/>.
    /// </remarks>
    /// <param name="guildId"></param>
    /// <param name="guild"></param>
    /// <returns></returns>
    public async Task<Guild> UpdateGuildAsync(ulong guildId, GuildJson guild)
    {
        GuildJson json = await SendRequestAsync<GuildJson, GuildJson>(HttpMethod.Patch, $"/guilds/{guildId}", guild, true);
        return Guild.Create(_client, json);
    }

    /// <summary>
    /// Delete a guild that you own.
    /// </summary>
    /// <param name="guildId"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public async Task DeleteGuildAsync(ulong guildId, DeleteGuildRequest data)
        => await SendRequestAsync(HttpMethod.Post, $"/guilds/{guildId}/delete", data, true);

    /// <summary>
    /// Get vanity url for a guild.
    /// </summary>
    /// <param name="guildId"></param>
    /// <returns></returns>
    public async Task<GuildVanityUrl> GetGuildVanityUrlAsync(ulong guildId)
    {
        GuildVanityUrlJson json = await SendRequestAsync<GuildVanityUrlJson>(HttpMethod.Get, $"/guilds/{guildId}/vanity-url", true);
        return GuildVanityUrl.Create(_client, json);
    }

    /// <summary>
    /// Update vanity url for a guild.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="GuildPermissions.ManageGuild"/>.
    /// </remarks>
    /// <param name="guildId"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public async Task UpdateGuildVanityUrlAsync(ulong guildId, UpdateGuildVanityUrlRequest data)
    {
        await SendRequestAsync(HttpMethod.Patch, $"/guilds/{guildId}/vanity-url", data, true);
    }

    /// <summary>
    /// Get a list of members for a guild, default and maximum 1000.
    /// </summary>
    /// <param name="guildId"></param>
    /// <param name="limit"></param>
    /// <param name="afterId"></param>
    /// <param name="queryParams"></param>
    /// <returns></returns>
    public async Task<IEnumerable<GuildMember>> GetMembersAsync(ulong guildId, int limit = 1000, ulong? afterId = null, RestClientQueryParams? queryParams = null)
    {
        queryParams ??= new RestClientQueryParams()
            .Add(QueryParams.Limit, limit)
            .AddIf(afterId != null, QueryParams.After, afterId);

        IEnumerable<GuildMemberJson> json = await SendRequestQueryParamsAsync<IEnumerable<GuildMemberJson>>(HttpMethod.Get, queryParams, $"/guilds/{guildId}/members", true);
        return json.Select(x => GuildMember.Create(_client, x));
    }

    /// <summary>
    /// Get current member for a guild.
    /// </summary>
    /// <param name="guildId"></param>
    /// <returns></returns>
    public async Task<GuildMember> GetCurrentMemberAsync(ulong guildId)
    {
        GuildMemberJson json = await SendRequestAsync<GuildMemberJson>(HttpMethod.Get, $"/guilds/{guildId}/members/@me", true);
        return GuildMember.Create(_client, json);
    }

    /// <summary>
    /// Get a member in a guild.
    /// </summary>
    /// <param name="guildId"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<GuildMember> GetMemberAsync(ulong guildId, ulong userId)
    {
        GuildMemberJson json = await SendRequestAsync<GuildMemberJson>(HttpMethod.Get, $"/guilds/{guildId}/members/{userId}", true);
        return GuildMember.Create(_client, json);
    }

    /// <summary>
    /// Update current member in a guild.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="GuildPermissions.ChangeNickname"/> to change your nickname.
    /// </remarks>
    /// <param name="guildId"></param>
    /// <param name="member"></param>
    /// <returns></returns>
    public async Task<GuildMember> UpdateCurrentMemberAsync(ulong guildId, GuildMemberJson member)
    {
        GuildMemberJson json = await SendRequestAsync<GuildMemberJson, GuildMemberJson>(HttpMethod.Patch, $"/guilds/{guildId}/members/@me", member, true);
        return GuildMember.Create(_client, json);
    }

    /// <summary>
    /// Update a member in a guild.
    /// </summary>
    /// <remarks>
    /// Requires either <see cref="GuildPermissions.ManageNicknames"/>, <see cref="GuildPermissions.DeafenMembers"/>, 
    /// <see cref="GuildPermissions.ManageRoles"/>, <see cref="GuildPermissions.ModerateMembers"/> or
    /// <see cref="GuildPermissions.MuteMembers"/> depending on property.
    /// </remarks>
    /// <param name="guildId"></param>
    /// <param name="userId"></param>
    /// <param name="member"></param>
    /// <returns></returns>
    public async Task<GuildMember> UpdateMemberAsync(ulong guildId, ulong userId, GuildMemberJson member)
    {
        GuildMemberJson json = await SendRequestAsync<GuildMemberJson, GuildMemberJson>(HttpMethod.Patch, $"/guilds/{guildId}/members/{userId}", member, true);
        return GuildMember.Create(_client, json);
    }

    /// <summary>
    /// Kick a member from the guild.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="GuildPermissions.KickMembers"/>.
    /// </remarks>
    /// <param name="guildId"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task KickMemberAsync(ulong guildId, ulong userId)
        => await SendRequestRawAsync(HttpMethod.Delete, $"/guilds/{guildId}/members/{userId}", true);

    /// <summary>
    /// Change ownership of a guild to another member.
    /// </summary>
    /// <remarks>
    /// Requires guild ownership and user account only.
    /// </remarks>
    /// <param name="guildId"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public async Task<Guild> TransferOwnershipAsync(ulong guildId, GuildTransferOwnershipRequest data)
    {
        GuildJson json = await SendRequestAsync<GuildJson, GuildTransferOwnershipRequest>(HttpMethod.Post, $"/guilds/{guildId}/transfer-ownership", data, true);
        return Guild.Create(_client, json);
    }

    /// <summary>
    /// Get bans for a guild.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="GuildPermissions.BanMembers"/>.
    /// </remarks>
    /// <param name="guildId"></param>
    /// <returns></returns>
    public async Task<IEnumerable<GuildBan>> GetBansAsync(ulong guildId)
    {
        IEnumerable<GuildBanJson> json = await SendRequestAsync<IEnumerable<GuildBanJson>>(HttpMethod.Get, $"/guilds/{guildId}/bans", true);
        return json.Select(x => GuildBan.Create(_client, x));
    }

    /// <summary>
    /// Ban a member from the guild.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="GuildPermissions.BanMembers"/>.
    /// </remarks>
    /// <param name="guildId"></param>
    /// <param name="userId"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public async Task BanMemberAsync(ulong guildId, ulong userId, CreateGuildBanRequest data)
        => await SendRequestAsync(HttpMethod.Put, $"/guilds/{guildId}/bans/{userId}", data, true);

    /// <summary>
    /// Unban a user from the guild.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="GuildPermissions.BanMembers"/>.
    /// </remarks>
    /// <param name="guildId"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task UnbanMemberAsync(ulong guildId, ulong userId)
        => await SendRequestRawAsync(HttpMethod.Delete, $"/guilds/{guildId}/bans/{userId}", true);

    /// <summary>
    /// Give role to a member in the guild.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="GuildPermissions.ManageRoles"/>.
    /// </remarks>
    /// <param name="guildId"></param>
    /// <param name="userId"></param>
    /// <param name="roleId"></param>
    /// <returns></returns>
    public async Task AddMemberRoleAsync(ulong guildId, ulong userId, ulong roleId)
        => await SendRequestRawAsync(HttpMethod.Put, $"/guilds/{guildId}/members/{userId}/roles/{roleId}", true);

    /// <summary>
    /// Remove role from a member in the guild.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="GuildPermissions.ManageRoles"/>.
    /// </remarks>
    /// <param name="guildId"></param>
    /// <param name="userId"></param>
    /// <param name="roleId"></param>
    /// <returns></returns>
    public async Task RemoveMemberRoleAsync(ulong guildId, ulong userId, ulong roleId)
        => await SendRequestRawAsync(HttpMethod.Delete, $"/guilds/{guildId}/members/{userId}/roles/{roleId}", true);

    /// <summary>
    /// Create a role in the guild.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="GuildPermissions.ManageRoles"/>.
    /// </remarks>
    /// <param name="guildId"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public async Task<Role> CreateRoleAsync(ulong guildId, CreateGuildRoleRequest data)
    {
        RoleJson json = await SendRequestAsync<RoleJson, CreateGuildRoleRequest>(HttpMethod.Post, $"/guilds/{guildId}/roles", data, true);
        return Role.Create(_client, json, guildId);
    }

    /// <summary>
    /// Update a role in the guild.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="GuildPermissions.ManageRoles"/>.
    /// </remarks>
    /// <param name="guildId"></param>
    /// <param name="roleId"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public async Task<Role> UpdateRoleAsync(ulong guildId, ulong roleId, UpdateGuildRoleRequest data)
    {
        RoleJson json = await SendRequestAsync<RoleJson, UpdateGuildRoleRequest>(HttpMethod.Patch, $"/guilds/{guildId}/roles/{roleId}", data, true);
        return Role.Create(_client, json, guildId);
    }

    /// <summary>
    /// Update role positions in the guild.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="GuildPermissions.ManageRoles"/>.
    /// </remarks>
    /// <param name="guildId"></param>
    /// <param name="positions"></param>
    /// <returns></returns>
    public async Task UpdateRolePositionsAsync(ulong guildId, IEnumerable<RolePositionItemJson> positions)
        => await SendRequestAsync(HttpMethod.Patch, $"/guilds/{guildId}/roles", positions, true);

    /// <summary>
    /// Delete a role in the guild.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="GuildPermissions.ManageRoles"/>.
    /// </remarks>
    /// <param name="guildId"></param>
    /// <param name="roleId"></param>
    /// <returns></returns>
    public async Task DeleteRoleAsync(ulong guildId, ulong roleId)
        => await SendRequestRawAsync(HttpMethod.Delete, $"/guilds/{guildId}/roles/{roleId}", true);

    /// <summary>
    /// Get all channels in the guild.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="ChannelPermissions.ViewChannel"/>.
    /// </remarks>
    /// <param name="guildId"></param>
    /// <returns></returns>
    public async Task<IEnumerable<Channel>> GetChannelsAsync(ulong guildId)
    {
        IEnumerable<ChannelJson> json = await SendRequestAsync<IEnumerable<ChannelJson>>(HttpMethod.Get, $"/guilds/{guildId}/channels", true);
        return json.Select(x => Channel.Create(_client, x));
    }

    /// <summary>
    /// Create a channel in the guild.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="GuildPermissions.ManageChannels"/>.
    /// </remarks>
    /// <param name="guildId"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public async Task<Channel> CreateGuildChannelAsync(ulong guildId, CreateGuildChannelRequest data)
    {
        ChannelJson json = await SendRequestAsync<ChannelJson, CreateGuildChannelRequest>(HttpMethod.Post, $"/guilds/{guildId}/channels", data, true);
        return Channel.Create(_client, json);
    }

    /// <summary>
    /// Update channel positions in a guild.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="GuildPermissions.ManageChannels"/>.
    /// </remarks>
    /// <param name="guildId"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public async Task UpdateChannelPositionsAsync(ulong guildId, IEnumerable<ChannelPositionUpdateRequestItem> data)
        => await SendRequestAsync(HttpMethod.Patch, $"/guilds/{guildId}/channels", data, true);

    /// <summary>
    /// Search the guild.
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    /// <param name="guildId"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public async Task<TResponse> SearchGuildAsync<TRequest, TResponse>(ulong guildId, TRequest data)
        => await SendRequestAsync<TResponse, TRequest>(HttpMethod.Post, $"/guilds/{guildId}/search", data, true);

    /// <summary>
    /// Search audit logs in the guild.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="GuildPermissions.ViewAuditLog"/>.
    /// </remarks>
    /// <param name="guildId"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public async Task<GuildAuditLogListJson> SearchAuditLogAsync(ulong guildId, GuildAuditLogListRequest data)
        => await SendRequestAsync<GuildAuditLogListJson, GuildAuditLogListRequest>(HttpMethod.Post, $"/guilds/{guildId}/audit-logs", data, true);

    /// <summary>
    /// Create emojis for the guild.
    /// </summary>
    /// <remarks>
    /// Requires either <see cref="GuildPermissions.CreateExpressions"/> or <see cref="GuildPermissions.ManageExpressions"/>.
    /// </remarks>
    /// <param name="guildId"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public async Task<GuildEmoji> CreateEmojiAsync(ulong guildId, CreateGuildEmojiRequest data)
    {
        GuildEmojiJson json = await SendRequestAsync<GuildEmojiJson, CreateGuildEmojiRequest>(HttpMethod.Post, $"/guilds/{guildId}/emojis", data, true);
        return GuildEmoji.Create(_client, json, guildId);
    }

    /// <summary>
    /// Create multiple emojis for a guild.
    /// </summary>
    /// <remarks>
    /// Requires either <see cref="GuildPermissions.CreateExpressions"/> or <see cref="GuildPermissions.ManageExpressions"/>.
    /// </remarks>
    /// <param name="guildId"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public async Task<GuildEmojiBulkCreateJson> CreateEmojiBulkAsync(ulong guildId, BulkCreateGuildEmojisRequest data)
        => await SendRequestAsync<GuildEmojiBulkCreateJson, BulkCreateGuildEmojisRequest>(HttpMethod.Post, $"/guilds/{guildId}/emojis/bulk", data, true);

    /// <summary>
    /// Get all emojis in the guild.
    /// </summary>
    /// <remarks>
    /// Requires either <see cref="GuildPermissions.CreateExpressions"/> or <see cref="GuildPermissions.ManageExpressions"/>.
    /// </remarks>
    /// <param name="guildId"></param>
    /// <returns></returns>
    public async Task<IEnumerable<GuildEmoji>> GetEmojisAsync(ulong guildId)
    {
        IEnumerable<GuildEmojiJson> json = await SendRequestAsync<IEnumerable<GuildEmojiJson>>(HttpMethod.Get, $"/guilds/{guildId}/emojis", true);
        return json.Select(x => GuildEmoji.Create(_client, x, guildId));
    }

    /// <summary>
    /// Update an emoji in the guild.
    /// </summary>
    /// <remarks>
    /// Requires either <see cref="GuildPermissions.CreateExpressions"/> or <see cref="GuildPermissions.ManageExpressions"/>.
    /// </remarks>
    /// <param name="guildId"></param>
    /// <param name="emojiId"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public async Task<GuildEmoji> UpdateEmojiAsync(ulong guildId, ulong emojiId, UpdateGuildEmojiRequest data)
    {
        GuildEmojiJson json = await SendRequestAsync<GuildEmojiJson, UpdateGuildEmojiRequest>(HttpMethod.Patch, $"/guilds/{guildId}/emojis/{emojiId}", data, true);
        return GuildEmoji.Create(_client, json, guildId);
    }

    /// <summary>
    /// Delete an emoji in the guild.
    /// </summary>
    /// <remarks>
    /// Requires either <see cref="GuildPermissions.CreateExpressions"/> or <see cref="GuildPermissions.ManageExpressions"/>.
    /// </remarks>
    /// <param name="guildId"></param>
    /// <param name="emojiId"></param>
    /// <returns></returns>
    public async Task DeleteEmojiAsync(ulong guildId, ulong emojiId)
        => await SendRequestRawAsync(HttpMethod.Delete, $"/guilds/{guildId}/emojis/{emojiId}", true);

    /// <summary>
    /// Create stickers for a guild.
    /// </summary>
    /// <remarks>
    /// Requires either <see cref="GuildPermissions.CreateExpressions"/> or <see cref="GuildPermissions.ManageExpressions"/>.
    /// </remarks>
    /// <param name="guildId"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public async Task<GuildSticker> CreateStickerAsync(ulong guildId, CreateGuildStickerRequest data)
    {
        GuildStickerJson json = await SendRequestAsync<GuildStickerJson, CreateGuildStickerRequest>(HttpMethod.Post, $"/guilds/{guildId}/stickers", data, true);
        return GuildSticker.Create(_client, json, guildId);
    }

    /// <summary>
    /// Create multiple stickers for a guild.
    /// </summary>
    /// <remarks>
    /// Requires either <see cref="GuildPermissions.CreateExpressions"/> or <see cref="GuildPermissions.ManageExpressions"/>.
    /// </remarks>
    /// <param name="guildId"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public async Task<GuildStickerBulkCreateJson> CreateStickerBulkAsync(ulong guildId, BulkCreateGuildStickersRequest data)
        => await SendRequestAsync<GuildStickerBulkCreateJson, BulkCreateGuildStickersRequest>(HttpMethod.Post, $"/guilds/{guildId}/stickers/bulk", data, true);

    /// <summary>
    /// Get all stickers in the guild.
    /// </summary>
    /// <remarks>
    /// Requires either <see cref="GuildPermissions.CreateExpressions"/> or <see cref="GuildPermissions.ManageExpressions"/>.
    /// </remarks>
    /// <param name="guildId"></param>
    /// <returns></returns>
    public async Task<IEnumerable<GuildSticker>> GetStickersAsync(ulong guildId)
    {
        IEnumerable<GuildStickerJson> json = await SendRequestAsync<IEnumerable<GuildStickerJson>>(HttpMethod.Get, $"/guilds/{guildId}/stickers", true);
        return json.Select(x => GuildSticker.Create(_client, x, guildId));
    }

    /// <summary>
    /// Update a sticker in the guild.
    /// </summary>
    /// <remarks>
    /// Requires either <see cref="GuildPermissions.CreateExpressions"/> or <see cref="GuildPermissions.ManageExpressions"/>.
    /// </remarks>
    /// <param name="guildId"></param>
    /// <param name="stickerId"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public async Task<GuildSticker> UpdateStickerAsync(ulong guildId, ulong stickerId, UpdateGuildStickerRequest data)
    {
        GuildStickerJson json = await SendRequestAsync<GuildStickerJson, UpdateGuildStickerRequest>(HttpMethod.Patch, $"/guilds/{guildId}/stickers/{stickerId}", data, true);
        return GuildSticker.Create(_client, json, guildId);
    }

    /// <summary>
    /// Delete a sticker in the guild.
    /// </summary>
    /// <remarks>
    /// Requires either <see cref="GuildPermissions.CreateExpressions"/> or <see cref="GuildPermissions.ManageExpressions"/>.
    /// </remarks>
    /// <param name="guildId"></param>
    /// <param name="stickerId"></param>
    /// <returns></returns>
    public async Task DeleteStickerAsync(ulong guildId, ulong stickerId)
        => await SendRequestRawAsync(HttpMethod.Delete, $"/guilds/{guildId}/stickers/{stickerId}", true);

    /// <summary>
    /// Get all invites for the guild.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="ChannelPermissions.ManageChannels"/>.
    /// </remarks>
    /// <param name="guildId"></param>
    /// <returns></returns>
    public async Task<IEnumerable<Invite>> GetGuildInvitesAsync(ulong guildId)
    {
        IEnumerable<InviteJson> json = await SendRequestAsync<IEnumerable<InviteJson>>(HttpMethod.Get, $"/guilds/{guildId}/invites", true);
        return json.Select(x => Invite.Create(_client, x));
    }

    /// <summary>
    /// Get all webhooks for the guild.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="ChannelPermissions.ManageWebhooks"/>.
    /// </remarks>
    /// <param name="guildId"></param>
    /// <returns></returns>
    public async Task<IEnumerable<Webhook>> GetGuildWebhooksAsync(ulong guildId)
    {
        IEnumerable<WebhookJson> json = await SendRequestAsync<IEnumerable<WebhookJson>>(HttpMethod.Get, $"/guilds/{guildId}/webhooks", true);
        return json.Select(x => Webhook.Create(_client, x));
    }

    #endregion

    #region Discovery API



    #endregion

    #region Klipy API
    /// <summary>
    /// Search klipy gifs.
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    public async Task<IEnumerable<Gif>> SearchKlipyAsync(string query)
    {
        IEnumerable<GifJson> json = await SendRequestAsync<IEnumerable<GifJson>>(HttpMethod.Get, $"/klipy/search?q={query}", true);
        return json.Select(x => Gif.Create(_client, x));
    }

    /// <summary>
    /// Get featured klipy gifs.
    /// </summary>
    /// <returns></returns>
    public async Task<GifFeaturedJson> GetKlipyFeaturedAsync()
    {
        return await SendRequestAsync<GifFeaturedJson>(HttpMethod.Get, "/klipy/featured", true);
    }

    /// <summary>
    /// Get trending klipy gifs.
    /// </summary>
    /// <returns></returns>
    public async Task<IEnumerable<Gif>> GetKlipyTrendingGifsAsync()
    {
        IEnumerable<GifJson> json = await SendRequestAsync<IEnumerable<GifJson>>(HttpMethod.Get, "/klipy/trending-gifs", true);
        return json.Select(x => Gif.Create(_client, x));
    }


    #endregion

    #region Apps API

    public async Task<CurrentApplication> GetCurrentApplicationAsync()
    {
        CurrentApplicationJson json = await SendRequestAsync<CurrentApplicationJson>(HttpMethod.Get, $"/oauth2/applications/@me", true);
        return CurrentApplication.Create(_client, json);
    }

    /// <summary>
    /// Get a public app.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<Application?> GetPublicAppAsync(ulong id)
    {
        ApplicationJson? json = await SendRequestAsync<ApplicationJson>(HttpMethod.Get, $"/oauth2/applications/{id}/public", false);
        if (json == null)
            return null;

        return Application.Create(_client, json);
    }

    #endregion

    #region Users API
    /// <summary>
    /// Get the current user.
    /// </summary>
    /// <returns></returns>
    public async Task<CurrentUser> GetCurrentUserAsync()
    {
        CurrentUserJson json = await SendRequestAsync<CurrentUserJson>(HttpMethod.Get, "/users/@me", true);
        return CurrentUser.Create(_client, json);
    }

    /// <summary>
    /// Update the current user.
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public async Task<CurrentUser> UpdateCurrentUserAsync(UserJson user)
    {
        CurrentUserJson json = await SendRequestAsync<CurrentUserJson, UserJson>(HttpMethod.Patch, "/users/@me", user, true);
        return CurrentUser.Create(_client, json);
    }

    /// <summary>
    /// Check if a username is available.
    /// </summary>
    /// <param name="username"></param>
    /// <param name="discriminator"></param>
    /// <returns></returns>
    public async Task<UsernameAvailableJson> CheckUsernameAvailabilityAsync(string username, string discriminator)
    {
        return await SendRequestAsync<UsernameAvailableJson>(HttpMethod.Get, $"/users/check-tag?username={username}&discriminator={discriminator}", true);
    }

    /// <summary>
    /// Get a user.
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<User> GetUserAsync(ulong userId)
    {
        UserJson json = await SendRequestAsync<UserJson>(HttpMethod.Get, $"/users/{userId}", true);
        return User.Create(_client, json);
    }

    /// <summary>
    /// Get a user's profile.
    /// </summary>
    /// <remarks>
    /// Requires mutual friend or guild.
    /// </remarks>
    /// <param name="targetId"></param>
    /// <param name="guildId"></param>
    /// <param name="mutualFriends"></param>
    /// <param name="mutualGuilds"></param>
    /// <returns></returns>
    public async Task<UserProfileResponse> GetUserProfileAsync(ulong targetId, string? guildId = null, bool mutualFriends = false, bool mutualGuilds = false)
        => await SendRequestAsync<UserProfileResponse>(HttpMethod.Get,
            new QueryBuilder($"/users/{targetId}/profile").With("guild_id", guildId).With("with_mutual_friends", mutualFriends).With("with_mutual_guilds", mutualGuilds).Build(), true);

    /// <summary>
    /// Get current user settings.
    /// </summary>
    /// <remarks>
    /// User account only.
    /// </remarks>
    /// <returns></returns>
    public async Task<UserSettings> GetCurrentUserSettingsAsync()
    {
        UserSettingsJson json = await SendRequestAsync<UserSettingsJson>(HttpMethod.Get, "/users/@me/settings", true);
        return UserSettings.Create(_client, json);
    }

    /// <summary>
    /// Update current user settings.
    /// </summary>
    /// <remarks>
    /// User account only.
    /// </remarks>
    /// <typeparam name="TRequest"></typeparam>
    /// <param name="settings"></param>
    /// <returns></returns>
    public async Task<UserSettings> UpdateCurrentUserSettingsAsync<TRequest>(TRequest settings)
    {
        UserSettingsJson json = await SendRequestAsync<UserSettingsJson, TRequest>(HttpMethod.Patch, "/users/@me/settings", settings, true);
        return UserSettings.Create(_client, json);
    }

    /// <summary>
    /// Set current user status.
    /// </summary>
    /// <param name="status"></param>
    /// <returns></returns>
    public async Task<UserSettings> SetCustomStatusAsync(UserCustomStatusJson status)
    {
        UserSettingsJson json = await SendRequestAsync<UserSettingsJson, UpdateCustomStatus>(HttpMethod.Patch, "/users/@me/settings", new UpdateCustomStatus(status), true);
        return UserSettings.Create(_client, json);
    }

    /// <summary>
    /// Get all current user notes.
    /// </summary>
    /// <typeparam name="TResponse"></typeparam>
    /// <returns></returns>
    public async Task<TResponse> GetCurrentUserNotesAsync<TResponse>()
        => await SendRequestAsync<TResponse>(HttpMethod.Get, "/users/@me/notes", true);

    /// <summary>
    /// Get current user note for target user.
    /// </summary>
    /// <typeparam name="TResponse"></typeparam>
    /// <param name="targetId"></param>
    /// <returns></returns>
    public async Task<TResponse> GetCurrentUserNoteAsync<TResponse>(ulong targetId)
        => await SendRequestAsync<TResponse>(HttpMethod.Get, $"/users/@me/notes/{targetId}", true);

    /// <summary>
    /// Add/update current user note for target user.
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    /// <param name="targetId"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public async Task<TResponse> PutCurrentUserNoteAsync<TRequest, TResponse>(ulong targetId, TRequest data)
        => await SendRequestAsync<TResponse, TRequest>(HttpMethod.Put, $"/users/@me/notes/{targetId}", data, true);

    /// <summary>
    /// Get current user mention notifications.
    /// </summary>
    /// <remarks>
    /// User account only.
    /// </remarks>
    /// <typeparam name="TResponse"></typeparam>
    /// <returns></returns>
    public async Task<TResponse> GetCurrentUserMentionsAsync<TResponse>()
        => await SendRequestAsync<TResponse>(HttpMethod.Get, "/users/@me/mentions", true);

    /// <summary>
    /// Delete current user mentioned notifications.
    /// </summary>
    /// <remarks>
    /// User account only.
    /// </remarks>
    /// <param name="messageId"></param>
    /// <returns></returns>
    public async Task DeleteCurrentUserMentionAsync(ulong messageId)
        => await SendRequestRawAsync(HttpMethod.Delete, $"/users/@me/mentions/{messageId}", true);

    /// <summary>
    /// Enable MFA for current user.
    /// </summary>
    /// <remarks>
    /// User account only.
    /// </remarks>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    /// <param name="data"></param>
    /// <returns></returns>
    public async Task<TResponse> PostCurrentUserMfaTotpEnableAsync<TRequest, TResponse>(TRequest data)
        => await SendRequestAsync<TResponse, TRequest>(HttpMethod.Post, "/users/@me/mfa/totp/enable", data, true);

    /// <summary>
    /// Disable MFA for current user.
    /// </summary>
    /// <remarks>
    /// User account only.
    /// </remarks>
    /// <typeparam name="TRequest"></typeparam>
    /// <param name="data"></param>
    /// <returns></returns>
    public async Task PostCurrentUserMfaTotpDisableAsync<TRequest>(TRequest data)
        => await SendRequestAsync<TRequest>(HttpMethod.Post, "/users/@me/mfa/totp/disable", data, true);

    /// <summary>
    /// Get/create MFA backup codes for current user.
    /// </summary>
    /// <remarks>
    /// User account only.
    /// </remarks>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    /// <param name="data"></param>
    /// <returns></returns>
    public async Task<TResponse> PostCurrentUserMfaBackupCodesAsync<TRequest, TResponse>(TRequest data)
        => await SendRequestAsync<TResponse, TRequest>(HttpMethod.Post, "/users/@me/mfa/backup-codes", data, true);

    /// <summary>
    /// Send phone verification for current user.
    /// </summary>
    /// <remarks>
    /// User account only.
    /// </remarks>
    /// <typeparam name="TRequest"></typeparam>
    /// <param name="data"></param>
    /// <returns></returns>
    public async Task PostCurrentUserPhoneSendVerificationAsync<TRequest>(TRequest data)
        => await SendRequestAsync<TRequest>(HttpMethod.Post, "/users/@me/phone/send-verification", data, true);

    /// <summary>
    /// Verify phone verification for current user.
    /// </summary>
    /// <remarks>
    /// User account only.
    /// </remarks>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    /// <param name="data"></param>
    /// <returns></returns>
    public async Task<TResponse> PostCurrentUserPhoneVerifyAsync<TRequest, TResponse>(TRequest data)
        => await SendRequestAsync<TResponse, TRequest>(HttpMethod.Post, "/users/@me/phone/verify", data, true);

    /// <summary>
    /// Set phone number for current user.
    /// </summary>
    /// <remarks>
    /// User account only.
    /// </remarks>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    /// <param name="data"></param>
    /// <returns></returns>
    public async Task<TResponse> PostCurrentUserPhoneAsync<TRequest, TResponse>(TRequest data)
        => await SendRequestAsync<TResponse, TRequest>(HttpMethod.Post, "/users/@me/phone", data, true);

    /// <summary>
    /// Remove phone number for current user.
    /// </summary>
    /// <remarks>
    /// User account only.
    /// </remarks>
    /// <typeparam name="TRequest"></typeparam>
    /// <param name="data"></param>
    /// <returns></returns>
    public async Task DeleteCurrentUserPhoneAsync<TRequest>(TRequest data)
        => await SendRequestAsync<TRequest>(HttpMethod.Delete, "/users/@me/phone", data, true);

    /// <summary>
    /// Enable MFA SMS for current user.
    /// </summary>
    /// <remarks>
    /// User account only.
    /// </remarks>
    /// <typeparam name="TRequest"></typeparam>
    /// <param name="data"></param>
    /// <returns></returns>
    public async Task PostCurrentUserMfaSmsEnableAsync<TRequest>(TRequest data)
        => await SendRequestAsync<TRequest>(HttpMethod.Post, "/users/@me/mfa/sms/enable", data, true);

    /// <summary>
    /// Disable MFA SMS for current user.
    /// </summary>
    /// <remarks>
    /// User account only.
    /// </remarks>
    /// <typeparam name="TRequest"></typeparam>
    /// <param name="data"></param>
    /// <returns></returns>
    public async Task PostCurrentUserMfaSmsDisableAsync<TRequest>(TRequest data)
        => await SendRequestAsync<TRequest>(HttpMethod.Post, "/users/@me/mfa/sms/disable", data, true);

    /// <summary>
    /// Get webauthn MFA for current user.
    /// </summary>
    /// <remarks>
    /// User account only.
    /// </remarks>
    /// <typeparam name="TResponse"></typeparam>
    /// <returns></returns>
    public async Task<TResponse> GetCurrentUserMfaWebauthnCredentialsAsync<TResponse>()
        => await SendRequestAsync<TResponse>(HttpMethod.Get, "/users/@me/mfa/webauthn/credentials", true);

    /// <summary>
    /// Register webauthn for current user.
    /// </summary>
    /// <remarks>
    /// User account only.
    /// </remarks>
    /// <typeparam name="TResponse"></typeparam>
    /// <returns></returns>
    public async Task<TResponse> PostCurrentUserMfaWebauthnCredentialsRegistrationOptionsAsync<TResponse>()
        => await SendRequestAsync<TResponse>(HttpMethod.Post, "/users/@me/mfa/webauthn/credentials/registration-options", true);

    /// <summary>
    /// Set webauthn MFA for current user.
    /// </summary>
    /// <remarks>
    /// User account only.
    /// </remarks>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    /// <param name="data"></param>
    /// <returns></returns>
    public async Task<TResponse> PostCurrentUserMfaWebauthnCredentialsAsync<TRequest, TResponse>(TRequest data)
        => await SendRequestAsync<TResponse, TRequest>(HttpMethod.Post, "/users/@me/mfa/webauthn/credentials", data, true);

    /// <summary>
    /// Update webauthn MFA for current user.
    /// </summary>
    /// <remarks>
    /// User account only.
    /// </remarks>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    /// <param name="credentialId"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public async Task<TResponse> PatchCurrentUserMfaWebauthnCredentialAsync<TRequest, TResponse>(ulong credentialId, TRequest data)
        => await SendRequestAsync<TResponse, TRequest>(HttpMethod.Patch, $"/users/@me/mfa/webauthn/credentials/{credentialId}", data, true);

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public async Task DeleteCurrentUserMfaWebauthnCredentialAsync(ulong credentialId)
        => await SendRequestRawAsync(HttpMethod.Delete, $"/users/@me/mfa/webauthn/credentials/{credentialId}", true);

    public async Task<TResponse> GetCurrentUserSavedMessagesAsync<TResponse>()
        => await SendRequestAsync<TResponse>(HttpMethod.Get, "/users/@me/saved-messages", true);

    public async Task<TResponse> PostCurrentUserSavedMessageAsync<TRequest, TResponse>(TRequest data)
        => await SendRequestAsync<TResponse, TRequest>(HttpMethod.Post, "/users/@me/saved-messages", data, true);

    public async Task DeleteCurrentUserSavedMessageAsync(ulong messageId)
        => await SendRequestRawAsync(HttpMethod.Delete, $"/users/@me/saved-messages/{messageId}", true);

    public async Task<TResponse> GetCurrentUserChannelsAsync<TResponse>()
        => await SendRequestAsync<TResponse>(HttpMethod.Get, "/users/@me/channels", true);

    public async Task<TResponse> PostCurrentUserChannelAsync<TRequest, TResponse>(TRequest data)
        => await SendRequestAsync<TResponse, TRequest>(HttpMethod.Post, "/users/@me/channels", data, true);

    public async Task PutCurrentUserChannelPinAsync(ulong channelId)
        => await SendRequestRawAsync(HttpMethod.Put, $"/users/@me/channels/{channelId}/pin", true);

    public async Task DeleteCurrentUserChannelPinAsync(ulong channelId)
        => await SendRequestRawAsync(HttpMethod.Delete, $"/users/@me/channels/{channelId}/pin", true);

    public async Task<TResponse> GetCurrentUserRelationshipsAsync<TResponse>()
        => await SendRequestAsync<TResponse>(HttpMethod.Get, "/users/@me/relationships", true);

    public async Task<TResponse> PostCurrentUserRelationshipAsync<TRequest, TResponse>(TRequest data)
        => await SendRequestAsync<TResponse, TRequest>(HttpMethod.Post, "/users/@me/relationships", data, true);

    public async Task<TResponse> PostCurrentUserRelationshipWithUserAsync<TRequest, TResponse>(ulong userId, TRequest data)
        => await SendRequestAsync<TResponse, TRequest>(HttpMethod.Post, $"/users/@me/relationships/{userId}", data, true);

    public async Task PutCurrentUserRelationshipAsync<TRequest>(ulong userId, TRequest data)
        => await SendRequestAsync<TRequest>(HttpMethod.Put, $"/users/@me/relationships/{userId}", data, true);

    public async Task DeleteCurrentUserRelationshipAsync(ulong userId)
        => await SendRequestRawAsync(HttpMethod.Delete, $"/users/@me/relationships/{userId}", true);

    public async Task<TResponse> PatchCurrentUserGuildSettingsSelfAsync<TRequest, TResponse>(TRequest data)
        => await SendRequestAsync<TResponse, TRequest>(HttpMethod.Patch, "/users/@me/guilds/@me/settings", data, true);

    public async Task<TResponse> PatchCurrentUserGuildSettingsAsync<TRequest, TResponse>(ulong guildId, TRequest data)
        => await SendRequestAsync<TResponse, TRequest>(HttpMethod.Patch, $"/users/@me/guilds/{guildId}/settings", data, true);

    public async Task PostCurrentUserDisableAsync<TRequest>(TRequest data)
        => await SendRequestAsync<TRequest>(HttpMethod.Post, "/users/@me/disable", data, true);

    public async Task PostCurrentUserDeleteAsync<TRequest>(TRequest data)
        => await SendRequestAsync<TRequest>(HttpMethod.Post, "/users/@me/delete", data, true);

    public async Task<TResponse> PostCurrentUserPushSubscribeAsync<TRequest, TResponse>(TRequest data)
        => await SendRequestAsync<TResponse, TRequest>(HttpMethod.Post, "/users/@me/push/subscribe", data, true);

    public async Task<TResponse> GetCurrentUserPushSubscriptionsAsync<TResponse>()
        => await SendRequestAsync<TResponse>(HttpMethod.Get, "/users/@me/push/subscriptions", true);

    public async Task DeleteCurrentUserPushSubscriptionAsync(ulong subscriptionId)
        => await SendRequestRawAsync(HttpMethod.Delete, $"/users/@me/push/subscriptions/{subscriptionId}", true);

    public async Task<TResponse> PostCurrentUserHarvestAsync<TRequest, TResponse>(TRequest data)
        => await SendRequestAsync<TResponse, TRequest>(HttpMethod.Post, "/users/@me/harvest", data, true);

    public async Task<TResponse> GetCurrentUserHarvestLatestAsync<TResponse>()
        => await SendRequestAsync<TResponse>(HttpMethod.Get, "/users/@me/harvest/latest", true);

    public async Task<TResponse> GetCurrentUserHarvestAsync<TResponse>(string harvestId)
        => await SendRequestAsync<TResponse>(HttpMethod.Get, $"/users/@me/harvest/{harvestId}", true);

    public async Task<TResponse> GetCurrentUserHarvestDownloadAsync<TResponse>(string harvestId)
        => await SendRequestAsync<TResponse>(HttpMethod.Get, $"/users/@me/harvest/{harvestId}/download", true);

    public async Task PostCurrentUserPreloadMessagesAsync<TRequest>(TRequest data)
        => await SendRequestAsync<TRequest>(HttpMethod.Post, "/users/@me/preload-messages", data, true);

    public async Task PostCurrentUserMessagesDeleteAsync<TRequest>(TRequest data)
        => await SendRequestAsync<TRequest>(HttpMethod.Post, "/users/@me/messages/delete", data, true);
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

    #endregion

    #region Webhooks API
    /// <summary>
    /// Get webhook in a guild channel.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="ChannelPermissions.ManageWebhooks"/>.
    /// </remarks>
    /// <param name="webhookId"></param>
    /// <returns></returns>
    public async Task<Webhook> GetWebhookAsync(ulong webhookId)
    {
        WebhookJson json = await SendRequestAsync<WebhookJson>(HttpMethod.Get, $"/webhooks/{webhookId}", true);
        return Webhook.Create(_client, json);
    }

    /// <summary>
    /// Update webhook in a guild channel.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="ChannelPermissions.ManageWebhooks"/>.
    /// </remarks>
    /// <typeparam name="TRequest"></typeparam>
    /// <param name="webhookId"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public async Task<Webhook> UpdateWebhookAsync<TRequest>(ulong webhookId, TRequest data)
    {
        WebhookJson json = await SendRequestAsync<WebhookJson, TRequest>(HttpMethod.Patch, $"/webhooks/{webhookId}", data, true);
        return Webhook.Create(_client, json);
    }

    /// <summary>
    /// Delete webhook in a guild channel.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="ChannelPermissions.ManageWebhooks"/>.
    /// </remarks>
    /// <param name="webhookId"></param>
    /// <returns></returns>
    public async Task DeleteWebhookAsync(ulong webhookId)
        => await SendRequestRawAsync(HttpMethod.Delete, $"/webhooks/{webhookId}", true);

    /// <summary>
    /// Get webhook using the webhook token.
    /// </summary>
    /// <param name="webhookId"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public async Task<Webhook> GetWebhookWithTokenAsync(ulong webhookId, string token)
    {
        WebhookJson json = await SendRequestAsync<WebhookJson>(HttpMethod.Get, $"/webhooks/{webhookId}/{token}", false);
        return Webhook.Create(_client, json);
    }

    /// <summary>
    /// Update webhook using the webhook token.
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <param name="webhookId"></param>
    /// <param name="token"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public async Task<Webhook> UpdateWebhookWithTokenAsync<TRequest>(ulong webhookId, string token, TRequest data)
    {
        WebhookJson json = await SendRequestAsync<WebhookJson, TRequest>(HttpMethod.Patch, $"/webhooks/{webhookId}/{token}", data, false);
        return Webhook.Create(_client, json);
    }

    /// <summary>
    /// Delete webhook using the webhook token.
    /// </summary>
    /// <param name="webhookId"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public async Task DeleteWebhookWithTokenAsync(ulong webhookId, string token)
        => await SendRequestRawAsync(HttpMethod.Delete, $"/webhooks/{webhookId}/{token}", false, false);

    /// <summary>
    /// Send message using the webhook token in a guild channel.
    /// </summary>
    /// <param name="webhookId"></param>
    /// <param name="token"></param>
    /// <param name="content"></param>
    /// <param name="embeds"></param>
    /// <param name="username"></param>
    /// <param name="avatarUrl"></param>
    /// <param name="reference"></param>
    /// <param name="allowedMentions"></param>
    /// <param name="flags"></param>
    /// <param name="nonce"></param>
    /// <param name="favoruteMemeId"></param>
    /// <param name="tts"></param>
    /// <param name="stickerIds"></param>
    /// <param name="attachments"></param>
    /// <returns></returns>
    public async Task ExecuteWebhookAsync(ulong webhookId, string token, string? content = null, List<EmbedRequest>? embeds = null,
        string? username = null, string? avatarUrl = null,
        MessageReferenceRequest? reference = null, AllowedMentionsRequest? allowedMentions = null, MessageFlag flags = MessageFlag.None,
        string? nonce = null, ulong? favoruteMemeId = null, bool? tts = null, List<ulong>? stickerIds = null, List<AttachmentRequest>? attachments = null)
    {
        MessageRequest req = new MessageRequest
        {
            Content = content,
            Embeds = embeds?.ToArray(),
            MessageReference = reference,
            AllowedMentions = allowedMentions,
            Flags = flags,
            Nonce = nonce,
            FavoriteMemeId = favoruteMemeId,
            IsTTS = tts,
            StickerIds = stickerIds,
            WebhookUsername = username,
            WebhookAvatarUrl = avatarUrl,
        };
        if ((attachments?.Count ?? 0) > 0)
        {
            List<KeyValuePair<string, (HttpContent content, string? filename)>> form = new List<KeyValuePair<string, (HttpContent content, string? filename)>>();
            for (int i = 0; i < attachments.Count; i++)
            {
                attachments[i].Id = (ulong)i;
                form.Add(new KeyValuePair<string, (HttpContent content, string? filename)>($"file[{i}]", (new StreamContent(attachments[i].Stream), attachments[i].Filename)));
            }
            req.Attachments = attachments.Select(x => x.ToJson()).ToList();
        }
        await SendRequestAsync(HttpMethod.Post, $"/webhooks/{webhookId}/{token}", req, true, false);
    }

    /// <summary>
    /// Delete a webhook message using the webhook token.
    /// </summary>
    /// <param name="webhookId"></param>
    /// <param name="token"></param>
    /// <param name="messageId"></param>
    /// <returns></returns>
    public async Task DeleteWebhookMessageAsync(ulong webhookId, string token, ulong messageId)
    {
        await SendRequestAsync(HttpMethod.Delete, $"/webhooks/{webhookId}/{token}/messages/{messageId}", true);
    }

    /// <summary>
    /// Modify a webhook message using the webhook token.
    /// </summary>
    /// <param name="webhookId"></param>
    /// <param name="token"></param>
    /// <param name="messageId"></param>
    /// <param name="content"></param>
    /// <param name="embeds"></param>
    /// <param name="reference"></param>
    /// <param name="allowedMentions"></param>
    /// <param name="flags"></param>
    /// <param name="nonce"></param>
    /// <param name="favoruteMemeId"></param>
    /// <param name="stickerIds"></param>
    /// <param name="attachments"></param>
    /// <returns></returns>
    public async Task<Message> EditWebhookMessageAsync(ulong webhookId, string token, ulong messageId, string? content = null, List<EmbedRequest>? embeds = null,
        MessageReferenceRequest? reference = null, AllowedMentionsRequest? allowedMentions = null, MessageFlag flags = MessageFlag.None,
        string? nonce = null, ulong? favoruteMemeId = null, List<ulong>? stickerIds = null, List<AttachmentRequest>? attachments = null)
    {
        MessageRequest req = new MessageRequest
        {
            Content = content,
            Embeds = embeds?.ToArray(),
            MessageReference = reference,
            AllowedMentions = allowedMentions,
            Flags = flags,
            Nonce = nonce,
            FavoriteMemeId = favoruteMemeId,
            StickerIds = stickerIds
        };
        if ((attachments?.Count ?? 0) > 0)
        {
            List<KeyValuePair<string, (HttpContent content, string? filename)>> form = new List<KeyValuePair<string, (HttpContent content, string? filename)>>();
            for (int i = 0; i < attachments.Count; i++)
            {
                attachments[i].Id = (ulong)i;
                form.Add(new KeyValuePair<string, (HttpContent content, string? filename)>($"file[{i}]", (new StreamContent(attachments[i].Stream), attachments[i].Filename)));
            }
            req.Attachments = attachments.Select(x => x.ToJson()).ToList();
        }
        MessageJson json = await SendRequestAsync<MessageJson, MessageRequest>(HttpMethod.Patch, $"/webhooks/{webhookId}/{token}/messages/{messageId}", req, true, false);
        return Message.Create(_client, json);
    }

    /// <summary>
    /// Send message using the webhook token in a guild channel and wait for response message.
    /// </summary>
    /// <param name="webhookId"></param>
    /// <param name="token"></param>
    /// <param name="content"></param>
    /// <param name="embeds"></param>
    /// <param name="username"></param>
    /// <param name="avatarUrl"></param>
    /// <param name="reference"></param>
    /// <param name="allowedMentions"></param>
    /// <param name="flags"></param>
    /// <param name="nonce"></param>
    /// <param name="favoruteMemeId"></param>
    /// <param name="tts"></param>
    /// <param name="stickerIds"></param>
    /// <returns></returns>
    public async Task<Message> ExecuteWebhookWaitAsync(ulong webhookId, string token, string? content = null, List<EmbedRequest>? embeds = null,
        string? username = null, string? avatarUrl = null,
        MessageReferenceRequest? reference = null, AllowedMentionsRequest? allowedMentions = null, MessageFlag flags = MessageFlag.None,
        string? nonce = null, ulong? favoruteMemeId = null, bool? tts = null, List<ulong>? stickerIds = null, List<AttachmentRequest>? attachments = null)
    {
        MessageRequest req = new MessageRequest
        {
            Content = content,
            Embeds = embeds?.ToArray(),
            MessageReference = reference,
            AllowedMentions = allowedMentions,
            Flags = flags,
            Nonce = nonce,
            FavoriteMemeId = favoruteMemeId,
            IsTTS = tts,
            StickerIds = stickerIds,
            WebhookUsername = username,
            WebhookAvatarUrl = avatarUrl,
        };
        if ((attachments?.Count ?? 0) > 0)
        {
            List<KeyValuePair<string, (HttpContent content, string? filename)>> form = new List<KeyValuePair<string, (HttpContent content, string? filename)>>();
            for (int i = 0; i < attachments.Count; i++)
            {
                attachments[i].Id = (ulong)i;
                form.Add(new KeyValuePair<string, (HttpContent content, string? filename)>($"file[{i}]", (new StreamContent(attachments[i].Stream), attachments[i].Filename)));
            }
            req.Attachments = attachments.Select(x => x.ToJson()).ToList();
        }
        MessageJson json = await SendRequestAsync<MessageJson, MessageRequest>(HttpMethod.Post, $"/webhooks/{webhookId}/{token}?wait=true", req, true);
        return Message.Create(_client, json);
    }
    #endregion

    #region OAuth API
    /// <summary>
    /// Get user from access token.
    /// </summary>
    /// <param name="accessToken"></param>
    /// <returns></returns>
    public async Task<FluxerOAuthUser> GetOAuthUserAsync(string accessToken)
    {
        FluxerOAuthUserJson json = await InternalSendRequestAsync<FluxerOAuthUserJson>(HttpMethod.Get, "/oauth2/userinfo", true, false, accessToken);
        return FluxerOAuthUser.Create(_client, json);
    }

    /// <summary>
    /// Get oauth token from access token.
    /// </summary>
    /// <param name="accessToken"></param>
    /// <returns></returns>
    public async Task<FluxerOAuthToken> GetOAuthTokenAsync(string accessToken)
    {
        FluxerOAuthTokenJson json = await InternalSendRequestAsync<FluxerOAuthTokenJson>(HttpMethod.Get, "/oauth2/@me", true, false, accessToken);
        return FluxerOAuthToken.Create(_client, json);
    }


    /// <summary>
    /// Get user guilds from access token.
    /// </summary>
    /// <param name="accessToken"></param>
    /// <returns></returns>
    public async Task<IEnumerable<Guild>> GetOAuthGuildsAsync(string accessToken)
    {
        IEnumerable<GuildJson> json = await InternalSendRequestAsync<IEnumerable<GuildJson>>(HttpMethod.Get, "/users/@me/guilds", true, false, accessToken);
        return json.Select(x => Guild.Create(_client, x));
    }

    /// <summary>
    /// Get user connections from access token.
    /// </summary>
    /// <param name="accessToken"></param>
    /// <returns></returns>
    public async Task<IEnumerable<UserConnection>> GetOAuthConnectionsAsync(string accessToken)
    {
        IEnumerable<UserConnectionJson> json = await InternalSendRequestAsync<IEnumerable<UserConnectionJson>>(HttpMethod.Get, "/users/@me/connections", true, false, accessToken);
        return json.Select(x => UserConnection.Create(_client, x));
    }

    /// <summary>
    /// Check if oauth token is valid.
    /// </summary>
    /// <param name="clientId"></param>
    /// <param name="clientSecret"></param>
    /// <param name="accessToken"></param>
    /// <returns></returns>
    public async Task<FluxerOAuthValidToken> GetOAuthValidTokenAsync(ulong clientId, string clientSecret, string accessToken)
    {
        return await InternalSendRequestFormAsync<FluxerOAuthValidTokenJson>(HttpMethod.Post, "/oauth2/introspect", true, new Dictionary<string, string>
        {
            { "client_id", clientId.ToString() },
            { "client_secret", clientSecret },
            { "token", accessToken }
        });
    }

    /// <summary>
    /// Get new access token from refresh token.
    /// </summary>
    /// <param name="clientId"></param>
    /// <param name="clientSecret"></param>
    /// <param name="refreshToken"></param>
    /// <returns></returns>
    public async Task<FluxerOAuthRefreshToken> GetOAuthRefreshTokenAsync(ulong clientId, string clientSecret, string refreshToken)
    {
        FluxerOAuthRefreshTokenJson json = await InternalSendRequestFormAsync<FluxerOAuthRefreshTokenJson>(HttpMethod.Post, "/oauth2/token", true, new Dictionary<string, string>
        {
            { "client_id", clientId.ToString() },
            { "client_secret", clientSecret },
            { "grant_type", "refresh_token" },
            { "refresh_token", refreshToken }
        });

        return FluxerOAuthRefreshToken.Create(_client, json);
    }

    /// <summary>
    /// Remove access for access token.
    /// </summary>
    /// <param name="clientId"></param>
    /// <param name="clientSecret"></param>
    /// <param name="accessToken"></param>
    /// <returns></returns>
    public async Task RevokeOAuthAccessTokenAsync(ulong clientId, string clientSecret, string accessToken)
    {
        await InternalSendRequestFormAsync<UserJson>(HttpMethod.Post, "/oauth2/token/revoke", true, new Dictionary<string, string>
        {
            { "client_id", clientId.ToString() },
            { "client_secret", clientSecret },
            { "token", accessToken },
            { "token_type_hint", "access_token" }
        });
    }

    /// <summary>
    /// Remove access for refresh token.
    /// </summary>
    /// <param name="clientId"></param>
    /// <param name="clientSecret"></param>
    /// <param name="refreshToken"></param>
    /// <returns></returns>
    public async Task RevokeOAuthRefreshTokenAsync(ulong clientId, string clientSecret, string refreshToken)
    {
        await InternalSendRequestFormAsync<UserJson>(HttpMethod.Post, "/oauth2/token/revoke", true, new Dictionary<string, string>
        {
            { "client_id", clientId.ToString() },
            { "client_secret", clientSecret },
            { "token", refreshToken },
            { "token_type_hint", "refresh_token" }
        });
    }
    #endregion

    #region Global Search

    public async Task<GlobalSearch> SearchGuildMessagesAsync(ulong guildId, GlobalSearchMessagesRequest request)
    {
        request.ContextGuildId = guildId;
        GlobalSearchJson json = await SendRequestAsync<GlobalSearchJson, GlobalSearchMessagesRequest>(HttpMethod.Post, "/search/messages", request, true);
        return GlobalSearch.Create(_client, json);
    }

    public async Task<GlobalSearch> SearchGuildChannelMessagesAsync(ulong guildId, ulong channelId, GlobalSearchMessagesRequest request)
    {
        request.ContextGuildId = guildId;
        request.ContextChannelId = channelId;
        GlobalSearchJson json = await SendRequestAsync<GlobalSearchJson, GlobalSearchMessagesRequest>(HttpMethod.Post, "/search/messages", request, true);
        return GlobalSearch.Create(_client, json);
    }

    public async Task<GlobalSearch> SearchChannelMessagesAsync(ulong channelId, GlobalSearchMessagesRequest request)
    {
        request.ContextChannelId = channelId;
        GlobalSearchJson json = await SendRequestAsync<GlobalSearchJson, GlobalSearchMessagesRequest>(HttpMethod.Post, "/search/messages", request, true);
        return GlobalSearch.Create(_client, json);
    }

    #endregion
}
