using Newtonsoft.Json;
using Serilog;

namespace Fluxer.Net;

public abstract class FluxerBaseClient
{
    internal ulong Id { get; set; }
    internal string Token { get; set; }
    internal ApiClient Rest { get; set; }
    public FluxerConfig Config { get; internal set; }
}

/// <summary>
/// Client used for connecting to the Fluxer API and Gateway.
/// </summary>
public class FluxerClient : FluxerBaseClient
{
    public FluxerClient(string token, FluxerConfig? config = null)
    {
        // Load token
        ValidateToken(token);
        base.Token = token;
        if (config == null)
            config = new FluxerConfig();
        Config = config;

        // Load logger
        if (config.RestSerilog == null)
        {
            Config.RestSerilog = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Console().CreateLogger();
        }

        Config.RestSerilog.Verbose("Loaded with config {@Config}", Config);

        if (config.GatewaySerilog == null)
        {
            Config.GatewaySerilog = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Console().CreateLogger();
        }

        // Set clients with reference to fluxer client
        base.Rest = new ApiClient(this);
        Gateway = new GatewayClient(this);
    }

    public new string Token => base.Token;

    /// <summary>
    /// Returns the raw token without any "Bot " prefix, for use in gateway IDENTIFY/RESUME packets.
    /// The gateway protocol expects only the raw token, not the HTTP authorization format.
    /// </summary>
    internal string RawToken => Token.StartsWith("Bot ", StringComparison.OrdinalIgnoreCase)
        ? Token[4..]
        : Token;



    public GatewayClient Gateway { get; }

    public new ApiClient Rest => base.Rest;

    internal static JsonSerializer _serializer = new JsonSerializer
    {
        NullValueHandling = NullValueHandling.Ignore,
    };

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
