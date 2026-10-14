using Fluxer.Net.Admin;
using Fluxer.Net.Extensions;
using Fluxer.Net.Gateway;
using Fluxer.Net.OAuth;

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
public class FluxerApiClient : FluxerBaseApiClient
{
    #region Declares
    private readonly FluxerBaseClient _client;
    private readonly bool _isWebhook;



    #endregion

    #region Meta
    /// <summary>
    /// Initializes a new instance of the <see cref="FluxerApiClient"/> class.
    /// </summary>
    /// <remarks>
    /// The client is automatically configured with:
    /// <list type="bullet">
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
        Initialize();
    }

    internal FluxerApiClient(FluxerAdminClient admin)
    {
        _client = admin;
        _config = admin.Config;
        _logger = admin.Config.RestSerilog;
        Initialize();
    }

    // <summary>
    // Helper method to wait for rate limiting before making a request.
    // </summary>
    // <param name="config">The rate limit configuration for this request.</param>
    // <param name="channelId">Optional channel ID for channel-specific rate limits.</param>
    // <param name="guildId">Optional guild ID for guild-specific rate limits.</param>
    // <param name="userId">Optional user ID for user-specific rate limits.</param>
    // <param name="webhookId">Optional webhook ID for webhook-specific rate limits.</param>
    // <param name="inviteCode">Optional invite code for invite-specific rate limits.</param>
    //private async Task WaitForRateLimitAsync(RateLimitConfig config, ulong? channelId = null, ulong? guildId = null, ulong? userId = null, ulong? webhookId = null, string inviteCode = null)
    //{
    //    if (!_config.EnableRateLimiting)
    //        return;

    //    RateLimitBucket bucket = RateLimitManager.GetBucket(config, channelId, guildId, userId, webhookId, inviteCode);
    //    await RateLimitManager.WaitForRateLimitAsync(bucket);
    //}


    #endregion

    /// <summary>
    /// Get info and config for the Fluxer instance, useful for self-hosted.
    /// </summary>
    /// <returns><see cref="Instance"/></returns>
    public async Task<Instance?> GetInstanceAsync(RequestOptions? options = null)
    {
        InstanceJson? json = await SendRequestAsync<InstanceJson>(HttpMethod.Get, "/.well-known/fluxer", false, options: options);
        if (json == null)
            return null;

        return Instance.Create(_client, json);
    }

    #region Expressions API
    /// <summary>
    /// Get a custom emoji.
    /// </summary>
    /// <returns><see cref="Emoji"/></returns>
    public async Task<Emoji?> GetEmojiAsync(ulong emojiId, RequestOptions? options = null)
    {
        EmojiJson? json = await SendRequestAsync<EmojiJson>(HttpMethod.Get, $"/emojis/{emojiId}/metadata", false, options: options);
        if (json == null)
            return null;

        return Emoji.Create(_client, json);
    }

    /// <summary>
    /// Get a custom sticker.
    /// </summary>
    /// <returns><see cref="Sticker"/></returns>
    public async Task<Sticker?> GetStickerAsync(ulong stickerId, RequestOptions? options = null)
    {
        StickerJson? json = await SendRequestAsync<StickerJson>(HttpMethod.Get, $"/stickers/{stickerId}/metadata", false, options: options);
        if (json == null)
            return null;

        return Sticker.Create(_client, json);
    }

    #endregion

    #region Auth API
    /// <summary>
    /// Login with a user account.
    /// </summary>
    /// <returns><see cref="Login"/></returns>
    public async Task<Login> LoginAsync(LoginRequestJson data, RequestOptions? options = null)
    {
        LoginJson json = await SendRequestAsync<LoginJson, LoginRequestJson>(HttpMethod.Post, "/auth/login", data, false, options: options);
        return Login.Create(_client, json);
    }

    /// <summary>
    /// Create a user account
    /// </summary>
    public async Task RegisterAsync<TRequest>(TRequest data, RequestOptions? options = null)
        => await SendRequestAsync<TRequest>(HttpMethod.Post, "/auth/register", data, true, false, options: options);

    /// <summary>
    /// Login with a user account that has MFA.
    /// </summary>
    public async Task LoginMfaTotpAsync<TRequest, TResponse>(TRequest data)
        => await SendRequestAsync<TResponse, TRequest>(HttpMethod.Post, "/auth/login/mfa/totp", data, true, false);

    /// <summary>
    /// Send a SMS code.
    /// </summary>
    public async Task SendMfaSmsCodeAsync()
        => await SendRequestRawAsync(HttpMethod.Post, "/auth/login/mfa/sms/send", true, false);

    /// <summary>
    /// Login with a SMS code.
    /// </summary>
    public async Task LoginMfaSmsAsync<TRequest, TResponse>(TRequest data, RequestOptions? options = null)
        => await SendRequestAsync<TResponse, TRequest>(HttpMethod.Post, "/auth/login/mfa/sms", data, true, false, options: options);

    /// <summary>
    /// Logout of the current session/auth.
    /// </summary>
    public async Task LogoutAsync()
        => await SendRequestRawAsync(HttpMethod.Post, "/auth/logout", true);

    /// <summary>
    /// Verify the user account email.
    /// </summary>
    public async Task VerifyEmailAsync<TRequest>(TRequest data)
        => await SendRequestAsync<TRequest>(HttpMethod.Post, "/auth/verify", data, true, false);

    /// <summary>
    /// Send an email verification for the user account.
    /// </summary>
    public async Task ResendVerificationEmailAsync()
        => await SendRequestRawAsync(HttpMethod.Post, "/auth/verify/resend", true);

    /// <summary>
    /// Request a password reset for the user account.
    /// </summary>
    public async Task ForgotPasswordAsync<TRequest>(TRequest data)
        => await SendRequestAsync<TRequest>(HttpMethod.Post, "/auth/forgot", data, true, false);

    /// <summary>
    /// Reset the password for the user account.
    /// </summary>
    public async Task ResetPasswordAsync<TRequest>(TRequest data)
        => await SendRequestAsync<TRequest>(HttpMethod.Post, "/auth/reset", data, true, false);

    /// <summary>
    /// Get all sessions for the user account.
    /// </summary>
    /// <returns>List of <see cref="AuthSession"/></returns>
    public async Task<IEnumerable<AuthSession>> GetSessionsAsync(RestClientQueryParams? queryParams)
    {
        IEnumerable<AuthSessionJson> json = await SendRequestAsync<IEnumerable<AuthSessionJson>>(HttpMethod.Get, "/auth/sessions", true);
        return json.Select(x => AuthSession.Create(_client, x));
    }

    /// <summary>
    /// Logout a specific session for the user account.
    /// </summary>
    public async Task LogoutSessionsAsync<TRequest>(TRequest data)
        => await SendRequestAsync<TRequest>(HttpMethod.Post, "/auth/sessions/logout", data, true);

    /// <summary>
    /// Authorize and IP for the user account.
    /// </summary>
    public async Task PostAuthAuthorizeIpAsync<TRequest>(TRequest data)
        => await SendRequestAsync<TRequest>(HttpMethod.Post, "/auth/authorize-ip", data, true);

    /// <summary>
    /// Setup webauth for the user account.
    /// </summary>
    public async Task<TResponse> PostAuthWebauthnAuthenticationOptionsAsync<TResponse>()
        => await SendRequestAsync<TResponse>(HttpMethod.Post, "/auth/webauthn/authentication-options", true, false);

    /// <summary>
    /// Setup webauth for the user account.
    /// </summary>
    public async Task<TResponse> PostAuthWebauthnAuthenticateAsync<TRequest, TResponse>(TRequest data, RequestOptions? options = null)
        => await SendRequestAsync<TResponse, TRequest>(HttpMethod.Post, "/auth/webauthn/authenticate", data, true, false, options: options);

    /// <summary>
    /// Setup webauth for the user account.
    /// </summary>
    public async Task<TResponse> PostAuthLoginMfaWebauthnAuthenticationOptionsAsync<TResponse>(RequestOptions? options = null)
        => await SendRequestAsync<TResponse>(HttpMethod.Post, "/auth/login/mfa/webauthn/authentication-options", true, false, options: options);

    /// <summary>
    /// Setup webauth for the user account.
    /// </summary>
    public async Task<TResponse> PostAuthLoginMfaWebauthnAsync<TRequest, TResponse>(TRequest data, RequestOptions? options = null)
        => await SendRequestAsync<TResponse, TRequest>(HttpMethod.Post, "/auth/login/mfa/webauthn", data, true, false, options: options);

    #endregion

    #region Channels API
    /// <summary>
    /// Create a DM channel.
    /// </summary>
    /// <returns><see cref="DMChannel"/></returns>
    public async Task<DMChannel> CreateDMChannelAsync(CreatePrivateChannelRequest req, RequestOptions? options = null)
    {
        ChannelJson json = await SendRequestAsync<ChannelJson, CreatePrivateChannelRequest>(HttpMethod.Post, $"/users/@me/channels", req, true, options: options);
        return (DMChannel)Channel.Create(_client, json);
    }

    /// <summary>
    /// Create a Group channel.
    /// </summary>
    /// <returns><see cref="GroupChannel"/></returns>
    public async Task<GroupChannel> CreateGroupChannelAsync(CreatePrivateChannelRequest req, RequestOptions? options = null)
    {
        ChannelJson json = await SendRequestAsync<ChannelJson, CreatePrivateChannelRequest>(HttpMethod.Post, $"/users/@me/channels", req, true, options: options);
        return (GroupChannel)Channel.Create(_client, json);
    }

    /// <summary>
    /// Get a channel.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="ChannelPermissions.ViewChannel"/> for guild channels.
    /// </remarks>
    /// <returns><see cref="Channel"/></returns>
    public async Task<Channel?> GetChannelAsync(ulong channelId, RequestOptions? options = null)
    {
        ChannelJson json = await SendRequestAsync<ChannelJson>(HttpMethod.Get, $"/channels/{channelId}", false, options: options);
        if (json == null)
            return null;

        return Channel.Create(_client, json);
    }

    /// <inheritdoc cref="GetChannelAsync(ulong, RequestOptions?)"/>
    public async Task<ThreadChannel?> GetThreadChannelAsync(ulong channelId, RequestOptions? options = null)
    {
        ChannelJson json = await SendRequestAsync<ChannelJson>(HttpMethod.Get, $"/channels/{channelId}", false, options: options);
        if (json == null)
            return null;

        return Channel.Create(_client, json) as ThreadChannel;
    }

    /// <summary>
    /// Get voice regions for a channel.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="ChannelPermissions.ViewChannel"/> for guild channels.
    /// </remarks>
    /// <returns>List of <see cref="RtcRegion"/></returns>
    public async Task<IEnumerable<RtcRegion>> GetChannelRtcRegionsAsync(ulong channelId, RequestOptions? options = null)
    {
        IEnumerable<RtcRegionJson> json = await SendRequestAsync<IEnumerable<RtcRegionJson>>(HttpMethod.Get, $"/channels/{channelId}/rtc-regions", true, options: options);
        return json.Select(x => RtcRegion.Create(_client, x));
    }

    /// <summary>
    /// Modify a channel.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="ChannelPermissions.ManageChannel"/> for guild channels.
    /// </remarks>
    /// <returns><see cref="Channel"/></returns>
    public async Task<Channel> UpdateChannelAsync(ulong channelId, ChannelJson channel, RequestOptions? options = null)
    {
        ChannelJson json = await SendRequestAsync<ChannelJson, ChannelJson>(HttpMethod.Patch, $"/channels/{channelId}", channel, true, options: options);
        return Channel.Create(_client, json);
    }

    /// <summary>
    /// Delete a channel.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="ChannelPermissions.ManageChannel"/> for guild channels.
    /// </remarks>
    public async Task DeleteChannelAsync(ulong channelId, RequestOptions? options = null)
        => await SendRequestRawAsync(HttpMethod.Delete, $"/channels/{channelId}", true, options: options);

    /// <summary>
    /// Clear message unreads for a channel.
    /// </summary>
    public async Task ClearMessageAcknowledgementAsync(ulong channelId, RequestOptions? options = null)
        => await SendRequestRawAsync(HttpMethod.Delete, $"/channels/{channelId}/messages/ack", true, options: options);

    /// <summary>
    /// Get messages in a channel.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="ChannelPermissions.ReadMessageHistory"/> for guild channels.
    /// </remarks>
    /// <returns>List of <see cref="Message"/></returns>
    public async Task<IEnumerable<Message>> GetMessagesAsync(ulong channelId, int limit = 100, ulong? beforeId = null, ulong? afterId = null, ulong? aroundId = null, RestClientQueryParams? queryParams = null, RequestOptions? options = null)
    {
        queryParams ??= new RestClientQueryParams()
            .Add(QueryParams.Limit, limit)
            .AddIf(beforeId != null, QueryParams.Before, beforeId)
            .AddIf(afterId != null, QueryParams.After, afterId)
            .AddIf(aroundId != null, QueryParams.Around, aroundId);

        IEnumerable<MessageJson> json = await SendRequestQueryParamsAsync<IEnumerable<MessageJson>>(HttpMethod.Get, queryParams, $"/channels/{channelId}/messages", true, options: options);
        return json.Select(x => Message.Create(_client, x));
    }

    /// <summary>
    /// Get a message in a channel.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="ChannelPermissions.ViewChannel"/> and <see cref="ChannelPermissions.ReadMessageHistory"/> for guild channels.
    /// </remarks>
    /// <returns><see cref="Message"/></returns>
    public async Task<Message?> GetMessageAsync(ulong channelId, ulong messageId, RequestOptions? options = null)
    {
        MessageJson? json = await SendRequestAsync<MessageJson>(HttpMethod.Get, $"/channels/{channelId}/messages/{messageId}", false, options: options);
        if (json == null)
            return null;

        return Message.Create(_client, json);
    }

    /// <summary>
    /// Crosspost a message.
    /// </summary>
    public async Task CrosspostMessageAsync(ulong channelId, ulong messageId, RequestOptions? options = null)
        => await SendRequestAsync<dynamic>(HttpMethod.Post, $"/channels/{channelId}/messages/{messageId}/crosspost", null, true, options: options);

    /// <summary>
    /// Follow a news channel.
    /// </summary>
    public async Task FollowChannelAsync(ulong channelId, ulong newsChannelId, RequestOptions? options = null)
        => await SendRequestAsync<dynamic>(HttpMethod.Post, $"/channels/{channelId}/followers", new FollowChannelRequest
        {
            ChannelId = newsChannelId,
        }, true, options: options);

    /// <summary>
    /// Search messages in a channel.
    /// </summary>
    /// <remarks>
    /// Not implemented or tested properly.
    /// </remarks>
    public async Task<TResponse> SearchChannelAsync<TRequest, TResponse>(ulong channelId, TRequest data, RequestOptions? options = null)
        => await SendRequestAsync<TResponse, TRequest>(HttpMethod.Post, $"/channels/{channelId}/search", data, true, options: options);

    /// <summary>
    /// Send a message in the channel.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="ChannelPermissions.ViewChannel"/> and <see cref="ChannelPermissions.SendMessages"/> for guild channels.
    /// </remarks>
    /// <returns><see cref="Message"/></returns>
    public async Task<Message> SendMessageAsync(ulong channelId, string? content = null, List<EmbedRequest>? embeds = null,
        MessageReferenceRequest? reference = null, AllowedMentionsRequest? allowedMentions = null, MessageFlag flags = MessageFlag.None,
        string? nonce = null, ulong? favoriteMediaId = null, bool? tts = null, List<ulong>? stickerIds = null, List<AttachmentRequest>? attachments = null, RequestOptions? options = null)
    {
        MessageRequest req = new MessageRequest
        {
            Content = content,
            Embeds = embeds?.ToArray(),
            MessageReference = reference,
            AllowedMentions = allowedMentions,
            Flags = flags,
            Nonce = nonce,
            FavoriteMediaId = favoriteMediaId,
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
            req, true, options: options);
        return Message.Create(_client, json);
    }

    /// <summary>
    /// Modify your message in a channel
    /// </summary>
    /// <remarks>
    /// Requires <see cref="ChannelPermissions.ViewChannel"/> and <see cref="ChannelPermissions.ReadMessageHistory"/> in a guild channel.
    /// </remarks>
    /// <returns><see cref="Message"/></returns>
    public async Task<Message> EditMessageAsync(ulong channelId, ulong messageId, string? content = null, List<EmbedRequest>? embeds = null,
        MessageReferenceRequest? reference = null, AllowedMentionsRequest? allowedMentions = null, MessageFlag flags = MessageFlag.None,
        string? nonce = null, ulong? favoriteMediaId = null, List<ulong>? stickerIds = null, List<AttachmentRequest>? attachments = null, RequestOptions? options = null)
    {
        MessageRequest req = new MessageRequest
        {
            Content = content,
            Embeds = embeds?.ToArray(),
            MessageReference = reference,
            AllowedMentions = allowedMentions,
            Flags = flags,
            Nonce = nonce,
            FavoriteMediaId = favoriteMediaId,
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

        MessageJson json = await SendRequestAsync<MessageJson, MessageRequest>(HttpMethod.Patch, $"/channels/{channelId}/messages/{messageId}", req, true, options: options);
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
    public async Task DeleteMessageAsync(ulong channelId, ulong messageId, RequestOptions? options = null)
        => await SendRequestRawAsync(HttpMethod.Delete, $"/channels/{channelId}/messages/{messageId}", true, options: options);

    /// <summary>
    /// Delete a attachment on a message.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="ChannelPermissions.ViewChannel"/> and <see cref="ChannelPermissions.ReadMessageHistory"/> in a guild channel.
    /// </remarks>
    public async Task DeleteMessageAttachmentAsync(ulong channelId, ulong messageId, ulong attachmentId, RequestOptions? options = null)
        => await SendRequestRawAsync(HttpMethod.Delete, $"/channels/{channelId}/messages/{messageId}/attachments/{attachmentId}", true, options: options);

    /// <summary>
    /// Delete many messages in a channel.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="ChannelPermissions.ViewChannel"/>, <see cref="ChannelPermissions.ReadMessageHistory"/> and <see cref="ChannelPermissions.ManageMessages"/> in a guild channel.
    /// </remarks>
    public async Task BulkDeleteMessagesAsync(ulong channelId, HashSet<ulong> messageIds, RequestOptions? options = null)
        => await SendRequestAsync(HttpMethod.Post, $"/channels/{channelId}/messages/bulk-delete", new BulkDeleteMessagesRequest
        {
            MessageIds = messageIds
        }, true, options: options);

    /// <summary>
    /// Send a user typing status for the channel.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="ChannelPermissions.ViewChannel"/> in a guild channel.
    /// </remarks>
    public async Task TriggerTypingIndicatorAsync(ulong channelId, RequestOptions? options = null)
        => await SendRequestRawAsync(HttpMethod.Post, $"/channels/{channelId}/typing", true, options: options);

    /// <summary>
    /// Acknowledge a message has been read for the current user.
    /// </summary>
    /// <remarks>
    /// User accounts only.
    /// </remarks>
    public async Task AcknowledgeMessageAsync(ulong channelId, ulong messageId, MessageAckJson details, RequestOptions? options = null)
        => await SendRequestAsync<MessageAckJson>(HttpMethod.Post, $"/channels/{channelId}/messages/{messageId}/ack", details, true, options: options);

    /// <summary>
    /// Get pinned messages in a channel.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="ChannelPermissions.ViewChannel"/> and <see cref="ChannelPermissions.ReadMessageHistory"/> in a guild channel.
    /// </remarks>
    /// <returns><see cref="ChannelPins"/></returns>
    public async Task<ChannelPins> GetPinnedMessagesAsync(ulong channelId, ChannelPinsQuery? query = null, RequestOptions? options = null)
    {
        ChannelPinsJson json = await SendRequestAsync<ChannelPinsJson>(HttpMethod.Get, $"/channels/{channelId}/pins?{query?.BuildQuery() ?? string.Empty}", true, options: options);
        return ChannelPins.Create(_client, json);
    }

    /// <summary>
    /// Pin a message in a channel.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="ChannelPermissions.ViewChannel"/>, <see cref="ChannelPermissions.ReadMessageHistory"/> and <see cref="ChannelPermissions.PinMessages"/> in a guild channel.
    /// </remarks>
    public async Task PinMessageAsync(ulong channelId, ulong messageId, RequestOptions? options = null)
        => await SendRequestRawAsync(HttpMethod.Put, $"/channels/{channelId}/pins/{messageId}", true, options: options);

    /// <summary>
    /// Remove a pinned message in a channel.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="ChannelPermissions.ViewChannel"/>, <see cref="ChannelPermissions.ReadMessageHistory"/> and <see cref="ChannelPermissions.PinMessages"/> in a guild channel.
    /// </remarks>
    public async Task UnpinMessageAsync(ulong channelId, ulong messageId, RequestOptions? options = null)
        => await SendRequestRawAsync(HttpMethod.Delete, $"/channels/{channelId}/pins/{messageId}", true, options: options);

    /// <summary>
    /// Get reaction users for a certain emoji on a message.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="ChannelPermissions.ViewChannel"/> and <see cref="ChannelPermissions.ReadMessageHistory"/> in a guild channel.
    /// </remarks>
    /// <returns>List of <see cref="User"/></returns>
    public async Task<IEnumerable<User>> GetReactionsForEmojiAsync(ulong channelId, ulong messageId, string emoji, RequestOptions? options = null)
    {
        IEnumerable<UserJson> json = await SendRequestAsync<IEnumerable<UserJson>>(HttpMethod.Get, $"/channels/{channelId}/messages/{messageId}/reactions/{emoji}", true, options: options);
        return json.Select(x => User.Create(_client, x));
    }

    /// <summary>
    /// Add a reaction to a message.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="ChannelPermissions.ViewChannel"/> and <see cref="ChannelPermissions.AddReactions"/> in a guild channel.
    /// </remarks>
    public async Task AddReactionAsync(ulong channelId, ulong messageId, string emoji, RequestOptions? options = null)
    {
        if (ulong.TryParse(emoji, out _))
            emoji = $"name:{emoji}";

        await SendRequestRawAsync(HttpMethod.Put, $"/channels/{channelId}/messages/{messageId}/reactions/{emoji}/@me", true, options: options);
    }

    /// <summary>
    /// Remove a reaction on a message.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="ChannelPermissions.ViewChannel"/> in a guild channel.
    /// </remarks>
    public async Task RemoveOwnReactionAsync(ulong channelId, ulong messageId, string emoji, RequestOptions? options = null)
    {
        if (ulong.TryParse(emoji, out _))
            emoji = $"name:{emoji}";

        await SendRequestRawAsync(HttpMethod.Delete, $"/channels/{channelId}/messages/{messageId}/reactions/{emoji}/@me", true, options: options);
    }

    /// <summary>
    /// Remove a user's reaction on a message.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="ChannelPermissions.ViewChannel"/>, <see cref="ChannelPermissions.ReadMessageHistory"/> and <see cref="ChannelPermissions.ManageMessages"/> in a guild channel.
    /// </remarks>
    public async Task RemoveUserReactionAsync(ulong channelId, ulong messageId, string emoji, ulong targetId, RequestOptions? options = null)
    {
        if (ulong.TryParse(emoji, out _))
            emoji = $"name:{emoji}";

        await SendRequestRawAsync(HttpMethod.Delete, $"/channels/{channelId}/messages/{messageId}/reactions/{emoji}/{targetId}", true, options: options);
    }

    /// <summary>
    /// Remove all reactions for a specific emoji on a message.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="ChannelPermissions.ViewChannel"/>, <see cref="ChannelPermissions.ReadMessageHistory"/> and <see cref="ChannelPermissions.ManageMessages"/> in a guild channel.
    /// </remarks>
    public async Task RemoveAllReactionsForEmojiAsync(ulong channelId, ulong messageId, string emoji, RequestOptions? options = null)
    {
        if (ulong.TryParse(emoji, out _))
            emoji = $"name:{emoji}";

        await SendRequestRawAsync(HttpMethod.Delete, $"/channels/{channelId}/messages/{messageId}/reactions/{emoji}", true, options: options);
    }

    /// <summary>
    /// Remove all reactions on a message.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="ChannelPermissions.ViewChannel"/>, <see cref="ChannelPermissions.ReadMessageHistory"/> and <see cref="ChannelPermissions.ManageMessages"/> in a guild channel.
    /// </remarks>
    public async Task RemoveAllReactionsAsync(ulong channelId, ulong messageId, RequestOptions? options = null)
        => await SendRequestRawAsync(HttpMethod.Delete, $"/channels/{channelId}/messages/{messageId}/reactions", true, options: options);

    /// <summary>
    /// Upload multiple attachments to a channel.
    /// </summary>
    public async Task<TResponse> UploadAttachmentsAsync<TRequest, TResponse>(ulong channelId, TRequest data, RequestOptions? options = null)
        => await SendRequestAsync<TResponse, TRequest>(HttpMethod.Post, $"/channels/{channelId}/attachments", data, true, options: options);

    /// <summary>
    /// Add a user to a group channel.
    /// </summary>
    /// <remarks>
    /// User account only.
    /// </remarks>
    public async Task AddRecipientAsync(ulong channelId, ulong userId, RequestOptions? options = null)
        => await SendRequestRawAsync(HttpMethod.Put, $"/channels/{channelId}/recipients/{userId}", true, options: options);

    /// <summary>
    /// Remove a user from the group channel.
    /// </summary>
    /// <remarks>
    /// User account only.
    /// </remarks>
    public async Task RemoveRecipientAsync(ulong channelId, ulong userId, RequestOptions? options = null)
        => await SendRequestRawAsync(HttpMethod.Delete, $"/channels/{channelId}/recipients/{userId}", true, options: options);

    /// <summary>
    /// Get if you can call a voice channel/group/dm.
    /// </summary>
    /// <returns><see cref="CallEligibility"/></returns>
    public async Task<CallEligibility> GetVoiceEligibilityAsync(ulong channelId, RequestOptions? options = null)
    {
        CallEligibilityJson json = await SendRequestAsync<CallEligibilityJson>(HttpMethod.Get, $"/channels/{channelId}/call", true, options: options);
        return CallEligibility.Create(_client, json);
    }

    /// <summary>
    /// Update the voice region for a voice channel.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="ChannelPermissions.ViewChannel"/> and <see cref="ChannelPermissions.UpdateRTCRegion"/> in a guild channel.
    /// </remarks>
    public async Task UpdateVoiceRegionAsync(ulong channelId, string? region, RequestOptions? options = null)
        => await SendRequestAsync(HttpMethod.Patch, $"/channels/{channelId}/call", new UpdateVoiceRegionRequest
        {
            Region = region
        }, true, options: options);

    /// <summary>
    /// Voice call users.
    /// </summary>
    /// <remarks>
    /// User account only.
    /// </remarks>
    public async Task RingCallAsync(ulong channelId, ulong[] recipients, RequestOptions? options = null)
        => await SendRequestAsync(HttpMethod.Post, $"/channels/{channelId}/call/ring", new VoiceRingRequest
        {
            Recipients = recipients
        }, true, options: options);

    /// <summary>
    /// Stop voice calling user.
    /// </summary>
    /// <remarks>
    /// User account only.
    /// </remarks>
    public async Task StopRingingAsync(ulong channelId, RequestOptions? options = null)
        => await SendRequestRawAsync(HttpMethod.Post, $"/channels/{channelId}/call/stop-ringing", true, options: options);

    /// <summary>
    /// Get invites for a guild channel.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="ChannelPermissions.ViewChannel"/> and <see cref="ChannelPermissions.ManageChannel"/>.
    /// </remarks>
    /// <returns>List of <see cref="Invite"/></returns>
    public async Task<IEnumerable<Invite>> GetChannelInvitesAsync(ulong channelId, RequestOptions? options = null)
    {
        IEnumerable<InviteJson> json = await SendRequestAsync<IEnumerable<InviteJson>>(HttpMethod.Get, $"/channels/{channelId}/invites", true, options: options);
        return json.Select(x => Invite.Create(_client, x));
    }

    /// <summary>
    /// Create invite for the guild channel.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="ChannelPermissions.ViewChannel"/> and <see cref="ChannelPermissions.CreateInstantInvite"/>.
    /// </remarks>
    /// <returns><see cref="Invite"/></returns>
    public async Task<Invite> CreateInviteAsync(ulong channelId, CreateInviteRequest data, RequestOptions? options = null)
    {
        InviteJson json = await SendRequestAsync<InviteJson, CreateInviteRequest>(HttpMethod.Post, $"/channels/{channelId}/invites", data, true, options: options);
        return Invite.Create(_client, json);
    }

    /// <summary>
    /// Get webhooks for a guild channel.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="ChannelPermissions.ViewChannel"/> and <see cref="ChannelPermissions.ManageWebhooks"/>.
    /// </remarks>
    /// <returns>List of <see cref="Webhook"/></returns>
    public async Task<IEnumerable<Webhook>> GetChannelWebhooksAsync(ulong channelId, RequestOptions? options = null)
    {
        IEnumerable<WebhookJson> json = await SendRequestAsync<IEnumerable<WebhookJson>>(HttpMethod.Get, $"/channels/{channelId}/webhooks", true, options: options);
        return json.Select(x => Webhook.Create(_client, x));
    }

    /// <summary>
    /// Create a webhook for the guild channel.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="ChannelPermissions.ViewChannel"/> and <see cref="ChannelPermissions.ManageWebhooks"/>.
    /// </remarks>
    /// <returns><see cref="Webhook"/></returns>
    public async Task<Webhook> CreateWebhookAsync(ulong channelId, string name, string? avatar = null, RequestOptions? options = null)
    {
        WebhookJson json = await SendRequestAsync<WebhookJson, CreateWebhookRequest>(HttpMethod.Post, $"/channels/{channelId}/webhooks", new CreateWebhookRequest
        {
            Name = name,
            Avatar = avatar
        }, true, options: options);
        return Webhook.Create(_client, json);
    }

    #endregion

    #region Threads API

    /// <summary>
    /// Create a private thread.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="ChannelPermissions.ViewChannel"/> and <see cref="ChannelPermissions.CreatePrivateThreads"/>.
    /// </remarks>
    /// <returns><see cref="ThreadChannel"/></returns>
    public async Task<ThreadChannel> CreatePrivateThreadAsync(ulong channelId, CreatePrivateThreadRequest req, RequestOptions? options = null)
    {
        ChannelJson json = await SendRequestAsync<ChannelJson, CreatePrivateThreadRequest>(HttpMethod.Post, $"/channels/{channelId}/threads", req, true, options: options);
        return (ThreadChannel)Channel.Create(_client, json);
    }

    /// <summary>
    /// Create a public thread.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="ChannelPermissions.ViewChannel"/> and <see cref="ChannelPermissions.CreatePublicThreads"/>.
    /// </remarks>
    /// <returns><see cref="ThreadChannel"/></returns>
    public async Task<ThreadChannel> CreatePublicThreadAsync(ulong channelId, CreatePublicThreadRequest req, RequestOptions? options = null)
    {
        ChannelJson json = await SendRequestAsync<ChannelJson, CreatePublicThreadRequest>(HttpMethod.Post, $"/channels/{channelId}/threads", req, true, options: options);
        return (ThreadChannel)Channel.Create(_client, json);
    }

    /// <summary>
    /// Create a thread from a message
    /// </summary>
    /// <remarks>
    /// Requires <see cref="ChannelPermissions.ViewChannel"/> and <see cref="ChannelPermissions.CreatePublicThreads"/>.
    /// </remarks>
    /// <returns><see cref="ThreadChannel"/></returns>
    public async Task<ThreadChannel> CreateMessageThreadAsync(ulong channelId, ulong messageId, CreatePublicThreadRequest req, RequestOptions? options = null)
    {
        ChannelJson json = await SendRequestAsync<ChannelJson, CreatePublicThreadRequest>(HttpMethod.Post, $"/channels/{channelId}/messages/{messageId}/threads", req, true, options: options);
        return (ThreadChannel)Channel.Create(_client, json);
    }

    /// <summary>
    /// Update a thread.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="ChannelPermissions.ManageThreads"/> or Thread Owner.
    /// </remarks>
    /// <returns><see cref="ThreadChannel"/></returns>
    public async Task<ThreadChannel> UpdateThreadAsync(ulong channelId, UpdateThreadRequest req, RequestOptions? options = null)
    {
        ChannelJson json = await SendRequestAsync<ChannelJson, UpdateThreadRequest>(HttpMethod.Patch, $"/channels/{channelId}/threads", req, true, options: options);
        return (ThreadChannel)Channel.Create(_client, json);
    }

    /// <summary>
    /// Join a thread.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="ChannelPermissions.ViewChannel"/>.
    /// </remarks>
    public async Task JoinThreadAsync(ulong channelId, RequestOptions? options = null)
    {
        await SendRequestAsync<dynamic>(HttpMethod.Put, $"/channels/{channelId}/thread-members/@me", null, true, options: options);
    }

    /// <summary>
    /// Leave a thread.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="ChannelPermissions.ViewChannel"/>.
    /// </remarks>
    public async Task LeaveThreadAsync(ulong channelId, RequestOptions? options = null)
    {
        await SendRequestAsync<dynamic>(HttpMethod.Delete, $"/channels/{channelId}/thread-members/@me", null, true, options: options);
    }

    /// <summary>
    /// Add a member to the thread.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="ChannelPermissions.ManageThreads"/> or Thread Owner.
    /// </remarks>
    public async Task AddThreadMemberAsync(ulong channelId, ulong userId, RequestOptions? options = null)
    {
        await SendRequestAsync<dynamic>(HttpMethod.Put, $"/channels/{channelId}/thread-members/{userId}", null, true, options: options);
    }

    /// <summary>
    /// Remove a member from a thread.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="ChannelPermissions.ManageThreads"/> or Thread Owner.
    /// </remarks>
    public async Task RemoveThreadMemberAsync(ulong channelId, ulong userId, RequestOptions? options = null)
    {
        await SendRequestAsync<dynamic>(HttpMethod.Delete, $"/channels/{channelId}/thread-members/{userId}", null, true, options: options);
    }

    #endregion

    #region Forums API

    /// <summary>
    /// Create a new post in a forum channel.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="ChannelPermissions.SendMessages"/>.
    /// </remarks>
    /// <returns><see cref="ThreadChannel"/></returns>
    public async Task<ThreadChannel> CreatePostAsync(ulong channelId, CreatePostRequest req, RequestOptions? options = null)
    {
        ChannelJson json = await SendRequestAsync<ChannelJson, CreatePostRequest>(HttpMethod.Post, $"/channels/{channelId}/threads", req, true, options: options);
        return (ThreadChannel)Channel.Create(_client, json);
    }

    /// <summary>
    /// Create a forum tag.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="ChannelPermissions.ManageChannel"/>.
    /// </remarks>
    /// <returns><see cref="ForumChannel"/></returns>
    public async Task<ForumChannel> CreateForumTagAsync(ulong channelId, CreateForumTagRequest req, RequestOptions? options = null)
    {
        ChannelJson json = await SendRequestAsync<ChannelJson, CreateForumTagRequest>(HttpMethod.Post, $"/channels/{channelId}/tags", req, true, options: options);
        return (ForumChannel)Channel.Create(_client, json);
    }


    /// <summary>
    /// Update a forum tag.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="ChannelPermissions.ManageChannel"/>.
    /// </remarks>
    /// <returns><see cref="ForumChannel"/></returns>
    public async Task<ForumChannel> UpdateForumTagAsync(ulong channelId, ulong tagId, CreateForumTagRequest req, RequestOptions? options = null)
    {
        ChannelJson json = await SendRequestAsync<ChannelJson, CreateForumTagRequest>(HttpMethod.Patch, $"/channels/{channelId}/tags/{tagId}", req, true, options: options);
        return (ForumChannel)Channel.Create(_client, json);
    }

    /// <summary>
    /// Delete a forum tag.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="ChannelPermissions.ManageChannel"/>.
    /// </remarks>
    /// <returns><see cref="ForumChannel"/></returns>
    public async Task<ForumChannel> DeleteForumTagAsync(ulong channelId, ulong tagId, RequestOptions? options = null)
    {
        ChannelJson json = await SendRequestAsync<ChannelJson, dynamic>(HttpMethod.Delete, $"/channels/{channelId}/tags/{tagId}", null, true, options: options);
        return (ForumChannel)Channel.Create(_client, json);
    }

    #endregion

    #region Attachments API
    /// <summary>
    /// Delete an attachment.
    /// </summary>
    public async Task DeleteAttachmentAsync(string uploadFilename, RequestOptions? options = null)
        => await SendRequestRawAsync(HttpMethod.Delete, $"/attachments/{uploadFilename}", true, options: options);

    #endregion

    #region Favorite Media API
    /// <summary>
    /// Get all your favorite media.
    /// </summary>
    /// <returns>List of <see cref="FavoriteMedia"/></returns>
    public async Task<IEnumerable<FavoriteMedia>> GetCurrentUserFavoriteMediaAsync(RequestOptions? options = null)
    {
        IEnumerable<FavoriteMediaJson> json = await SendRequestAsync<IEnumerable<FavoriteMediaJson>>(HttpMethod.Get, "/users/@me/memes", true, options: options);
        return json.Select(x => FavoriteMedia.Create(_client, x));
    }

    /// <summary>
    /// Add a favorite media.
    /// </summary>
    /// <returns><see cref="FavoriteMedia"/></returns>
    public async Task<FavoriteMedia> PostCurrentUserFavoriteMediaAsync<TRequest>(TRequest data, RequestOptions? options = null)
    {
        FavoriteMediaJson json = await SendRequestAsync<FavoriteMediaJson, TRequest>(HttpMethod.Post, "/users/@me/memes", data, true, options: options);
        return FavoriteMedia.Create(_client, json);
    }

    /// <summary>
    /// Get your favorite media.
    /// </summary>
    /// <returns><see cref="FavoriteMedia"/></returns>
    public async Task<FavoriteMedia> GetCurrentUserFavoriteMediaAsync(ulong mediaId, RequestOptions? options = null)
    {
        FavoriteMediaJson json = await SendRequestAsync<FavoriteMediaJson>(HttpMethod.Get, $"/users/@me/memes/{mediaId}", true, options: options);
        return FavoriteMedia.Create(_client, json);
    }

    /// <summary>
    /// Update your favorite media.
    /// </summary>
    /// <returns><see cref="FavoriteMedia"/></returns>
    public async Task<FavoriteMedia> PatchCurrentUserFavoriteMediaAsync<TRequest>(ulong mediaId, TRequest data, RequestOptions? options = null)
    {
        FavoriteMediaJson json = await SendRequestAsync<FavoriteMediaJson, TRequest>(HttpMethod.Patch, $"/users/@me/memes/{mediaId}", data, true, options: options);
        return FavoriteMedia.Create(_client, json);
    }

    /// <summary>
    /// Delete your favorite media.
    /// </summary>
    public async Task DeleteCurrentUserFavoriteMediaAsync(ulong mediaId, RequestOptions? options = null)
        => await SendRequestRawAsync(HttpMethod.Delete, $"/users/@me/memes/{mediaId}", true, options: options);

    #endregion

    #region Invites API
    /// <summary>
    /// Get invite info from invite code.
    /// </summary>
    /// <returns><see cref="PartialInvite"/></returns>
    public async Task<PartialInvite?> GetInviteAsync(string inviteCode, RequestOptions? options = null)
    {
        PartialInviteJson? json = await SendRequestAsync<PartialInviteJson>(HttpMethod.Get, $"/invites/{inviteCode}", false, options: options);
        if (json == null)
            return null;

        return PartialInvite.Create(_client, json);
    }

    /// <summary>
    /// Join a guild by invite code.
    /// </summary>
    /// <remarks>
    /// User account only.
    /// </remarks>
    /// <returns><see cref="PartialInvite"/></returns>
    public async Task<PartialInvite> JoinGuildAsync(string inviteCode, RequestOptions? options = null)
    {
        PartialInviteJson json = await SendRequestAsync<PartialInviteJson>(HttpMethod.Post, $"/invites/{inviteCode}", true, options: options);
        return PartialInvite.Create(_client, json);
    }

    /// <summary>
    /// Delete an invite.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="ChannelPermissions.ViewChannel"/> and <see cref="ChannelPermissions.ManageChannel"/>.
    /// </remarks>
    public async Task DeleteInviteAsync(string inviteCode, RequestOptions? options = null)
        => await SendRequestRawAsync(HttpMethod.Delete, $"/invites/{inviteCode}", true, options: options);

    #endregion

    #region Read States API
    /// <summary>
    /// Clear read message stats for guilds or channels.
    /// </summary>
    /// <remarks>
    /// User account only.
    /// </remarks>
    public async Task PostReadStatesAckBulkAsync<TRequest>(TRequest data, RequestOptions? options = null)
        => await SendRequestAsync<TRequest>(HttpMethod.Post, "/read-states/ack-bulk", data, true, options: options);

    #endregion

    #region Guilds API
    /// <summary>
    /// Create a guild.
    /// </summary>
    /// <remarks>
    /// User account only.
    /// </remarks>
    /// <returns><see cref="Guild"/></returns>
    public async Task<Guild> CreateGuildAsync(CreateGuildRequest data, RequestOptions? options = null)
    {
        GuildJson json = await SendRequestAsync<GuildJson, CreateGuildRequest>(HttpMethod.Post, "/guilds", data, true, options: options);
        return Guild.Create(_client, json);
    }

    /// <summary>
    /// Get all guilds for the current user.
    /// </summary>
    /// <returns>List of <see cref="Guild"/></returns>
    public async Task<IEnumerable<Guild>> GetCurrentUserGuildsAsync(RequestOptions? options = null)
    {
        IEnumerable<GuildJson> json = await SendRequestAsync<IEnumerable<GuildJson>>(HttpMethod.Get, "/users/@me/guilds", true, options: options);
        return json.Select(x => Guild.Create(_client, x));
    }

    /// <summary>
    /// Leave a guild for the current user.
    /// </summary>
    public async Task LeaveGuildAsync(ulong guildId, RequestOptions? options = null)
        => await SendRequestRawAsync(HttpMethod.Delete, $"/users/@me/guilds/{guildId}", true, options: options);

    /// <summary>
    /// Get a guild that the current user is in.
    /// </summary>
    /// <returns><see cref="Guild"/></returns>
    public async Task<Guild?> GetGuildAsync(ulong guildId, RequestOptions? options = null)
    {
        GuildJson? json = await SendRequestAsync<GuildJson>(HttpMethod.Get, $"/guilds/{guildId}", false, options: options);
        if (json == null)
            return null;

        return Guild.Create(_client, json);
    }

    /// <summary>
    /// Update a guild that you manage.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="GuildPermissions.ManageGuild"/>.
    /// </remarks>
    /// <returns><see cref="Guild"/></returns>
    public async Task<Guild> UpdateGuildAsync(ulong guildId, GuildJson guild, RequestOptions? options = null)
    {
        GuildJson json = await SendRequestAsync<GuildJson, GuildJson>(HttpMethod.Patch, $"/guilds/{guildId}", guild, true, options: options);
        return Guild.Create(_client, json);
    }

    /// <summary>
    /// Delete a guild that you own.
    /// </summary>
    public async Task DeleteGuildAsync(ulong guildId, DeleteGuildRequest data, RequestOptions? options = null)
        => await SendRequestAsync(HttpMethod.Post, $"/guilds/{guildId}/delete", data, true, options: options);

    /// <summary>
    /// Get vanity url for a guild.
    /// </summary>
    /// <returns><see cref="GuildVanityUrl"/></returns>
    public async Task<GuildVanityUrl> GetGuildVanityUrlAsync(ulong guildId, RequestOptions? options = null)
    {
        GuildVanityUrlJson json = await SendRequestAsync<GuildVanityUrlJson>(HttpMethod.Get, $"/guilds/{guildId}/vanity-url", true, options: options);
        return GuildVanityUrl.Create(_client, json);
    }

    /// <summary>
    /// Update vanity url for a guild.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="GuildPermissions.ManageGuild"/>.
    /// </remarks>
    public async Task UpdateGuildVanityUrlAsync(ulong guildId, UpdateGuildVanityUrlRequest data, RequestOptions? options = null)
    {
        await SendRequestAsync(HttpMethod.Patch, $"/guilds/{guildId}/vanity-url", data, true, options: options);
    }

    /// <summary>
    /// Get a list of members for a guild, default and maximum 1000.
    /// </summary>
    /// <returns>List of <see cref="GuildMember"/></returns>
    public async Task<IEnumerable<GuildMember>> GetMembersAsync(ulong guildId, int limit = 1000, ulong? afterId = null, RestClientQueryParams? queryParams = null, RequestOptions? options = null)
    {
        queryParams ??= new RestClientQueryParams()
            .Add(QueryParams.Limit, limit)
            .AddIf(afterId != null, QueryParams.After, afterId);

        IEnumerable<GuildMemberJson> json = await SendRequestQueryParamsAsync<IEnumerable<GuildMemberJson>>(HttpMethod.Get, queryParams, $"/guilds/{guildId}/members", true, options: options);
        return json.Select(x => GuildMember.Create(_client, x));
    }

    /// <summary>
    /// Get current member for a guild.
    /// </summary>
    /// <returns><see cref="GuildMember"/></returns>
    public async Task<GuildMember?> GetCurrentMemberAsync(ulong guildId, RequestOptions? options = null)
    {
        GuildMemberJson? json = await SendRequestAsync<GuildMemberJson>(HttpMethod.Get, $"/guilds/{guildId}/members/@me", false, options: options);
        if (json == null)
            return null;

        return GuildMember.Create(_client, json);
    }

    /// <summary>
    /// Get a member in a guild.
    /// </summary>
    /// <returns><see cref="GuildMember"/></returns>
    public async Task<GuildMember?> GetMemberAsync(ulong guildId, ulong userId, RequestOptions? options = null)
    {
        GuildMemberJson json = await SendRequestAsync<GuildMemberJson>(HttpMethod.Get, $"/guilds/{guildId}/members/{userId}", false, options: options);
        if (json == null)
            return null;

        return GuildMember.Create(_client, json);
    }

    /// <summary>
    /// Update current member in a guild.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="GuildPermissions.ChangeNickname"/> to change your nickname.
    /// </remarks>
    /// <returns><see cref="GuildMember"/></returns>
    public async Task<GuildMember> UpdateCurrentMemberAsync(ulong guildId, GuildMemberJson member, RequestOptions? options = null)
    {
        GuildMemberJson json = await SendRequestAsync<GuildMemberJson, GuildMemberJson>(HttpMethod.Patch, $"/guilds/{guildId}/members/@me", member, true, options: options);
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
    /// <returns><see cref="GuildMember"/></returns>
    public async Task<GuildMember> UpdateMemberAsync(ulong guildId, ulong userId, GuildMemberJson member, RequestOptions? options = null)
    {
        GuildMemberJson json = await SendRequestAsync<GuildMemberJson, GuildMemberJson>(HttpMethod.Patch, $"/guilds/{guildId}/members/{userId}", member, true, options: options);
        return GuildMember.Create(_client, json);
    }

    /// <summary>
    /// Kick a member from the guild.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="GuildPermissions.KickMembers"/>.
    /// </remarks>
    public async Task KickMemberAsync(ulong guildId, ulong userId, string? reason = null, RequestOptions? options = null)
    {
        if (!string.IsNullOrEmpty(reason))
        {
            options ??= new RequestOptions();
            options.AuditLogReason = reason;
        }
        await SendRequestRawAsync(HttpMethod.Delete, $"/guilds/{guildId}/members/{userId}", true, options: options);
    }

    /// <summary>
    /// Change ownership of a guild to another member.
    /// </summary>
    /// <remarks>
    /// Requires guild ownership and user account only.
    /// </remarks>
    /// <returns><see cref="Guild"/></returns>
    public async Task<Guild> TransferOwnershipAsync(ulong guildId, GuildTransferOwnershipRequest data, RequestOptions? options = null)
    {
        GuildJson json = await SendRequestAsync<GuildJson, GuildTransferOwnershipRequest>(HttpMethod.Post, $"/guilds/{guildId}/transfer-ownership", data, true, options: options);
        return Guild.Create(_client, json);
    }

    /// <summary>
    /// Get bans for a guild.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="GuildPermissions.BanMembers"/>.
    /// </remarks>
    /// <returns>List of <see cref="GuildBan"/></returns>
    public async Task<IEnumerable<GuildBan>> GetBansAsync(ulong guildId, RequestOptions? options = null)
    {
        IEnumerable<GuildBanJson> json = await SendRequestAsync<IEnumerable<GuildBanJson>>(HttpMethod.Get, $"/guilds/{guildId}/bans", true, options: options);
        return json.Select(x => GuildBan.Create(_client, x));
    }

    /// <summary>
    /// Ban a member from the guild.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="GuildPermissions.BanMembers"/>.
    /// </remarks>
    public async Task BanMemberAsync(ulong guildId, ulong userId, string? reason = null, int? deleteMessageSeconds = null, int? banDurationSeconds = null, RequestOptions? options = null)
    {
        if (deleteMessageSeconds.HasValue)
        {
            if (deleteMessageSeconds.Value < 0)
                throw new Exception("Ban member delete message seconds can't be less then 0 seconds.");

            if (deleteMessageSeconds.Value > 604800)
                throw new Exception("Ban member delete message seconds can't be more than 7 days.");
        }

        if (banDurationSeconds.HasValue)
        {
            if (banDurationSeconds.Value < 60)
                throw new Exception("Ban member delete message seconds can't be less then 1 minute.");

            if (banDurationSeconds.Value > 63072000)
                throw new Exception("Ban member delete message seconds can't be more than 2 years.");
        }
        if (!string.IsNullOrEmpty(reason))
        {
            options ??= new RequestOptions();
            options.AuditLogReason = reason;
        }

        await SendRequestAsync(HttpMethod.Put, $"/guilds/{guildId}/bans/{userId}", new CreateGuildBanRequest
        {
            DeleteMessageSeconds = deleteMessageSeconds,
            BanDurationSeconds = banDurationSeconds,
        }, true, options: options);
    }

    /// <summary>
    /// Unban a user from the guild.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="GuildPermissions.BanMembers"/>.
    /// </remarks>
    public async Task UnbanMemberAsync(ulong guildId, ulong userId, RequestOptions? options = null)
        => await SendRequestRawAsync(HttpMethod.Delete, $"/guilds/{guildId}/bans/{userId}", true, options: options);

    /// <summary>
    /// Give role to a member in the guild.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="GuildPermissions.ManageRoles"/>.
    /// </remarks>
    public async Task AddMemberRoleAsync(ulong guildId, ulong userId, ulong roleId, RequestOptions? options = null)
        => await SendRequestRawAsync(HttpMethod.Put, $"/guilds/{guildId}/members/{userId}/roles/{roleId}", true, options: options);

    /// <summary>
    /// Remove role from a member in the guild.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="GuildPermissions.ManageRoles"/>.
    /// </remarks>
    public async Task RemoveMemberRoleAsync(ulong guildId, ulong userId, ulong roleId, RequestOptions? options = null)
        => await SendRequestRawAsync(HttpMethod.Delete, $"/guilds/{guildId}/members/{userId}/roles/{roleId}", true, options: options);

    /// <summary>
    /// Create a role in the guild.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="GuildPermissions.ManageRoles"/>.
    /// </remarks>
    /// <returns><see cref="Role"/></returns>
    public async Task<Role> CreateRoleAsync(ulong guildId, CreateGuildRoleRequest data, RequestOptions? options = null)
    {
        RoleJson json = await SendRequestAsync<RoleJson, CreateGuildRoleRequest>(HttpMethod.Post, $"/guilds/{guildId}/roles", data, true, options: options);
        return Role.Create(_client, json, guildId);
    }

    /// <summary>
    /// Update a role in the guild.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="GuildPermissions.ManageRoles"/>.
    /// </remarks>
    /// <returns><see cref="Role"/></returns>
    public async Task<Role> UpdateRoleAsync(ulong guildId, ulong roleId, UpdateGuildRoleRequest data, RequestOptions? options = null)
    {
        RoleJson json = await SendRequestAsync<RoleJson, UpdateGuildRoleRequest>(HttpMethod.Patch, $"/guilds/{guildId}/roles/{roleId}", data, true, options: options);
        return Role.Create(_client, json, guildId);
    }

    /// <summary>
    /// Update role positions in the guild.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="GuildPermissions.ManageRoles"/>.
    /// </remarks>
    public async Task UpdateRolePositionsAsync(ulong guildId, IEnumerable<RolePositionItemJson> positions, RequestOptions? options = null)
        => await SendRequestAsync(HttpMethod.Patch, $"/guilds/{guildId}/roles", positions, true, options: options);

    /// <summary>
    /// Delete a role in the guild.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="GuildPermissions.ManageRoles"/>.
    /// </remarks>
    public async Task DeleteRoleAsync(ulong guildId, ulong roleId, RequestOptions? options = null)
        => await SendRequestRawAsync(HttpMethod.Delete, $"/guilds/{guildId}/roles/{roleId}", true, options: options);

    /// <summary>
    /// Get all channels in the guild.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="ChannelPermissions.ViewChannel"/>.
    /// </remarks>
    /// <returns>List of <see cref="Channel"/></returns>
    public async Task<IEnumerable<Channel>> GetChannelsAsync(ulong guildId, RequestOptions? options = null)
    {
        IEnumerable<ChannelJson> json = await SendRequestAsync<IEnumerable<ChannelJson>>(HttpMethod.Get, $"/guilds/{guildId}/channels", true, options: options);
        return json.Select(x => Channel.Create(_client, x));
    }

    /// <summary>
    /// Create a channel in the guild.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="GuildPermissions.ManageChannels"/>.
    /// </remarks>
    /// <returns><see cref="Channel"/></returns>
    public async Task<Channel> CreateGuildChannelAsync(ulong guildId, CreateGuildChannelRequest data, RequestOptions? options = null)
    {
        ChannelJson json = await SendRequestAsync<ChannelJson, CreateGuildChannelRequest>(HttpMethod.Post, $"/guilds/{guildId}/channels", data, true, options: options);
        return Channel.Create(_client, json);
    }

    /// <inheritdoc cref="CreateGuildChannelAsync(ulong, CreateGuildChannelRequest, RequestOptions?)" />
    public async Task<CategoryChannel> CreateCategoryChannelAsync(ulong guildId, CreateCategoryChannelRequest data, RequestOptions? options = null)
    {
        ChannelJson json = await SendRequestAsync<ChannelJson, CreateCategoryChannelRequest>(HttpMethod.Post, $"/guilds/{guildId}/channels", data, true, options: options);
        return (CategoryChannel)Channel.Create(_client, json);
    }

    /// <inheritdoc cref="CreateGuildChannelAsync(ulong, CreateGuildChannelRequest, RequestOptions?)" />
    public async Task<TextChannel> CreateTextChannelAsync(ulong guildId, CreateTextChannelRequest data, RequestOptions? options = null)
    {
        ChannelJson json = await SendRequestAsync<ChannelJson, CreateTextChannelRequest>(HttpMethod.Post, $"/guilds/{guildId}/channels", data, true, options: options);
        return (TextChannel)Channel.Create(_client, json);
    }

    /// <inheritdoc cref="CreateGuildChannelAsync(ulong, CreateGuildChannelRequest, RequestOptions?)" />
    public async Task<VoiceChannel> CreateVoiceChannelAsync(ulong guildId, CreateVoiceChannelRequest data, RequestOptions? options = null)
    {
        ChannelJson json = await SendRequestAsync<ChannelJson, CreateVoiceChannelRequest>(HttpMethod.Post, $"/guilds/{guildId}/channels", data, true, options: options);
        return (VoiceChannel)Channel.Create(_client, json);
    }

    /// <inheritdoc cref="CreateGuildChannelAsync(ulong, CreateGuildChannelRequest, RequestOptions?)" />
    public async Task<LinkChannel> CreateLinkChannelAsync(ulong guildId, CreateLinkChannelRequest data, RequestOptions? options = null)
    {
        ChannelJson json = await SendRequestAsync<ChannelJson, CreateLinkChannelRequest>(HttpMethod.Post, $"/guilds/{guildId}/channels", data, true, options: options);
        return (LinkChannel)Channel.Create(_client, json);
    }

    /// <inheritdoc cref="CreateGuildChannelAsync(ulong, CreateGuildChannelRequest, RequestOptions?)" />
    public async Task<NewsChannel> CreateNewsChannelAsync(ulong guildId, CreateNewsChannelRequest data, RequestOptions? options = null)
    {
        ChannelJson json = await SendRequestAsync<ChannelJson, CreateNewsChannelRequest>(HttpMethod.Post, $"/guilds/{guildId}/channels", data, true, options: options);
        return (NewsChannel)Channel.Create(_client, json);
    }

    /// <inheritdoc cref="CreateGuildChannelAsync(ulong, CreateGuildChannelRequest, RequestOptions?)" />
    public async Task<ForumChannel> CreateForumChannelAsync(ulong guildId, CreateForumChannelRequest data, RequestOptions? options = null)
    {
        ChannelJson json = await SendRequestAsync<ChannelJson, CreateForumChannelRequest>(HttpMethod.Post, $"/guilds/{guildId}/channels", data, true, options: options);
        return (ForumChannel)Channel.Create(_client, json);
    }

    /// <inheritdoc cref="CreateGuildChannelAsync(ulong, CreateGuildChannelRequest, RequestOptions?)" />
    public async Task<MediaChannel> CreateMediaChannelAsync(ulong guildId, CreateMediaChannelRequest data, RequestOptions? options = null)
    {
        ChannelJson json = await SendRequestAsync<ChannelJson, CreateMediaChannelRequest>(HttpMethod.Post, $"/guilds/{guildId}/channels", data, true, options: options);
        return (MediaChannel)Channel.Create(_client, json);
    }

    /// <summary>
    /// Update channel positions in a guild.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="GuildPermissions.ManageChannels"/>.
    /// </remarks>
    public async Task UpdateChannelPositionsAsync(ulong guildId, IEnumerable<ChannelPositionUpdateRequestItem> list, RequestOptions? options = null)
        => await SendRequestAsync(HttpMethod.Patch, $"/guilds/{guildId}/channels", list, true, options: options);

    /// <summary>
    /// Search the guild.
    /// </summary>
    public async Task<TResponse> SearchGuildAsync<TRequest, TResponse>(ulong guildId, TRequest data, RequestOptions? options = null)
        => await SendRequestAsync<TResponse, TRequest>(HttpMethod.Post, $"/guilds/{guildId}/search", data, true, options: options);

    /// <summary>
    /// Search audit logs in the guild.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="GuildPermissions.ViewAuditLog"/>.
    /// </remarks>
    /// <returns><see cref="AuditLogListJson"/></returns>
    public async Task<AuditLogListJson> GetAuditLogAsync(ulong guildId, GuildAuditLogListRequest data, RequestOptions? options = null)
        => await SendRequestAsync<AuditLogListJson, GuildAuditLogListRequest>(HttpMethod.Post, $"/guilds/{guildId}/audit-logs", data, true, options: options);

    /// <summary>
    /// Create emojis for the guild.
    /// </summary>
    /// <remarks>
    /// Requires either <see cref="GuildPermissions.CreateExpressions"/> or <see cref="GuildPermissions.ManageExpressions"/>.
    /// </remarks>
    /// <returns><see cref="GuildEmoji"/></returns>
    public async Task<GuildEmoji> CreateEmojiAsync(ulong guildId, CreateGuildEmojiRequest data, RequestOptions? options = null)
    {
        GuildEmojiJson json = await SendRequestAsync<GuildEmojiJson, CreateGuildEmojiRequest>(HttpMethod.Post, $"/guilds/{guildId}/emojis", data, true, options: options);
        return GuildEmoji.Create(_client, json, guildId);
    }

    /// <summary>
    /// Create multiple emojis for a guild.
    /// </summary>
    /// <remarks>
    /// Requires either <see cref="GuildPermissions.CreateExpressions"/> or <see cref="GuildPermissions.ManageExpressions"/>.
    /// </remarks>
    /// <returns><see cref="GuildEmojiBulkCreateJson"/></returns>
    public async Task<GuildEmojiBulkCreateJson> CreateEmojiBulkAsync(ulong guildId, BulkCreateGuildEmojisRequest data, RequestOptions? options = null)
        => await SendRequestAsync<GuildEmojiBulkCreateJson, BulkCreateGuildEmojisRequest>(HttpMethod.Post, $"/guilds/{guildId}/emojis/bulk", data, true, options: options);

    /// <summary>
    /// Get all emojis in the guild.
    /// </summary>
    /// <remarks>
    /// Requires either <see cref="GuildPermissions.CreateExpressions"/> or <see cref="GuildPermissions.ManageExpressions"/>.
    /// </remarks>
    /// <returns>List of <see cref="GuildEmoji"/></returns>
    public async Task<IEnumerable<GuildEmoji>> GetEmojisAsync(ulong guildId, RequestOptions? options = null)
    {
        IEnumerable<GuildEmojiJson> json = await SendRequestAsync<IEnumerable<GuildEmojiJson>>(HttpMethod.Get, $"/guilds/{guildId}/emojis", true, options: options);
        return json.Select(x => GuildEmoji.Create(_client, x, guildId));
    }

    /// <summary>
    /// Update an emoji in the guild.
    /// </summary>
    /// <remarks>
    /// Requires either <see cref="GuildPermissions.CreateExpressions"/> or <see cref="GuildPermissions.ManageExpressions"/>.
    /// </remarks>
    /// <returns><see cref="GuildEmoji"/></returns>
    public async Task<GuildEmoji> UpdateEmojiAsync(ulong guildId, ulong emojiId, UpdateGuildEmojiRequest data, RequestOptions? options = null)
    {
        GuildEmojiJson json = await SendRequestAsync<GuildEmojiJson, UpdateGuildEmojiRequest>(HttpMethod.Patch, $"/guilds/{guildId}/emojis/{emojiId}", data, true, options: options);
        return GuildEmoji.Create(_client, json, guildId);
    }

    /// <summary>
    /// Delete an emoji in the guild.
    /// </summary>
    /// <remarks>
    /// Requires either <see cref="GuildPermissions.CreateExpressions"/> or <see cref="GuildPermissions.ManageExpressions"/>.
    /// </remarks>
    public async Task DeleteEmojiAsync(ulong guildId, ulong emojiId, RequestOptions? options = null)
        => await SendRequestRawAsync(HttpMethod.Delete, $"/guilds/{guildId}/emojis/{emojiId}", true, options: options);

    /// <summary>
    /// Create stickers for a guild.
    /// </summary>
    /// <remarks>
    /// Requires either <see cref="GuildPermissions.CreateExpressions"/> or <see cref="GuildPermissions.ManageExpressions"/>.
    /// </remarks>
    /// <returns><see cref="GuildSticker"/></returns>
    public async Task<GuildSticker> CreateStickerAsync(ulong guildId, CreateGuildStickerRequest data, RequestOptions? options = null)
    {
        GuildStickerJson json = await SendRequestAsync<GuildStickerJson, CreateGuildStickerRequest>(HttpMethod.Post, $"/guilds/{guildId}/stickers", data, true, options: options);
        return GuildSticker.Create(_client, json, guildId);
    }

    /// <summary>
    /// Create multiple stickers for a guild.
    /// </summary>
    /// <remarks>
    /// Requires either <see cref="GuildPermissions.CreateExpressions"/> or <see cref="GuildPermissions.ManageExpressions"/>.
    /// </remarks>
    /// <returns><see cref="GuildStickerBulkCreateJson"/></returns>
    public async Task<GuildStickerBulkCreateJson> CreateStickerBulkAsync(ulong guildId, BulkCreateGuildStickersRequest data, RequestOptions? options = null)
        => await SendRequestAsync<GuildStickerBulkCreateJson, BulkCreateGuildStickersRequest>(HttpMethod.Post, $"/guilds/{guildId}/stickers/bulk", data, true, options: options);

    /// <summary>
    /// Get all stickers in the guild.
    /// </summary>
    /// <remarks>
    /// Requires either <see cref="GuildPermissions.CreateExpressions"/> or <see cref="GuildPermissions.ManageExpressions"/>.
    /// </remarks>
    /// <returns>List of <see cref="GuildSticker"/></returns>
    public async Task<IEnumerable<GuildSticker>> GetStickersAsync(ulong guildId, RequestOptions? options = null)
    {
        IEnumerable<GuildStickerJson> json = await SendRequestAsync<IEnumerable<GuildStickerJson>>(HttpMethod.Get, $"/guilds/{guildId}/stickers", true, options: options);
        return json.Select(x => GuildSticker.Create(_client, x, guildId));
    }

    /// <summary>
    /// Update a sticker in the guild.
    /// </summary>
    /// <remarks>
    /// Requires either <see cref="GuildPermissions.CreateExpressions"/> or <see cref="GuildPermissions.ManageExpressions"/>.
    /// </remarks>
    /// <returns><see cref="GuildSticker"/></returns>
    public async Task<GuildSticker> UpdateStickerAsync(ulong guildId, ulong stickerId, UpdateGuildStickerRequest data, RequestOptions? options = null)
    {
        GuildStickerJson json = await SendRequestAsync<GuildStickerJson, UpdateGuildStickerRequest>(HttpMethod.Patch, $"/guilds/{guildId}/stickers/{stickerId}", data, true, options: options);
        return GuildSticker.Create(_client, json, guildId);
    }

    /// <summary>
    /// Delete a sticker in the guild.
    /// </summary>
    /// <remarks>
    /// Requires either <see cref="GuildPermissions.CreateExpressions"/> or <see cref="GuildPermissions.ManageExpressions"/>.
    /// </remarks>
    public async Task DeleteStickerAsync(ulong guildId, ulong stickerId, RequestOptions? options = null)
        => await SendRequestRawAsync(HttpMethod.Delete, $"/guilds/{guildId}/stickers/{stickerId}", true, options: options);

    /// <summary>
    /// Get all invites for the guild.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="ChannelPermissions.ManageChannel"/>.
    /// </remarks>
    /// <returns>List of <see cref="Invite"/></returns>
    public async Task<IEnumerable<Invite>> GetGuildInvitesAsync(ulong guildId, RequestOptions? options = null)
    {
        IEnumerable<InviteJson> json = await SendRequestAsync<IEnumerable<InviteJson>>(HttpMethod.Get, $"/guilds/{guildId}/invites", true, options: options);
        return json.Select(x => Invite.Create(_client, x));
    }

    /// <summary>
    /// Get all webhooks for the guild.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="ChannelPermissions.ManageWebhooks"/>.
    /// </remarks>
    /// <returns>List of <see cref="Webhook"/></returns>
    public async Task<IEnumerable<Webhook>> GetGuildWebhooksAsync(ulong guildId, RequestOptions? options = null)
    {
        IEnumerable<WebhookJson> json = await SendRequestAsync<IEnumerable<WebhookJson>>(HttpMethod.Get, $"/guilds/{guildId}/webhooks", true, options: options);
        return json.Select(x => Webhook.Create(_client, x));
    }

    #endregion

    #region Discovery API



    #endregion

    #region Klipy API
    /// <summary>
    /// Search klipy gifs.
    /// </summary>
    /// <returns>List of <see cref="Gif"/></returns>
    public async Task<IEnumerable<Gif>> SearchKlipyAsync(string query, RequestOptions? options = null)
    {
        IEnumerable<GifJson> json = await SendRequestAsync<IEnumerable<GifJson>>(HttpMethod.Get, $"/klipy/search?q={query}", true, options: options);
        return json.Select(x => Gif.Create(_client, x));
    }

    /// <summary>
    /// Get featured klipy gifs.
    /// </summary>
    /// <returns><see cref="GifFeaturedJson"/></returns>
    public async Task<GifFeaturedJson> GetKlipyFeaturedAsync(RequestOptions? options = null)
    {
        return await SendRequestAsync<GifFeaturedJson>(HttpMethod.Get, "/klipy/featured", true, options: options);
    }

    /// <summary>
    /// Get trending klipy gifs.
    /// </summary>
    /// <returns>List of <see cref="Gif"/></returns>
    public async Task<IEnumerable<Gif>> GetKlipyTrendingGifsAsync(RequestOptions? options = null)
    {
        IEnumerable<GifJson> json = await SendRequestAsync<IEnumerable<GifJson>>(HttpMethod.Get, "/klipy/trending-gifs", true, options: options);
        return json.Select(x => Gif.Create(_client, x));
    }


    #endregion

    #region Apps API

    /// <summary>
    /// Gets the current application data.
    /// </summary>
    /// <returns><see cref="CurrentApplication"/></returns>
    public async Task<CurrentApplication> GetCurrentApplicationAsync(RequestOptions? options = null)
    {
        CurrentApplicationJson json = await SendRequestAsync<CurrentApplicationJson>(HttpMethod.Get, $"/oauth2/applications/@me", true, options: options);
        return CurrentApplication.Create(_client, json);
    }

    /// <summary>
    /// Get a public app.
    /// </summary>
    /// <returns><see cref="Application"/></returns>
    public async Task<Application?> GetPublicAppAsync(ulong id, RequestOptions? options = null)
    {
        ApplicationJson? json = await SendRequestAsync<ApplicationJson>(HttpMethod.Get, $"/oauth2/applications/{id}/public", false, options: options);
        if (json == null)
            return null;

        return Application.Create(_client, json);
    }

    #endregion

    #region Users API
    /// <summary>
    /// Get the current user.
    /// </summary>
    /// <returns><see cref="CurrentUser"/></returns>
    public async Task<CurrentUser> GetCurrentUserAsync(RequestOptions? options = null)
    {
        CurrentUserJson json = await SendRequestAsync<CurrentUserJson>(HttpMethod.Get, "/users/@me", true, options: options);
        return CurrentUser.Create(_client, json);
    }

    /// <summary>
    /// Update the current user.
    /// </summary>
    /// <returns><see cref="CurrentUser"/></returns>
    public async Task<CurrentUser> UpdateCurrentUserAsync(UserJson user, RequestOptions? options = null)
    {
        CurrentUserJson json = await SendRequestAsync<CurrentUserJson, UserJson>(HttpMethod.Patch, "/users/@me", user, true, options: options);
        return CurrentUser.Create(_client, json);
    }

    /// <summary>
    /// Check if a username is available.
    /// </summary>
    /// <returns><see cref="UsernameAvailableJson"/></returns>
    public async Task<UsernameAvailableJson> CheckUsernameAvailabilityAsync(string username, string discriminator, RequestOptions? options = null)
    {
        return await SendRequestAsync<UsernameAvailableJson>(HttpMethod.Get, $"/users/check-tag?username={username}&discriminator={discriminator}", true, options: options);
    }

    /// <summary>
    /// Get a user.
    /// </summary>
    /// <returns><see cref="User"/></returns>
    public async Task<User?> GetUserAsync(ulong userId, RequestOptions? options = null)
    {
        UserJson? json = await SendRequestAsync<UserJson>(HttpMethod.Get, $"/users/{userId}", false, options: options);
        if (json == null)
            return null;

        return User.Create(_client, json);
    }

    /// <summary>
    /// Get a user's profile.
    /// </summary>
    /// <remarks>
    /// Requires mutual friend or guild.
    /// </remarks>
    /// <returns><see cref="UserProfileResponse"/></returns>
    public async Task<UserProfileResponse> GetUserProfileAsync(ulong targetId, string? guildId = null, bool mutualFriends = false, bool mutualGuilds = false, RequestOptions? options = null)
        => await SendRequestAsync<UserProfileResponse>(HttpMethod.Get,
            new QueryBuilder($"/users/{targetId}/profile").With("guild_id", guildId).With("with_mutual_friends", mutualFriends).With("with_mutual_guilds", mutualGuilds).Build(), true, options: options);

    /// <summary>
    /// Get current user settings.
    /// </summary>
    /// <remarks>
    /// User account only.
    /// </remarks>
    /// <returns><see cref="UserSettings"/></returns>
    public async Task<UserSettings> GetCurrentUserSettingsAsync(RequestOptions? options = null)
    {
        UserSettingsJson json = await SendRequestAsync<UserSettingsJson>(HttpMethod.Get, "/users/@me/settings", true, options: options);
        return UserSettings.Create(_client, json);
    }

    /// <summary>
    /// Update current user settings.
    /// </summary>
    /// <remarks>
    /// User account only.
    /// </remarks>
    /// <returns><see cref="UserSettings"/></returns>
    public async Task<UserSettings> UpdateCurrentUserSettingsAsync<TRequest>(TRequest settings, RequestOptions? options = null)
    {
        UserSettingsJson json = await SendRequestAsync<UserSettingsJson, TRequest>(HttpMethod.Patch, "/users/@me/settings", settings, true, options: options);
        return UserSettings.Create(_client, json);
    }

    /// <summary>
    /// Set current user status.
    /// </summary>
    /// <returns><see cref="UserSettings"/></returns>
    public async Task<UserSettings> SetCustomStatusAsync(UserCustomStatusJson status, RequestOptions? options = null)
    {
        UserSettingsJson json = await SendRequestAsync<UserSettingsJson, UpdateCustomStatus>(HttpMethod.Patch, "/users/@me/settings", new UpdateCustomStatus(status), true, options: options);
        return UserSettings.Create(_client, json);
    }

    /// <summary>
    /// Get all current user notes.
    /// </summary>
    public async Task<TResponse> GetCurrentUserNotesAsync<TResponse>(RequestOptions? options = null)
        => await SendRequestAsync<TResponse>(HttpMethod.Get, "/users/@me/notes", true, options: options);

    /// <summary>
    /// Get current user note for target user.
    /// </summary>
    public async Task<TResponse> GetCurrentUserNoteAsync<TResponse>(ulong targetId, RequestOptions? options = null)
        => await SendRequestAsync<TResponse>(HttpMethod.Get, $"/users/@me/notes/{targetId}", true, options: options);

    /// <summary>
    /// Add/update current user note for target user.
    /// </summary>
    public async Task<TResponse> PutCurrentUserNoteAsync<TRequest, TResponse>(ulong targetId, TRequest data, RequestOptions? options = null)
        => await SendRequestAsync<TResponse, TRequest>(HttpMethod.Put, $"/users/@me/notes/{targetId}", data, true, options: options);

    /// <summary>
    /// Get current user mention notifications.
    /// </summary>
    /// <remarks>
    /// User account only.
    /// </remarks>
    public async Task<TResponse> GetCurrentUserMentionsAsync<TResponse>(RequestOptions? options = null)
        => await SendRequestAsync<TResponse>(HttpMethod.Get, "/users/@me/mentions", true, options: options);

    /// <summary>
    /// Delete current user mentioned notifications.
    /// </summary>
    /// <remarks>
    /// User account only.
    /// </remarks>
    public async Task DeleteCurrentUserMentionAsync(ulong messageId, RequestOptions? options = null)
        => await SendRequestRawAsync(HttpMethod.Delete, $"/users/@me/mentions/{messageId}", true, options: options);

    /// <summary>
    /// Enable MFA for current user.
    /// </summary>
    /// <remarks>
    /// User account only.
    /// </remarks>
    public async Task<TResponse> PostCurrentUserMfaTotpEnableAsync<TRequest, TResponse>(TRequest data, RequestOptions? options = null)
        => await SendRequestAsync<TResponse, TRequest>(HttpMethod.Post, "/users/@me/mfa/totp/enable", data, true, options: options);

    /// <summary>
    /// Disable MFA for current user.
    /// </summary>
    /// <remarks>
    /// User account only.
    /// </remarks>
    public async Task PostCurrentUserMfaTotpDisableAsync<TRequest>(TRequest data, RequestOptions? options = null)
        => await SendRequestAsync<TRequest>(HttpMethod.Post, "/users/@me/mfa/totp/disable", data, true, options: options);

    /// <summary>
    /// Get/create MFA backup codes for current user.
    /// </summary>
    /// <remarks>
    /// User account only.
    /// </remarks>
    public async Task<TResponse> PostCurrentUserMfaBackupCodesAsync<TRequest, TResponse>(TRequest data, RequestOptions? options = null)
        => await SendRequestAsync<TResponse, TRequest>(HttpMethod.Post, "/users/@me/mfa/backup-codes", data, true, options: options);

    /// <summary>
    /// Send phone verification for current user.
    /// </summary>
    /// <remarks>
    /// User account only.
    /// </remarks>
    public async Task PostCurrentUserPhoneSendVerificationAsync<TRequest>(TRequest data, RequestOptions? options = null)
        => await SendRequestAsync<TRequest>(HttpMethod.Post, "/users/@me/phone/send-verification", data, true, options: options);

    /// <summary>
    /// Verify phone verification for current user.
    /// </summary>
    /// <remarks>
    /// User account only.
    /// </remarks>
    public async Task<TResponse> PostCurrentUserPhoneVerifyAsync<TRequest, TResponse>(TRequest data, RequestOptions? options = null)
        => await SendRequestAsync<TResponse, TRequest>(HttpMethod.Post, "/users/@me/phone/verify", data, true, options: options);

    /// <summary>
    /// Set phone number for current user.
    /// </summary>
    /// <remarks>
    /// User account only.
    /// </remarks>
    public async Task<TResponse> PostCurrentUserPhoneAsync<TRequest, TResponse>(TRequest data, RequestOptions? options = null)
        => await SendRequestAsync<TResponse, TRequest>(HttpMethod.Post, "/users/@me/phone", data, true, options: options);

    /// <summary>
    /// Remove phone number for current user.
    /// </summary>
    /// <remarks>
    /// User account only.
    /// </remarks>
    public async Task DeleteCurrentUserPhoneAsync<TRequest>(TRequest data, RequestOptions? options = null)
        => await SendRequestAsync<TRequest>(HttpMethod.Delete, "/users/@me/phone", data, true, options: options);

    /// <summary>
    /// Enable MFA SMS for current user.
    /// </summary>
    /// <remarks>
    /// User account only.
    /// </remarks>
    public async Task PostCurrentUserMfaSmsEnableAsync<TRequest>(TRequest data, RequestOptions? options = null)
        => await SendRequestAsync<TRequest>(HttpMethod.Post, "/users/@me/mfa/sms/enable", data, true, options: options);

    /// <summary>
    /// Disable MFA SMS for current user.
    /// </summary>
    /// <remarks>
    /// User account only.
    /// </remarks>
    public async Task PostCurrentUserMfaSmsDisableAsync<TRequest>(TRequest data, RequestOptions? options = null)
        => await SendRequestAsync<TRequest>(HttpMethod.Post, "/users/@me/mfa/sms/disable", data, true, options: options);

    /// <summary>
    /// Get webauthn MFA for current user.
    /// </summary>
    /// <remarks>
    /// User account only.
    /// </remarks>
    public async Task<TResponse> GetCurrentUserMfaWebauthnCredentialsAsync<TResponse>(RequestOptions? options = null)
        => await SendRequestAsync<TResponse>(HttpMethod.Get, "/users/@me/mfa/webauthn/credentials", true, options: options);

    /// <summary>
    /// Register webauthn for current user.
    /// </summary>
    /// <remarks>
    /// User account only.
    /// </remarks>
    public async Task<TResponse> PostCurrentUserMfaWebauthnCredentialsRegistrationOptionsAsync<TResponse>(RequestOptions? options = null)
        => await SendRequestAsync<TResponse>(HttpMethod.Post, "/users/@me/mfa/webauthn/credentials/registration-options", true, options: options);

    /// <summary>
    /// Set webauthn MFA for current user.
    /// </summary>
    /// <remarks>
    /// User account only.
    /// </remarks>
    public async Task<TResponse> PostCurrentUserMfaWebauthnCredentialsAsync<TRequest, TResponse>(TRequest data, RequestOptions? options = null)
        => await SendRequestAsync<TResponse, TRequest>(HttpMethod.Post, "/users/@me/mfa/webauthn/credentials", data, true, options: options);

    /// <summary>
    /// Update webauthn MFA for current user.
    /// </summary>
    /// <remarks>
    /// User account only.
    /// </remarks>
    public async Task<TResponse> PatchCurrentUserMfaWebauthnCredentialAsync<TRequest, TResponse>(ulong credentialId, TRequest data, RequestOptions? options = null)
        => await SendRequestAsync<TResponse, TRequest>(HttpMethod.Patch, $"/users/@me/mfa/webauthn/credentials/{credentialId}", data, true, options: options);

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public async Task DeleteCurrentUserMfaWebauthnCredentialAsync(ulong credentialId, RequestOptions? options = null)
        => await SendRequestRawAsync(HttpMethod.Delete, $"/users/@me/mfa/webauthn/credentials/{credentialId}", true, options: options);

    public async Task<TResponse> GetCurrentUserSavedMessagesAsync<TResponse>(RequestOptions? options = null)
        => await SendRequestAsync<TResponse>(HttpMethod.Get, "/users/@me/saved-messages", true, options: options);

    public async Task<TResponse> PostCurrentUserSavedMessageAsync<TRequest, TResponse>(TRequest data)
        => await SendRequestAsync<TResponse, TRequest>(HttpMethod.Post, "/users/@me/saved-messages", data, true);

    public async Task DeleteCurrentUserSavedMessageAsync(ulong messageId, RequestOptions? options = null)
        => await SendRequestRawAsync(HttpMethod.Delete, $"/users/@me/saved-messages/{messageId}", true, options: options);

    public async Task<TResponse> GetCurrentUserChannelsAsync<TResponse>(RequestOptions? options = null)
        => await SendRequestAsync<TResponse>(HttpMethod.Get, "/users/@me/channels", true, options: options);

    public async Task<TResponse> PostCurrentUserChannelAsync<TRequest, TResponse>(TRequest data)
        => await SendRequestAsync<TResponse, TRequest>(HttpMethod.Post, "/users/@me/channels", data, true);

    public async Task PutCurrentUserChannelPinAsync(ulong channelId, RequestOptions? options = null)
        => await SendRequestRawAsync(HttpMethod.Put, $"/users/@me/channels/{channelId}/pin", true, options: options);

    public async Task DeleteCurrentUserChannelPinAsync(ulong channelId, RequestOptions? options = null)
        => await SendRequestRawAsync(HttpMethod.Delete, $"/users/@me/channels/{channelId}/pin", true, options: options);

    public async Task<TResponse> GetCurrentUserRelationshipsAsync<TResponse>(RequestOptions? options = null)
        => await SendRequestAsync<TResponse>(HttpMethod.Get, "/users/@me/relationships", true, options: options);

    public async Task<TResponse> PostCurrentUserRelationshipAsync<TRequest, TResponse>(TRequest data, RequestOptions? options = null)
        => await SendRequestAsync<TResponse, TRequest>(HttpMethod.Post, "/users/@me/relationships", data, true, options: options);

    public async Task<TResponse> PostCurrentUserRelationshipWithUserAsync<TRequest, TResponse>(ulong userId, TRequest data, RequestOptions? options = null)
        => await SendRequestAsync<TResponse, TRequest>(HttpMethod.Post, $"/users/@me/relationships/{userId}", data, true, options: options);

    public async Task PutCurrentUserRelationshipAsync<TRequest>(ulong userId, TRequest data, RequestOptions? options = null)
        => await SendRequestAsync<TRequest>(HttpMethod.Put, $"/users/@me/relationships/{userId}", data, true, options: options);

    public async Task DeleteCurrentUserRelationshipAsync(ulong userId, RequestOptions? options = null)
        => await SendRequestRawAsync(HttpMethod.Delete, $"/users/@me/relationships/{userId}", true, options: options);

    public async Task<TResponse> PatchCurrentUserGuildSettingsSelfAsync<TRequest, TResponse>(TRequest data, RequestOptions? options = null)
        => await SendRequestAsync<TResponse, TRequest>(HttpMethod.Patch, "/users/@me/guilds/@me/settings", data, true, options: options);

    public async Task<TResponse> PatchCurrentUserGuildSettingsAsync<TRequest, TResponse>(ulong guildId, TRequest data, RequestOptions? options = null)
        => await SendRequestAsync<TResponse, TRequest>(HttpMethod.Patch, $"/users/@me/guilds/{guildId}/settings", data, true, options: options);

    public async Task PostCurrentUserDisableAsync<TRequest>(TRequest data, RequestOptions? options = null)
        => await SendRequestAsync<TRequest>(HttpMethod.Post, "/users/@me/disable", data, true, options: options);

    public async Task PostCurrentUserDeleteAsync<TRequest>(TRequest data, RequestOptions? options = null)
        => await SendRequestAsync<TRequest>(HttpMethod.Post, "/users/@me/delete", data, true, options: options);

    public async Task<TResponse> PostCurrentUserPushSubscribeAsync<TRequest, TResponse>(TRequest data, RequestOptions? options = null)
        => await SendRequestAsync<TResponse, TRequest>(HttpMethod.Post, "/users/@me/push/subscribe", data, true, options: options);

    public async Task<TResponse> GetCurrentUserPushSubscriptionsAsync<TResponse>(RequestOptions? options = null)
        => await SendRequestAsync<TResponse>(HttpMethod.Get, "/users/@me/push/subscriptions", true, options: options);

    public async Task DeleteCurrentUserPushSubscriptionAsync(ulong subscriptionId, RequestOptions? options = null)
        => await SendRequestRawAsync(HttpMethod.Delete, $"/users/@me/push/subscriptions/{subscriptionId}", true, options: options);

    public async Task<TResponse> PostCurrentUserHarvestAsync<TRequest, TResponse>(TRequest data, RequestOptions? options = null)
        => await SendRequestAsync<TResponse, TRequest>(HttpMethod.Post, "/users/@me/harvest", data, true, options: options);

    public async Task<TResponse> GetCurrentUserHarvestLatestAsync<TResponse>(RequestOptions? options = null)
        => await SendRequestAsync<TResponse>(HttpMethod.Get, "/users/@me/harvest/latest", true, options: options);

    public async Task<TResponse> GetCurrentUserHarvestAsync<TResponse>(string harvestId, RequestOptions? options = null)
        => await SendRequestAsync<TResponse>(HttpMethod.Get, $"/users/@me/harvest/{harvestId}", true, options: options);

    public async Task<TResponse> GetCurrentUserHarvestDownloadAsync<TResponse>(string harvestId, RequestOptions? options = null)
        => await SendRequestAsync<TResponse>(HttpMethod.Get, $"/users/@me/harvest/{harvestId}/download", true, options: options);

    public async Task PostCurrentUserPreloadMessagesAsync<TRequest>(TRequest data, RequestOptions? options = null)
        => await SendRequestAsync<TRequest>(HttpMethod.Post, "/users/@me/preload-messages", data, true, options: options);

    public async Task PostCurrentUserMessagesDeleteAsync<TRequest>(TRequest data, RequestOptions? options = null)
        => await SendRequestAsync<TRequest>(HttpMethod.Post, "/users/@me/messages/delete", data, true, options: options);
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

    #endregion

    #region Webhooks API
    /// <summary>
    /// Get webhook in a guild channel.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="ChannelPermissions.ManageWebhooks"/>.
    /// </remarks>
    /// <returns><see cref="Webhook"/></returns>
    public async Task<Webhook?> GetWebhookAsync(ulong webhookId, RequestOptions? options = null)
    {
        WebhookJson? json = await SendRequestAsync<WebhookJson>(HttpMethod.Get, $"/webhooks/{webhookId}", false, options: options);
        if (json == null)
            return null;

        return Webhook.Create(_client, json);
    }

    /// <summary>
    /// Update webhook in a guild channel.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="ChannelPermissions.ManageWebhooks"/>.
    /// </remarks>
    /// <returns><see cref="Webhook"/></returns>
    public async Task<Webhook> UpdateWebhookAsync<TRequest>(ulong webhookId, TRequest data, RequestOptions? options = null)
    {
        WebhookJson json = await SendRequestAsync<WebhookJson, TRequest>(HttpMethod.Patch, $"/webhooks/{webhookId}", data, true, options: options);
        return Webhook.Create(_client, json);
    }

    /// <summary>
    /// Delete webhook in a guild channel.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="ChannelPermissions.ManageWebhooks"/>.
    /// </remarks>
    public async Task DeleteWebhookAsync(ulong webhookId, RequestOptions? options = null)
        => await SendRequestRawAsync(HttpMethod.Delete, $"/webhooks/{webhookId}", true, options: options);

    /// <summary>
    /// Get webhook using the webhook token.
    /// </summary>
    /// <returns><see cref="Webhook"/></returns>
    public async Task<Webhook?> GetWebhookWithTokenAsync(ulong webhookId, string token, RequestOptions? options = null)
    {
        WebhookJson? json = await SendRequestAsync<WebhookJson>(HttpMethod.Get, $"/webhooks/{webhookId}/{token}", false, options: options);
        if (json == null)
            return null;

        return Webhook.Create(_client, json);
    }

    /// <summary>
    /// Update webhook using the webhook token.
    /// </summary>
    /// <returns><see cref="Webhook"/></returns>
    public async Task<Webhook> UpdateWebhookWithTokenAsync<TRequest>(ulong webhookId, string token, TRequest data, RequestOptions? options = null)
    {
        WebhookJson json = await SendRequestAsync<WebhookJson, TRequest>(HttpMethod.Patch, $"/webhooks/{webhookId}/{token}", data, false, options: options);
        return Webhook.Create(_client, json);
    }

    /// <summary>
    /// Delete webhook using the webhook token.
    /// </summary>
    public async Task DeleteWebhookWithTokenAsync(ulong webhookId, string token, RequestOptions? options = null)
        => await SendRequestRawAsync(HttpMethod.Delete, $"/webhooks/{webhookId}/{token}", false, false, options: options);

    /// <summary>
    /// Send message using the webhook token in a guild channel.
    /// </summary>
    public async Task ExecuteWebhookAsync(ulong webhookId, string token, string? content = null, List<EmbedRequest>? embeds = null,
        string? username = null, string? avatarUrl = null,
        MessageReferenceRequest? reference = null, AllowedMentionsRequest? allowedMentions = null, MessageFlag flags = MessageFlag.None,
        string? nonce = null, ulong? favoriteMediaId = null, bool? tts = null, List<ulong>? stickerIds = null, List<AttachmentRequest>? attachments = null, RequestOptions? options = null)
    {
        MessageRequest req = new MessageRequest
        {
            Content = content,
            Embeds = embeds?.ToArray(),
            MessageReference = reference,
            AllowedMentions = allowedMentions,
            Flags = flags,
            Nonce = nonce,
            FavoriteMediaId = favoriteMediaId,
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
        await SendRequestAsync(HttpMethod.Post, $"/webhooks/{webhookId}/{token}", req, true, false, options: options);
    }

    /// <summary>
    /// Delete a webhook message using the webhook token.
    /// </summary>
    public async Task DeleteWebhookMessageAsync(ulong webhookId, string token, ulong messageId, RequestOptions? options = null)
    {
        await SendRequestAsync(HttpMethod.Delete, $"/webhooks/{webhookId}/{token}/messages/{messageId}", true, options: options);
    }

    /// <summary>
    /// Modify a webhook message using the webhook token.
    /// </summary>
    /// <returns><see cref="Message"/></returns>
    public async Task<Message> EditWebhookMessageAsync(ulong webhookId, string token, ulong messageId, string? content = null, List<EmbedRequest>? embeds = null,
        MessageReferenceRequest? reference = null, AllowedMentionsRequest? allowedMentions = null, MessageFlag flags = MessageFlag.None,
        string? nonce = null, ulong? favoriteMediaId = null, List<ulong>? stickerIds = null, List<AttachmentRequest>? attachments = null, RequestOptions? options = null)
    {
        MessageRequest req = new MessageRequest
        {
            Content = content,
            Embeds = embeds?.ToArray(),
            MessageReference = reference,
            AllowedMentions = allowedMentions,
            Flags = flags,
            Nonce = nonce,
            FavoriteMediaId = favoriteMediaId,
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
        MessageJson json = await SendRequestAsync<MessageJson, MessageRequest>(HttpMethod.Patch, $"/webhooks/{webhookId}/{token}/messages/{messageId}", req, true, false, options: options);
        return Message.Create(_client, json);
    }

    /// <summary>
    /// Send message using the webhook token in a guild channel and wait for response message.
    /// </summary>
    /// <returns><see cref="Message"/></returns>
    public async Task<Message> ExecuteWebhookWaitAsync(ulong webhookId, string token, string? content = null, List<EmbedRequest>? embeds = null,
        string? username = null, string? avatarUrl = null,
        MessageReferenceRequest? reference = null, AllowedMentionsRequest? allowedMentions = null, MessageFlag flags = MessageFlag.None,
        string? nonce = null, ulong? favoriteMediaId = null, bool? tts = null, List<ulong>? stickerIds = null, List<AttachmentRequest>? attachments = null, RequestOptions? options = null)
    {
        MessageRequest req = new MessageRequest
        {
            Content = content,
            Embeds = embeds?.ToArray(),
            MessageReference = reference,
            AllowedMentions = allowedMentions,
            Flags = flags,
            Nonce = nonce,
            FavoriteMediaId = favoriteMediaId,
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
        MessageJson json = await SendRequestAsync<MessageJson, MessageRequest>(HttpMethod.Post, $"/webhooks/{webhookId}/{token}?wait=true", req, true, options: options);
        return Message.Create(_client, json);
    }
    #endregion

    #region OAuth API
    /// <summary>
    /// Get user from access token.
    /// </summary>
    /// <returns><see cref="FluxerOAuthUser"/></returns>
    public async Task<FluxerOAuthUser?> GetOAuthUserAsync(string accessToken, RequestOptions? options = null)
    {
        FluxerOAuthUserJson? json = await InternalSendRequestAsync<FluxerOAuthUserJson>(HttpMethod.Get, "/oauth2/userinfo", false, false, accessToken, options: options);
        if (json == null)
            return null;

        return FluxerOAuthUser.Create(_client, json);
    }

    /// <summary>
    /// Get oauth token from access token.
    /// </summary>
    /// <returns><see cref="FluxerOAuthToken"/></returns>
    public async Task<FluxerOAuthToken?> GetOAuthTokenAsync(string accessToken, RequestOptions? options = null)
    {
        FluxerOAuthTokenJson? json = await InternalSendRequestAsync<FluxerOAuthTokenJson>(HttpMethod.Get, "/oauth2/@me", false, false, accessToken, options: options);
        if (json == null)
            return null;

        return FluxerOAuthToken.Create(_client, json);
    }


    /// <summary>
    /// Get user guilds from access token.
    /// </summary>
    /// <returns>List of <see cref="Guild"/></returns>
    public async Task<IEnumerable<Guild>> GetOAuthGuildsAsync(string accessToken, RequestOptions? options = null)
    {
        IEnumerable<GuildJson> json = await InternalSendRequestAsync<IEnumerable<GuildJson>>(HttpMethod.Get, "/users/@me/guilds", true, false, accessToken, options: options);
        return json.Select(x => Guild.Create(_client, x));
    }

    /// <summary>
    /// Get user connections from access token.
    /// </summary>
    /// <returns>List of <see cref="UserConnection"/></returns>
    public async Task<IEnumerable<UserConnection>> GetOAuthConnectionsAsync(string accessToken, RequestOptions? options = null)
    {
        IEnumerable<UserConnectionJson> json = await InternalSendRequestAsync<IEnumerable<UserConnectionJson>>(HttpMethod.Get, "/users/@me/connections", true, false, accessToken, options: options);
        return json.Select(x => UserConnection.Create(_client, x));
    }

    /// <summary>
    /// Check if oauth token is valid.
    /// </summary>
    /// /// <returns><see cref="FluxerOAuthValidToken"/></returns>
    public async Task<FluxerOAuthValidToken?> GetOAuthValidTokenAsync(ulong clientId, string clientSecret, string accessToken, RequestOptions? options = null)
    {
        FluxerOAuthValidTokenJson? json = await InternalSendRequestFormAsync<FluxerOAuthValidTokenJson>(HttpMethod.Post, "/oauth2/introspect", false, new Dictionary<string, string>
        {
            { "client_id", clientId.ToString() },
            { "client_secret", clientSecret },
            { "token", accessToken }
        }, options: options);

        if (json == null)
            return null;

        return FluxerOAuthValidToken.Create(_client, json);
    }

    /// <summary>
    /// Get new access token from refresh token.
    /// </summary>
    /// <returns><see cref="FluxerOAuthRefreshToken"/></returns>
    public async Task<FluxerOAuthRefreshToken> ExchangeOAuthRefreshTokenAsync(ulong clientId, string clientSecret, string refreshToken, RequestOptions? options = null)
    {
        FluxerOAuthRefreshTokenJson json = await InternalSendRequestFormAsync<FluxerOAuthRefreshTokenJson>(HttpMethod.Post, "/oauth2/token", true, new Dictionary<string, string>
        {
            { "client_id", clientId.ToString() },
            { "client_secret", clientSecret },
            { "grant_type", "refresh_token" },
            { "refresh_token", refreshToken }
        }, options: options);

        return FluxerOAuthRefreshToken.Create(_client, json);
    }

    /// <summary>
    /// Remove access for access token.
    /// </summary>
    public async Task RevokeOAuthAccessTokenAsync(ulong clientId, string clientSecret, string accessToken, RequestOptions? options = null)
    {
        await InternalSendRequestFormAsync<UserJson>(HttpMethod.Post, "/oauth2/token/revoke", true, new Dictionary<string, string>
        {
            { "client_id", clientId.ToString() },
            { "client_secret", clientSecret },
            { "token", accessToken },
            { "token_type_hint", "access_token" }
        }, options: options);
    }

    /// <summary>
    /// Remove access for refresh token.
    /// </summary>
    public async Task RevokeOAuthRefreshTokenAsync(ulong clientId, string clientSecret, string refreshToken, RequestOptions? options = null)
    {
        await InternalSendRequestFormAsync<UserJson>(HttpMethod.Post, "/oauth2/token/revoke", true, new Dictionary<string, string>
        {
            { "client_id", clientId.ToString() },
            { "client_secret", clientSecret },
            { "token", refreshToken },
            { "token_type_hint", "refresh_token" }
        }, options: options);
    }
    #endregion

    #region Global Search

    /// <inheritdoc cref="SearchChannelMessagesAsync(ulong, GlobalSearchMessagesRequest, RequestOptions?)"/>
    public async Task<GlobalSearch> SearchGuildMessagesAsync(ulong guildId, GlobalSearchMessagesRequest request, RequestOptions? options = null)
    {
        request.ContextGuildId = guildId;
        GlobalSearchJson json = await SendRequestAsync<GlobalSearchJson, GlobalSearchMessagesRequest>(HttpMethod.Post, "/search/messages", request, true, options: options);
        return GlobalSearch.Create(_client, json);
    }

    /// <inheritdoc cref="SearchChannelMessagesAsync(ulong, GlobalSearchMessagesRequest, RequestOptions?)"/>
    public async Task<GlobalSearch> SearchGuildChannelMessagesAsync(ulong guildId, ulong channelId, GlobalSearchMessagesRequest request, RequestOptions? options = null)
    {
        request.ContextGuildId = guildId;
        request.ContextChannelId = channelId;
        GlobalSearchJson json = await SendRequestAsync<GlobalSearchJson, GlobalSearchMessagesRequest>(HttpMethod.Post, "/search/messages", request, true, options: options);
        return GlobalSearch.Create(_client, json);
    }

    /// <summary>
    /// Search messages globally using either guild or channel.
    /// </summary>
    /// <remarks>
    /// Page needs to be 1 and message scope should not be changed when using bots.
    /// </remarks>
    /// <returns><see cref="GlobalSearch"/></returns>
    public async Task<GlobalSearch> SearchChannelMessagesAsync(ulong channelId, GlobalSearchMessagesRequest request, RequestOptions? options = null)
    {
        request.ContextChannelId = channelId;
        GlobalSearchJson json = await SendRequestAsync<GlobalSearchJson, GlobalSearchMessagesRequest>(HttpMethod.Post, "/search/messages", request, true, options: options);
        return GlobalSearch.Create(_client, json);
    }

    #endregion
}
