using System.Collections.Generic;

namespace Fluxer.Net.RateLimiting;

/// <summary>
/// Configuration for a rate limit bucket
/// </summary>
public class RateLimitConfig
{
    public string Bucket { get; set; }
    public int Limit { get; set; }
    public int WindowMs { get; set; }
    public bool ExemptFromGlobal { get; set; } = false;

    public RateLimitConfig(string bucket, int limit, int windowMs, bool exemptFromGlobal = false)
    {
        Bucket = bucket;
        Limit = limit;
        WindowMs = windowMs;
        ExemptFromGlobal = exemptFromGlobal;
    }
}

/// <summary>
/// Static configuration for all Fluxer API rate limit buckets
/// Matches the RateLimitConfig.ts from the Fluxer API
/// </summary>
public static class RateLimitConfigs
{
    // Auth rate limits
    public static readonly RateLimitConfig AUTH_REGISTER = new("auth:register", 10, 10000);
    public static readonly RateLimitConfig AUTH_LOGIN = new("auth:login", 10, 10000);
    public static readonly RateLimitConfig AUTH_LOGIN_MFA = new("auth:login:mfa", 20, 10000);
    public static readonly RateLimitConfig AUTH_VERIFY_EMAIL = new("auth:verify", 10, 60000);
    public static readonly RateLimitConfig AUTH_RESEND_VERIFICATION = new("auth:verify:resend", 10, 60000);
    public static readonly RateLimitConfig AUTH_FORGOT_PASSWORD = new("auth:forgot", 5, 60000);
    public static readonly RateLimitConfig AUTH_RESET_PASSWORD = new("auth:reset", 10, 60000);
    public static readonly RateLimitConfig AUTH_SESSIONS_GET = new("auth:sessions", 40, 10000);
    public static readonly RateLimitConfig AUTH_SESSIONS_LOGOUT = new("auth:sessions:logout", 20, 10000);
    public static readonly RateLimitConfig AUTH_AUTHORIZE_IP = new("auth:authorize_ip", 5, 60000);
    public static readonly RateLimitConfig AUTH_LOGOUT = new("auth:logout", 20, 10000);
    public static readonly RateLimitConfig AUTH_WEBAUTHN_OPTIONS = new("auth:webauthn:options", 20, 10000);
    public static readonly RateLimitConfig AUTH_WEBAUTHN_AUTHENTICATE = new("auth:webauthn:authenticate", 10, 10000);

    // User rate limits
    public static readonly RateLimitConfig USER_GET = new("user:read::user_id", 100, 10000);
    public static readonly RateLimitConfig USER_GET_PROFILE = new("user:profile::target_id", 100, 10000);
    public static readonly RateLimitConfig USER_CHECK_TAG = new("user:check_tag", 60, 10000);
    public static readonly RateLimitConfig USER_UPDATE_SELF = new("user:update", 20, 60000);
    public static readonly RateLimitConfig USER_ACCOUNT_DISABLE = new("user:account:disable", 5, 3600000);
    public static readonly RateLimitConfig USER_ACCOUNT_DELETE = new("user:account:delete", 5, 3600000);
    public static readonly RateLimitConfig USER_DATA_HARVEST = new("user:data:harvest", 1, 3600000);
    public static readonly RateLimitConfig USER_PRELOAD_MESSAGES = new("user:preload_messages", 40, 10000);
    public static readonly RateLimitConfig USER_BULK_MESSAGE_DELETE = new("user:messages:bulk_delete", 6, 60000);
    public static readonly RateLimitConfig USER_SETTINGS_GET = new("user:settings:get", 40, 10000);
    public static readonly RateLimitConfig USER_SETTINGS_UPDATE = new("user:settings:update", 20, 10000);
    public static readonly RateLimitConfig USER_GUILD_SETTINGS_UPDATE = new("user:guild_settings:update", 30, 10000);
    public static readonly RateLimitConfig USER_CHANNELS = new("user:channels", 40, 10000);
    public static readonly RateLimitConfig USER_RELATIONSHIPS_LIST = new("user:relationships:list", 40, 10000);
    public static readonly RateLimitConfig USER_FRIEND_REQUEST_SEND = new("user:friend_request:send", 10, 60000);
    public static readonly RateLimitConfig USER_FRIEND_REQUEST_ACCEPT = new("user:friend_request:accept", 20, 10000);
    public static readonly RateLimitConfig USER_BLOCK = new("user:block", 20, 10000);
    public static readonly RateLimitConfig USER_RELATIONSHIP_DELETE = new("user:relationship:delete", 30, 10000);
    public static readonly RateLimitConfig USER_NOTES_READ = new("user:notes:read", 60, 10000);
    public static readonly RateLimitConfig USER_NOTES_WRITE = new("user:notes:write", 40, 10000);
    public static readonly RateLimitConfig USER_BETA_CODES_READ = new("user:beta_codes:read", 40, 10000);
    public static readonly RateLimitConfig USER_BETA_CODES_CREATE = new("user:beta_codes:create", 6, 60000);
    public static readonly RateLimitConfig USER_BETA_CODES_DELETE = new("user:beta_codes:delete", 20, 10000);
    public static readonly RateLimitConfig USER_MENTIONS_READ = new("user:mentions:read", 40, 10000);
    public static readonly RateLimitConfig USER_MENTIONS_DELETE = new("user:mentions:delete", 60, 10000);
    public static readonly RateLimitConfig USER_SAVED_MESSAGES_READ = new("user:saved_messages:read", 40, 10000);
    public static readonly RateLimitConfig USER_SAVED_MESSAGES_WRITE = new("user:saved_messages:write", 30, 10000);
    public static readonly RateLimitConfig USER_MFA_TOTP_ENABLE = new("user:mfa:totp:enable", 10, 60000);
    public static readonly RateLimitConfig USER_MFA_TOTP_DISABLE = new("user:mfa:totp:disable", 10, 60000);
    public static readonly RateLimitConfig USER_MFA_BACKUP_CODES = new("user:mfa:backup_codes", 6, 60000);

    // MFA rate limits
    public static readonly RateLimitConfig MFA_SMS_ENABLE = new("mfa:sms:enable", 10, 60000);
    public static readonly RateLimitConfig MFA_SMS_DISABLE = new("mfa:sms:disable", 10, 60000);
    public static readonly RateLimitConfig MFA_WEBAUTHN_LIST = new("mfa:webauthn:list", 40, 10000);
    public static readonly RateLimitConfig MFA_WEBAUTHN_REGISTRATION_OPTIONS = new("mfa:webauthn:registration_options", 20, 10000);
    public static readonly RateLimitConfig MFA_WEBAUTHN_REGISTER = new("mfa:webauthn:register", 10, 60000);
    public static readonly RateLimitConfig MFA_WEBAUTHN_UPDATE = new("mfa:webauthn:update", 20, 10000);
    public static readonly RateLimitConfig MFA_WEBAUTHN_DELETE = new("mfa:webauthn:delete", 10, 60000);

    // Phone rate limits
    public static readonly RateLimitConfig PHONE_SEND_VERIFICATION = new("phone:send_verification", 5, 60000);
    public static readonly RateLimitConfig PHONE_VERIFY_CODE = new("phone:verify_code", 10, 60000);
    public static readonly RateLimitConfig PHONE_ADD = new("phone:add", 10, 60000);
    public static readonly RateLimitConfig PHONE_REMOVE = new("phone:remove", 10, 60000);

    // Push notification rate limits
    public static readonly RateLimitConfig USER_PUSH_SUBSCRIBE = new("user:push:subscribe", 20, 60000);
    public static readonly RateLimitConfig USER_PUSH_UNSUBSCRIBE = new("user:push:unsubscribe", 40, 60000);
    public static readonly RateLimitConfig USER_PUSH_LIST = new("user:push:list", 40, 10000);

    // Channel rate limits
    public static readonly RateLimitConfig CHANNEL_GET = new("channel:read::channel_id", 100, 10000);
    public static readonly RateLimitConfig CHANNEL_UPDATE = new("channel:update::channel_id", 20, 10000);
    public static readonly RateLimitConfig CHANNEL_DELETE = new("channel:delete::channel_id", 20, 10000);
    public static readonly RateLimitConfig CHANNEL_READ_STATE_DELETE = new("channel:read_state:delete::channel_id", 40, 10000);
    public static readonly RateLimitConfig CHANNEL_MESSAGES_GET = new("channel:messages:read::channel_id", 100, 10000);
    public static readonly RateLimitConfig CHANNEL_MESSAGE_GET = new("channel:message:read::channel_id", 100, 10000);
    public static readonly RateLimitConfig CHANNEL_MESSAGE_CREATE = new("channel:message:create::channel_id", 20, 10000);
    public static readonly RateLimitConfig CHANNEL_MESSAGE_UPDATE = new("channel:message:update::channel_id", 20, 10000);
    public static readonly RateLimitConfig CHANNEL_MESSAGE_DELETE = new("channel:message:delete::channel_id", 20, 10000);
    public static readonly RateLimitConfig CHANNEL_MESSAGE_BULK_DELETE = new("channel:message:bulk_delete::channel_id", 10, 10000);
    public static readonly RateLimitConfig CHANNEL_MESSAGE_ACK = new("channel:message:ack::channel_id", 100, 10000);
    public static readonly RateLimitConfig CHANNEL_SEARCH = new("channel:search::channel_id", 20, 10000);
    public static readonly RateLimitConfig CHANNEL_ATTACHMENT_UPLOAD = new("channel:attachment:upload::channel_id", 10, 10000);
    public static readonly RateLimitConfig ATTACHMENT_DELETE = new("attachment:delete", 40, 10000);
    public static readonly RateLimitConfig CHANNEL_TYPING = new("channel:typing::channel_id", 20, 10000);
    public static readonly RateLimitConfig CHANNEL_PINS = new("channel:pins::channel_id", 20, 10000);
    public static readonly RateLimitConfig CHANNEL_REACTIONS = new("channel:reactions::channel_id", 30, 10000);
    public static readonly RateLimitConfig CHANNEL_CALL_GET = new("channel:call:get::channel_id", 60, 10000);
    public static readonly RateLimitConfig CHANNEL_CALL_UPDATE = new("channel:call:update::channel_id", 10, 10000);
    public static readonly RateLimitConfig CHANNEL_CALL_RING = new("channel:call:ring::channel_id", 5, 10000);
    public static readonly RateLimitConfig CHANNEL_CALL_STOP_RINGING = new("channel:call:stop_ringing::channel_id", 20, 10000);

    // Guild rate limits
    public static readonly RateLimitConfig GUILD_CREATE = new("guild:create", 10, 60000);
    public static readonly RateLimitConfig GUILD_LIST = new("guild:list", 40, 10000);
    public static readonly RateLimitConfig GUILD_GET = new("guild:read::guild_id", 100, 10000);
    public static readonly RateLimitConfig GUILD_UPDATE = new("guild:update::guild_id", 20, 10000);
    public static readonly RateLimitConfig GUILD_DELETE = new("guild:delete::guild_id", 10, 60000);
    public static readonly RateLimitConfig GUILD_LEAVE = new("guild:leave::guild_id", 10, 10000);
    public static readonly RateLimitConfig GUILD_VANITY_URL_GET = new("guild:vanity_url:get::guild_id", 100, 10000);
    public static readonly RateLimitConfig GUILD_VANITY_URL_PATCH = new("guild:vanity_url:patch::guild_id", 10, 60000);
    public static readonly RateLimitConfig GUILD_MEMBERS = new("guild:members::guild_id", 40, 10000);
    public static readonly RateLimitConfig GUILD_MEMBER_UPDATE = new("guild:member:update::guild_id", 20, 10000);
    public static readonly RateLimitConfig GUILD_MEMBER_REMOVE = new("guild:member:remove::guild_id", 20, 10000);
    public static readonly RateLimitConfig GUILD_MEMBER_ROLE_ADD = new("guild:member:role:add::guild_id", 20, 10000);
    public static readonly RateLimitConfig GUILD_MEMBER_ROLE_REMOVE = new("guild:member:role:remove::guild_id", 20, 10000);
    public static readonly RateLimitConfig GUILD_CHANNELS_LIST = new("guild:channels:list::guild_id", 60, 10000);
    public static readonly RateLimitConfig GUILD_CHANNEL_CREATE = new("guild:channel:create::guild_id", 10, 60000);
    public static readonly RateLimitConfig GUILD_CHANNEL_POSITIONS = new("guild:channel:positions::guild_id", 30, 10000);
    public static readonly RateLimitConfig GUILD_SEARCH = new("guild:search::guild_id", 20, 10000);
    public static readonly RateLimitConfig GUILD_ROLE_CREATE = new("guild:role:create::guild_id", 10, 60000);
    public static readonly RateLimitConfig GUILD_ROLE_UPDATE = new("guild:role:update::guild_id", 20, 10000);
    public static readonly RateLimitConfig GUILD_ROLE_DELETE = new("guild:role:delete::guild_id", 10, 60000);
    public static readonly RateLimitConfig GUILD_ROLE_POSITIONS = new("guild:role:positions::guild_id", 20, 10000);
    public static readonly RateLimitConfig GUILD_EMOJIS_LIST = new("guild:emojis:list::guild_id", 60, 10000);
    public static readonly RateLimitConfig GUILD_EMOJI_CREATE = new("guild:emoji:create::guild_id", 20, 10000);
    public static readonly RateLimitConfig GUILD_EMOJI_BULK_CREATE = new("guild:emoji:bulk_create::guild_id", 6, 60000);
    public static readonly RateLimitConfig GUILD_EMOJI_UPDATE = new("guild:emoji:update::guild_id", 20, 10000);
    public static readonly RateLimitConfig GUILD_EMOJI_DELETE = new("guild:emoji:delete::guild_id", 20, 10000);
    public static readonly RateLimitConfig GUILD_STICKERS_LIST = new("guild:sticker:list::guild_id", 60, 10000);
    public static readonly RateLimitConfig GUILD_STICKER_CREATE = new("guild:sticker:create::guild_id", 20, 10000);
    public static readonly RateLimitConfig GUILD_STICKER_BULK_CREATE = new("guild:sticker:bulk_create::guild_id", 6, 60000);
    public static readonly RateLimitConfig GUILD_STICKER_UPDATE = new("guild:sticker:update::guild_id", 20, 10000);
    public static readonly RateLimitConfig GUILD_STICKER_DELETE = new("guild:sticker:delete::guild_id", 20, 10000);

    // Invite rate limits
    public static readonly RateLimitConfig INVITE_GET = new("invite:read::invite_code", 100, 10000);
    public static readonly RateLimitConfig INVITE_ACCEPT = new("invite:accept", 10, 10000);
    public static readonly RateLimitConfig INVITE_CREATE = new("invite:create::channel_id", 20, 60000);
    public static readonly RateLimitConfig INVITE_DELETE = new("invite:delete::invite_code", 20, 10000);
    public static readonly RateLimitConfig INVITE_LIST_CHANNEL = new("invite:list::channel_id", 40, 10000);
    public static readonly RateLimitConfig INVITE_LIST_GUILD = new("invite:list::guild_id", 40, 10000);

    // Webhook rate limits
    public static readonly RateLimitConfig WEBHOOK_LIST_GUILD = new("webhook:list::guild_id", 40, 10000);
    public static readonly RateLimitConfig WEBHOOK_LIST_CHANNEL = new("webhook:list::channel_id", 40, 10000);
    public static readonly RateLimitConfig WEBHOOK_CREATE = new("webhook:create::channel_id", 10, 60000);
    public static readonly RateLimitConfig WEBHOOK_GET = new("webhook:read::webhook_id", 100, 10000);
    public static readonly RateLimitConfig WEBHOOK_UPDATE = new("webhook:update::webhook_id", 20, 10000);
    public static readonly RateLimitConfig WEBHOOK_DELETE = new("webhook:delete::webhook_id", 20, 10000);
    public static readonly RateLimitConfig WEBHOOK_EXECUTE = new("webhook:execute::webhook_id", 60, 60000, true);
    public static readonly RateLimitConfig WEBHOOK_GITHUB = new("webhook:github::webhook_id", 200, 60000, true);

    // Tenor rate limits
    public static readonly RateLimitConfig TENOR_SEARCH = new("tenor:search", 40, 10000);
    public static readonly RateLimitConfig TENOR_FEATURED = new("tenor:featured", 40, 10000);
    public static readonly RateLimitConfig TENOR_TRENDING = new("tenor:trending", 40, 10000);
    public static readonly RateLimitConfig TENOR_SUGGEST = new("tenor:suggest", 40, 10000);
    public static readonly RateLimitConfig TENOR_REGISTER_SHARE = new("tenor:register_share", 60, 10000);

    // Read state rate limits
    public static readonly RateLimitConfig READ_STATE_ACK_BULK = new("read_state:ack_bulk", 20, 10000);

    // Stripe/Premium rate limits
    public static readonly RateLimitConfig STRIPE_VISIONARY_SLOTS = new("stripe:visionary:slots", 20, 10000);
    public static readonly RateLimitConfig STRIPE_PRICE_IDS = new("stripe:price:ids", 40, 10000);
    public static readonly RateLimitConfig STRIPE_CHECKOUT_SUBSCRIPTION = new("stripe:checkout:subscription", 3, 60000);
    public static readonly RateLimitConfig STRIPE_CHECKOUT_GIFT = new("stripe:checkout:gift", 3, 60000);
    public static readonly RateLimitConfig STRIPE_CUSTOMER_PORTAL = new("stripe:customer_portal", 5, 60000);
    public static readonly RateLimitConfig STRIPE_SUBSCRIPTION_CANCEL = new("stripe:subscription:cancel", 5, 60000);
    public static readonly RateLimitConfig STRIPE_SUBSCRIPTION_REACTIVATE = new("stripe:subscription:reactivate", 5, 60000);
    public static readonly RateLimitConfig STRIPE_VISIONARY_REJOIN = new("stripe:visionary:rejoin", 5, 60000);

    // User harvest rate limits
    public static readonly RateLimitConfig USER_HARVEST_LATEST = new("user:harvest:latest", 40, 10000);
    public static readonly RateLimitConfig USER_HARVEST_STATUS = new("user:harvest:status", 40, 10000);
    public static readonly RateLimitConfig USER_HARVEST_DOWNLOAD = new("user:harvest:download", 20, 10000);

    // Gift rate limits
    public static readonly RateLimitConfig GIFT_CODE_GET = new("gift:get", 60, 10000);
    public static readonly RateLimitConfig GIFT_CODE_REDEEM = new("gift:redeem", 10, 60000);
    public static readonly RateLimitConfig GIFTS_LIST = new("gifts:list", 40, 10000);

    // Report rate limits
    public static readonly RateLimitConfig REPORT_CREATE = new("report:create", 10, 3600000);

    // Favorite meme rate limits
    public static readonly RateLimitConfig FAVORITE_MEME_LIST = new("favorite_meme:list", 60, 10000);
    public static readonly RateLimitConfig FAVORITE_MEME_GET = new("favorite_meme:get", 100, 10000);
    public static readonly RateLimitConfig FAVORITE_MEME_CREATE_FROM_MESSAGE = new("favorite_meme:create:message", 10, 60000);
    public static readonly RateLimitConfig FAVORITE_MEME_CREATE_FROM_URL = new("favorite_meme:create:url", 20, 60000);
    public static readonly RateLimitConfig FAVORITE_MEME_UPDATE = new("favorite_meme:update", 30, 10000);
    public static readonly RateLimitConfig FAVORITE_MEME_DELETE = new("favorite_meme:delete", 30, 10000);

    // Default rate limit
    public static readonly RateLimitConfig DEFAULT = new("default", 60, 10000);
}
