using System.Collections.Generic;

namespace Fluxer.Net.RateLimiting;

/// <summary>
/// Maps API routes to their corresponding rate limit configurations.
/// This provides a centralized reference for rate limit buckets across the API.
///
/// Usage example in ApiClient methods:
/// <code>
/// public async Task&lt;Channel&gt; GetChannel(ulong channelId)
/// {
///     await WaitForRateLimit(RateLimitConfigs.CHANNEL_GET, channelId: channelId);
///     return await MakeFluxerApiRequestR&lt;Channel&gt;(HttpMethod.Get, $"/channels/{channelId}", true);
/// }
/// </code>
/// </summary>
public static class RateLimitMappings
{
    /// <summary>
    /// Dictionary mapping route patterns to their rate limit configurations.
    /// Key format: "METHOD /route/pattern"
    /// </summary>
    public static readonly Dictionary<string, RateLimitConfig> RouteToConfig = new()
    {
        // Auth API
        ["POST /auth/register"] = RateLimitConfigs.AUTH_REGISTER,
        ["POST /auth/login"] = RateLimitConfigs.AUTH_LOGIN,
        ["POST /auth/login/mfa/totp"] = RateLimitConfigs.AUTH_LOGIN_MFA,
        ["POST /auth/login/mfa/sms/send"] = RateLimitConfigs.AUTH_LOGIN_MFA,
        ["POST /auth/login/mfa/sms"] = RateLimitConfigs.AUTH_LOGIN_MFA,
        ["POST /auth/logout"] = RateLimitConfigs.AUTH_LOGOUT,
        ["POST /auth/verify"] = RateLimitConfigs.AUTH_VERIFY_EMAIL,
        ["POST /auth/verify/resend"] = RateLimitConfigs.AUTH_RESEND_VERIFICATION,
        ["POST /auth/forgot"] = RateLimitConfigs.AUTH_FORGOT_PASSWORD,
        ["POST /auth/reset"] = RateLimitConfigs.AUTH_RESET_PASSWORD,
        ["GET /auth/sessions"] = RateLimitConfigs.AUTH_SESSIONS_GET,
        ["POST /auth/sessions/logout"] = RateLimitConfigs.AUTH_SESSIONS_LOGOUT,
        ["POST /auth/authorize-ip"] = RateLimitConfigs.AUTH_AUTHORIZE_IP,
        ["POST /auth/webauthn/authentication-options"] = RateLimitConfigs.AUTH_WEBAUTHN_OPTIONS,
        ["POST /auth/webauthn/authenticate"] = RateLimitConfigs.AUTH_WEBAUTHN_AUTHENTICATE,
        ["POST /auth/login/mfa/webauthn/authentication-options"] = RateLimitConfigs.AUTH_WEBAUTHN_OPTIONS,
        ["POST /auth/login/mfa/webauthn"] = RateLimitConfigs.AUTH_LOGIN_MFA,

        // Channel API
        ["GET /channels/:channel_id"] = RateLimitConfigs.CHANNEL_GET,
        ["PATCH /channels/:channel_id"] = RateLimitConfigs.CHANNEL_UPDATE,
        ["DELETE /channels/:channel_id"] = RateLimitConfigs.CHANNEL_DELETE,
        ["DELETE /channels/:channel_id/messages/ack"] = RateLimitConfigs.CHANNEL_READ_STATE_DELETE,
        ["GET /channels/:channel_id/messages"] = RateLimitConfigs.CHANNEL_MESSAGES_GET,
        ["GET /channels/:channel_id/messages/:message_id"] = RateLimitConfigs.CHANNEL_MESSAGE_GET,
        ["POST /channels/:channel_id/search"] = RateLimitConfigs.CHANNEL_SEARCH,
        ["POST /channels/:channel_id/messages"] = RateLimitConfigs.CHANNEL_MESSAGE_CREATE,
        ["PATCH /channels/:channel_id/messages/:message_id"] = RateLimitConfigs.CHANNEL_MESSAGE_UPDATE,
        ["DELETE /channels/:channel_id/messages/:message_id"] = RateLimitConfigs.CHANNEL_MESSAGE_DELETE,
        ["POST /channels/:channel_id/messages/bulk-delete"] = RateLimitConfigs.CHANNEL_MESSAGE_BULK_DELETE,
        ["POST /channels/:channel_id/typing"] = RateLimitConfigs.CHANNEL_TYPING,
        ["POST /channels/:channel_id/messages/:message_id/ack"] = RateLimitConfigs.CHANNEL_MESSAGE_ACK,
        ["GET /channels/:channel_id/pins"] = RateLimitConfigs.CHANNEL_PINS,
        ["PUT /channels/:channel_id/pins/:message_id"] = RateLimitConfigs.CHANNEL_PINS,
        ["DELETE /channels/:channel_id/pins/:message_id"] = RateLimitConfigs.CHANNEL_PINS,
        ["GET /channels/:channel_id/messages/:message_id/reactions/:emoji"] = RateLimitConfigs.CHANNEL_REACTIONS,
        ["PUT /channels/:channel_id/messages/:message_id/reactions/:emoji/@me"] = RateLimitConfigs.CHANNEL_REACTIONS,
        ["DELETE /channels/:channel_id/messages/:message_id/reactions/:emoji/@me"] = RateLimitConfigs.CHANNEL_REACTIONS,
        ["DELETE /channels/:channel_id/messages/:message_id/reactions/:emoji/:target_id"] = RateLimitConfigs.CHANNEL_REACTIONS,
        ["DELETE /channels/:channel_id/messages/:message_id/reactions/:emoji"] = RateLimitConfigs.CHANNEL_REACTIONS,
        ["DELETE /channels/:channel_id/messages/:message_id/reactions"] = RateLimitConfigs.CHANNEL_REACTIONS,
        ["POST /channels/:channel_id/attachments"] = RateLimitConfigs.CHANNEL_ATTACHMENT_UPLOAD,
        ["GET /channels/:channel_id/call"] = RateLimitConfigs.CHANNEL_CALL_GET,
        ["PATCH /channels/:channel_id/call"] = RateLimitConfigs.CHANNEL_CALL_UPDATE,
        ["POST /channels/:channel_id/call/ring"] = RateLimitConfigs.CHANNEL_CALL_RING,
        ["POST /channels/:channel_id/call/stop-ringing"] = RateLimitConfigs.CHANNEL_CALL_STOP_RINGING,
        ["GET /channels/:channel_id/invites"] = RateLimitConfigs.INVITE_LIST_CHANNEL,
        ["POST /channels/:channel_id/invites"] = RateLimitConfigs.INVITE_CREATE,
        ["GET /channels/:channel_id/webhooks"] = RateLimitConfigs.WEBHOOK_LIST_CHANNEL,
        ["POST /channels/:channel_id/webhooks"] = RateLimitConfigs.WEBHOOK_CREATE,

        // Attachment API
        ["DELETE /attachments/:upload_filename"] = RateLimitConfigs.ATTACHMENT_DELETE,

        // Meme API
        ["GET /users/@me/memes"] = RateLimitConfigs.FAVORITE_MEME_LIST,
        ["POST /users/@me/memes"] = RateLimitConfigs.FAVORITE_MEME_CREATE_FROM_URL,
        ["GET /users/@me/memes/:meme_id"] = RateLimitConfigs.FAVORITE_MEME_GET,
        ["PATCH /users/@me/memes/:meme_id"] = RateLimitConfigs.FAVORITE_MEME_UPDATE,
        ["DELETE /users/@me/memes/:meme_id"] = RateLimitConfigs.FAVORITE_MEME_DELETE,
        ["POST /channels/:channel_id/messages/:message_id/memes"] = RateLimitConfigs.FAVORITE_MEME_CREATE_FROM_MESSAGE,

        // Invite API
        ["GET /invites/:invite_code"] = RateLimitConfigs.INVITE_GET,
        ["POST /invites/:invite_code"] = RateLimitConfigs.INVITE_ACCEPT,
        ["DELETE /invites/:invite_code"] = RateLimitConfigs.INVITE_DELETE,

        // Read State API
        ["POST /read-states/ack-bulk"] = RateLimitConfigs.READ_STATE_ACK_BULK,

        // Report API
        ["POST /reports/message"] = RateLimitConfigs.REPORT_CREATE,
        ["POST /reports/user"] = RateLimitConfigs.REPORT_CREATE,
        ["POST /reports/guild"] = RateLimitConfigs.REPORT_CREATE,

        // Guild API
        ["POST /guilds"] = RateLimitConfigs.GUILD_CREATE,
        ["GET /users/@me/guilds"] = RateLimitConfigs.GUILD_LIST,
        ["DELETE /users/@me/guilds/:guild_id"] = RateLimitConfigs.GUILD_LEAVE,
        ["GET /guilds/:guild_id"] = RateLimitConfigs.GUILD_GET,
        ["PATCH /guilds/:guild_id"] = RateLimitConfigs.GUILD_UPDATE,
        ["POST /guilds/:guild_id/delete"] = RateLimitConfigs.GUILD_DELETE,
        ["GET /guilds/:guild_id/vanity-url"] = RateLimitConfigs.GUILD_VANITY_URL_GET,
        ["PATCH /guilds/:guild_id/vanity-url"] = RateLimitConfigs.GUILD_VANITY_URL_PATCH,
        ["GET /guilds/:guild_id/members"] = RateLimitConfigs.GUILD_MEMBERS,
        ["GET /guilds/:guild_id/members/@me"] = RateLimitConfigs.GUILD_MEMBERS,
        ["GET /guilds/:guild_id/members/:user_id"] = RateLimitConfigs.GUILD_MEMBERS,
        ["PATCH /guilds/:guild_id/members/@me"] = RateLimitConfigs.GUILD_MEMBER_UPDATE,
        ["PATCH /guilds/:guild_id/members/:user_id"] = RateLimitConfigs.GUILD_MEMBER_UPDATE,
        ["DELETE /guilds/:guild_id/members/:user_id"] = RateLimitConfigs.GUILD_MEMBER_REMOVE,
        ["PUT /guilds/:guild_id/members/:user_id/roles/:role_id"] = RateLimitConfigs.GUILD_MEMBER_ROLE_ADD,
        ["DELETE /guilds/:guild_id/members/:user_id/roles/:role_id"] = RateLimitConfigs.GUILD_MEMBER_ROLE_REMOVE,
        ["POST /guilds/:guild_id/roles"] = RateLimitConfigs.GUILD_ROLE_CREATE,
        ["PATCH /guilds/:guild_id/roles/:role_id"] = RateLimitConfigs.GUILD_ROLE_UPDATE,
        ["PATCH /guilds/:guild_id/roles"] = RateLimitConfigs.GUILD_ROLE_POSITIONS,
        ["DELETE /guilds/:guild_id/roles/:role_id"] = RateLimitConfigs.GUILD_ROLE_DELETE,
        ["GET /guilds/:guild_id/channels"] = RateLimitConfigs.GUILD_CHANNELS_LIST,
        ["POST /guilds/:guild_id/channels"] = RateLimitConfigs.GUILD_CHANNEL_CREATE,
        ["PATCH /guilds/:guild_id/channels"] = RateLimitConfigs.GUILD_CHANNEL_POSITIONS,
        ["POST /guilds/:guild_id/search"] = RateLimitConfigs.GUILD_SEARCH,
        ["POST /guilds/:guild_id/emojis"] = RateLimitConfigs.GUILD_EMOJI_CREATE,
        ["POST /guilds/:guild_id/emojis/bulk"] = RateLimitConfigs.GUILD_EMOJI_BULK_CREATE,
        ["GET /guilds/:guild_id/emojis"] = RateLimitConfigs.GUILD_EMOJIS_LIST,
        ["PATCH /guilds/:guild_id/emojis/:emoji_id"] = RateLimitConfigs.GUILD_EMOJI_UPDATE,
        ["DELETE /guilds/:guild_id/emojis/:emoji_id"] = RateLimitConfigs.GUILD_EMOJI_DELETE,
        ["POST /guilds/:guild_id/stickers"] = RateLimitConfigs.GUILD_STICKER_CREATE,
        ["POST /guilds/:guild_id/stickers/bulk"] = RateLimitConfigs.GUILD_STICKER_BULK_CREATE,
        ["GET /guilds/:guild_id/stickers"] = RateLimitConfigs.GUILD_STICKERS_LIST,
        ["PATCH /guilds/:guild_id/stickers/:sticker_id"] = RateLimitConfigs.GUILD_STICKER_UPDATE,
        ["DELETE /guilds/:guild_id/stickers/:sticker_id"] = RateLimitConfigs.GUILD_STICKER_DELETE,
        ["GET /guilds/:guild_id/invites"] = RateLimitConfigs.INVITE_LIST_GUILD,
        ["GET /guilds/:guild_id/webhooks"] = RateLimitConfigs.WEBHOOK_LIST_GUILD,

        // Tenor API
        ["GET /tenor/search"] = RateLimitConfigs.TENOR_SEARCH,
        ["GET /tenor/featured"] = RateLimitConfigs.TENOR_FEATURED,
        ["GET /tenor/trending-gifs"] = RateLimitConfigs.TENOR_TRENDING,
        ["POST /tenor/register-share"] = RateLimitConfigs.TENOR_REGISTER_SHARE,
        ["GET /tenor/suggest"] = RateLimitConfigs.TENOR_SUGGEST,

        // User API
        ["GET /users/@me"] = RateLimitConfigs.USER_GET,
        ["PATCH /users/@me"] = RateLimitConfigs.USER_UPDATE_SELF,
        ["GET /users/check-tag"] = RateLimitConfigs.USER_CHECK_TAG,
        ["GET /users/:user_id"] = RateLimitConfigs.USER_GET,
        ["GET /users/:target_id/profile"] = RateLimitConfigs.USER_GET_PROFILE,
        ["GET /users/@me/settings"] = RateLimitConfigs.USER_SETTINGS_GET,
        ["PATCH /users/@me/settings"] = RateLimitConfigs.USER_SETTINGS_UPDATE,
        ["GET /users/@me/notes"] = RateLimitConfigs.USER_NOTES_READ,
        ["GET /users/@me/notes/:target_id"] = RateLimitConfigs.USER_NOTES_READ,
        ["PUT /users/@me/notes/:target_id"] = RateLimitConfigs.USER_NOTES_WRITE,
        ["GET /users/@me/beta-codes"] = RateLimitConfigs.USER_BETA_CODES_READ,
        ["POST /users/@me/beta-codes"] = RateLimitConfigs.USER_BETA_CODES_CREATE,
        ["DELETE /users/@me/beta-codes/:code"] = RateLimitConfigs.USER_BETA_CODES_DELETE,
        ["GET /users/@me/mentions"] = RateLimitConfigs.USER_MENTIONS_READ,
        ["DELETE /users/@me/mentions/:message_id"] = RateLimitConfigs.USER_MENTIONS_DELETE,
        ["POST /users/@me/mfa/totp/enable"] = RateLimitConfigs.USER_MFA_TOTP_ENABLE,
        ["POST /users/@me/mfa/totp/disable"] = RateLimitConfigs.USER_MFA_TOTP_DISABLE,
        ["POST /users/@me/mfa/backup-codes"] = RateLimitConfigs.USER_MFA_BACKUP_CODES,
        ["POST /users/@me/phone/send-verification"] = RateLimitConfigs.PHONE_SEND_VERIFICATION,
        ["POST /users/@me/phone/verify"] = RateLimitConfigs.PHONE_VERIFY_CODE,
        ["POST /users/@me/phone"] = RateLimitConfigs.PHONE_ADD,
        ["DELETE /users/@me/phone"] = RateLimitConfigs.PHONE_REMOVE,
        ["POST /users/@me/mfa/sms/enable"] = RateLimitConfigs.MFA_SMS_ENABLE,
        ["POST /users/@me/mfa/sms/disable"] = RateLimitConfigs.MFA_SMS_DISABLE,
        ["GET /users/@me/mfa/webauthn/credentials"] = RateLimitConfigs.MFA_WEBAUTHN_LIST,
        ["POST /users/@me/mfa/webauthn/credentials/registration-options"] = RateLimitConfigs.MFA_WEBAUTHN_REGISTRATION_OPTIONS,
        ["POST /users/@me/mfa/webauthn/credentials"] = RateLimitConfigs.MFA_WEBAUTHN_REGISTER,
        ["PATCH /users/@me/mfa/webauthn/credentials/:credential_id"] = RateLimitConfigs.MFA_WEBAUTHN_UPDATE,
        ["DELETE /users/@me/mfa/webauthn/credentials/:credential_id"] = RateLimitConfigs.MFA_WEBAUTHN_DELETE,
        ["GET /users/@me/saved-messages"] = RateLimitConfigs.USER_SAVED_MESSAGES_READ,
        ["POST /users/@me/saved-messages"] = RateLimitConfigs.USER_SAVED_MESSAGES_WRITE,
        ["DELETE /users/@me/saved-messages/:message_id"] = RateLimitConfigs.USER_SAVED_MESSAGES_WRITE,
        ["GET /users/@me/channels"] = RateLimitConfigs.USER_CHANNELS,
        ["POST /users/@me/channels"] = RateLimitConfigs.USER_CHANNELS,
        ["GET /users/@me/relationships"] = RateLimitConfigs.USER_RELATIONSHIPS_LIST,
        ["POST /users/@me/relationships"] = RateLimitConfigs.USER_FRIEND_REQUEST_SEND,
        ["POST /users/@me/relationships/:user_id"] = RateLimitConfigs.USER_FRIEND_REQUEST_SEND,
        ["PUT /users/@me/relationships/:user_id"] = RateLimitConfigs.USER_FRIEND_REQUEST_ACCEPT,
        ["DELETE /users/@me/relationships/:user_id"] = RateLimitConfigs.USER_RELATIONSHIP_DELETE,
        ["PATCH /users/@me/guilds/@me/settings"] = RateLimitConfigs.USER_GUILD_SETTINGS_UPDATE,
        ["PATCH /users/@me/guilds/:guild_id/settings"] = RateLimitConfigs.USER_GUILD_SETTINGS_UPDATE,
        ["POST /users/@me/disable"] = RateLimitConfigs.USER_ACCOUNT_DISABLE,
        ["POST /users/@me/delete"] = RateLimitConfigs.USER_ACCOUNT_DELETE,
        ["POST /users/@me/push/subscribe"] = RateLimitConfigs.USER_PUSH_SUBSCRIBE,
        ["GET /users/@me/push/subscriptions"] = RateLimitConfigs.USER_PUSH_LIST,
        ["DELETE /users/@me/push/subscriptions/:subscription_id"] = RateLimitConfigs.USER_PUSH_UNSUBSCRIBE,
        ["POST /users/@me/harvest"] = RateLimitConfigs.USER_DATA_HARVEST,
        ["GET /users/@me/harvest/latest"] = RateLimitConfigs.USER_HARVEST_LATEST,
        ["GET /users/@me/harvest/:harvestId"] = RateLimitConfigs.USER_HARVEST_STATUS,
        ["GET /users/@me/harvest/:harvestId/download"] = RateLimitConfigs.USER_HARVEST_DOWNLOAD,
        ["POST /users/@me/preload-messages"] = RateLimitConfigs.USER_PRELOAD_MESSAGES,
        ["POST /users/@me/messages/delete"] = RateLimitConfigs.USER_BULK_MESSAGE_DELETE,

        // Webhook API
        ["GET /webhooks/:webhook_id"] = RateLimitConfigs.WEBHOOK_GET,
        ["PATCH /webhooks/:webhook_id"] = RateLimitConfigs.WEBHOOK_UPDATE,
        ["DELETE /webhooks/:webhook_id"] = RateLimitConfigs.WEBHOOK_DELETE,
        ["GET /webhooks/:webhook_id/:token"] = RateLimitConfigs.WEBHOOK_GET,
        ["PATCH /webhooks/:webhook_id/:token"] = RateLimitConfigs.WEBHOOK_UPDATE,
        ["DELETE /webhooks/:webhook_id/:token"] = RateLimitConfigs.WEBHOOK_DELETE,
        ["POST /webhooks/:webhook_id/:token"] = RateLimitConfigs.WEBHOOK_EXECUTE,
        ["POST /webhooks/:webhook_id/:token/github"] = RateLimitConfigs.WEBHOOK_GITHUB,

        // Stripe/Premium API
        ["GET /premium/visionary/slots"] = RateLimitConfigs.STRIPE_VISIONARY_SLOTS,
        ["GET /premium/price-ids"] = RateLimitConfigs.STRIPE_PRICE_IDS,
        ["POST /premium/customer-portal"] = RateLimitConfigs.STRIPE_CUSTOMER_PORTAL,
        ["POST /premium/cancel-subscription"] = RateLimitConfigs.STRIPE_SUBSCRIPTION_CANCEL,
        ["POST /premium/reactivate-subscription"] = RateLimitConfigs.STRIPE_SUBSCRIPTION_REACTIVATE,
        ["POST /premium/visionary/rejoin"] = RateLimitConfigs.STRIPE_VISIONARY_REJOIN,
        ["POST /stripe/checkout/subscription"] = RateLimitConfigs.STRIPE_CHECKOUT_SUBSCRIPTION,
        ["POST /stripe/checkout/gift"] = RateLimitConfigs.STRIPE_CHECKOUT_GIFT,

        // Gift API
        ["GET /gifts/:code"] = RateLimitConfigs.GIFT_CODE_GET,
        ["POST /gifts/:code/redeem"] = RateLimitConfigs.GIFT_CODE_REDEEM,
        ["GET /users/@me/gifts"] = RateLimitConfigs.GIFTS_LIST,
    };
}
