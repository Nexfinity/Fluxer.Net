using Fluxer.Net.Extensions;
using Fluxer.Net.Gateway.Data;
using Fluxer.Net.RateLimiting;
using Newtonsoft.Json;
using Serilog.Core;
using System.Net;
using System.Net.Http.Headers;
using System.Reflection;

namespace Fluxer.Net;

/// <summary>
/// REST API client for the Fluxer platform. Provides methods for all Fluxer API endpoints
/// including authentication, channels, guilds, users, messages, and more.
/// </summary>
/// <remarks>
/// This client handles HTTP requests to the Fluxer API with automatic rate limiting,
/// JSON serialization, and error handling. It supports both synchronous operations via
/// REST and can be paired with <see cref="GatewayClient"/> for real-time events.
/// </remarks>
public class ApiClient
{
    #region Declares
    //private FluxerClient? _client;
    private string _token;
    private FluxerConfig _config;
    private FluxerClient _client;

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
    private readonly Logger _logger;
#pragma warning restore CS0169
    #endregion

    #region Meta
    /// <summary>
    /// Initializes a new instance of the <see cref="ApiClient"/> class.
    /// </summary>
    /// <param name="token">The authentication token for API requests.</param>
    /// <param name="config">Configuration options including API endpoints, rate limiting, and logging.</param>
    /// <remarks>
    /// The client is automatically configured with:
    /// <list type="bullet">
    /// <item>Rate limiting enabled by default (configurable via <see cref="FluxerConfig.EnableRateLimiting"/>)</item>
    /// <item>Serilog logger for request/response tracking</item>
    /// <item>HTTP client for connection pooling</item>
    /// </list>
    /// </remarks>
    internal ApiClient(FluxerClient client)
    {
        _client = client;
        _token = client.Token;
        _config = client.Config;
        _logger = client.Config.RestSerilog;
        Initialize();
    }

    internal ApiClient(FluxerWebhookClient webhook)
    {
        _config = webhook.Config;
        _logger = webhook.Config.RestSerilog;
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

        var bucket = RateLimitManager.GetBucket(config, channelId, guildId, userId, webhookId, inviteCode);
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
    /// <returns>The deserialized response object.</returns>
    /// <exception cref="FluxerApiException">Thrown when <paramref name="throwOnNonSuccess"/> is true and the API returns a non-success status code.</exception>
    public async Task<TResponse> MakeFluxerApiRequestAsync<TResponse, TSend>(HttpMethod method, string route, TSend data, bool throwOnNonSuccess = false, bool authorize = true,
        ICollection<KeyValuePair<string, (HttpContent content, string? filename)>>? otherFormData = null) where TResponse : Entity
    {
        var rawContent = JsonConvert.SerializeObject(data, FluxerClient._serializerSettings);
        _logger.Verbose("Sending {@Enums} to {Route}", rawContent, route);
        var req = new HttpRequestMessage()
        {
            Method = method,
            RequestUri = new(_config.RealApiBaseUrl + route)
        };
        if (otherFormData != null)
        {
            var form = new MultipartFormDataContent
            {
                {
                    new StringContent(rawContent, new MediaTypeHeaderValue("application/json")),
                    "payload_json"
                }
            };
            foreach (var (key, (content, filename)) in otherFormData)
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
            req.Content = new StringContent(rawContent, new MediaTypeHeaderValue("application/json"));
        }
        if (!string.IsNullOrEmpty(_token) && authorize)
            req.Headers.Add("Authorization", _token);
        var result = await HttpClient.SendAsync(req);

        _logger.Debug("Made {Method} request to {Route}", method, route);
        var resp = await result.Content.ReadAsStringAsync();
        _logger.Verbose("Received {Code}:{Result} from {Route}", result.StatusCode, resp, route);

        if (throwOnNonSuccess && !result.IsSuccessStatusCode)
            throw new FluxerApiException($"Fluxer returned a non-success code {result.StatusCode}", resp);

        TResponse response = JsonConvert.DeserializeObject<TResponse>(resp);
        response.Client = _client;

        return response;
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
    public async Task<HttpStatusCode> MakeFluxerApiRequestAsync<TSend>(HttpMethod method, string route, TSend data, bool throwOnNonSuccess = false, bool authorize = true)
    {
        _logger.Verbose("Sending {@Enums} to {Route}", data, route);
        var req = new HttpRequestMessage()
        {
            Method = method,
            Content = new StringContent(JsonConvert.SerializeObject(data, FluxerClient._serializerSettings),
            new MediaTypeHeaderValue("application/json")),
            RequestUri = new(_config.RealApiBaseUrl + route)
        };
        if (!string.IsNullOrEmpty(_token) && authorize)
            req.Headers.Add("Authorization", _token);
        var result = await HttpClient.SendAsync(req);

        _logger.Debug("Made {Method} request to {Route}", method, route);
        var resp = await result.Content.ReadAsStringAsync();
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
    public async Task<TResponse> MakeFluxerApiRequestAsync<TResponse>(HttpMethod method, string route, bool throwOnNonSuccess = false, bool authorize = true) where TResponse : Entity
    {
        var req = new HttpRequestMessage()
        {
            Method = method,
            RequestUri = new(_config.RealApiBaseUrl + route)
        };
        if (!string.IsNullOrEmpty(_token) && authorize)
            req.Headers.Add("Authorization", _token);
        var result = await HttpClient.SendAsync(req);

        _logger.Debug("Made {Method} request to {Route}", method, route);
        var resp = await result.Content.ReadAsStringAsync();
        _logger.Verbose("Received {Code}:{Result} from {Route}", result.StatusCode, resp, route);

        if (throwOnNonSuccess && !result.IsSuccessStatusCode)
            throw new FluxerApiException($"Fluxer returned a non-success code {result.StatusCode}", resp);

        TResponse response = JsonConvert.DeserializeObject<TResponse>(resp);
        response.Client = _client;

        return response;
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
    public async Task<List<TResponse>> MakeFluxerApiRequestListAsync<TResponse>(HttpMethod method, string route, bool throwOnNonSuccess = false, bool authorize = true) where TResponse : Entity
    {
        var req = new HttpRequestMessage()
        {
            Method = method,
            RequestUri = new(_config.RealApiBaseUrl + route)
        };
        if (!string.IsNullOrEmpty(_token) && authorize)
            req.Headers.Add("Authorization", _token);
        var result = await HttpClient.SendAsync(req);

        _logger.Debug("Made {Method} request to {Route}", method, route);
        var resp = await result.Content.ReadAsStringAsync();
        _logger.Verbose("Received {Code}:{Result} from {Route}", result.StatusCode, resp, route);

        if (throwOnNonSuccess && !result.IsSuccessStatusCode)
            throw new FluxerApiException($"Fluxer returned a non-success code {result.StatusCode}", resp);

        List<TResponse> response = JsonConvert.DeserializeObject<List<TResponse>>(resp);
        foreach (var i in response)
        {
            i.Client = _client;
        }

        return response;
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
    public async Task<HttpStatusCode> MakeFluxerApiRequestRawAsync(HttpMethod method, string route, bool throwOnNonSuccess = false, bool authorize = true)
    {
        var req = new HttpRequestMessage()
        {
            Method = method,
            RequestUri = new(_config.RealApiBaseUrl + route)
        };
        if (!string.IsNullOrEmpty(_token) && authorize)
            req.Headers.Add("Authorization", _token);
        var result = await HttpClient.SendAsync(req);

        _logger.Debug("Made {Method} request to {Route} with response code {Code}", method, route, result.StatusCode);
        if (throwOnNonSuccess && !result.IsSuccessStatusCode)
            throw new FluxerApiException($"Fluxer returned a non-success code {result.StatusCode}", await result.Content.ReadAsStringAsync());

        return result.StatusCode;
    }
    #endregion

    #region Auth API

    public async Task<LoginResponse> LoginAsync(LoginRequest data)
        => await MakeFluxerApiRequestAsync<LoginResponse, LoginRequest>(HttpMethod.Post, "/auth/login", data, false);

    public async Task RegisterAsync<TRequest>(TRequest data)
        => await MakeFluxerApiRequestAsync<TRequest>(HttpMethod.Post, "/auth/register", data, true, false);

    public async Task LoginMfaTotpAsync<TRequest, TResponse>(TRequest data) where TResponse : Entity
        => await MakeFluxerApiRequestAsync<TResponse, TRequest>(HttpMethod.Post, "/auth/login/mfa/totp", data, true, false);

    public async Task SendMfaSmsCodeAsync()
        => await MakeFluxerApiRequestRawAsync(HttpMethod.Post, "/auth/login/mfa/sms/send", true, false);

    public async Task LoginMfaSmsAsync<TRequest, TResponse>(TRequest data) where TResponse : Entity
        => await MakeFluxerApiRequestAsync<TResponse, TRequest>(HttpMethod.Post, "/auth/login/mfa/sms", data, true, false);

    public async Task LogoutAsync()
        => await MakeFluxerApiRequestRawAsync(HttpMethod.Post, "/auth/logout", true);

    public async Task VerifyEmailAsync<TRequest>(TRequest data)
        => await MakeFluxerApiRequestAsync<TRequest>(HttpMethod.Post, "/auth/verify", data, true, false);

    public async Task ResendVerificationEmailAsync()
        => await MakeFluxerApiRequestRawAsync(HttpMethod.Post, "/auth/verify/resend", true);

    public async Task ForgotPasswordAsync<TRequest>(TRequest data)
        => await MakeFluxerApiRequestAsync<TRequest>(HttpMethod.Post, "/auth/forgot", data, true, false);

    public async Task ResetPasswordAsync<TRequest>(TRequest data)
        => await MakeFluxerApiRequestAsync<TRequest>(HttpMethod.Post, "/auth/reset", data, true, false);

    public async Task<TResponse> GetSessionsAsync<TResponse>() where TResponse : Entity
        => await MakeFluxerApiRequestAsync<TResponse>(HttpMethod.Get, "/auth/sessions", true);

    public async Task LogoutSessionsAsync<TRequest>(TRequest data)
        => await MakeFluxerApiRequestAsync<TRequest>(HttpMethod.Post, "/auth/sessions/logout", data, true);

    public async Task PostAuthAuthorizeIpAsync<TRequest>(TRequest data)
        => await MakeFluxerApiRequestAsync<TRequest>(HttpMethod.Post, "/auth/authorize-ip", data, true);

    public async Task<TResponse> PostAuthWebauthnAuthenticationOptionsAsync<TResponse>() where TResponse : Entity
        => await MakeFluxerApiRequestAsync<TResponse>(HttpMethod.Post, "/auth/webauthn/authentication-options", true, false);

    public async Task<TResponse> PostAuthWebauthnAuthenticateAsync<TRequest, TResponse>(TRequest data) where TResponse : Entity
        => await MakeFluxerApiRequestAsync<TResponse, TRequest>(HttpMethod.Post, "/auth/webauthn/authenticate", data, true, false);

    public async Task<TResponse> PostAuthLoginMfaWebauthnAuthenticationOptionsAsync<TResponse>() where TResponse : Entity
        => await MakeFluxerApiRequestAsync<TResponse>(HttpMethod.Post, "/auth/login/mfa/webauthn/authentication-options", true, false);

    public async Task<TResponse> PostAuthLoginMfaWebauthnAsync<TRequest, TResponse>(TRequest data) where TResponse : Entity
        => await MakeFluxerApiRequestAsync<TResponse, TRequest>(HttpMethod.Post, "/auth/login/mfa/webauthn", data, true, false);

    public async Task<TResponse> PostAuthRedeemBetaCodeAsync<TRequest, TResponse>(TRequest data) where TResponse : Entity
        => await MakeFluxerApiRequestAsync<TResponse, TRequest>(HttpMethod.Post, "/auth/redeem-beta-code", data, true);

    #endregion

    #region Channels API

    public async Task<Channel> GetChannelAsync(ulong channelId)
        => await MakeFluxerApiRequestAsync<Channel>(HttpMethod.Get, $"/channels/{channelId}", true);

    public async Task<TResponse> GetChannelRtcRegionsAsync<TResponse>(ulong channelId) where TResponse : Entity
        => await MakeFluxerApiRequestAsync<TResponse>(HttpMethod.Get, $"/channels/{channelId}/rtc-regions", true);

    public async Task<Channel> UpdateChannelAsync(ulong channelId, Channel channel)
        => await MakeFluxerApiRequestAsync<Channel, Channel>(HttpMethod.Patch, $"/channels/{channelId}", channel, true);

    public async Task DeleteChannelAsync(ulong channelId)
        => await MakeFluxerApiRequestRawAsync(HttpMethod.Delete, $"/channels/{channelId}", true);

    public async Task ClearMessageAcknowledgementAsync(ulong channelId)
        => await MakeFluxerApiRequestRawAsync(HttpMethod.Delete, $"/channels/{channelId}/messages/ack", true);

    public async Task<List<MessageBaseResponse>> GetMessagesAsync(ulong channelId)
        => await MakeFluxerApiRequestListAsync<MessageBaseResponse>(HttpMethod.Get, $"/channels/{channelId}/messages", true);

    public async Task<MessageBaseResponse> GetMessageAsync(ulong channelId, ulong messageId)
        => await MakeFluxerApiRequestAsync<MessageBaseResponse>(HttpMethod.Get, $"/channels/{channelId}/messages/{messageId}", true);

    public async Task<TResponse> SearchChannelAsync<TRequest, TResponse>(ulong channelId, TRequest data) where TResponse : Entity
        => await MakeFluxerApiRequestAsync<TResponse, TRequest>(HttpMethod.Post, $"/channels/{channelId}/search", data, true);

    public async Task<MessageBaseResponse> SendMessageAsync(ulong channelId, Message message, StreamAttachment[]? attachments = null)
    {
        if ((attachments?.Length ?? 0) < 1)
        {
            return await MakeFluxerApiRequestAsync<MessageBaseResponse, Message>(HttpMethod.Post, $"/channels/{channelId}/messages", message, true);
        }
        var form = new List<KeyValuePair<string, (HttpContent content, string? filename)>>();
        for (int i = 0; i < attachments.Length; i++)
        {
            attachments[i].Id = (ulong)i;
            form.Add(new KeyValuePair<string, (HttpContent content, string? filename)>($"file[{i}]", (new StreamContent(attachments[i].Stream), attachments[i].Filename)));
        }
        message.Attachments = attachments.Cast<Attachment>().ToList();

        return await MakeFluxerApiRequestAsync<MessageBaseResponse, Message>(HttpMethod.Post, $"/channels/{channelId}/messages", message, true);
    }

    public async Task<MessageBaseResponse> EditMessageAsync(ulong channelId, ulong messageId, MessageUpdateRequest message)
        => await MakeFluxerApiRequestAsync<MessageBaseResponse, MessageUpdateRequest>(HttpMethod.Patch, $"/channels/{channelId}/messages/{messageId}", message, true);

    public async Task DeleteMessageAsync(ulong channelId, ulong messageId)
        => await MakeFluxerApiRequestRawAsync(HttpMethod.Delete, $"/channels/{channelId}/messages/{messageId}", true);

    public async Task DeleteMessageAttachmentAsync(ulong channelId, ulong messageId, ulong attachmentId)
        => await MakeFluxerApiRequestRawAsync(HttpMethod.Delete, $"/channels/{channelId}/messages/{messageId}/attachments/{attachmentId}", true);

    public async Task BulkDeleteMessagesAsync(ulong channelId, BulkDeleteMessagesRequest data)
        => await MakeFluxerApiRequestAsync(HttpMethod.Post, $"/channels/{channelId}/messages/bulk-delete", data, true);

    public async Task TriggerTypingIndicatorAsync(ulong channelId)
        => await MakeFluxerApiRequestRawAsync(HttpMethod.Post, $"/channels/{channelId}/typing", true);

    public async Task AcknowledgeMessageAsync(ulong channelId, ulong messageId, MessageAck details)
        => await MakeFluxerApiRequestAsync<MessageAck>(HttpMethod.Post, $"/channels/{channelId}/messages/{messageId}/ack", details, true);

    public async Task<ChannelPinsResponse> GetPinnedMessagesAsync(ulong channelId, ChannelPinsQuery? query = null)
        => await MakeFluxerApiRequestAsync<ChannelPinsResponse>(HttpMethod.Get, $"/channels/{channelId}/pins?{query?.BuildQuery() ?? string.Empty}", true);

    public async Task PinMessageAsync(ulong channelId, ulong messageId)
        => await MakeFluxerApiRequestRawAsync(HttpMethod.Put, $"/channels/{channelId}/pins/{messageId}", true);

    public async Task UnpinMessageAsync(ulong channelId, ulong messageId)
        => await MakeFluxerApiRequestRawAsync(HttpMethod.Delete, $"/channels/{channelId}/pins/{messageId}", true);

    public async Task<List<UserPartialResponse>> GetReactionsAsync(ulong channelId, ulong messageId, string emoji)
        => await MakeFluxerApiRequestListAsync<UserPartialResponse>(HttpMethod.Get, $"/channels/{channelId}/messages/{messageId}/reactions/{emoji}", true);

    public async Task AddReactionAsync(ulong channelId, ulong messageId, string emoji)
        => await MakeFluxerApiRequestRawAsync(HttpMethod.Put, $"/channels/{channelId}/messages/{messageId}/reactions/{emoji}/@me", true);

    public async Task RemoveOwnReactionAsync(ulong channelId, ulong messageId, string emoji)
        => await MakeFluxerApiRequestRawAsync(HttpMethod.Delete, $"/channels/{channelId}/messages/{messageId}/reactions/{emoji}/@me", true);

    public async Task RemoveUserReactionAsync(ulong channelId, ulong messageId, string emoji, ulong targetId)
        => await MakeFluxerApiRequestRawAsync(HttpMethod.Delete, $"/channels/{channelId}/messages/{messageId}/reactions/{emoji}/{targetId}", true);

    public async Task RemoveAllReactionsForEmojiAsync(ulong channelId, ulong messageId, string emoji)
        => await MakeFluxerApiRequestRawAsync(HttpMethod.Delete, $"/channels/{channelId}/messages/{messageId}/reactions/{emoji}", true);

    public async Task RemoveAllReactionsAsync(ulong channelId, ulong messageId)
        => await MakeFluxerApiRequestRawAsync(HttpMethod.Delete, $"/channels/{channelId}/messages/{messageId}/reactions", true);

    public async Task<TResponse> UploadAttachmentsAsync<TRequest, TResponse>(ulong channelId, TRequest data) where TResponse : Entity
        => await MakeFluxerApiRequestAsync<TResponse, TRequest>(HttpMethod.Post, $"/channels/{channelId}/attachments", data, true);

    public async Task AddRecipientAsync(ulong channelId, ulong userId)
        => await MakeFluxerApiRequestRawAsync(HttpMethod.Put, $"/channels/{channelId}/recipients/{userId}", true);

    public async Task RemoveRecipientAsync(ulong channelId, ulong userId)
        => await MakeFluxerApiRequestRawAsync(HttpMethod.Delete, $"/channels/{channelId}/recipients/{userId}", true);

    public async Task<CallEligibilityResponse> GetCallAsync(ulong channelId)
        => await MakeFluxerApiRequestAsync<CallEligibilityResponse>(HttpMethod.Get, $"/channels/{channelId}/call", true);

    public async Task<TResponse> UpdateCallAsync<TRequest, TResponse>(ulong channelId, TRequest data) where TResponse : Entity
        => await MakeFluxerApiRequestAsync<TResponse, TRequest>(HttpMethod.Patch, $"/channels/{channelId}/call", data, true);

    public async Task RingCallAsync<TRequest>(ulong channelId, TRequest data)
        => await MakeFluxerApiRequestAsync<TRequest>(HttpMethod.Post, $"/channels/{channelId}/call/ring", data, true);

    public async Task StopRingingAsync(ulong channelId)
        => await MakeFluxerApiRequestRawAsync(HttpMethod.Post, $"/channels/{channelId}/call/stop-ringing", true);

    public async Task<List<Invite>> GetChannelInvitesAsync(ulong channelId)
        => await MakeFluxerApiRequestListAsync<Invite>(HttpMethod.Get, $"/channels/{channelId}/invites", true);

    public async Task<TResponse> CreateInviteAsync<TRequest, TResponse>(ulong channelId, TRequest data) where TResponse : Entity
        => await MakeFluxerApiRequestAsync<TResponse, TRequest>(HttpMethod.Post, $"/channels/{channelId}/invites", data, true);

    public async Task<TResponse> GetChannelWebhooksAsync<TResponse>(ulong channelId) where TResponse : Entity
        => await MakeFluxerApiRequestAsync<TResponse>(HttpMethod.Get, $"/channels/{channelId}/webhooks", true);

    public async Task<TResponse> CreateWebhookAsync<TRequest, TResponse>(ulong channelId, TRequest data) where TResponse : Entity
        => await MakeFluxerApiRequestAsync<TResponse, TRequest>(HttpMethod.Post, $"/channels/{channelId}/webhooks", data, true);

    #endregion

    #region Attachments API

    public async Task DeleteAttachmentAsync(string uploadFilename)
        => await MakeFluxerApiRequestRawAsync(HttpMethod.Delete, $"/attachments/{uploadFilename}", true);

    #endregion

    #region Memes API

    public async Task<TResponse> GetCurrentUserMemesAsync<TResponse>() where TResponse : Entity
        => await MakeFluxerApiRequestAsync<TResponse>(HttpMethod.Get, "/users/@me/memes", true);

    public async Task<TResponse> PostCurrentUserMemeAsync<TRequest, TResponse>(TRequest data) where TResponse : Entity
        => await MakeFluxerApiRequestAsync<TResponse, TRequest>(HttpMethod.Post, "/users/@me/memes", data, true);

    public async Task PostChannelMessageMemeAsync<TRequest>(ulong channelId, ulong messageId, TRequest data)
        => await MakeFluxerApiRequestAsync<TRequest>(HttpMethod.Post, $"/channels/{channelId}/messages/{messageId}/memes", data, true);

    public async Task<TResponse> GetCurrentUserMemeAsync<TResponse>(ulong memeId) where TResponse : Entity
        => await MakeFluxerApiRequestAsync<TResponse>(HttpMethod.Get, $"/users/@me/memes/{memeId}", true);

    public async Task<TResponse> PatchCurrentUserMemeAsync<TRequest, TResponse>(ulong memeId, TRequest data) where TResponse : Entity
        => await MakeFluxerApiRequestAsync<TResponse, TRequest>(HttpMethod.Patch, $"/users/@me/memes/{memeId}", data, true);

    public async Task DeleteCurrentUserMemeAsync(ulong memeId)
        => await MakeFluxerApiRequestRawAsync(HttpMethod.Delete, $"/users/@me/memes/{memeId}", true);

    #endregion

    #region Invites API

    public async Task<TResponse> GetInviteAsync<TResponse>(string inviteCode) where TResponse : Entity
        => await MakeFluxerApiRequestAsync<TResponse>(HttpMethod.Get, $"/invites/{inviteCode}", true);

    public async Task<GuildProperties> JoinGuildAsync(string inviteCode)
        => await MakeFluxerApiRequestAsync<GuildProperties>(HttpMethod.Post, $"/invites/{inviteCode}", true);

    public async Task DeleteInviteAsync(string inviteCode)
        => await MakeFluxerApiRequestRawAsync(HttpMethod.Delete, $"/invites/{inviteCode}", true);

    #endregion

    #region Read States API

    public async Task PostReadStatesAckBulkAsync<TRequest>(TRequest data)
        => await MakeFluxerApiRequestAsync<TRequest>(HttpMethod.Post, "/read-states/ack-bulk", data, true);

    #endregion

    #region Reports API

    public async Task PostReportMessageAsync<TRequest>(TRequest data)
        => await MakeFluxerApiRequestAsync<TRequest>(HttpMethod.Post, "/reports/message", data, true);

    public async Task PostReportUserAsync<TRequest>(TRequest data)
        => await MakeFluxerApiRequestAsync<TRequest>(HttpMethod.Post, "/reports/user", data, true);

    public async Task PostReportGuildAsync<TRequest>(TRequest data)
        => await MakeFluxerApiRequestAsync<TRequest>(HttpMethod.Post, "/reports/guild", data, true);

    #endregion

    #region Guilds API

    public async Task<GuildProperties> CreateGuildAsync(GuildCreateRequest data)
        => await MakeFluxerApiRequestAsync<GuildProperties, GuildCreateRequest>(HttpMethod.Post, "/guilds", data, true);

    public async Task<List<GuildProperties>> GetCurrentUserGuildsAsync()
        => await MakeFluxerApiRequestListAsync<GuildProperties>(HttpMethod.Get, "/users/@me/guilds", true);

    public async Task LeaveGuildAsync(ulong guildId)
        => await MakeFluxerApiRequestRawAsync(HttpMethod.Delete, $"/users/@me/guilds/{guildId}", true);

    public async Task<GuildProperties> GetGuildAsync(ulong guildId)
        => await MakeFluxerApiRequestAsync<GuildProperties>(HttpMethod.Get, $"/guilds/{guildId}", true);

    public async Task<GuildProperties> UpdateGuildAsync(ulong guildId, GuildProperties guild)
        => await MakeFluxerApiRequestAsync<GuildProperties, GuildProperties>(HttpMethod.Patch, $"/guilds/{guildId}", guild, true);

    public async Task DeleteGuildAsync(ulong guildId, GuildDeleteRequest data)
        => await MakeFluxerApiRequestAsync(HttpMethod.Post, $"/guilds/{guildId}/delete", data, true);

    public async Task<GuildVanityUrlResponse> GetGuildVanityUrlAsync(ulong guildId)
        => await MakeFluxerApiRequestAsync<GuildVanityUrlResponse>(HttpMethod.Get, $"/guilds/{guildId}/vanity-url", true);

    public async Task<GuildVanityUrlUpdateResponse> UpdateGuildVanityUrlAsync(ulong guildId, GuildVanityUrlUpdateRequest data)
        => await MakeFluxerApiRequestAsync<GuildVanityUrlUpdateResponse, GuildVanityUrlUpdateRequest>(HttpMethod.Patch, $"/guilds/{guildId}/vanity-url", data, true);

    public async Task<List<User>> GetMembersAsync(ulong guildId)
        => await MakeFluxerApiRequestListAsync<User>(HttpMethod.Get, $"/guilds/{guildId}/members", true);

    public async Task<GuildMember> GetCurrentMemberAsync(ulong guildId)
        => await MakeFluxerApiRequestAsync<GuildMember>(HttpMethod.Get, $"/guilds/{guildId}/members/@me", true);

    public async Task<User> GetMemberAsync(ulong guildId, ulong userId)
        => await MakeFluxerApiRequestAsync<User>(HttpMethod.Get, $"/guilds/{guildId}/members/{userId}", true);

    public async Task<GuildMember> UpdateCurrentMemberAsync(ulong guildId, GuildMember member)
        => await MakeFluxerApiRequestAsync<GuildMember, GuildMember>(HttpMethod.Patch, $"/guilds/{guildId}/members/@me", member, true);

    public async Task<GuildMember> UpdateMemberAsync(ulong guildId, ulong userId, GuildMember member)
        => await MakeFluxerApiRequestAsync<GuildMember, GuildMember>(HttpMethod.Patch, $"/guilds/{guildId}/members/{userId}", member, true);

    public async Task KickMemberAsync(ulong guildId, ulong userId)
        => await MakeFluxerApiRequestRawAsync(HttpMethod.Delete, $"/guilds/{guildId}/members/{userId}", true);

    public async Task TransferOwnershipAsync(ulong guildId, GuildTransferOwnershipRequest data)
        => await MakeFluxerApiRequestAsync(HttpMethod.Post, $"/guilds/{guildId}/transfer-ownership", data, true);

    public async Task<IReadOnlyCollection<GuildBanResponse>> GetBansAsync(ulong guildId)
        => await MakeFluxerApiRequestListAsync<GuildBanResponse>(HttpMethod.Get, $"/guilds/{guildId}/bans", true);

    public async Task BanMemberAsync(ulong guildId, ulong userId, GuildBanCreateRequest data)
        => await MakeFluxerApiRequestAsync(HttpMethod.Put, $"/guilds/{guildId}/bans/{userId}", data, true);

    public async Task UnbanMemberAsync(ulong guildId, ulong userId)
        => await MakeFluxerApiRequestRawAsync(HttpMethod.Delete, $"/guilds/{guildId}/bans/{userId}", true);

    public async Task AddMemberRoleAsync(ulong guildId, ulong userId, ulong roleId)
        => await MakeFluxerApiRequestRawAsync(HttpMethod.Put, $"/guilds/{guildId}/members/{userId}/roles/{roleId}", true);

    public async Task RemoveMemberRoleAsync(ulong guildId, ulong userId, ulong roleId)
        => await MakeFluxerApiRequestRawAsync(HttpMethod.Delete, $"/guilds/{guildId}/members/{userId}/roles/{roleId}", true);

    public async Task<GuildRole> CreateRoleAsync(ulong guildId, GuildRoleCreateRequest data)
        => await MakeFluxerApiRequestAsync<GuildRole, GuildRoleCreateRequest>(HttpMethod.Post, $"/guilds/{guildId}/roles", data, true);

    public async Task<GuildRole> UpdateRoleAsync(ulong guildId, ulong roleId, GuildRoleUpdateRequest data)
        => await MakeFluxerApiRequestAsync<GuildRole, GuildRoleUpdateRequest>(HttpMethod.Patch, $"/guilds/{guildId}/roles/{roleId}", data, true);

    public async Task UpdateRolePositionsAsync(ulong guildId, IEnumerable<GuildRolePositionItem> positions)
        => await MakeFluxerApiRequestAsync(HttpMethod.Patch, $"/guilds/{guildId}/roles", positions, true);

    public async Task DeleteRoleAsync(ulong guildId, ulong roleId)
        => await MakeFluxerApiRequestRawAsync(HttpMethod.Delete, $"/guilds/{guildId}/roles/{roleId}", true);

    public async Task<List<Channel>> GetChannelsAsync(ulong guildId)
        => await MakeFluxerApiRequestListAsync<Channel>(HttpMethod.Get, $"/guilds/{guildId}/channels", true);

    public async Task<Channel> CreateChannelAsync<TRequest>(ulong guildId, TRequest data)
        where TRequest : ChannelCreateRequest
        => await MakeFluxerApiRequestAsync<Channel, TRequest>(HttpMethod.Post, $"/guilds/{guildId}/channels", data, true);

    public async Task UpdateChannelPositionsAsync(ulong guildId, IEnumerable<ChannelPositionUpdateRequestItem> data)
        => await MakeFluxerApiRequestAsync(HttpMethod.Patch, $"/guilds/{guildId}/channels", data, true);

    public async Task<TResponse> SearchGuildAsync<TRequest, TResponse>(ulong guildId, TRequest data) where TResponse : Entity
        => await MakeFluxerApiRequestAsync<TResponse, TRequest>(HttpMethod.Post, $"/guilds/{guildId}/search", data, true);

    [Obsolete("API Endpoint no longer exists")]
    public async Task<TResponse> GetGuildAuditLogFiltersAsync<TResponse>(ulong guildId) where TResponse : Entity
        => await MakeFluxerApiRequestAsync<TResponse>(HttpMethod.Get, $"/guilds/{guildId}/audit-logs/filters", true);

    public async Task<GuildAuditLogListResponse> SearchAuditLogAsync(ulong guildId, GuildAuditLogListRequest data)
        => await MakeFluxerApiRequestAsync<GuildAuditLogListResponse, GuildAuditLogListRequest>(HttpMethod.Post, $"/guilds/{guildId}/audit-logs", data, true);

    public async Task<GuildEmojiResponse> CreateEmojiAsync(ulong guildId, GuildEmojiCreateRequest data)
        => await MakeFluxerApiRequestAsync<GuildEmojiResponse, GuildEmojiCreateRequest>(HttpMethod.Post, $"/guilds/{guildId}/emojis", data, true);

    public async Task<GuildEmojiBulkCreateResponse> CreateEmojiBulkAsync(ulong guildId, GuildEmojiBulkCreateRequest data)
        => await MakeFluxerApiRequestAsync<GuildEmojiBulkCreateResponse, GuildEmojiBulkCreateRequest>(HttpMethod.Post, $"/guilds/{guildId}/emojis/bulk", data, true);

    public async Task<IEnumerable<GuildEmojiWithUserResponse>> GetEmojisAsync(ulong guildId)
        => await MakeFluxerApiRequestListAsync<GuildEmojiWithUserResponse>(HttpMethod.Get, $"/guilds/{guildId}/emojis", true);

    public async Task<GuildEmojiResponse> UpdateEmojiAsync(ulong guildId, ulong emojiId, GuildEmojiUpdateRequest data)
        => await MakeFluxerApiRequestAsync<GuildEmojiResponse, GuildEmojiUpdateRequest>(HttpMethod.Patch, $"/guilds/{guildId}/emojis/{emojiId}", data, true);

    public async Task DeleteEmojiAsync(ulong guildId, ulong emojiId)
        => await MakeFluxerApiRequestRawAsync(HttpMethod.Delete, $"/guilds/{guildId}/emojis/{emojiId}", true);

    public async Task<GuildStickerResponse> CreateStickerAsync(ulong guildId, GuildStickerCreateRequest data)
        => await MakeFluxerApiRequestAsync<GuildStickerResponse, GuildStickerCreateRequest>(HttpMethod.Post, $"/guilds/{guildId}/stickers", data, true);

    public async Task<GuildStickerBulkCreateResponse> CreateStickerBulkAsync(ulong guildId, GuildStickerBulkCreateRequest data)
        => await MakeFluxerApiRequestAsync<GuildStickerBulkCreateResponse, GuildStickerBulkCreateRequest>(HttpMethod.Post, $"/guilds/{guildId}/stickers/bulk", data, true);

    public async Task<IEnumerable<GuildStickerWithUserResponse>> GetStickersAsync(ulong guildId)
        => await MakeFluxerApiRequestListAsync<GuildStickerWithUserResponse>(HttpMethod.Get, $"/guilds/{guildId}/stickers", true);

    public async Task<GuildStickerResponse> UpdateStickerAsync(ulong guildId, ulong stickerId, GuildStickerUpdateRequest data)
        => await MakeFluxerApiRequestAsync<GuildStickerResponse, GuildStickerUpdateRequest>(HttpMethod.Patch, $"/guilds/{guildId}/stickers/{stickerId}", data, true);

    public async Task DeleteStickerAsync(ulong guildId, ulong stickerId)
        => await MakeFluxerApiRequestRawAsync(HttpMethod.Delete, $"/guilds/{guildId}/stickers/{stickerId}", true);

    public async Task<List<Invite>> GetGuildInvitesAsync(ulong guildId)
        => await MakeFluxerApiRequestListAsync<Invite>(HttpMethod.Get, $"/guilds/{guildId}/invites", true);

    public async Task<IEnumerable<WebhookResponse>> GetGuildWebhooksAsync(ulong guildId)
        => await MakeFluxerApiRequestListAsync<WebhookResponse>(HttpMethod.Get, $"/guilds/{guildId}/webhooks", true);

    #endregion

    #region Tenor API

    public async Task<TResponse> GetTenorSearchAsync<TResponse>(string query) where TResponse : Entity
        => await MakeFluxerApiRequestAsync<TResponse>(HttpMethod.Get, $"/tenor/search?q={query}", true);

    public async Task<TResponse> GetTenorFeaturedAsync<TResponse>() where TResponse : Entity
        => await MakeFluxerApiRequestAsync<TResponse>(HttpMethod.Get, "/tenor/featured", true);

    public async Task<TResponse> GetTenorTrendingGifsAsync<TResponse>() where TResponse : Entity
        => await MakeFluxerApiRequestAsync<TResponse>(HttpMethod.Get, "/tenor/trending-gifs", true);

    public async Task PostTenorRegisterShareAsync<TRequest>(TRequest data)
        => await MakeFluxerApiRequestAsync<TRequest>(HttpMethod.Post, "/tenor/register-share", data, true);

    public async Task<TResponse> GetTenorSuggestAsync<TResponse>(string query) where TResponse : Entity
        => await MakeFluxerApiRequestAsync<TResponse>(HttpMethod.Get, $"/tenor/suggest?q={query}", true);

    #endregion

    #region Users API

    public async Task<User> GetCurrentUserAsync()
        => await MakeFluxerApiRequestAsync<User>(HttpMethod.Get, "/users/@me", true);

    public async Task<User> UpdateCurrentUserAsync(User user)
        => await MakeFluxerApiRequestAsync<User, User>(HttpMethod.Patch, "/users/@me", user, true);

    public async Task<TResponse> CheckUsernameAvailabilityAsync<TResponse>(string tag) where TResponse : Entity
        => await MakeFluxerApiRequestAsync<TResponse>(HttpMethod.Get, $"/users/check-tag?tag={tag}", true);

    public async Task<User> GetUserAsync(ulong userId)
        => await MakeFluxerApiRequestAsync<User>(HttpMethod.Get, $"/users/{userId}", true);

    public async Task<TResponse> GetUserProfileAsync<TResponse>(ulong targetId) where TResponse : Entity
        => await MakeFluxerApiRequestAsync<TResponse>(HttpMethod.Get, $"/users/{targetId}/profile", true);

    public async Task<UserSettings> GetCurrentUserSettingsAsync()
        => await MakeFluxerApiRequestAsync<UserSettings>(HttpMethod.Get, "/users/@me/settings", true);

    public async Task<UserSettings> UpdateCurrentUserSettingsAsync<TRequest>(TRequest settings)
        => await MakeFluxerApiRequestAsync<UserSettings, TRequest>(HttpMethod.Patch, "/users/@me/settings", settings, true);

    public async Task<UserSettings> SetCustomStatusAsync(UserCustomStatus status)
        => await MakeFluxerApiRequestAsync<UserSettings, ModifyCustomStatus>(HttpMethod.Patch, "/users/@me/settings", new ModifyCustomStatus(status), true);

    public async Task<TResponse> GetCurrentUserNotesAsync<TResponse>() where TResponse : Entity
        => await MakeFluxerApiRequestAsync<TResponse>(HttpMethod.Get, "/users/@me/notes", true);

    public async Task<TResponse> GetCurrentUserNoteAsync<TResponse>(ulong targetId) where TResponse : Entity
        => await MakeFluxerApiRequestAsync<TResponse>(HttpMethod.Get, $"/users/@me/notes/{targetId}", true);

    public async Task<TResponse> PutCurrentUserNoteAsync<TRequest, TResponse>(ulong targetId, TRequest data) where TResponse : Entity
        => await MakeFluxerApiRequestAsync<TResponse, TRequest>(HttpMethod.Put, $"/users/@me/notes/{targetId}", data, true);

    public async Task<TResponse> GetCurrentUserBetaCodesAsync<TResponse>() where TResponse : Entity
        => await MakeFluxerApiRequestAsync<TResponse>(HttpMethod.Get, "/users/@me/beta-codes", true);

    public async Task<TResponse> PostCurrentUserBetaCodeAsync<TRequest, TResponse>(TRequest data) where TResponse : Entity
        => await MakeFluxerApiRequestAsync<TResponse, TRequest>(HttpMethod.Post, "/users/@me/beta-codes", data, true);

    public async Task DeleteCurrentUserBetaCodeAsync(string code)
        => await MakeFluxerApiRequestRawAsync(HttpMethod.Delete, $"/users/@me/beta-codes/{code}", true);

    public async Task<TResponse> GetCurrentUserMentionsAsync<TResponse>() where TResponse : Entity
        => await MakeFluxerApiRequestAsync<TResponse>(HttpMethod.Get, "/users/@me/mentions", true);

    public async Task DeleteCurrentUserMentionAsync(ulong messageId)
        => await MakeFluxerApiRequestRawAsync(HttpMethod.Delete, $"/users/@me/mentions/{messageId}", true);

    public async Task<TResponse> PostCurrentUserMfaTotpEnableAsync<TRequest, TResponse>(TRequest data) where TResponse : Entity
        => await MakeFluxerApiRequestAsync<TResponse, TRequest>(HttpMethod.Post, "/users/@me/mfa/totp/enable", data, true);

    public async Task PostCurrentUserMfaTotpDisableAsync<TRequest>(TRequest data)
        => await MakeFluxerApiRequestAsync<TRequest>(HttpMethod.Post, "/users/@me/mfa/totp/disable", data, true);

    public async Task<TResponse> PostCurrentUserMfaBackupCodesAsync<TRequest, TResponse>(TRequest data) where TResponse : Entity
        => await MakeFluxerApiRequestAsync<TResponse, TRequest>(HttpMethod.Post, "/users/@me/mfa/backup-codes", data, true);

    public async Task PostCurrentUserPhoneSendVerificationAsync<TRequest>(TRequest data)
        => await MakeFluxerApiRequestAsync<TRequest>(HttpMethod.Post, "/users/@me/phone/send-verification", data, true);

    public async Task<TResponse> PostCurrentUserPhoneVerifyAsync<TRequest, TResponse>(TRequest data) where TResponse : Entity
        => await MakeFluxerApiRequestAsync<TResponse, TRequest>(HttpMethod.Post, "/users/@me/phone/verify", data, true);

    public async Task<TResponse> PostCurrentUserPhoneAsync<TRequest, TResponse>(TRequest data) where TResponse : Entity
        => await MakeFluxerApiRequestAsync<TResponse, TRequest>(HttpMethod.Post, "/users/@me/phone", data, true);

    public async Task DeleteCurrentUserPhoneAsync<TRequest>(TRequest data)
        => await MakeFluxerApiRequestAsync<TRequest>(HttpMethod.Delete, "/users/@me/phone", data, true);

    public async Task PostCurrentUserMfaSmsEnableAsync<TRequest>(TRequest data)
        => await MakeFluxerApiRequestAsync<TRequest>(HttpMethod.Post, "/users/@me/mfa/sms/enable", data, true);

    public async Task PostCurrentUserMfaSmsDisableAsync<TRequest>(TRequest data)
        => await MakeFluxerApiRequestAsync<TRequest>(HttpMethod.Post, "/users/@me/mfa/sms/disable", data, true);

    public async Task<TResponse> GetCurrentUserMfaWebauthnCredentialsAsync<TResponse>() where TResponse : Entity
        => await MakeFluxerApiRequestAsync<TResponse>(HttpMethod.Get, "/users/@me/mfa/webauthn/credentials", true);

    public async Task<TResponse> PostCurrentUserMfaWebauthnCredentialsRegistrationOptionsAsync<TResponse>() where TResponse : Entity
        => await MakeFluxerApiRequestAsync<TResponse>(HttpMethod.Post, "/users/@me/mfa/webauthn/credentials/registration-options", true);

    public async Task<TResponse> PostCurrentUserMfaWebauthnCredentialsAsync<TRequest, TResponse>(TRequest data) where TResponse : Entity
        => await MakeFluxerApiRequestAsync<TResponse, TRequest>(HttpMethod.Post, "/users/@me/mfa/webauthn/credentials", data, true);

    public async Task<TResponse> PatchCurrentUserMfaWebauthnCredentialAsync<TRequest, TResponse>(ulong credentialId, TRequest data) where TResponse : Entity
        => await MakeFluxerApiRequestAsync<TResponse, TRequest>(HttpMethod.Patch, $"/users/@me/mfa/webauthn/credentials/{credentialId}", data, true);

    public async Task DeleteCurrentUserMfaWebauthnCredentialAsync(ulong credentialId)
        => await MakeFluxerApiRequestRawAsync(HttpMethod.Delete, $"/users/@me/mfa/webauthn/credentials/{credentialId}", true);

    public async Task<TResponse> GetCurrentUserSavedMessagesAsync<TResponse>() where TResponse : Entity
        => await MakeFluxerApiRequestAsync<TResponse>(HttpMethod.Get, "/users/@me/saved-messages", true);

    public async Task<TResponse> PostCurrentUserSavedMessageAsync<TRequest, TResponse>(TRequest data) where TResponse : Entity
        => await MakeFluxerApiRequestAsync<TResponse, TRequest>(HttpMethod.Post, "/users/@me/saved-messages", data, true);

    public async Task DeleteCurrentUserSavedMessageAsync(ulong messageId)
        => await MakeFluxerApiRequestRawAsync(HttpMethod.Delete, $"/users/@me/saved-messages/{messageId}", true);

    public async Task<TResponse> GetCurrentUserChannelsAsync<TResponse>() where TResponse : Entity
        => await MakeFluxerApiRequestAsync<TResponse>(HttpMethod.Get, "/users/@me/channels", true);

    public async Task<TResponse> PostCurrentUserChannelAsync<TRequest, TResponse>(TRequest data) where TResponse : Entity
        => await MakeFluxerApiRequestAsync<TResponse, TRequest>(HttpMethod.Post, "/users/@me/channels", data, true);

    public async Task PutCurrentUserChannelPinAsync(ulong channelId)
        => await MakeFluxerApiRequestRawAsync(HttpMethod.Put, $"/users/@me/channels/{channelId}/pin", true);

    public async Task DeleteCurrentUserChannelPinAsync(ulong channelId)
        => await MakeFluxerApiRequestRawAsync(HttpMethod.Delete, $"/users/@me/channels/{channelId}/pin", true);

    public async Task<TResponse> GetCurrentUserRelationshipsAsync<TResponse>() where TResponse : Entity
        => await MakeFluxerApiRequestAsync<TResponse>(HttpMethod.Get, "/users/@me/relationships", true);

    public async Task<TResponse> PostCurrentUserRelationshipAsync<TRequest, TResponse>(TRequest data) where TResponse : Entity
        => await MakeFluxerApiRequestAsync<TResponse, TRequest>(HttpMethod.Post, "/users/@me/relationships", data, true);

    public async Task<TResponse> PostCurrentUserRelationshipWithUserAsync<TRequest, TResponse>(ulong userId, TRequest data) where TResponse : Entity
        => await MakeFluxerApiRequestAsync<TResponse, TRequest>(HttpMethod.Post, $"/users/@me/relationships/{userId}", data, true);

    public async Task PutCurrentUserRelationshipAsync<TRequest>(ulong userId, TRequest data)
        => await MakeFluxerApiRequestAsync<TRequest>(HttpMethod.Put, $"/users/@me/relationships/{userId}", data, true);

    public async Task DeleteCurrentUserRelationshipAsync(ulong userId)
        => await MakeFluxerApiRequestRawAsync(HttpMethod.Delete, $"/users/@me/relationships/{userId}", true);

    public async Task<TResponse> PatchCurrentUserGuildSettingsSelfAsync<TRequest, TResponse>(TRequest data) where TResponse : Entity
        => await MakeFluxerApiRequestAsync<TResponse, TRequest>(HttpMethod.Patch, "/users/@me/guilds/@me/settings", data, true);

    public async Task<TResponse> PatchCurrentUserGuildSettingsAsync<TRequest, TResponse>(ulong guildId, TRequest data) where TResponse : Entity
        => await MakeFluxerApiRequestAsync<TResponse, TRequest>(HttpMethod.Patch, $"/users/@me/guilds/{guildId}/settings", data, true);

    public async Task PostCurrentUserDisableAsync<TRequest>(TRequest data)
        => await MakeFluxerApiRequestAsync<TRequest>(HttpMethod.Post, "/users/@me/disable", data, true);

    public async Task PostCurrentUserDeleteAsync<TRequest>(TRequest data)
        => await MakeFluxerApiRequestAsync<TRequest>(HttpMethod.Post, "/users/@me/delete", data, true);

    public async Task<TResponse> PostCurrentUserPushSubscribeAsync<TRequest, TResponse>(TRequest data) where TResponse : Entity
        => await MakeFluxerApiRequestAsync<TResponse, TRequest>(HttpMethod.Post, "/users/@me/push/subscribe", data, true);

    public async Task<TResponse> GetCurrentUserPushSubscriptionsAsync<TResponse>() where TResponse : Entity
        => await MakeFluxerApiRequestAsync<TResponse>(HttpMethod.Get, "/users/@me/push/subscriptions", true);

    public async Task DeleteCurrentUserPushSubscriptionAsync(ulong subscriptionId)
        => await MakeFluxerApiRequestRawAsync(HttpMethod.Delete, $"/users/@me/push/subscriptions/{subscriptionId}", true);

    public async Task<TResponse> PostCurrentUserHarvestAsync<TRequest, TResponse>(TRequest data) where TResponse : Entity
        => await MakeFluxerApiRequestAsync<TResponse, TRequest>(HttpMethod.Post, "/users/@me/harvest", data, true);

    public async Task<TResponse> GetCurrentUserHarvestLatestAsync<TResponse>() where TResponse : Entity
        => await MakeFluxerApiRequestAsync<TResponse>(HttpMethod.Get, "/users/@me/harvest/latest", true);

    public async Task<TResponse> GetCurrentUserHarvestAsync<TResponse>(string harvestId) where TResponse : Entity
        => await MakeFluxerApiRequestAsync<TResponse>(HttpMethod.Get, $"/users/@me/harvest/{harvestId}", true);

    public async Task<TResponse> GetCurrentUserHarvestDownloadAsync<TResponse>(string harvestId) where TResponse : Entity
        => await MakeFluxerApiRequestAsync<TResponse>(HttpMethod.Get, $"/users/@me/harvest/{harvestId}/download", true);

    public async Task PostCurrentUserPreloadMessagesAsync<TRequest>(TRequest data)
        => await MakeFluxerApiRequestAsync<TRequest>(HttpMethod.Post, "/users/@me/preload-messages", data, true);

    public async Task PostCurrentUserMessagesDeleteAsync<TRequest>(TRequest data)
        => await MakeFluxerApiRequestAsync<TRequest>(HttpMethod.Post, "/users/@me/messages/delete", data, true);

    #endregion

    #region Webhooks API

    public async Task<TResponse> GetWebhookAsync<TResponse>(ulong webhookId) where TResponse : Entity
        => await MakeFluxerApiRequestAsync<TResponse>(HttpMethod.Get, $"/webhooks/{webhookId}", true);

    public async Task<TResponse> UpdateWebhookAsync<TRequest, TResponse>(ulong webhookId, TRequest data) where TResponse : Entity
        => await MakeFluxerApiRequestAsync<TResponse, TRequest>(HttpMethod.Patch, $"/webhooks/{webhookId}", data, true);

    public async Task DeleteWebhookAsync(ulong webhookId)
        => await MakeFluxerApiRequestRawAsync(HttpMethod.Delete, $"/webhooks/{webhookId}", true);

    public async Task<TResponse> GetWebhookWithTokenAsync<TResponse>(ulong webhookId, string token) where TResponse : Entity
        => await MakeFluxerApiRequestAsync<TResponse>(HttpMethod.Get, $"/webhooks/{webhookId}/{token}", false);

    public async Task<TResponse> UpdateWebhookWithTokenAsync<TRequest, TResponse>(ulong webhookId, string token, TRequest data) where TResponse : Entity
        => await MakeFluxerApiRequestAsync<TResponse, TRequest>(HttpMethod.Patch, $"/webhooks/{webhookId}/{token}", data, false);

    public async Task DeleteWebhookWithTokenAsync(ulong webhookId, string token)
        => await MakeFluxerApiRequestRawAsync(HttpMethod.Delete, $"/webhooks/{webhookId}/{token}", false, false);

    public async Task ExecuteWebhookAsync<TRequest>(ulong webhookId, string token, TRequest data)
        => await MakeFluxerApiRequestAsync<TRequest>(HttpMethod.Post, $"/webhooks/{webhookId}/{token}", data, true, false);

    public async Task ExecuteGithubWebhookAsync<TRequest>(ulong webhookId, string token, TRequest data)
        => await MakeFluxerApiRequestAsync<TRequest>(HttpMethod.Post, $"/webhooks/{webhookId}/{token}/github", data, true, false);

    public async Task PostWebhookLivekitAsync<TRequest>(TRequest data)
        => await MakeFluxerApiRequestAsync<TRequest>(HttpMethod.Post, "/webhooks/livekit", data, true, false);

    public async Task PostWebhookSendgridAsync<TRequest>(TRequest data)
        => await MakeFluxerApiRequestAsync<TRequest>(HttpMethod.Post, "/webhooks/sendgrid", data, true, false);

    #endregion

    #region Stripe API

    public async Task PostStripeWebhookAsync<TRequest>(TRequest data)
        => await MakeFluxerApiRequestAsync<TRequest>(HttpMethod.Post, "/stripe/webhook", data, true, false);

    public async Task<TResponse> PostStripeCheckoutSubscriptionAsync<TRequest, TResponse>(TRequest data) where TResponse : Entity
        => await MakeFluxerApiRequestAsync<TResponse, TRequest>(HttpMethod.Post, "/stripe/checkout/subscription", data, true);

    public async Task<TResponse> PostStripeCheckoutGiftAsync<TRequest, TResponse>(TRequest data) where TResponse : Entity
        => await MakeFluxerApiRequestAsync<TResponse, TRequest>(HttpMethod.Post, "/stripe/checkout/gift", data, true);

    #endregion

    #region Gifts API

    public async Task<TResponse> GetGiftAsync<TResponse>(string code) where TResponse : Entity
        => await MakeFluxerApiRequestAsync<TResponse>(HttpMethod.Get, $"/gifts/{code}", true);

    public async Task<TResponse> PostGiftRedeemAsync<TRequest, TResponse>(string code, TRequest data) where TResponse : Entity
        => await MakeFluxerApiRequestAsync<TResponse, TRequest>(HttpMethod.Post, $"/gifts/{code}/redeem", data, true);

    public async Task<TResponse> GetCurrentUserGiftsAsync<TResponse>() where TResponse : Entity
        => await MakeFluxerApiRequestAsync<TResponse>(HttpMethod.Get, "/users/@me/gifts", true);

    #endregion

    #region Premium API

    public async Task<TResponse> GetPremiumVisionarySlotsAsync<TResponse>() where TResponse : Entity
        => await MakeFluxerApiRequestAsync<TResponse>(HttpMethod.Get, "/premium/visionary/slots", true);

    public async Task<TResponse> GetPremiumPriceIdsAsync<TResponse>() where TResponse : Entity
        => await MakeFluxerApiRequestAsync<TResponse>(HttpMethod.Get, "/premium/price-ids", true);

    public async Task<TResponse> PostPremiumCustomerPortalAsync<TRequest, TResponse>(TRequest data) where TResponse : Entity
        => await MakeFluxerApiRequestAsync<TResponse, TRequest>(HttpMethod.Post, "/premium/customer-portal", data, true);

    public async Task PostPremiumCancelSubscriptionAsync<TRequest>(TRequest data)
        => await MakeFluxerApiRequestAsync<TRequest>(HttpMethod.Post, "/premium/cancel-subscription", data, true);

    public async Task PostPremiumReactivateSubscriptionAsync()
        => await MakeFluxerApiRequestRawAsync(HttpMethod.Post, "/premium/reactivate-subscription", true);

    public async Task PostPremiumVisionaryRejoinAsync()
        => await MakeFluxerApiRequestRawAsync(HttpMethod.Post, "/premium/visionary/rejoin", true);

    public async Task PostPremiumOperatorRejoinAsync()
        => await MakeFluxerApiRequestRawAsync(HttpMethod.Post, "/premium/operator/rejoin", true);

    #endregion

    #region Misc API

    public async Task<TResponse> PostRpcAsync<TRequest, TResponse>(TRequest data) where TResponse : Entity
        => await MakeFluxerApiRequestAsync<TResponse, TRequest>(HttpMethod.Post, "/_rpc", data, true);

    #endregion
}
