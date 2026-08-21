using Fluxer.Net.Extensions;
using Fluxer.Net.Gateway;
using Fluxer.Net.Rest;
using Newtonsoft.Json;
using Serilog;

namespace Fluxer.Net;

/// <summary>
/// Base client used for all Fluxer http requests.
/// </summary>
public abstract class FluxerBaseClient
{
    internal ulong Id { get; set; }

    /// <summary>
    /// Token for the current user.
    /// </summary>
    internal string Token { get; set; }

    /// <summary>
    /// Http client for Fluxer with requests.
    /// </summary>
    internal FluxerApiClient Rest { get; set; }

    /// <summary>
    /// Client options and settings to configure.
    /// </summary>
    public FluxerConfig Config { get; internal set; }
}

/// <summary>
/// Client used for connecting to the Fluxer API and Gateway.
/// </summary>
public class FluxerClient : FluxerBaseClient
{
    /// <summary>
    /// Create a Fluxer client.
    /// </summary>
    /// <param name="token"></param>
    /// <param name="config"></param>
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
        base.Rest = new FluxerApiClient(this);
        Gateway = new FluxerGatewayClient(this);
    }

    /// <summary>
    /// Use canary features for the API.
    /// </summary>
    /// <returns></returns>
    public FluxerClient UseCanary()
    {
        Config.ApiBaseUrl = "https://api.canary.fluxer.app/v{v}";
        return this;
    }

    /// <inheritdoc cref="FluxerBaseClient.Token" />
    public new string Token => base.Token;

    /// <summary>
    /// Returns the raw token without any "Bot " prefix, for use in gateway IDENTIFY/RESUME packets.
    /// The gateway protocol expects only the raw token, not the HTTP authorization format.
    /// </summary>
    internal string RawToken => Token.StartsWith("Bot ", StringComparison.OrdinalIgnoreCase)
        ? Token[4..]
        : Token;


    /// <summary>
    /// Gateway client for Fluxer with events.
    /// </summary>
    public FluxerGatewayClient Gateway { get; }

    /// <inheritdoc cref="FluxerBaseClient.Rest" />
    public new FluxerApiClient Rest => base.Rest;

    /// <summary>
    /// This will update your config urls to use the instance.
    /// </summary>
    /// <param name="apiUrl"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="Exception"></exception>
    public async Task<Instance> LoginAsync(string apiUrl = null)
    {
        if (string.IsNullOrEmpty(apiUrl))
            apiUrl = Config.RealApiBaseUrl;

        if (!Uri.TryCreate(apiUrl, UriKind.Absolute, out Uri apiUri))
            throw new ArgumentException(nameof(apiUrl), "API url is invalid.");

        InstanceJson? instance = await Rest.InternalSendRequestAsync<InstanceJson>(HttpMethod.Get, new Uri(apiUri, $"/v{Config.Version}/.well-known/fluxer").AbsoluteUri, throwOnNonSuccess: true, authorize: false, useConfigUrl: false);
        if (instance == null)
            throw new Exception("Failed to get instance data.");

        Config.ApiBaseUrl = $"{instance.Endpoints.ApiPublic}/v{Config.Version}";
        Config.GatewayUrl = $"{instance.Endpoints.Gateway}/?v=1&encoding=json";
        Config.MediaUrl = instance.Endpoints.Media;
        Config.StaticUrl = instance.Endpoints.Static;
        Config.AdminUrl = instance.Endpoints.Admin;
        Config.InviteUrl = instance.Endpoints.Invite;
        Config.GiftUrl = instance.Endpoints.Gift;
        Config.WebUrl = instance.Endpoints.WebApp;

        return Instance.Create(this, instance);
    }

    /// <summary>
    /// Start the gateway session to recieve events.
    /// </summary>
    /// <returns></returns>
    public Task StartAsync() => Gateway.ConnectAsync();

    internal static JsonSerializer _gatewaySerializer { get; set; } = CreateGatewaySerializer();

    internal static JsonSerializer CreateGatewaySerializer()
    {
        var serializer = new JsonSerializer
        {
            ContractResolver = new FluxerContractResolver(),
            NullValueHandling = NullValueHandling.Ignore
        };
        serializer.Converters.Add(new StringUInt64Converter());
        serializer.Converters.Add(new GuildPermissionsConverter());
        serializer.Converters.Add(new ChannelPermissionsConverter());
        return serializer;
    }


    internal static JsonSerializerSettings _restSerializer { get; set; } = CreateRestSerializer();

    internal static JsonSerializerSettings CreateRestSerializer()
    {
        var serializer = new JsonSerializerSettings
        {
            ContractResolver = new FluxerContractResolver(),
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
            throw new ArgumentNullException(
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
