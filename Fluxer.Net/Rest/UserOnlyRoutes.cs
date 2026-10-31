namespace Fluxer.Net.Rest;

/// <summary>
/// The API routes Fluxer serves to logged-in users only, taken from the server's <c>DefaultUserOnly</c> guards.
/// </summary>
public static class UserOnlyRoutes
{
    private static (string Method, string[] Segments)[]? _routes;

    /// <summary>
    /// Whether <paramref name="route"/>, called with <paramref name="method"/>, is only served to users.
    /// </summary>
    /// <param name="method">The HTTP method.</param>
    /// <param name="route">The route, with or without a query string.</param>
    public static bool IsUserOnly(HttpMethod method, string route)
    {
        _routes ??= Table.Select(x => (x.Method, x.Route.Split('/', StringSplitOptions.RemoveEmptyEntries))).ToArray();
        int query = route.IndexOf('?');
        string[] segments = (query < 0 ? route : route.Substring(0, query)).Split('/', StringSplitOptions.RemoveEmptyEntries);
        foreach ((string Method, string[] Segments) candidate in _routes)
        {
            if (candidate.Method == method.Method && Matches(candidate.Segments, segments))
                return true;
        }

        return false;
    }

    private static bool Matches(string[] pattern, string[] segments)
    {
        if (pattern.Length != segments.Length)
            return false;

        for (int i = 0; i < pattern.Length; i++)
        {
            string expected = pattern[i];
            if (expected == ":gifprefix")
            {
                if (segments[i] != "gifs" && segments[i] != "klipy")
                    return false;
            }
            else if (expected[0] == ':')
            {
                if (segments[i].StartsWith("@", StringComparison.Ordinal))
                    return false;
            }
            else if (!string.Equals(expected, segments[i], StringComparison.Ordinal))
                return false;
        }

        return true;
    }

    /// <summary>
    /// Every user-only route as the server declares it. Segments starting with <c>:</c> match any value except
    /// literals such as <c>@me</c>, and <c>:gifprefix</c> matches <c>gifs</c> or <c>klipy</c>.
    /// </summary>
    public static IReadOnlyList<(string Method, string Route)> All => Table;

    private static readonly (string Method, string Route)[] Table =
    {
        ("DELETE", "/oauth2/applications/:id"),
        ("DELETE", "/oauth2/@me/authorizations/:applicationId"),
        ("DELETE", "/operator-pass/offers/:offer_id"),
        ("DELETE", "/premium/store/purchases/:purchase_id"),
        ("DELETE", "/streams/:stream_key/preview"),
        ("DELETE", "/users/@me/authorized-ips"),
        ("DELETE", "/users/@me/connections/:type/:connection_id"),
        ("DELETE", "/users/@me/entrance-sounds/:sound_id"),
        ("DELETE", "/users/@me/guild-visits/:guild_id"),
        ("DELETE", "/users/@me/memes/:meme_id"),
        ("DELETE", "/users/@me/mentions/:message_id"),
        ("DELETE", "/users/@me/messages/delete"),
        ("DELETE", "/users/@me/mfa/webauthn/credentials/:credential_id"),
        ("DELETE", "/users/@me/mobile-devices/:device_id"),
        ("DELETE", "/users/@me/push/subscriptions/:subscription_id"),
        ("DELETE", "/users/@me/relationships/:user_id"),
        ("DELETE", "/users/@me/saved-messages/:message_id"),
        ("GET", "/auth/sessions"),
        ("GET", "/channels/:channel_id/call"),
        ("GET", "/channels/:channel_id/rtc-regions"),
        ("GET", "/discovery/guilds/:guild_id/channels/:channel_id"),
        ("GET", "/:gifprefix/featured"),
        ("GET", "/:gifprefix/search"),
        ("GET", "/:gifprefix/suggest"),
        ("GET", "/oauth2/applications/:id"),
        ("GET", "/oauth2/@me"),
        ("GET", "/oauth2/@me/authorizations"),
        ("GET", "/operator-pass/listing"),
        ("GET", "/operator-pass/offers/:offer_id"),
        ("GET", "/operator-pass/passes"),
        ("GET", "/operator-pass/purchases"),
        ("GET", "/premium/current-subscription-price"),
        ("GET", "/premium/refund-eligibility"),
        ("GET", "/premium/state"),
        ("GET", "/premium/store"),
        ("GET", "/premium/store/purchases"),
        ("GET", "/streams/:stream_key/preview"),
        ("GET", "/users/@me/applications"),
        ("GET", "/users/@me/connections"),
        ("GET", "/users/@me/entrance-sounds"),
        ("GET", "/users/@me/gifts"),
        ("GET", "/users/@me/guild-visits"),
        ("GET", "/users/@me/harvest/:harvestId"),
        ("GET", "/users/@me/harvest/:harvestId/download"),
        ("GET", "/users/@me/harvest/latest"),
        ("GET", "/users/@me/memes"),
        ("GET", "/users/@me/memes/:meme_id"),
        ("GET", "/users/@me/mentions"),
        ("GET", "/users/@me/mfa/webauthn/credentials"),
        ("GET", "/users/@me/mfa/webauthn/migration"),
        ("GET", "/users/@me/mobile-devices"),
        ("GET", "/users/@me/notes"),
        ("GET", "/users/@me/notes/:target_id"),
        ("GET", "/users/@me/push/subscriptions"),
        ("GET", "/users/@me/recovery-kit"),
        ("GET", "/users/@me/relationships"),
        ("GET", "/users/@me/saved-messages"),
        ("GET", "/users/@me/settings"),
        ("GET", "/users/@me/sudo/mfa-methods"),
        ("PATCH", "/channels/:channel_id/call"),
        ("PATCH", "/channels/:channel_id/thread-members/@me/settings"),
        ("PATCH", "/oauth2/applications/:id"),
        ("PATCH", "/oauth2/applications/:id/bot"),
        ("PATCH", "/premium/perks-disabled"),
        ("PATCH", "/streams/:stream_key/stream"),
        ("PATCH", "/users/@me"),
        ("PATCH", "/users/@me/connections/reorder"),
        ("PATCH", "/users/@me/connections/:type/:connection_id"),
        ("PATCH", "/users/@me/entrance-sounds/:sound_id"),
        ("PATCH", "/users/@me/guilds/:guild_id/settings"),
        ("PATCH", "/users/@me/guilds/@me/settings"),
        ("PATCH", "/users/@me/memes/:meme_id"),
        ("PATCH", "/users/@me/mfa/webauthn/credentials/:credential_id"),
        ("PATCH", "/users/@me/relationships/:user_id"),
        ("PATCH", "/users/@me/settings"),
        ("POST", "/auth/origin-handoff"),
        ("POST", "/auth/sessions/logout"),
        ("POST", "/auth/verify/resend"),
        ("POST", "/channels/:channel_id/call/end"),
        ("POST", "/channels/:channel_id/call/ring"),
        ("POST", "/channels/:channel_id/call/stop-ringing"),
        ("POST", "/channels/:channel_id/messages/:message_id/ack"),
        ("POST", "/channels/:channel_id/messages/:message_id/memes"),
        ("POST", "/channels/messages/bulk"),
        ("POST", "/discovery/guilds/:guild_id/join"),
        ("POST", "/discovery/guilds/:guild_id/visit"),
        ("POST", "/:gifprefix/register-share"),
        ("POST", "/gifts/:code/redeem"),
        ("POST", "/invites/:invite_code"),
        ("POST", "/invites/:invite_code/visit"),
        ("POST", "/oauth2/applications/:id/bot/reset-token"),
        ("POST", "/oauth2/applications/:id/client-secret/reset"),
        ("POST", "/oauth2/authorize/consent"),
        ("POST", "/oauth2/@me/authorizations/revoke"),
        ("POST", "/operator-pass/checkout"),
        ("POST", "/operator-pass/offers/:offer_id/accept"),
        ("POST", "/operator-pass/offers/:offer_id/decline"),
        ("POST", "/operator-pass/passes/:pass_id/offers"),
        ("POST", "/operator-pass/passes/:pass_id/withdraw"),
        ("POST", "/operator-pass/purchases/:purchase_id/withdraw"),
        ("POST", "/premium/cancel-pending-subscription-change"),
        ("POST", "/premium/cancel-subscription"),
        ("POST", "/premium/change-subscription"),
        ("POST", "/premium/customer-portal"),
        ("POST", "/premium/grace/end"),
        ("POST", "/premium/reactivate-subscription"),
        ("POST", "/premium/refund-latest"),
        ("POST", "/premium/store/app-store/transactions"),
        ("POST", "/premium/store/google-play/purchases"),
        ("POST", "/premium/switch-to-list-price"),
        ("POST", "/premium/visionary/rejoin"),
        ("POST", "/read-states/ack"),
        ("POST", "/read-states/ack-bulk"),
        ("POST", "/reports/flows/message/submissions"),
        ("POST", "/reports/flows/user/submissions"),
        ("POST", "/reports/guild"),
        ("POST", "/reports/message"),
        ("POST", "/reports/user"),
        ("POST", "/streams/:stream_key/preview"),
        ("POST", "/streams/:stream_key/preview/upload-url"),
        ("POST", "/stripe/checkout/gift"),
        ("POST", "/stripe/checkout/subscription"),
        ("POST", "/unfurl"),
        ("POST", "/users/@me/claim"),
        ("POST", "/users/@me/connections"),
        ("POST", "/users/@me/connections/bluesky/authorize"),
        ("POST", "/users/@me/connections/verify"),
        ("POST", "/users/@me/delete"),
        ("POST", "/users/@me/disable"),
        ("POST", "/users/@me/email-change/apply"),
        ("POST", "/users/@me/email-change/bounced/request-new"),
        ("POST", "/users/@me/email-change/bounced/resend-new"),
        ("POST", "/users/@me/email-change/bounced/verify-new"),
        ("POST", "/users/@me/email-change/request-new"),
        ("POST", "/users/@me/email-change/resend-new"),
        ("POST", "/users/@me/email-change/resend-original"),
        ("POST", "/users/@me/email-change/start"),
        ("POST", "/users/@me/email-change/verify-new"),
        ("POST", "/users/@me/email-change/verify-original"),
        ("POST", "/users/@me/email-verification/confirm"),
        ("POST", "/users/@me/email-verification/send"),
        ("POST", "/users/@me/entrance-sounds"),
        ("POST", "/users/@me/favorite-gifs/resolve"),
        ("POST", "/users/@me/guild-visits/:guild_id/join"),
        ("POST", "/users/@me/harvest"),
        ("POST", "/users/@me/harvest/filtered"),
        ("POST", "/users/@me/memes"),
        ("POST", "/users/@me/mentions/read"),
        ("POST", "/users/@me/messages/bulk-delete-mine"),
        ("POST", "/users/@me/messages/delete"),
        ("POST", "/users/@me/mfa/backup-codes"),
        ("POST", "/users/@me/mfa/backup-codes/challenge"),
        ("POST", "/users/@me/mfa/backup-codes/challenge/regenerate"),
        ("POST", "/users/@me/mfa/backup-codes/challenge/resend"),
        ("POST", "/users/@me/mfa/backup-codes/challenge/verify"),
        ("POST", "/users/@me/mfa/totp/disable"),
        ("POST", "/users/@me/mfa/totp/enable"),
        ("POST", "/users/@me/mfa/webauthn/credentials"),
        ("POST", "/users/@me/mfa/webauthn/credentials/registration-options"),
        ("POST", "/users/@me/mfa/webauthn/migration"),
        ("POST", "/users/@me/mfa/webauthn/migration/registration-options"),
        ("POST", "/users/@me/mobile-devices"),
        ("POST", "/users/@me/mobile-devices/unregister"),
        ("POST", "/users/@me/passkey-bridge"),
        ("POST", "/users/@me/passkey-bridge/:ceremony_id/redeem"),
        ("POST", "/users/@me/password"),
        ("POST", "/users/@me/password-change/complete"),
        ("POST", "/users/@me/password-change/resend"),
        ("POST", "/users/@me/password-change/start"),
        ("POST", "/users/@me/password-change/verify"),
        ("POST", "/users/@me/premium/reset"),
        ("POST", "/users/@me/profile-badges/preview"),
        ("POST", "/users/@me/push/rotate"),
        ("POST", "/users/@me/push/subscribe"),
        ("POST", "/users/@me/recovery-kit"),
        ("POST", "/users/@me/relationships"),
        ("POST", "/users/@me/relationships/bulk-ignore"),
        ("POST", "/users/@me/relationships/:user_id"),
        ("POST", "/users/@me/saved-messages"),
        ("POST", "/users/@me/sso/link/start"),
        ("POST", "/users/@me/sudo/email-code"),
        ("POST", "/users/@me/sudo/webauthn/authentication-options"),
        ("POST", "/users/@me/terms-acceptance"),
        ("POST", "/users/@me/themes"),
        ("POST", "/voice/channels/:channel_id/entrance-sound"),
        ("POST", "/voice/p2p/connection-reports"),
        ("PUT", "/operator-pass/listing"),
        ("PUT", "/users/@me/date-of-birth"),
        ("PUT", "/users/@me/entrance-sound-selections"),
        ("PUT", "/users/@me/mfa/webauthn/two-factor"),
        ("PUT", "/users/@me/notes/:target_id"),
        ("PUT", "/users/@me/relationships/:user_id"),
        ("PUT", "/users/@me/settings/voice-activity-sharing"),
    };
}
