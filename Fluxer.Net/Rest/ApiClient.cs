using Fluxer.Net.Data.AuditLogs;
using Fluxer.Net.Data.Auth;
using Fluxer.Net.Data.Channels;
using Fluxer.Net.Data.Guilds;
using Fluxer.Net.Data.Invites;
using Fluxer.Net.Data.Messages;
using Fluxer.Net.Data.Users;
using Fluxer.Net.Data.Voice;
using Fluxer.Net.Data.Webhooks;
using Fluxer.Net.Extensions;
using Fluxer.Net.Gateway.Data;
using Fluxer.Net.RateLimiting;
using Fluxer.Net.Rest.Requests;
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
    private async Task WaitForRateLimit(RateLimitConfig config, ulong? channelId = null, ulong? guildId = null, ulong? userId = null, ulong? webhookId = null, string inviteCode = null)
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
    public async Task<TResponse> MakeFluxerApiRequestRS<TResponse, TSend>(HttpMethod method, string route, TSend data, bool throwOnNonSuccess = false, bool authorize = true,
        ICollection<KeyValuePair<string, (HttpContent content, string? filename)>>? otherFormData = null)
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
    public async Task<HttpStatusCode> MakeFluxerApiRequestS<TSend>(HttpMethod method, string route, TSend data, bool throwOnNonSuccess = false, bool authorize = true)
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
    public async Task<TResponse> MakeFluxerApiRequestR<TResponse>(HttpMethod method, string route, bool throwOnNonSuccess = false, bool authorize = true)
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
    public async Task<HttpStatusCode> MakeFluxerApiRequest(HttpMethod method, string route, bool throwOnNonSuccess = false, bool authorize = true)
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

    public async Task<LoginResponse> Login(LoginRequest data)
        => await MakeFluxerApiRequestRS<LoginResponse, LoginRequest>(HttpMethod.Post, "/auth/login", data, false);

    public async Task Register<TRequest>(TRequest data)
        => await MakeFluxerApiRequestS<TRequest>(HttpMethod.Post, "/auth/register", data, true, false);

    public async Task LoginMfaTotp<TRequest, TResponse>(TRequest data)
        => await MakeFluxerApiRequestRS<TResponse, TRequest>(HttpMethod.Post, "/auth/login/mfa/totp", data, true, false);

    public async Task SendMfaSmsCode()
        => await MakeFluxerApiRequest(HttpMethod.Post, "/auth/login/mfa/sms/send", true, false);

    public async Task LoginMfaSms<TRequest, TResponse>(TRequest data)
        => await MakeFluxerApiRequestRS<TResponse, TRequest>(HttpMethod.Post, "/auth/login/mfa/sms", data, true, false);

    public async Task Logout()
        => await MakeFluxerApiRequest(HttpMethod.Post, "/auth/logout", true);

    public async Task VerifyEmail<TRequest>(TRequest data)
        => await MakeFluxerApiRequestS<TRequest>(HttpMethod.Post, "/auth/verify", data, true, false);

    public async Task ResendVerificationEmail()
        => await MakeFluxerApiRequest(HttpMethod.Post, "/auth/verify/resend", true);

    public async Task ForgotPassword<TRequest>(TRequest data)
        => await MakeFluxerApiRequestS<TRequest>(HttpMethod.Post, "/auth/forgot", data, true, false);

    public async Task ResetPassword<TRequest>(TRequest data)
        => await MakeFluxerApiRequestS<TRequest>(HttpMethod.Post, "/auth/reset", data, true, false);

    public async Task<TResponse> GetSessions<TResponse>()
        => await MakeFluxerApiRequestR<TResponse>(HttpMethod.Get, "/auth/sessions", true);

    public async Task LogoutSessions<TRequest>(TRequest data)
        => await MakeFluxerApiRequestS<TRequest>(HttpMethod.Post, "/auth/sessions/logout", data, true);

    public async Task PostAuthAuthorizeIp<TRequest>(TRequest data)
        => await MakeFluxerApiRequestS<TRequest>(HttpMethod.Post, "/auth/authorize-ip", data, true);

    public async Task<TResponse> PostAuthWebauthnAuthenticationOptions<TResponse>()
        => await MakeFluxerApiRequestR<TResponse>(HttpMethod.Post, "/auth/webauthn/authentication-options", true, false);

    public async Task<TResponse> PostAuthWebauthnAuthenticate<TRequest, TResponse>(TRequest data)
        => await MakeFluxerApiRequestRS<TResponse, TRequest>(HttpMethod.Post, "/auth/webauthn/authenticate", data, true, false);

    public async Task<TResponse> PostAuthLoginMfaWebauthnAuthenticationOptions<TResponse>()
        => await MakeFluxerApiRequestR<TResponse>(HttpMethod.Post, "/auth/login/mfa/webauthn/authentication-options", true, false);

    public async Task<TResponse> PostAuthLoginMfaWebauthn<TRequest, TResponse>(TRequest data)
        => await MakeFluxerApiRequestRS<TResponse, TRequest>(HttpMethod.Post, "/auth/login/mfa/webauthn", data, true, false);

    public async Task<TResponse> PostAuthRedeemBetaCode<TRequest, TResponse>(TRequest data)
        => await MakeFluxerApiRequestRS<TResponse, TRequest>(HttpMethod.Post, "/auth/redeem-beta-code", data, true);

    #endregion

    #region Channels API

    public async Task<Channel> GetChannel(ulong channelId)
        => await MakeFluxerApiRequestR<Channel>(HttpMethod.Get, $"/channels/{channelId}", true);

    public async Task<TResponse> GetChannelRtcRegions<TResponse>(ulong channelId)
        => await MakeFluxerApiRequestR<TResponse>(HttpMethod.Get, $"/channels/{channelId}/rtc-regions", true);

    public async Task<Channel> UpdateChannel(ulong channelId, Channel channel)
        => await MakeFluxerApiRequestRS<Channel, Channel>(HttpMethod.Patch, $"/channels/{channelId}", channel, true);

    public async Task DeleteChannel(ulong channelId)
        => await MakeFluxerApiRequest(HttpMethod.Delete, $"/channels/{channelId}", true);

    public async Task ClearMessageAcknowledgement(ulong channelId)
        => await MakeFluxerApiRequest(HttpMethod.Delete, $"/channels/{channelId}/messages/ack", true);

    public async Task<List<MessageBaseResponse>> GetMessages(ulong channelId)
        => await MakeFluxerApiRequestR<List<MessageBaseResponse>>(HttpMethod.Get, $"/channels/{channelId}/messages", true);

    public async Task<MessageBaseResponse> GetMessage(ulong channelId, ulong messageId)
        => await MakeFluxerApiRequestR<MessageBaseResponse>(HttpMethod.Get, $"/channels/{channelId}/messages/{messageId}", true);

    public async Task<TResponse> SearchChannel<TRequest, TResponse>(ulong channelId, TRequest data)
        => await MakeFluxerApiRequestRS<TResponse, TRequest>(HttpMethod.Post, $"/channels/{channelId}/search", data, true);

    public async Task<MessageBaseResponse> SendMessage(ulong channelId, Message message, StreamAttachment[]? attachments = null)
    {
        if ((attachments?.Length ?? 0) < 1)
        {
            return await MakeFluxerApiRequestRS<MessageBaseResponse, Message>(HttpMethod.Post, $"/channels/{channelId}/messages", message, true);
        }
        var form = new List<KeyValuePair<string, (HttpContent content, string? filename)>>();
        for (int i = 0; i < attachments.Length; i++)
        {
            attachments[i].Id = (ulong)i;
            form.Add(new KeyValuePair<string, (HttpContent content, string? filename)>($"file[{i}]", (new StreamContent(attachments[i].Stream), attachments[i].Filename)));
        }
        message.Attachments = attachments.Cast<Attachment>().ToList();
        return await MakeFluxerApiRequestRS<MessageBaseResponse, Message>(HttpMethod.Post, $"/channels/{channelId}/messages", message, true);
    }

    public async Task<MessageBaseResponse> EditMessage(ulong channelId, ulong messageId, MessageUpdateRequest message)
        => await MakeFluxerApiRequestRS<MessageBaseResponse, MessageUpdateRequest>(HttpMethod.Patch, $"/channels/{channelId}/messages/{messageId}", message, true);

    public async Task DeleteMessage(ulong channelId, ulong messageId)
        => await MakeFluxerApiRequest(HttpMethod.Delete, $"/channels/{channelId}/messages/{messageId}", true);

    public async Task DeleteMessageAttachment(ulong channelId, ulong messageId, ulong attachmentId)
        => await MakeFluxerApiRequest(HttpMethod.Delete, $"/channels/{channelId}/messages/{messageId}/attachments/{attachmentId}", true);

    public async Task BulkDeleteMessages(ulong channelId, BulkDeleteMessagesRequest data)
        => await MakeFluxerApiRequestS(HttpMethod.Post, $"/channels/{channelId}/messages/bulk-delete", data, true);

    public async Task TriggerTypingIndicator(ulong channelId)
        => await MakeFluxerApiRequest(HttpMethod.Post, $"/channels/{channelId}/typing", true);

    public async Task AcknowledgeMessage(ulong channelId, ulong messageId, MessageAck details)
        => await MakeFluxerApiRequestS<MessageAck>(HttpMethod.Post, $"/channels/{channelId}/messages/{messageId}/ack", details, true);

    public async Task<ChannelPinsResponse> GetPinnedMessages(ulong channelId, ChannelPinsQuery? query = null)
        => await MakeFluxerApiRequestR<ChannelPinsResponse>(HttpMethod.Get, $"/channels/{channelId}/pins?{query?.BuildQuery() ?? string.Empty}", true);

    public async Task PinMessage(ulong channelId, ulong messageId)
        => await MakeFluxerApiRequest(HttpMethod.Put, $"/channels/{channelId}/pins/{messageId}", true);

    public async Task UnpinMessage(ulong channelId, ulong messageId)
        => await MakeFluxerApiRequest(HttpMethod.Delete, $"/channels/{channelId}/pins/{messageId}", true);

    public async Task<List<UserPartialResponse>> GetReactions(ulong channelId, ulong messageId, string emoji)
        => await MakeFluxerApiRequestR<List<UserPartialResponse>>(HttpMethod.Get, $"/channels/{channelId}/messages/{messageId}/reactions/{emoji}", true);

    public async Task AddReaction(ulong channelId, ulong messageId, string emoji)
        => await MakeFluxerApiRequest(HttpMethod.Put, $"/channels/{channelId}/messages/{messageId}/reactions/{emoji}/@me", true);

    public async Task RemoveOwnReaction(ulong channelId, ulong messageId, string emoji)
        => await MakeFluxerApiRequest(HttpMethod.Delete, $"/channels/{channelId}/messages/{messageId}/reactions/{emoji}/@me", true);

    public async Task RemoveUserReaction(ulong channelId, ulong messageId, string emoji, ulong targetId)
        => await MakeFluxerApiRequest(HttpMethod.Delete, $"/channels/{channelId}/messages/{messageId}/reactions/{emoji}/{targetId}", true);

    public async Task RemoveAllReactionsForEmoji(ulong channelId, ulong messageId, string emoji)
        => await MakeFluxerApiRequest(HttpMethod.Delete, $"/channels/{channelId}/messages/{messageId}/reactions/{emoji}", true);

    public async Task RemoveAllReactions(ulong channelId, ulong messageId)
        => await MakeFluxerApiRequest(HttpMethod.Delete, $"/channels/{channelId}/messages/{messageId}/reactions", true);

    public async Task<TResponse> UploadAttachments<TRequest, TResponse>(ulong channelId, TRequest data)
        => await MakeFluxerApiRequestRS<TResponse, TRequest>(HttpMethod.Post, $"/channels/{channelId}/attachments", data, true);

    public async Task AddRecipient(ulong channelId, ulong userId)
        => await MakeFluxerApiRequest(HttpMethod.Put, $"/channels/{channelId}/recipients/{userId}", true);

    public async Task RemoveRecipient(ulong channelId, ulong userId)
        => await MakeFluxerApiRequest(HttpMethod.Delete, $"/channels/{channelId}/recipients/{userId}", true);

    public async Task<CallEligibilityResponse> GetCall(ulong channelId)
        => await MakeFluxerApiRequestR<CallEligibilityResponse>(HttpMethod.Get, $"/channels/{channelId}/call", true);

    public async Task<TResponse> UpdateCall<TRequest, TResponse>(ulong channelId, TRequest data)
        => await MakeFluxerApiRequestRS<TResponse, TRequest>(HttpMethod.Patch, $"/channels/{channelId}/call", data, true);

    public async Task RingCall<TRequest>(ulong channelId, TRequest data)
        => await MakeFluxerApiRequestS<TRequest>(HttpMethod.Post, $"/channels/{channelId}/call/ring", data, true);

    public async Task StopRinging(ulong channelId)
        => await MakeFluxerApiRequest(HttpMethod.Post, $"/channels/{channelId}/call/stop-ringing", true);

    public async Task<List<Invite>> GetChannelInvites(ulong channelId)
        => await MakeFluxerApiRequestR<List<Invite>>(HttpMethod.Get, $"/channels/{channelId}/invites", true);

    public async Task<TResponse> CreateInvite<TRequest, TResponse>(ulong channelId, TRequest data)
        => await MakeFluxerApiRequestRS<TResponse, TRequest>(HttpMethod.Post, $"/channels/{channelId}/invites", data, true);

    public async Task<TResponse> GetChannelWebhooks<TResponse>(ulong channelId)
        => await MakeFluxerApiRequestR<TResponse>(HttpMethod.Get, $"/channels/{channelId}/webhooks", true);

    public async Task<TResponse> CreateWebhook<TRequest, TResponse>(ulong channelId, TRequest data)
        => await MakeFluxerApiRequestRS<TResponse, TRequest>(HttpMethod.Post, $"/channels/{channelId}/webhooks", data, true);

    #endregion

    #region Attachments API

    public async Task DeleteAttachment(string uploadFilename)
        => await MakeFluxerApiRequest(HttpMethod.Delete, $"/attachments/{uploadFilename}", true);

    #endregion

    #region Memes API

    public async Task<TResponse> GetCurrentUserMemes<TResponse>()
        => await MakeFluxerApiRequestR<TResponse>(HttpMethod.Get, "/users/@me/memes", true);

    public async Task<TResponse> PostCurrentUserMeme<TRequest, TResponse>(TRequest data)
        => await MakeFluxerApiRequestRS<TResponse, TRequest>(HttpMethod.Post, "/users/@me/memes", data, true);

    public async Task PostChannelMessageMeme<TRequest>(ulong channelId, ulong messageId, TRequest data)
        => await MakeFluxerApiRequestS<TRequest>(HttpMethod.Post, $"/channels/{channelId}/messages/{messageId}/memes", data, true);

    public async Task<TResponse> GetCurrentUserMeme<TResponse>(ulong memeId)
        => await MakeFluxerApiRequestR<TResponse>(HttpMethod.Get, $"/users/@me/memes/{memeId}", true);

    public async Task<TResponse> PatchCurrentUserMeme<TRequest, TResponse>(ulong memeId, TRequest data)
        => await MakeFluxerApiRequestRS<TResponse, TRequest>(HttpMethod.Patch, $"/users/@me/memes/{memeId}", data, true);

    public async Task DeleteCurrentUserMeme(ulong memeId)
        => await MakeFluxerApiRequest(HttpMethod.Delete, $"/users/@me/memes/{memeId}", true);

    #endregion

    #region Invites API

    public async Task<TResponse> GetInvite<TResponse>(string inviteCode)
        => await MakeFluxerApiRequestR<TResponse>(HttpMethod.Get, $"/invites/{inviteCode}", true);

    public async Task<GuildProperties> JoinGuild(string inviteCode)
        => await MakeFluxerApiRequestR<GuildProperties>(HttpMethod.Post, $"/invites/{inviteCode}", true);

    public async Task DeleteInvite(string inviteCode)
        => await MakeFluxerApiRequest(HttpMethod.Delete, $"/invites/{inviteCode}", true);

    #endregion

    #region Read States API

    public async Task PostReadStatesAckBulk<TRequest>(TRequest data)
        => await MakeFluxerApiRequestS<TRequest>(HttpMethod.Post, "/read-states/ack-bulk", data, true);

    #endregion

    #region Reports API

    public async Task PostReportMessage<TRequest>(TRequest data)
        => await MakeFluxerApiRequestS<TRequest>(HttpMethod.Post, "/reports/message", data, true);

    public async Task PostReportUser<TRequest>(TRequest data)
        => await MakeFluxerApiRequestS<TRequest>(HttpMethod.Post, "/reports/user", data, true);

    public async Task PostReportGuild<TRequest>(TRequest data)
        => await MakeFluxerApiRequestS<TRequest>(HttpMethod.Post, "/reports/guild", data, true);

    #endregion

    #region Guilds API

    public async Task<GuildProperties> CreateGuild(GuildCreateRequest data)
        => await MakeFluxerApiRequestRS<GuildProperties, GuildCreateRequest>(HttpMethod.Post, "/guilds", data, true);

    public async Task<List<GuildProperties>> GetCurrentUserGuilds()
        => await MakeFluxerApiRequestR<List<GuildProperties>>(HttpMethod.Get, "/users/@me/guilds", true);

    public async Task LeaveGuild(ulong guildId)
        => await MakeFluxerApiRequest(HttpMethod.Delete, $"/users/@me/guilds/{guildId}", true);

    public async Task<GuildProperties> GetGuild(ulong guildId)
        => await MakeFluxerApiRequestR<GuildProperties>(HttpMethod.Get, $"/guilds/{guildId}", true);

    public async Task<GuildProperties> UpdateGuild(ulong guildId, GuildProperties guild)
        => await MakeFluxerApiRequestRS<GuildProperties, GuildProperties>(HttpMethod.Patch, $"/guilds/{guildId}", guild, true);

    public async Task DeleteGuild(ulong guildId, GuildDeleteRequest data)
        => await MakeFluxerApiRequestS(HttpMethod.Post, $"/guilds/{guildId}/delete", data, true);

    public async Task<GuildVanityUrlResponse> GetGuildVanityUrl(ulong guildId)
        => await MakeFluxerApiRequestR<GuildVanityUrlResponse>(HttpMethod.Get, $"/guilds/{guildId}/vanity-url", true);

    public async Task<GuildVanityUrlUpdateResponse> UpdateGuildVanityUrl(ulong guildId, GuildVanityUrlUpdateRequest data)
        => await MakeFluxerApiRequestRS<GuildVanityUrlUpdateResponse, GuildVanityUrlUpdateRequest>(HttpMethod.Patch, $"/guilds/{guildId}/vanity-url", data, true);

    public async Task<List<User>> GetMembers(ulong guildId)
        => await MakeFluxerApiRequestR<List<User>>(HttpMethod.Get, $"/guilds/{guildId}/members", true);

    public async Task<GuildMember> GetCurrentMember(ulong guildId)
        => await MakeFluxerApiRequestR<GuildMember>(HttpMethod.Get, $"/guilds/{guildId}/members/@me", true);

    public async Task<User> GetMember(ulong guildId, ulong userId)
        => await MakeFluxerApiRequestR<User>(HttpMethod.Get, $"/guilds/{guildId}/members/{userId}", true);

    public async Task<GuildMember> UpdateCurrentMember(ulong guildId, GuildMember member)
        => await MakeFluxerApiRequestRS<GuildMember, GuildMember>(HttpMethod.Patch, $"/guilds/{guildId}/members/@me", member, true);

    public async Task<GuildMember> UpdateMember(ulong guildId, ulong userId, GuildMember member)
        => await MakeFluxerApiRequestRS<GuildMember, GuildMember>(HttpMethod.Patch, $"/guilds/{guildId}/members/{userId}", member, true);

    public async Task KickMember(ulong guildId, ulong userId)
        => await MakeFluxerApiRequest(HttpMethod.Delete, $"/guilds/{guildId}/members/{userId}", true);

    public async Task TransferOwnership(ulong guildId, GuildTransferOwnershipRequest data)
        => await MakeFluxerApiRequestS(HttpMethod.Post, $"/guilds/{guildId}/transfer-ownership", data, true);

    public async Task<IReadOnlyCollection<GuildBanResponse>> GetBans(ulong guildId)
        => await MakeFluxerApiRequestR<IReadOnlyCollection<GuildBanResponse>>(HttpMethod.Get, $"/guilds/{guildId}/bans", true);

    public async Task BanMember(ulong guildId, ulong userId, GuildBanCreateRequest data)
        => await MakeFluxerApiRequestS(HttpMethod.Put, $"/guilds/{guildId}/bans/{userId}", data, true);

    public async Task UnbanMember(ulong guildId, ulong userId)
        => await MakeFluxerApiRequest(HttpMethod.Delete, $"/guilds/{guildId}/bans/{userId}", true);

    public async Task AddMemberRole(ulong guildId, ulong userId, ulong roleId)
        => await MakeFluxerApiRequest(HttpMethod.Put, $"/guilds/{guildId}/members/{userId}/roles/{roleId}", true);

    public async Task RemoveMemberRole(ulong guildId, ulong userId, ulong roleId)
        => await MakeFluxerApiRequest(HttpMethod.Delete, $"/guilds/{guildId}/members/{userId}/roles/{roleId}", true);

    public async Task<GuildRole> CreateRole(ulong guildId, GuildRoleCreateRequest data)
        => await MakeFluxerApiRequestRS<GuildRole, GuildRoleCreateRequest>(HttpMethod.Post, $"/guilds/{guildId}/roles", data, true);

    public async Task<GuildRole> UpdateRole(ulong guildId, ulong roleId, GuildRoleUpdateRequest data)
        => await MakeFluxerApiRequestRS<GuildRole, GuildRoleUpdateRequest>(HttpMethod.Patch, $"/guilds/{guildId}/roles/{roleId}", data, true);

    public async Task UpdateRolePositions(ulong guildId, IEnumerable<GuildRolePositionItem> positions)
        => await MakeFluxerApiRequestS(HttpMethod.Patch, $"/guilds/{guildId}/roles", positions, true);

    public async Task DeleteRole(ulong guildId, ulong roleId)
        => await MakeFluxerApiRequest(HttpMethod.Delete, $"/guilds/{guildId}/roles/{roleId}", true);

    public async Task<List<Channel>> GetChannels(ulong guildId)
        => await MakeFluxerApiRequestR<List<Channel>>(HttpMethod.Get, $"/guilds/{guildId}/channels", true);

    public async Task<Channel> CreateChannel<TRequest>(ulong guildId, TRequest data)
        where TRequest : ChannelCreateRequest
        => await MakeFluxerApiRequestRS<Channel, TRequest>(HttpMethod.Post, $"/guilds/{guildId}/channels", data, true);

    public async Task UpdateChannelPositions(ulong guildId, IEnumerable<ChannelPositionUpdateRequestItem> data)
        => await MakeFluxerApiRequestS(HttpMethod.Patch, $"/guilds/{guildId}/channels", data, true);

    public async Task<TResponse> SearchGuild<TRequest, TResponse>(ulong guildId, TRequest data)
        => await MakeFluxerApiRequestRS<TResponse, TRequest>(HttpMethod.Post, $"/guilds/{guildId}/search", data, true);

    [Obsolete("API Endpoint no longer exists")]
    public async Task<TResponse> GetGuildAuditLogFilters<TResponse>(ulong guildId)
        => await MakeFluxerApiRequestR<TResponse>(HttpMethod.Get, $"/guilds/{guildId}/audit-logs/filters", true);

    public async Task<GuildAuditLogListResponse> SearchAuditLog(ulong guildId, GuildAuditLogListRequest data)
        => await MakeFluxerApiRequestRS<GuildAuditLogListResponse, GuildAuditLogListRequest>(HttpMethod.Post, $"/guilds/{guildId}/audit-logs", data, true);

    public async Task<GuildEmojiResponse> CreateEmoji(ulong guildId, GuildEmojiCreateRequest data)
        => await MakeFluxerApiRequestRS<GuildEmojiResponse, GuildEmojiCreateRequest>(HttpMethod.Post, $"/guilds/{guildId}/emojis", data, true);

    public async Task<GuildEmojiBulkCreateResponse> CreateEmojiBulk(ulong guildId, GuildEmojiBulkCreateRequest data)
        => await MakeFluxerApiRequestRS<GuildEmojiBulkCreateResponse, GuildEmojiBulkCreateRequest>(HttpMethod.Post, $"/guilds/{guildId}/emojis/bulk", data, true);

    public async Task<IEnumerable<GuildEmojiWithUserResponse>> GetEmojis(ulong guildId)
        => await MakeFluxerApiRequestR<IEnumerable<GuildEmojiWithUserResponse>>(HttpMethod.Get, $"/guilds/{guildId}/emojis", true);

    public async Task<GuildEmojiResponse> UpdateEmoji(ulong guildId, ulong emojiId, GuildEmojiUpdateRequest data)
        => await MakeFluxerApiRequestRS<GuildEmojiResponse, GuildEmojiUpdateRequest>(HttpMethod.Patch, $"/guilds/{guildId}/emojis/{emojiId}", data, true);

    public async Task DeleteEmoji(ulong guildId, ulong emojiId)
        => await MakeFluxerApiRequest(HttpMethod.Delete, $"/guilds/{guildId}/emojis/{emojiId}", true);

    public async Task<GuildStickerResponse> CreateSticker(ulong guildId, GuildStickerCreateRequest data)
        => await MakeFluxerApiRequestRS<GuildStickerResponse, GuildStickerCreateRequest>(HttpMethod.Post, $"/guilds/{guildId}/stickers", data, true);

    public async Task<GuildStickerBulkCreateResponse> CreateStickerBulk(ulong guildId, GuildStickerBulkCreateRequest data)
        => await MakeFluxerApiRequestRS<GuildStickerBulkCreateResponse, GuildStickerBulkCreateRequest>(HttpMethod.Post, $"/guilds/{guildId}/stickers/bulk", data, true);

    public async Task<IEnumerable<GuildStickerWithUserResponse>> GetStickers(ulong guildId)
        => await MakeFluxerApiRequestR<IEnumerable<GuildStickerWithUserResponse>>(HttpMethod.Get, $"/guilds/{guildId}/stickers", true);

    public async Task<GuildStickerResponse> UpdateSticker(ulong guildId, ulong stickerId, GuildStickerUpdateRequest data)
        => await MakeFluxerApiRequestRS<GuildStickerResponse, GuildStickerUpdateRequest>(HttpMethod.Patch, $"/guilds/{guildId}/stickers/{stickerId}", data, true);

    public async Task DeleteSticker(ulong guildId, ulong stickerId)
        => await MakeFluxerApiRequest(HttpMethod.Delete, $"/guilds/{guildId}/stickers/{stickerId}", true);

    public async Task<List<Invite>> GetGuildInvites(ulong guildId)
        => await MakeFluxerApiRequestR<List<Invite>>(HttpMethod.Get, $"/guilds/{guildId}/invites", true);

    public async Task<IEnumerable<WebhookResponse>> GetGuildWebhooks(ulong guildId)
        => await MakeFluxerApiRequestR<IEnumerable<WebhookResponse>>(HttpMethod.Get, $"/guilds/{guildId}/webhooks", true);

    #endregion

    #region Tenor API

    public async Task<TResponse> GetTenorSearch<TResponse>(string query)
        => await MakeFluxerApiRequestR<TResponse>(HttpMethod.Get, $"/tenor/search?q={query}", true);

    public async Task<TResponse> GetTenorFeatured<TResponse>()
        => await MakeFluxerApiRequestR<TResponse>(HttpMethod.Get, "/tenor/featured", true);

    public async Task<TResponse> GetTenorTrendingGifs<TResponse>()
        => await MakeFluxerApiRequestR<TResponse>(HttpMethod.Get, "/tenor/trending-gifs", true);

    public async Task PostTenorRegisterShare<TRequest>(TRequest data)
        => await MakeFluxerApiRequestS<TRequest>(HttpMethod.Post, "/tenor/register-share", data, true);

    public async Task<TResponse> GetTenorSuggest<TResponse>(string query)
        => await MakeFluxerApiRequestR<TResponse>(HttpMethod.Get, $"/tenor/suggest?q={query}", true);

    #endregion

    #region Users API

    public async Task<User> GetCurrentUser()
        => await MakeFluxerApiRequestR<User>(HttpMethod.Get, "/users/@me", true);

    public async Task<User> UpdateCurrentUser(User user)
        => await MakeFluxerApiRequestRS<User, User>(HttpMethod.Patch, "/users/@me", user, true);

    public async Task<TResponse> CheckUsernameAvailability<TResponse>(string tag)
        => await MakeFluxerApiRequestR<TResponse>(HttpMethod.Get, $"/users/check-tag?tag={tag}", true);

    public async Task<User> GetUser(ulong userId)
        => await MakeFluxerApiRequestR<User>(HttpMethod.Get, $"/users/{userId}", true);

    public async Task<TResponse> GetUserProfile<TResponse>(ulong targetId)
        => await MakeFluxerApiRequestR<TResponse>(HttpMethod.Get, $"/users/{targetId}/profile", true);

    public async Task<UserSettings> GetCurrentUserSettings()
        => await MakeFluxerApiRequestR<UserSettings>(HttpMethod.Get, "/users/@me/settings", true);

    public async Task<UserSettings> UpdateCurrentUserSettings<TRequest>(TRequest settings)
        => await MakeFluxerApiRequestRS<UserSettings, TRequest>(HttpMethod.Patch, "/users/@me/settings", settings, true);

    public async Task<UserSettings> SetCustomStatus(UserCustomStatus status)
        => await MakeFluxerApiRequestRS<UserSettings, ModifyCustomStatus>(HttpMethod.Patch, "/users/@me/settings", new ModifyCustomStatus(status), true);

    public async Task<TResponse> GetCurrentUserNotes<TResponse>()
        => await MakeFluxerApiRequestR<TResponse>(HttpMethod.Get, "/users/@me/notes", true);

    public async Task<TResponse> GetCurrentUserNote<TResponse>(ulong targetId)
        => await MakeFluxerApiRequestR<TResponse>(HttpMethod.Get, $"/users/@me/notes/{targetId}", true);

    public async Task<TResponse> PutCurrentUserNote<TRequest, TResponse>(ulong targetId, TRequest data)
        => await MakeFluxerApiRequestRS<TResponse, TRequest>(HttpMethod.Put, $"/users/@me/notes/{targetId}", data, true);

    public async Task<TResponse> GetCurrentUserBetaCodes<TResponse>()
        => await MakeFluxerApiRequestR<TResponse>(HttpMethod.Get, "/users/@me/beta-codes", true);

    public async Task<TResponse> PostCurrentUserBetaCode<TRequest, TResponse>(TRequest data)
        => await MakeFluxerApiRequestRS<TResponse, TRequest>(HttpMethod.Post, "/users/@me/beta-codes", data, true);

    public async Task DeleteCurrentUserBetaCode(string code)
        => await MakeFluxerApiRequest(HttpMethod.Delete, $"/users/@me/beta-codes/{code}", true);

    public async Task<TResponse> GetCurrentUserMentions<TResponse>()
        => await MakeFluxerApiRequestR<TResponse>(HttpMethod.Get, "/users/@me/mentions", true);

    public async Task DeleteCurrentUserMention(ulong messageId)
        => await MakeFluxerApiRequest(HttpMethod.Delete, $"/users/@me/mentions/{messageId}", true);

    public async Task<TResponse> PostCurrentUserMfaTotpEnable<TRequest, TResponse>(TRequest data)
        => await MakeFluxerApiRequestRS<TResponse, TRequest>(HttpMethod.Post, "/users/@me/mfa/totp/enable", data, true);

    public async Task PostCurrentUserMfaTotpDisable<TRequest>(TRequest data)
        => await MakeFluxerApiRequestS<TRequest>(HttpMethod.Post, "/users/@me/mfa/totp/disable", data, true);

    public async Task<TResponse> PostCurrentUserMfaBackupCodes<TRequest, TResponse>(TRequest data)
        => await MakeFluxerApiRequestRS<TResponse, TRequest>(HttpMethod.Post, "/users/@me/mfa/backup-codes", data, true);

    public async Task PostCurrentUserPhoneSendVerification<TRequest>(TRequest data)
        => await MakeFluxerApiRequestS<TRequest>(HttpMethod.Post, "/users/@me/phone/send-verification", data, true);

    public async Task<TResponse> PostCurrentUserPhoneVerify<TRequest, TResponse>(TRequest data)
        => await MakeFluxerApiRequestRS<TResponse, TRequest>(HttpMethod.Post, "/users/@me/phone/verify", data, true);

    public async Task<TResponse> PostCurrentUserPhone<TRequest, TResponse>(TRequest data)
        => await MakeFluxerApiRequestRS<TResponse, TRequest>(HttpMethod.Post, "/users/@me/phone", data, true);

    public async Task DeleteCurrentUserPhone<TRequest>(TRequest data)
        => await MakeFluxerApiRequestS<TRequest>(HttpMethod.Delete, "/users/@me/phone", data, true);

    public async Task PostCurrentUserMfaSmsEnable<TRequest>(TRequest data)
        => await MakeFluxerApiRequestS<TRequest>(HttpMethod.Post, "/users/@me/mfa/sms/enable", data, true);

    public async Task PostCurrentUserMfaSmsDisable<TRequest>(TRequest data)
        => await MakeFluxerApiRequestS<TRequest>(HttpMethod.Post, "/users/@me/mfa/sms/disable", data, true);

    public async Task<TResponse> GetCurrentUserMfaWebauthnCredentials<TResponse>()
        => await MakeFluxerApiRequestR<TResponse>(HttpMethod.Get, "/users/@me/mfa/webauthn/credentials", true);

    public async Task<TResponse> PostCurrentUserMfaWebauthnCredentialsRegistrationOptions<TResponse>()
        => await MakeFluxerApiRequestR<TResponse>(HttpMethod.Post, "/users/@me/mfa/webauthn/credentials/registration-options", true);

    public async Task<TResponse> PostCurrentUserMfaWebauthnCredentials<TRequest, TResponse>(TRequest data)
        => await MakeFluxerApiRequestRS<TResponse, TRequest>(HttpMethod.Post, "/users/@me/mfa/webauthn/credentials", data, true);

    public async Task<TResponse> PatchCurrentUserMfaWebauthnCredential<TRequest, TResponse>(ulong credentialId, TRequest data)
        => await MakeFluxerApiRequestRS<TResponse, TRequest>(HttpMethod.Patch, $"/users/@me/mfa/webauthn/credentials/{credentialId}", data, true);

    public async Task DeleteCurrentUserMfaWebauthnCredential(ulong credentialId)
        => await MakeFluxerApiRequest(HttpMethod.Delete, $"/users/@me/mfa/webauthn/credentials/{credentialId}", true);

    public async Task<TResponse> GetCurrentUserSavedMessages<TResponse>()
        => await MakeFluxerApiRequestR<TResponse>(HttpMethod.Get, "/users/@me/saved-messages", true);

    public async Task<TResponse> PostCurrentUserSavedMessage<TRequest, TResponse>(TRequest data)
        => await MakeFluxerApiRequestRS<TResponse, TRequest>(HttpMethod.Post, "/users/@me/saved-messages", data, true);

    public async Task DeleteCurrentUserSavedMessage(ulong messageId)
        => await MakeFluxerApiRequest(HttpMethod.Delete, $"/users/@me/saved-messages/{messageId}", true);

    public async Task<TResponse> GetCurrentUserChannels<TResponse>()
        => await MakeFluxerApiRequestR<TResponse>(HttpMethod.Get, "/users/@me/channels", true);

    public async Task<TResponse> PostCurrentUserChannel<TRequest, TResponse>(TRequest data)
        => await MakeFluxerApiRequestRS<TResponse, TRequest>(HttpMethod.Post, "/users/@me/channels", data, true);

    public async Task PutCurrentUserChannelPin(ulong channelId)
        => await MakeFluxerApiRequest(HttpMethod.Put, $"/users/@me/channels/{channelId}/pin", true);

    public async Task DeleteCurrentUserChannelPin(ulong channelId)
        => await MakeFluxerApiRequest(HttpMethod.Delete, $"/users/@me/channels/{channelId}/pin", true);

    public async Task<TResponse> GetCurrentUserRelationships<TResponse>()
        => await MakeFluxerApiRequestR<TResponse>(HttpMethod.Get, "/users/@me/relationships", true);

    public async Task<TResponse> PostCurrentUserRelationship<TRequest, TResponse>(TRequest data)
        => await MakeFluxerApiRequestRS<TResponse, TRequest>(HttpMethod.Post, "/users/@me/relationships", data, true);

    public async Task<TResponse> PostCurrentUserRelationshipWithUser<TRequest, TResponse>(ulong userId, TRequest data)
        => await MakeFluxerApiRequestRS<TResponse, TRequest>(HttpMethod.Post, $"/users/@me/relationships/{userId}", data, true);

    public async Task PutCurrentUserRelationship<TRequest>(ulong userId, TRequest data)
        => await MakeFluxerApiRequestS<TRequest>(HttpMethod.Put, $"/users/@me/relationships/{userId}", data, true);

    public async Task DeleteCurrentUserRelationship(ulong userId)
        => await MakeFluxerApiRequest(HttpMethod.Delete, $"/users/@me/relationships/{userId}", true);

    public async Task<TResponse> PatchCurrentUserGuildSettingsSelf<TRequest, TResponse>(TRequest data)
        => await MakeFluxerApiRequestRS<TResponse, TRequest>(HttpMethod.Patch, "/users/@me/guilds/@me/settings", data, true);

    public async Task<TResponse> PatchCurrentUserGuildSettings<TRequest, TResponse>(ulong guildId, TRequest data)
        => await MakeFluxerApiRequestRS<TResponse, TRequest>(HttpMethod.Patch, $"/users/@me/guilds/{guildId}/settings", data, true);

    public async Task PostCurrentUserDisable<TRequest>(TRequest data)
        => await MakeFluxerApiRequestS<TRequest>(HttpMethod.Post, "/users/@me/disable", data, true);

    public async Task PostCurrentUserDelete<TRequest>(TRequest data)
        => await MakeFluxerApiRequestS<TRequest>(HttpMethod.Post, "/users/@me/delete", data, true);

    public async Task<TResponse> PostCurrentUserPushSubscribe<TRequest, TResponse>(TRequest data)
        => await MakeFluxerApiRequestRS<TResponse, TRequest>(HttpMethod.Post, "/users/@me/push/subscribe", data, true);

    public async Task<TResponse> GetCurrentUserPushSubscriptions<TResponse>()
        => await MakeFluxerApiRequestR<TResponse>(HttpMethod.Get, "/users/@me/push/subscriptions", true);

    public async Task DeleteCurrentUserPushSubscription(ulong subscriptionId)
        => await MakeFluxerApiRequest(HttpMethod.Delete, $"/users/@me/push/subscriptions/{subscriptionId}", true);

    public async Task<TResponse> PostCurrentUserHarvest<TRequest, TResponse>(TRequest data)
        => await MakeFluxerApiRequestRS<TResponse, TRequest>(HttpMethod.Post, "/users/@me/harvest", data, true);

    public async Task<TResponse> GetCurrentUserHarvestLatest<TResponse>()
        => await MakeFluxerApiRequestR<TResponse>(HttpMethod.Get, "/users/@me/harvest/latest", true);

    public async Task<TResponse> GetCurrentUserHarvest<TResponse>(string harvestId)
        => await MakeFluxerApiRequestR<TResponse>(HttpMethod.Get, $"/users/@me/harvest/{harvestId}", true);

    public async Task<TResponse> GetCurrentUserHarvestDownload<TResponse>(string harvestId)
        => await MakeFluxerApiRequestR<TResponse>(HttpMethod.Get, $"/users/@me/harvest/{harvestId}/download", true);

    public async Task PostCurrentUserPreloadMessages<TRequest>(TRequest data)
        => await MakeFluxerApiRequestS<TRequest>(HttpMethod.Post, "/users/@me/preload-messages", data, true);

    public async Task PostCurrentUserMessagesDelete<TRequest>(TRequest data)
        => await MakeFluxerApiRequestS<TRequest>(HttpMethod.Post, "/users/@me/messages/delete", data, true);

    #endregion

    #region Webhooks API

    public async Task<TResponse> GetWebhook<TResponse>(ulong webhookId)
        => await MakeFluxerApiRequestR<TResponse>(HttpMethod.Get, $"/webhooks/{webhookId}", true);

    public async Task<TResponse> UpdateWebhook<TRequest, TResponse>(ulong webhookId, TRequest data)
        => await MakeFluxerApiRequestRS<TResponse, TRequest>(HttpMethod.Patch, $"/webhooks/{webhookId}", data, true);

    public async Task DeleteWebhook(ulong webhookId)
        => await MakeFluxerApiRequest(HttpMethod.Delete, $"/webhooks/{webhookId}", true);

    public async Task<TResponse> GetWebhookWithToken<TResponse>(ulong webhookId, string token)
        => await MakeFluxerApiRequestR<TResponse>(HttpMethod.Get, $"/webhooks/{webhookId}/{token}", false);

    public async Task<TResponse> UpdateWebhookWithToken<TRequest, TResponse>(ulong webhookId, string token, TRequest data)
        => await MakeFluxerApiRequestRS<TResponse, TRequest>(HttpMethod.Patch, $"/webhooks/{webhookId}/{token}", data, false);

    public async Task DeleteWebhookWithToken(ulong webhookId, string token)
        => await MakeFluxerApiRequest(HttpMethod.Delete, $"/webhooks/{webhookId}/{token}", false, false);

    public async Task ExecuteWebhook<TRequest>(ulong webhookId, string token, TRequest data)
        => await MakeFluxerApiRequestS<TRequest>(HttpMethod.Post, $"/webhooks/{webhookId}/{token}", data, true, false);

    public async Task ExecuteGithubWebhook<TRequest>(ulong webhookId, string token, TRequest data)
        => await MakeFluxerApiRequestS<TRequest>(HttpMethod.Post, $"/webhooks/{webhookId}/{token}/github", data, true, false);

    public async Task PostWebhookLivekit<TRequest>(TRequest data)
        => await MakeFluxerApiRequestS<TRequest>(HttpMethod.Post, "/webhooks/livekit", data, true, false);

    public async Task PostWebhookSendgrid<TRequest>(TRequest data)
        => await MakeFluxerApiRequestS<TRequest>(HttpMethod.Post, "/webhooks/sendgrid", data, true, false);

    #endregion

    #region Stripe API

    public async Task PostStripeWebhook<TRequest>(TRequest data)
        => await MakeFluxerApiRequestS<TRequest>(HttpMethod.Post, "/stripe/webhook", data, true, false);

    public async Task<TResponse> PostStripeCheckoutSubscription<TRequest, TResponse>(TRequest data)
        => await MakeFluxerApiRequestRS<TResponse, TRequest>(HttpMethod.Post, "/stripe/checkout/subscription", data, true);

    public async Task<TResponse> PostStripeCheckoutGift<TRequest, TResponse>(TRequest data)
        => await MakeFluxerApiRequestRS<TResponse, TRequest>(HttpMethod.Post, "/stripe/checkout/gift", data, true);

    #endregion

    #region Gifts API

    public async Task<TResponse> GetGift<TResponse>(string code)
        => await MakeFluxerApiRequestR<TResponse>(HttpMethod.Get, $"/gifts/{code}", true);

    public async Task<TResponse> PostGiftRedeem<TRequest, TResponse>(string code, TRequest data)
        => await MakeFluxerApiRequestRS<TResponse, TRequest>(HttpMethod.Post, $"/gifts/{code}/redeem", data, true);

    public async Task<TResponse> GetCurrentUserGifts<TResponse>()
        => await MakeFluxerApiRequestR<TResponse>(HttpMethod.Get, "/users/@me/gifts", true);

    #endregion

    #region Premium API

    public async Task<TResponse> GetPremiumVisionarySlots<TResponse>()
        => await MakeFluxerApiRequestR<TResponse>(HttpMethod.Get, "/premium/visionary/slots", true);

    public async Task<TResponse> GetPremiumPriceIds<TResponse>()
        => await MakeFluxerApiRequestR<TResponse>(HttpMethod.Get, "/premium/price-ids", true);

    public async Task<TResponse> PostPremiumCustomerPortal<TRequest, TResponse>(TRequest data)
        => await MakeFluxerApiRequestRS<TResponse, TRequest>(HttpMethod.Post, "/premium/customer-portal", data, true);

    public async Task PostPremiumCancelSubscription<TRequest>(TRequest data)
        => await MakeFluxerApiRequestS<TRequest>(HttpMethod.Post, "/premium/cancel-subscription", data, true);

    public async Task PostPremiumReactivateSubscription()
        => await MakeFluxerApiRequest(HttpMethod.Post, "/premium/reactivate-subscription", true);

    public async Task PostPremiumVisionaryRejoin()
        => await MakeFluxerApiRequest(HttpMethod.Post, "/premium/visionary/rejoin", true);

    public async Task PostPremiumOperatorRejoin()
        => await MakeFluxerApiRequest(HttpMethod.Post, "/premium/operator/rejoin", true);

    #endregion

    #region Misc API

    public async Task<TResponse> PostRpc<TRequest, TResponse>(TRequest data)
        => await MakeFluxerApiRequestRS<TResponse, TRequest>(HttpMethod.Post, "/_rpc", data, true);

    #endregion
}
