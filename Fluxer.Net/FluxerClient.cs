using Fluxer.Net.Extensions;
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

    internal static JsonSerializer _serializer { get; set; } = CreateGatewaySerializer();

    /// <summary>
    /// This will update your config urls to use the instance.
    /// </summary>
    /// <param name="apiUrl"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="Exception"></exception>
    public async Task LoginAsync(string apiUrl = null)
    {
        if (string.IsNullOrEmpty(apiUrl))
            apiUrl = Config.RealApiBaseUrl;

        if (!Uri.TryCreate(apiUrl, UriKind.Absolute, out Uri apiUri))
            throw new ArgumentNullException(nameof(apiUrl), "API url is invalid.");

        InstanceJson? instance = await Rest.InternalSendRequestAsync<InstanceJson>(HttpMethod.Get, new Uri(apiUri, $"/v{Config.Version}/.well-known/fluxer").AbsoluteUri, throwOnNonSuccess: true, authorize: false, useConfigUrl: false);
        if (instance == null)
            throw new Exception("Failed to get instance data.");

        Config.FluxerApiBaseUrl = $"{instance.Endpoints.ApiPublic}/v{Config.Version}";
        Config.FluxerGatewayUrl = $"{instance.Endpoints.Gateway}/?v=1&encoding=json";
        Config.MediaUrl = instance.Endpoints.Media;
        Config.StaticUrl = instance.Endpoints.Static;
        Config.AdminUrl = instance.Endpoints.Admin;
        Config.InviteUrl = instance.Endpoints.Invite;
        Config.GiftUrl = instance.Endpoints.Gift;
    }

    internal static JsonSerializer CreateGatewaySerializer()
    {
        var serializer = new JsonSerializer
        {
            NullValueHandling = NullValueHandling.Ignore
        };
        serializer.Converters.Add(new StringUInt64Converter());
        serializer.Converters.Add(new GuildPermissionsConverter());
        serializer.Converters.Add(new ChannelPermissionsConverter());
        return serializer;
    }

    // Used by both api and gateway
    internal static JsonSerializerSettings _serializerSettings { get; set; } = CreateRestSerializer();

    internal static JsonSerializerSettings CreateRestSerializer()
    {
        var serializer = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore
        };
        serializer.Converters.Add(new GuildPermissionsConverter());
        serializer.Converters.Add(new ChannelPermissionsConverter());
        return serializer;
    }

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
