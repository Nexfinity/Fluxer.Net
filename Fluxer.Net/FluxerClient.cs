using Newtonsoft.Json;
using Serilog;

namespace Fluxer.Net;

/// <summary>
/// Client used for connecting to the Fluxer API and Gateway.
/// </summary>
public class FluxerClient
{
    public FluxerClient(string token, FluxerConfig? config = null)
    {
        // Load token
        ValidateToken(token);
        Token = token;
        if (config == null)
            config = new FluxerConfig();
        Config = config;

        // Load logger
        if (config.Serilog == null)
        {
            Config.Serilog = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Console().CreateLogger();
        }

        Config.Serilog.Verbose("Loaded with config {@Config}", Config);

        // Set clients with reference to fluxer client
        Api = new ApiClient(this);
        Gateway = new GatewayClient(this);
    }

    public string Token { get; }

    public FluxerConfig Config { get; }

    /// <summary>
    /// Returns the raw token without any "Bot " prefix, for use in gateway IDENTIFY/RESUME packets.
    /// The gateway protocol expects only the raw token, not the HTTP authorization format.
    /// </summary>
    internal string RawToken => Token.StartsWith("Bot ", StringComparison.OrdinalIgnoreCase)
        ? Token[4..]
        : Token;

    public ApiClient Api { get; }

    public GatewayClient Gateway { get; }

    // Used by both api and gateway
    internal static JsonSerializerSettings _serializerSettings = new JsonSerializerSettings()
    {
        NullValueHandling = NullValueHandling.Ignore
    };

    /// <summary>
    /// Validates that the token has a recognized prefix for the Fluxer API.
    /// </summary>
    /// <param name="token">The token to validate.</param>
    /// <exception cref="ArgumentException">
    /// Thrown when the token is null, empty, or does not begin with a valid prefix.
    /// Bot tokens must start with <c>Bot </c> (including the trailing space).
    /// User tokens must start with <c>flx_</c>.
    /// </exception>
    public static void ValidateToken(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            throw new ArgumentException(
                "Token must not be null or empty.", nameof(token));

        if (!token.StartsWith("Bot ", StringComparison.Ordinal) &&
            !token.StartsWith("flx_", StringComparison.Ordinal))
            throw new ArgumentException(
                $"Invalid token format. Bot tokens must be prefixed with 'Bot ' (e.g. 'Bot <token>') " +
                $"and user tokens must be prefixed with 'flx_'. " +
                $"Received token starting with: '{(token.Length > 8 ? token[..8] : token)}...'",
                nameof(token));
    }
}
