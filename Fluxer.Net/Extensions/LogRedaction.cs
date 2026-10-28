using System.Text.RegularExpressions;

namespace Fluxer.Net;

internal static class LogRedaction
{
    private static readonly Regex SecretFields = new Regex(
        "\"(token|access_token|refresh_token|client_secret|password|e2ee_key)\"\\s*:\\s*\"[^\"]*\"",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static readonly Regex WebhookRoute = new Regex(
        "(/webhooks/\\d+/)[^/?\\s\"]+",
        RegexOptions.Compiled);

    public static string? Redact(string? text)
    {
        if (string.IsNullOrEmpty(text))
            return text;

        text = SecretFields.Replace(text, match => $"\"{match.Groups[1].Value}\":\"[redacted]\"");
        return WebhookRoute.Replace(text, "$1[redacted]");
    }
}
