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

    /// <summary>
    /// Get the current user's experiment assignments, such as whether channel threads are enabled.
    /// </summary>
    /// <returns><see cref="ExperimentAssignmentsJson"/></returns>
    public async Task<ExperimentAssignmentsJson> GetExperimentsAsync(RequestOptions? options = null)
        => await SendRequestAsync<ExperimentAssignmentsJson>(HttpMethod.Get, "/experiments", true, options: options);

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

    /// <summary>
    /// Get the guild a custom emoji belongs to.
    /// </summary>
    /// <returns><see cref="ExpressionSourceGuildJson"/></returns>
    public async Task<ExpressionSourceGuildJson> GetEmojiSourceAsync(ulong emojiId, RequestOptions? options = null)
        => await SendRequestAsync<ExpressionSourceGuildJson>(HttpMethod.Get, $"/emojis/{emojiId}/source", true, options: options);

    /// <summary>
    /// Get the guild a custom sticker belongs to.
    /// </summary>
    /// <returns><see cref="ExpressionSourceGuildJson"/></returns>
    public async Task<ExpressionSourceGuildJson> GetStickerSourceAsync(ulong stickerId, RequestOptions? options = null)
        => await SendRequestAsync<ExpressionSourceGuildJson>(HttpMethod.Get, $"/stickers/{stickerId}/source", true, options: options);

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
    [Obsolete("Fluxer removed phone numbers and SMS verification.")]
    public async Task SendMfaSmsCodeAsync()
        => await SendRequestRawAsync(HttpMethod.Post, "/auth/login/mfa/sms/send", true, false);

    /// <summary>
    /// Login with a SMS code.
    /// </summary>
    [Obsolete("Fluxer removed phone numbers and SMS verification.")]
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
    [Obsolete("Every property of ChannelJson is sent, so this also resets the channel's age restriction and flags. Use the UpdateChannelRequest overload.")]
    public async Task<Channel> UpdateChannelAsync(ulong channelId, ChannelJson channel, RequestOptions? options = null)
    {
        ChannelJson json = await SendRequestAsync<ChannelJson, ChannelJson>(HttpMethod.Patch, $"/channels/{channelId}", channel, true, options: options);
        return Channel.Create(_client, json);
    }

    /// <summary>
    /// Update a guild channel. Only the properties set on <paramref name="request"/> are changed.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="ChannelPermissions.ManageChannel"/>, and <see cref="ChannelPermissions.ManagePermissions"/>
    /// to change permission overwrites.
    /// </remarks>
    /// <returns><see cref="Channel"/></returns>
    public async Task<Channel> UpdateChannelAsync(ulong channelId, UpdateChannelRequest request, RequestOptions? options = null)
    {
        ChannelJson json = await SendRequestAsync<ChannelJson, UpdateChannelRequest>(HttpMethod.Patch, $"/channels/{channelId}", request, true, options: options);
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
    /// Get how many channels and guilds follow an announcement channel.
    /// </summary>
    /// <returns><see cref="ChannelFollowerStatsJson"/></returns>
    public async Task<ChannelFollowerStatsJson> GetFollowerStatsAsync(ulong channelId, RequestOptions? options = null)
        => await SendRequestAsync<ChannelFollowerStatsJson>(HttpMethod.Get, $"/channels/{channelId}/follower-stats", true, options: options);

    /// <summary>
    /// Get the guild a published announcement was copied from.
    /// </summary>
    /// <returns><see cref="CrosspostSourceGuildJson"/></returns>
    public async Task<CrosspostSourceGuildJson> GetCrosspostSourceAsync(ulong channelId, ulong messageId, RequestOptions? options = null)
    {
        CrosspostSourceJson json = await SendRequestAsync<CrosspostSourceJson>(HttpMethod.Get, $"/channels/{channelId}/messages/{messageId}/crosspost-source", true, options: options);
        return json.Guild;
    }

    /// <summary>
    /// Get a channel's slowmode and how long the current user must wait before sending.
    /// </summary>
    /// <returns><see cref="SlowmodeStateJson"/></returns>
    public async Task<SlowmodeStateJson> GetSlowmodeStateAsync(ulong channelId, RequestOptions? options = null)
        => await SendRequestAsync<SlowmodeStateJson>(HttpMethod.Get, $"/channels/{channelId}/slowmode", true, options: options);

    /// <summary>
    /// Search messages in a channel.
    /// </summary>
    /// <remarks>
    /// Not implemented or tested properly.
    /// </remarks>
    [Obsolete("Fluxer removed this route. Use SearchChannelMessagesAsync.")]
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
                form.Add(new KeyValuePair<string, (HttpContent content, string? filename)>($"files[{i}]", (new StreamContent(attachments[i].Stream), attachments[i].Filename)));
            }
            req.Attachments = attachments.Select(x => x.ToJson()).ToList();

            MessageJson json2 = await SendRequestAsync<MessageJson, MessageRequest>(HttpMethod.Post,
            _isWebhook ? $"/webhooks/{channelId}/{_token}?wait=true" : $"/channels/{channelId}/messages",
            req, true, otherFormData: form, options: options);

            return Message.Create(_client, json2);
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
    /// Delete every message the current user sent in a channel. Fluxer deletes them in the background.
    /// </summary>
    /// <param name="verification">The password or MFA code user accounts must confirm with.</param>
    public async Task DeleteOwnMessagesAsync(ulong channelId, SudoVerificationRequest? verification = null, RequestOptions? options = null)
        => await SendRequestAsync(HttpMethod.Post, $"/channels/{channelId}/messages/bulk-delete-mine", verification ?? new SudoVerificationRequest(), true, options: options);

    /// <summary>
    /// Delete every message in the current user's Saved Notes channel.
    /// </summary>
    /// <returns>The number of deleted messages</returns>
    public async Task<int> PurgeSavedNotesAsync(ulong channelId, RequestOptions? options = null)
    {
        MessagePurgeJson json = await SendRequestAsync<MessagePurgeJson>(HttpMethod.Post, $"/channels/{channelId}/messages/purge", true, options: options);
        return json.DeletedCount;
    }

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
        ChannelPinsJson json = await SendRequestAsync<ChannelPinsJson>(HttpMethod.Get, $"/channels/{channelId}/messages/pins?{query?.BuildQuery() ?? string.Empty}", true, options: options);
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
    /// Mark the new pins of a channel as seen.
    /// </summary>
    public async Task AcknowledgePinsAsync(ulong channelId, RequestOptions? options = null)
        => await SendRequestRawAsync(HttpMethod.Post, $"/channels/{channelId}/pins/ack", true, options: options);

    /// <summary>
    /// Create or replace the permission overwrite for a role or member on a guild channel.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="ChannelPermissions.ManagePermissions"/> in the channel. Not available on threads.
    /// </remarks>
    public async Task SetPermissionOverwriteAsync(ulong channelId, ulong targetId, PermissionOverwriteType type,
        ChannelPermission allow, ChannelPermission deny, RequestOptions? options = null)
        => await SendRequestAsync(HttpMethod.Put, $"/channels/{channelId}/permissions/{targetId}", new PermissionOverwriteRequest
        {
            Type = type,
            Allow = (ulong)allow,
            Deny = (ulong)deny
        }, true, options: options);

    /// <summary>
    /// Remove the permission overwrite for a role or member from a guild channel.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="ChannelPermissions.ManagePermissions"/> in the channel.
    /// </remarks>
    public async Task DeletePermissionOverwriteAsync(ulong channelId, ulong targetId, RequestOptions? options = null)
        => await SendRequestRawAsync(HttpMethod.Delete, $"/channels/{channelId}/permissions/{targetId}", true, options: options);

    /// <summary>
    /// Get reaction users for a certain emoji on a message.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="ChannelPermissions.ViewChannel"/> and <see cref="ChannelPermissions.ReadMessageHistory"/> in a guild channel.
    /// </remarks>
    /// <returns>List of <see cref="User"/></returns>
    public async Task<IEnumerable<User>> GetReactionsForEmojiAsync(ulong channelId, ulong messageId, string emoji, RequestOptions? options = null)
    {
        if (ulong.TryParse(emoji, out _))
            emoji = $"name:{emoji}";

        bool encode = !emoji.Contains(":");

        IEnumerable<UserJson> json = await SendRequestAsync<IEnumerable<UserJson>>(HttpMethod.Get, $"/channels/{channelId}/messages/{messageId}/reactions/{(encode ? System.Web.HttpUtility.UrlEncode(emoji) : emoji)}", true, options: options);
        return json.Select(x => User.Create(_client, x));
    }

    /// <summary>
    /// Get a page of the users who reacted to a message with an emoji.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="ChannelPermissions.ViewChannel"/> and <see cref="ChannelPermissions.ReadMessageHistory"/> in a guild channel.
    /// </remarks>
    /// <param name="afterId">Only users after this one, such as <see cref="ReactionUserPage.NextAfterId"/> of the previous page.</param>
    /// <returns><see cref="ReactionUserPage"/></returns>
    public async Task<ReactionUserPage> GetReactionUsersAsync(ulong channelId, ulong messageId, string emoji, int? limit = null, ulong? afterId = null, RequestOptions? options = null)
    {
        RestClientQueryParams queryParams = new RestClientQueryParams()
            .AddIf(limit != null, QueryParams.Limit, limit)
            .AddIf(afterId != null, QueryParams.After, afterId);

        ReactionUserPageJson json = await SendRequestQueryParamsAsync<ReactionUserPageJson>(HttpMethod.Get, queryParams, $"/channels/{channelId}/messages/{messageId}/reactions/{emoji}/users", true, options: options);
        return ReactionUserPage.Create(_client, json);
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

        bool encode = !emoji.Contains(":");

        await SendRequestRawAsync(HttpMethod.Put, $"/channels/{channelId}/messages/{messageId}/reactions/{(encode ? System.Web.HttpUtility.UrlEncode(emoji) : emoji)}/@me", true, options: options);
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

        bool encode = !emoji.Contains(":");

        await SendRequestRawAsync(HttpMethod.Delete, $"/channels/{channelId}/messages/{messageId}/reactions/{(encode ? System.Web.HttpUtility.UrlEncode(emoji) : emoji)}/@me", true, options: options);
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

        bool encode = !emoji.Contains(":");

        await SendRequestRawAsync(HttpMethod.Delete, $"/channels/{channelId}/messages/{messageId}/reactions/{(encode ? System.Web.HttpUtility.UrlEncode(emoji) : emoji)}/{targetId}", true, options: options);
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

        bool encode = !emoji.Contains(":");

        await SendRequestRawAsync(HttpMethod.Delete, $"/channels/{channelId}/messages/{messageId}/reactions/{(encode ? System.Web.HttpUtility.UrlEncode(emoji) : emoji)}", true, options: options);
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
        ChannelJson json = await SendRequestAsync<ChannelJson, UpdateThreadRequest>(HttpMethod.Patch, $"/channels/{channelId}", req, true, options: options);
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

    /// <summary>
    /// Get the members of a thread.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="ChannelPermissions.ViewChannel"/>.
    /// </remarks>
    /// <param name="withMember">Whether to include each member's guild member.</param>
    /// <returns>List of <see cref="ThreadMember"/></returns>
    public async Task<IEnumerable<ThreadMember>> GetThreadMembersAsync(ulong channelId, bool withMember = false, int? limit = null, ulong? afterId = null, RequestOptions? options = null)
    {
        RestClientQueryParams queryParams = new RestClientQueryParams()
            .AddIf(withMember, "with_member", "true")
            .AddIf(limit != null, QueryParams.Limit, limit)
            .AddIf(afterId != null, QueryParams.After, afterId);

        IEnumerable<ThreadMemberJson> json = await SendRequestQueryParamsAsync<IEnumerable<ThreadMemberJson>>(HttpMethod.Get, queryParams, $"/channels/{channelId}/thread-members", true, options: options);
        return json.Select(x => ThreadMember.Create(_client, x));
    }

    /// <summary>
    /// Get a member of a thread.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="ChannelPermissions.ViewChannel"/>.
    /// </remarks>
    /// <param name="withMember">Whether to include the guild member.</param>
    /// <returns><see cref="ThreadMember"/></returns>
    public async Task<ThreadMember> GetThreadMemberAsync(ulong channelId, ulong userId, bool withMember = false, RequestOptions? options = null)
    {
        RestClientQueryParams queryParams = new RestClientQueryParams()
            .AddIf(withMember, "with_member", "true");

        ThreadMemberJson json = await SendRequestQueryParamsAsync<ThreadMemberJson>(HttpMethod.Get, queryParams, $"/channels/{channelId}/thread-members/{userId}", true, options: options);
        return ThreadMember.Create(_client, json);
    }

    /// <summary>
    /// Get the active threads of a guild.
    /// </summary>
    /// <returns><see cref="ThreadList"/></returns>
    public async Task<ThreadList> GetActiveThreadsAsync(ulong guildId, RequestOptions? options = null)
    {
        ThreadListJson json = await SendRequestAsync<ThreadListJson>(HttpMethod.Get, $"/guilds/{guildId}/threads/active", true, options: options);
        return ThreadList.Create(_client, json);
    }

    /// <summary>
    /// Get the archived public threads of a channel, newest first.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="ChannelPermissions.ReadMessageHistory"/>.
    /// </remarks>
    /// <param name="before">Only threads archived before this time.</param>
    /// <returns><see cref="ThreadList"/></returns>
    public async Task<ThreadList> GetPublicArchivedThreadsAsync(ulong channelId, int? limit = null, DateTimeOffset? before = null, RequestOptions? options = null)
        => await GetArchivedThreadsAsync($"/channels/{channelId}/threads/archived/public", limit, before?.ToString("O"), options);

    /// <summary>
    /// Get the archived private threads of a channel, newest first.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="ChannelPermissions.ReadMessageHistory"/> and <see cref="ChannelPermissions.ManageThreads"/>.
    /// </remarks>
    /// <param name="before">Only threads archived before this time.</param>
    /// <returns><see cref="ThreadList"/></returns>
    public async Task<ThreadList> GetPrivateArchivedThreadsAsync(ulong channelId, int? limit = null, DateTimeOffset? before = null, RequestOptions? options = null)
        => await GetArchivedThreadsAsync($"/channels/{channelId}/threads/archived/private", limit, before?.ToString("O"), options);

    /// <summary>
    /// Get the archived private threads of a channel that the current user joined.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="ChannelPermissions.ReadMessageHistory"/>.
    /// </remarks>
    /// <param name="beforeId">Only threads created before this thread.</param>
    /// <returns><see cref="ThreadList"/></returns>
    public async Task<ThreadList> GetJoinedPrivateArchivedThreadsAsync(ulong channelId, int? limit = null, ulong? beforeId = null, RequestOptions? options = null)
        => await GetArchivedThreadsAsync($"/channels/{channelId}/users/@me/threads/archived/private", limit, beforeId?.ToString(), options);

    private async Task<ThreadList> GetArchivedThreadsAsync(string route, int? limit, string? before, RequestOptions? options)
    {
        RestClientQueryParams queryParams = new RestClientQueryParams()
            .AddIf(limit != null, QueryParams.Limit, limit)
            .AddIf(before != null, QueryParams.Before, before);

        ThreadListJson json = await SendRequestQueryParamsAsync<ThreadListJson>(HttpMethod.Get, queryParams, route, true, options: options);
        return ThreadList.Create(_client, json);
    }

    /// <summary>
    /// Search the threads of a channel by name, tags and archive state.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="ChannelPermissions.ReadMessageHistory"/>. Returns no threads while Fluxer is still indexing the channel.
    /// </remarks>
    /// <returns><see cref="ThreadList"/></returns>
    public async Task<ThreadList> SearchThreadsAsync(ulong channelId, ThreadSearchQuery query, RequestOptions? options = null)
    {
        ThreadListJson json = await SendRequestAsync<ThreadListJson>(HttpMethod.Get, $"/channels/{channelId}/threads/search?{query.BuildQuery()}", true, options: options);
        return ThreadList.Create(_client, json);
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
        ChannelJson json = await SendRequestAsync<ChannelJson, CreateForumTagRequest>(HttpMethod.Put, $"/channels/{channelId}/tags/{tagId}", req, true, options: options);
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

    /// <summary>
    /// Get the author and first message of up to 100 posts in a forum or media channel.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="ChannelPermissions.ViewChannel"/>.
    /// </remarks>
    /// <returns>Each post's <see cref="ForumPostData"/>, by post id</returns>
    public async Task<IReadOnlyDictionary<ulong, ForumPostData>> GetPostDataAsync(ulong channelId, IEnumerable<ulong> postIds, RequestOptions? options = null)
    {
        ForumPostDataListJson json = await SendRequestAsync<ForumPostDataListJson, ForumPostDataRequest>(HttpMethod.Post, $"/channels/{channelId}/post-data", new ForumPostDataRequest
        {
            ThreadIds = postIds
        }, true, options: options);
        return json.Threads.ToDictionary(x => x.Key, x => ForumPostData.Create(_client, x.Value));
    }

    #endregion

    #region Attachments API
    /// <summary>
    /// Get fresh signed URLs for up to 50 attachment URLs whose signature expired.
    /// </summary>
    /// <returns>One <see cref="RefreshedAttachmentUrlJson"/> per URL, in the order given</returns>
    public async Task<IReadOnlyList<RefreshedAttachmentUrlJson>> RefreshAttachmentUrlsAsync(IEnumerable<string> urls, RequestOptions? options = null)
    {
        RefreshedAttachmentUrlListJson json = await SendRequestAsync<RefreshedAttachmentUrlListJson, RefreshAttachmentUrlsRequest>(HttpMethod.Post, "/attachments/refresh-urls", new RefreshAttachmentUrlsRequest
        {
            Urls = urls
        }, true, options: options);
        return json.Urls;
    }

    /// <summary>
    /// Finish multipart uploads of large attachments, so they can be sent in a message.
    /// </summary>
    /// <returns>The upload filename of each finished upload</returns>
    public async Task<IReadOnlyList<string>> CompleteAttachmentUploadsAsync(ulong channelId, IEnumerable<CompleteAttachmentUploadItem> uploads, RequestOptions? options = null)
    {
        CompletedAttachmentUploadListJson json = await SendRequestAsync<CompletedAttachmentUploadListJson, CompleteAttachmentUploadsRequest>(HttpMethod.Post, $"/channels/{channelId}/attachments/complete", new CompleteAttachmentUploadsRequest
        {
            Uploads = uploads
        }, true, options: options);
        return json.Uploads.Select(x => x.UploadFilename).ToArray();
    }

    /// <summary>
    /// Delete an attachment.
    /// </summary>
    [Obsolete("Fluxer removed this route.")]
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
    [Obsolete("Every property of GuildJson is sent, so this also resets settings that were not meant to change. Use the UpdateGuildRequest overload.")]
    public async Task<Guild> UpdateGuildAsync(ulong guildId, GuildJson guild, RequestOptions? options = null)
    {
        GuildJson json = await SendRequestAsync<GuildJson, GuildJson>(HttpMethod.Patch, $"/guilds/{guildId}", guild, true, options: options);
        return Guild.Create(_client, json);
    }

    /// <summary>
    /// Update a guild that you manage. Only the properties set on <paramref name="request"/> are changed.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="GuildPermissions.ManageGuild"/>.
    /// </remarks>
    /// <returns><see cref="Guild"/></returns>
    public async Task<Guild> UpdateGuildAsync(ulong guildId, UpdateGuildRequest request, RequestOptions? options = null)
    {
        GuildJson json = await SendRequestAsync<GuildJson, UpdateGuildRequest>(HttpMethod.Patch, $"/guilds/{guildId}", request, true, options: options);
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
    /// Search the members of a guild by name, roles, join time and more.
    /// </summary>
    /// <remarks>
    /// Results can be incomplete while <see cref="GuildMemberSearchJson.IsIndexing"/> is true.
    /// </remarks>
    /// <returns><see cref="GuildMemberSearchJson"/></returns>
    public async Task<GuildMemberSearchJson> SearchMembersAsync(ulong guildId, GuildMemberSearchRequest request, RequestOptions? options = null)
        => await SendRequestAsync<GuildMemberSearchJson, GuildMemberSearchRequest>(HttpMethod.Post, $"/guilds/{guildId}/members-search", request, true, options: options);

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
    [Obsolete("Every property of GuildMemberJson is sent, including its empty role list, so this replaces the member's roles and voice state. Use the UpdateGuildMemberRequest overload.")]
    public async Task<GuildMember> UpdateMemberAsync(ulong guildId, ulong userId, GuildMemberJson member, RequestOptions? options = null)
    {
        GuildMemberJson json = await SendRequestAsync<GuildMemberJson, GuildMemberJson>(HttpMethod.Patch, $"/guilds/{guildId}/members/{userId}", member, true, options: options);
        return GuildMember.Create(_client, json);
    }

    /// <summary>
    /// Update a member of the guild. Only the properties set on <paramref name="request"/> are changed.
    /// </summary>
    /// <remarks>
    /// Changing the nickname requires <see cref="GuildPermissions.ManageNicknames"/>, roles require
    /// <see cref="GuildPermissions.ManageRoles"/>, a timeout requires <see cref="GuildPermissions.ModerateMembers"/>
    /// and muting, deafening or moving requires the matching voice permission.
    /// </remarks>
    public async Task<GuildMember> UpdateMemberAsync(ulong guildId, ulong userId, UpdateGuildMemberRequest request, RequestOptions? options = null)
    {
        GuildMemberJson json = await SendRequestAsync<GuildMemberJson, UpdateGuildMemberRequest>(HttpMethod.Patch, $"/guilds/{guildId}/members/{userId}", request, true, options: options);
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
    /// Get all roles in the guild.
    /// </summary>
    /// <returns>List of <see cref="Role"/></returns>
    public async Task<IEnumerable<Role>> GetRolesAsync(ulong guildId, RequestOptions? options = null)
    {
        IEnumerable<RoleJson> json = await SendRequestAsync<IEnumerable<RoleJson>>(HttpMethod.Get, $"/guilds/{guildId}/roles", true, options: options);
        return json.Select(x => Role.Create(_client, x, guildId));
    }

    /// <summary>
    /// Set the order hoisted roles are shown in the member list, separate from their permission order.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="GuildPermissions.ManageRoles"/>.
    /// </remarks>
    public async Task UpdateRoleHoistPositionsAsync(ulong guildId, IEnumerable<RoleHoistPositionItemJson> positions, RequestOptions? options = null)
        => await SendRequestAsync(HttpMethod.Patch, $"/guilds/{guildId}/roles/hoist-positions", positions, true, options: options);

    /// <summary>
    /// Show hoisted roles in the member list in their permission order again.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="GuildPermissions.ManageRoles"/>.
    /// </remarks>
    public async Task ResetRoleHoistPositionsAsync(ulong guildId, RequestOptions? options = null)
        => await SendRequestRawAsync(HttpMethod.Delete, $"/guilds/{guildId}/roles/hoist-positions", true, options: options);

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
    [Obsolete("Fluxer removed this route. Use SearchGuildMessagesAsync.")]
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
    {
        RestClientQueryParams queryParams = new RestClientQueryParams()
            .AddIf(data.Limit != null, QueryParams.Limit, data.Limit)
            .AddIf(data.Before != null, QueryParams.Before, data.Before)
            .AddIf(data.After != null, QueryParams.After, data.After)
            .AddIf(data.UserId != null, QueryParams.UserId, data.UserId)
            .AddIf(data.ActionType != null, QueryParams.ActionType, (int?)data.ActionType);

        return await SendRequestQueryParamsAsync<AuditLogListJson>(HttpMethod.Get, queryParams, $"/guilds/{guildId}/audit-logs", true, options: options);
    }

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
    /// Copy an emoji from another guild into the guild, keeping its name and image.
    /// </summary>
    /// <remarks>
    /// Requires either <see cref="GuildPermissions.CreateExpressions"/> or <see cref="GuildPermissions.ManageExpressions"/>.
    /// </remarks>
    /// <returns><see cref="GuildEmoji"/></returns>
    public async Task<GuildEmoji> CloneEmojiAsync(ulong guildId, ulong sourceEmojiId, RequestOptions? options = null)
    {
        GuildEmojiJson json = await SendRequestAsync<GuildEmojiJson, CloneGuildExpressionRequest>(HttpMethod.Post, $"/guilds/{guildId}/emojis/clone", new CloneGuildExpressionRequest
        {
            SourceEmojiId = sourceEmojiId
        }, true, options: options);
        return GuildEmoji.Create(_client, json, guildId);
    }

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
    /// Copy a sticker from another guild into the guild, keeping its name, description, tags and image.
    /// </summary>
    /// <remarks>
    /// Requires either <see cref="GuildPermissions.CreateExpressions"/> or <see cref="GuildPermissions.ManageExpressions"/>.
    /// </remarks>
    /// <returns><see cref="GuildSticker"/></returns>
    public async Task<GuildSticker> CloneStickerAsync(ulong guildId, ulong sourceStickerId, RequestOptions? options = null)
    {
        GuildStickerJson json = await SendRequestAsync<GuildStickerJson, CloneGuildExpressionRequest>(HttpMethod.Post, $"/guilds/{guildId}/stickers/clone", new CloneGuildExpressionRequest
        {
            SourceStickerId = sourceStickerId
        }, true, options: options);
        return GuildSticker.Create(_client, json, guildId);
    }

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

    /// <summary>
    /// Get the categories guilds can be listed under in discovery.
    /// </summary>
    /// <returns>List of <see cref="DiscoveryCategoryJson"/></returns>
    public async Task<IReadOnlyList<DiscoveryCategoryJson>> GetDiscoveryCategoriesAsync(RequestOptions? options = null)
        => await SendRequestAsync<DiscoveryCategoryJson[]>(HttpMethod.Get, "/discovery/categories", true, options: options);

    /// <summary>
    /// Search the guilds listed in discovery.
    /// </summary>
    /// <param name="sortBy">"member_count", "online_count" or "relevance".</param>
    /// <returns><see cref="DiscoveryGuildListJson"/></returns>
    public async Task<DiscoveryGuildListJson> SearchDiscoveryGuildsAsync(string? query = null, int? category = null, string? language = null, string? tag = null,
        string? sortBy = null, int? limit = null, int? offset = null, RequestOptions? options = null)
    {
        RestClientQueryParams queryParams = new RestClientQueryParams()
            .AddIf(query != null, "query", query)
            .AddIf(category != null, "category", category)
            .AddIf(language != null, "language", language)
            .AddIf(tag != null, "tag", tag)
            .AddIf(sortBy != null, "sort_by", sortBy)
            .AddIf(limit != null, QueryParams.Limit, limit)
            .AddIf(offset != null, "offset", offset);

        return await SendRequestQueryParamsAsync<DiscoveryGuildListJson>(HttpMethod.Get, queryParams, "/discovery/guilds", true, options: options);
    }

    /// <summary>
    /// Get a guild's discovery application and whether it can apply.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="GuildPermissions.ManageGuild"/>.
    /// </remarks>
    /// <returns><see cref="DiscoveryInfoJson"/></returns>
    public async Task<DiscoveryInfoJson> GetDiscoveryStatusAsync(ulong guildId, RequestOptions? options = null)
        => await SendRequestAsync<DiscoveryInfoJson>(HttpMethod.Get, $"/guilds/{guildId}/discovery", true, options: options);

    /// <summary>
    /// Apply for a guild to be listed in discovery.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="GuildPermissions.ManageGuild"/>.
    /// </remarks>
    /// <returns><see cref="DiscoveryApplicationJson"/></returns>
    public async Task<DiscoveryApplicationJson> ApplyForDiscoveryAsync(ulong guildId, DiscoveryApplicationRequest request, RequestOptions? options = null)
        => await SendRequestAsync<DiscoveryApplicationJson, DiscoveryApplicationRequest>(HttpMethod.Post, $"/guilds/{guildId}/discovery", request, true, options: options);

    /// <summary>
    /// Change a guild's discovery listing. Only the fields that are set change.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="GuildPermissions.ManageGuild"/>.
    /// </remarks>
    /// <returns><see cref="DiscoveryApplicationJson"/></returns>
    public async Task<DiscoveryApplicationJson> UpdateDiscoveryApplicationAsync(ulong guildId, DiscoveryApplicationRequest request, RequestOptions? options = null)
        => await SendRequestAsync<DiscoveryApplicationJson, DiscoveryApplicationRequest>(HttpMethod.Patch, $"/guilds/{guildId}/discovery", request, true, options: options);

    /// <summary>
    /// Withdraw a guild's discovery application, or remove it from discovery.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="GuildPermissions.ManageGuild"/>.
    /// </remarks>
    public async Task WithdrawDiscoveryApplicationAsync(ulong guildId, RequestOptions? options = null)
        => await SendRequestRawAsync(HttpMethod.Delete, $"/guilds/{guildId}/discovery", true, options: options);

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
    /// Get the latest message in each of up to 100 channels.
    /// </summary>
    /// <returns>The latest <see cref="Message"/> by channel id, or null for a channel without messages</returns>
    public async Task<IReadOnlyDictionary<ulong, Message?>> PreloadMessagesAsync(IEnumerable<ulong> channelIds, RequestOptions? options = null)
    {
        Dictionary<ulong, MessageJson?> json = await SendRequestAsync<Dictionary<ulong, MessageJson?>, PreloadMessagesRequest>(HttpMethod.Post, "/users/@me/channels/messages/preload", new PreloadMessagesRequest
        {
            ChannelIds = channelIds
        }, true, options: options);
        return json.ToDictionary(x => x.Key, x => x.Value == null ? null : Message.Create(_client, x.Value));
    }

    /// <summary>
    /// Delete every message the current user sent in a guild. Fluxer deletes them in the background.
    /// </summary>
    /// <param name="verification">The password or MFA code user accounts must confirm with.</param>
    public async Task DeleteOwnGuildMessagesAsync(ulong guildId, SudoVerificationRequest? verification = null, RequestOptions? options = null)
        => await SendRequestAsync(HttpMethod.Post, $"/users/@me/guilds/{guildId}/messages/bulk-delete-mine", verification ?? new SudoVerificationRequest(), true, options: options);

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
    [Obsolete("Every property of UserJson is sent, so this also resets profile fields that were not meant to change. Use the UpdateCurrentUserRequest overload.")]
    public async Task<CurrentUser> UpdateCurrentUserAsync(UserJson user, RequestOptions? options = null)
    {
        CurrentUserJson json = await SendRequestAsync<CurrentUserJson, UserJson>(HttpMethod.Patch, "/users/@me", user, true, options: options);
        return CurrentUser.Create(_client, json);
    }

    /// <summary>
    /// Update the current user. Only the properties set on <paramref name="request"/> are changed.
    /// </summary>
    /// <returns><see cref="CurrentUser"/></returns>
    public async Task<CurrentUser> UpdateCurrentUserAsync(UpdateCurrentUserRequest request, RequestOptions? options = null)
    {
        CurrentUserJson json = await SendRequestAsync<CurrentUserJson, UpdateCurrentUserRequest>(HttpMethod.Patch, "/users/@me", request, true, options: options);
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
    /// Requires mutual friend or guild, this may also fail if hidden profile is enabled.
    /// </remarks>
    /// <returns><see cref="UserProfileResponse"/></returns>
    public async Task<UserProfileResponse?> GetUserProfileAsync(ulong targetId, ulong? guildId = null, bool mutualFriends = false, bool mutualGuilds = false, RequestOptions? options = null)
    {
        UserProfileResponse? data = await SendRequestAsync<UserProfileResponse>(HttpMethod.Get,
            new QueryBuilder($"/users/{targetId}/profile").WithIf("guild_id", guildId.GetValueOrDefault(), guildId.HasValue).With("with_mutual_friends", mutualFriends).With("with_mutual_guilds", mutualGuilds).Build(), false, options: options);

        return data;
    }

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
    [Obsolete("Fluxer removed phone numbers and SMS verification.")]
    public async Task PostCurrentUserPhoneSendVerificationAsync<TRequest>(TRequest data, RequestOptions? options = null)
        => await SendRequestAsync<TRequest>(HttpMethod.Post, "/users/@me/phone/send-verification", data, true, options: options);

    /// <summary>
    /// Verify phone verification for current user.
    /// </summary>
    /// <remarks>
    /// User account only.
    /// </remarks>
    [Obsolete("Fluxer removed phone numbers and SMS verification.")]
    public async Task<TResponse> PostCurrentUserPhoneVerifyAsync<TRequest, TResponse>(TRequest data, RequestOptions? options = null)
        => await SendRequestAsync<TResponse, TRequest>(HttpMethod.Post, "/users/@me/phone/verify", data, true, options: options);

    /// <summary>
    /// Set phone number for current user.
    /// </summary>
    /// <remarks>
    /// User account only.
    /// </remarks>
    [Obsolete("Fluxer removed phone numbers and SMS verification.")]
    public async Task<TResponse> PostCurrentUserPhoneAsync<TRequest, TResponse>(TRequest data, RequestOptions? options = null)
        => await SendRequestAsync<TResponse, TRequest>(HttpMethod.Post, "/users/@me/phone", data, true, options: options);

    /// <summary>
    /// Remove phone number for current user.
    /// </summary>
    /// <remarks>
    /// User account only.
    /// </remarks>
    [Obsolete("Fluxer removed phone numbers and SMS verification.")]
    public async Task DeleteCurrentUserPhoneAsync<TRequest>(TRequest data, RequestOptions? options = null)
        => await SendRequestAsync<TRequest>(HttpMethod.Delete, "/users/@me/phone", data, true, options: options);

    /// <summary>
    /// Enable MFA SMS for current user.
    /// </summary>
    /// <remarks>
    /// User account only.
    /// </remarks>
    [Obsolete("Fluxer removed phone numbers and SMS verification.")]
    public async Task PostCurrentUserMfaSmsEnableAsync<TRequest>(TRequest data, RequestOptions? options = null)
        => await SendRequestAsync<TRequest>(HttpMethod.Post, "/users/@me/mfa/sms/enable", data, true, options: options);

    /// <summary>
    /// Disable MFA SMS for current user.
    /// </summary>
    /// <remarks>
    /// User account only.
    /// </remarks>
    [Obsolete("Fluxer removed phone numbers and SMS verification.")]
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
    /// <param name="threadId">Thread in the webhook channel to send into.</param>
    /// <param name="threadName">Name of the post to create when the webhook belongs to a forum or media channel.</param>
    /// <param name="appliedTags">Tags to apply to the post created by <paramref name="threadName"/>.</param>
    public async Task ExecuteWebhookAsync(ulong webhookId, string token, string? content = null, List<EmbedRequest>? embeds = null,
        string? username = null, string? avatarUrl = null,
        MessageReferenceRequest? reference = null, AllowedMentionsRequest? allowedMentions = null, MessageFlag flags = MessageFlag.None,
        string? nonce = null, ulong? favoriteMediaId = null, bool? tts = null, List<ulong>? stickerIds = null, List<AttachmentRequest>? attachments = null, RequestOptions? options = null,
        ulong? threadId = null, string? threadName = null, List<ulong>? appliedTags = null)
    {
        MessageRequest req = BuildWebhookExecuteRequest(content, embeds, username, avatarUrl, reference, allowedMentions, flags, nonce, favoriteMediaId, tts, stickerIds, attachments, threadName, appliedTags);
        await SendRequestAsync(HttpMethod.Post, WebhookRoute(webhookId, token, null, false, threadId), req, true, false, options: options);
    }

    /// <summary>
    /// Get a message previously sent by the webhook.
    /// </summary>
    /// <param name="threadId">Thread in the webhook channel the message was sent in.</param>
    /// <returns><see cref="Message"/></returns>
    public async Task<Message> GetWebhookMessageAsync(ulong webhookId, string token, ulong messageId, ulong? threadId = null, RequestOptions? options = null)
    {
        MessageJson json = await SendRequestAsync<MessageJson>(HttpMethod.Get, WebhookRoute(webhookId, token, messageId, false, threadId), true, false, options: options);
        return Message.Create(_client, json);
    }

    /// <summary>
    /// Delete a webhook message using the webhook token.
    /// </summary>
    /// <param name="threadId">Thread in the webhook channel the message was sent in.</param>
    public async Task DeleteWebhookMessageAsync(ulong webhookId, string token, ulong messageId, RequestOptions? options = null, ulong? threadId = null)
    {
        await SendRequestRawAsync(HttpMethod.Delete, WebhookRoute(webhookId, token, messageId, false, threadId), true, false, options: options);
    }

    /// <summary>
    /// Modify a webhook message using the webhook token.
    /// </summary>
    /// <param name="threadId">Thread in the webhook channel the message was sent in.</param>
    /// <returns><see cref="Message"/></returns>
    public async Task<Message> EditWebhookMessageAsync(ulong webhookId, string token, ulong messageId, string? content = null, List<EmbedRequest>? embeds = null,
        MessageReferenceRequest? reference = null, AllowedMentionsRequest? allowedMentions = null, MessageFlag flags = MessageFlag.None,
        string? nonce = null, ulong? favoriteMediaId = null, List<ulong>? stickerIds = null, List<AttachmentRequest>? attachments = null, RequestOptions? options = null,
        ulong? threadId = null)
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
        MessageJson json = await SendRequestAsync<MessageJson, MessageRequest>(HttpMethod.Patch, WebhookRoute(webhookId, token, messageId, false, threadId), req, true, false, options: options);
        return Message.Create(_client, json);
    }

    /// <summary>
    /// Send message using the webhook token in a guild channel and wait for response message.
    /// </summary>
    /// <param name="threadId">Thread in the webhook channel to send into.</param>
    /// <param name="threadName">Name of the post to create when the webhook belongs to a forum or media channel.</param>
    /// <param name="appliedTags">Tags to apply to the post created by <paramref name="threadName"/>.</param>
    /// <returns><see cref="Message"/></returns>
    public async Task<Message> ExecuteWebhookWaitAsync(ulong webhookId, string token, string? content = null, List<EmbedRequest>? embeds = null,
        string? username = null, string? avatarUrl = null,
        MessageReferenceRequest? reference = null, AllowedMentionsRequest? allowedMentions = null, MessageFlag flags = MessageFlag.None,
        string? nonce = null, ulong? favoriteMediaId = null, bool? tts = null, List<ulong>? stickerIds = null, List<AttachmentRequest>? attachments = null, RequestOptions? options = null,
        ulong? threadId = null, string? threadName = null, List<ulong>? appliedTags = null)
    {
        MessageRequest req = BuildWebhookExecuteRequest(content, embeds, username, avatarUrl, reference, allowedMentions, flags, nonce, favoriteMediaId, tts, stickerIds, attachments, threadName, appliedTags);
        MessageJson json = await SendRequestAsync<MessageJson, MessageRequest>(HttpMethod.Post, WebhookRoute(webhookId, token, null, true, threadId), req, true, options: options);
        return Message.Create(_client, json);
    }

    internal static string WebhookRoute(ulong webhookId, string token, ulong? messageId, bool wait, ulong? threadId)
    {
        string route = $"/webhooks/{webhookId}/{token}";
        if (messageId != null)
            route += $"/messages/{messageId}";

        List<string> query = new List<string>();
        if (wait)
            query.Add("wait=true");
        if (threadId != null)
            query.Add($"thread_id={threadId}");

        return query.Count == 0 ? route : $"{route}?{string.Join("&", query)}";
    }

    private static MessageRequest BuildWebhookExecuteRequest(string? content, List<EmbedRequest>? embeds, string? username, string? avatarUrl,
        MessageReferenceRequest? reference, AllowedMentionsRequest? allowedMentions, MessageFlag flags, string? nonce, ulong? favoriteMediaId,
        bool? tts, List<ulong>? stickerIds, List<AttachmentRequest>? attachments, string? threadName, List<ulong>? appliedTags)
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
            ThreadName = threadName,
            AppliedTags = appliedTags,
        };
        if ((attachments?.Count ?? 0) > 0)
        {
            for (int i = 0; i < attachments.Count; i++)
                attachments[i].Id = (ulong)i;
            req.Attachments = attachments.Select(x => x.ToJson()).ToList();
        }
        return req;
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
