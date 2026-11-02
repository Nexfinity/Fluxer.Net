using Fluxer.Net.Extensions;
using Fluxer.Net.Gateway;
using Fluxer.Net.Rest;
using Newtonsoft.Json;
using Serilog;

namespace Fluxer.Net;

/// <summary>
/// Base client for the Fluxer client types, holding the token, the REST client and the config.
/// </summary>
public abstract class FluxerBaseClient
{
    internal ulong Id { get; set; }

    /// <summary>
    /// Token for the current user.
    /// </summary>
    internal string Token { get; set; }

    /// <summary>
    /// The REST client used for requests to the Fluxer API.
    /// </summary>
    public FluxerApiClient Rest { get; internal set; }

    /// <summary>
    /// The settings the client was created with.
    /// </summary>
    public FluxerConfig Config { get; internal set; }

    /// <summary>
    /// The voice provider that <see cref="IVoiceChannel.ConnectAsync"/> joins through, or null when voice is not set up.
    /// Creating a FluxerVoiceClient from Fluxer.Net.Voice sets it.
    /// </summary>
    public IVoiceProvider? Voice { get; set; }

    internal IVoiceProvider RequireVoice() => Voice ?? throw new InvalidOperationException(
        "Voice is not set up. Add the Fluxer.Net.Voice package and create a FluxerVoiceClient for this client.");
}

/// <summary>
/// Client used for connecting to the Fluxer API and Gateway.
/// </summary>
public class FluxerClient : FluxerBaseClient
{
    /// <summary>
    /// Creates a Fluxer client.
    /// </summary>
    /// <param name="token">The bot token, prefixed with <c>Bot </c>, or a user token prefixed with <c>flx_</c>.</param>
    /// <param name="config">The settings to use. Defaults are used when null.</param>
    /// <exception cref="ArgumentNullException">The token is null, empty or whitespace.</exception>
    /// <exception cref="ArgumentException">The token has neither the bot nor the user prefix.</exception>
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
    /// Points the REST API at the canary host.
    /// </summary>
    /// <returns>This client.</returns>
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
    /// The gateway client that receives events for this client.
    /// </summary>
    public FluxerGatewayClient Gateway { get; }

    /// <inheritdoc cref="FluxerBaseClient.Rest" />
    public new FluxerApiClient Rest => base.Rest;

    /// <summary>
    /// Reads the well-known endpoints of an instance and points the config URLs at them.
    /// </summary>
    /// <param name="hostUrl">The instance's API host. Defaults to the configured API URL without its version path when null or empty.</param>
    /// <returns>The instance the client is now pointed at.</returns>
    /// <exception cref="ArgumentException">The host URL is not an absolute URL.</exception>
    /// <exception cref="Exception">The instance returned no data.</exception>
    public async Task<Instance> LoginAsync(string hostUrl = null)
    {
        if (string.IsNullOrEmpty(hostUrl))
            hostUrl = Config.RealApiBaseUrl.Replace("v" + Config.Version, null);

        if (!Uri.TryCreate(hostUrl, UriKind.Absolute, out Uri apiUri))
            throw new ArgumentException(nameof(hostUrl), "API url is invalid.");

        string route = (apiUri + $".well-known/fluxer");

        InstanceJson? instance = await Rest.InternalSendRequestAsync<InstanceJson>(HttpMethod.Get, route, throwOnNonSuccess: true, authorize: false, useConfigUrl: false);
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
    /// Starts the gateway session that receives events.
    /// </summary>
    public Task StartAsync() => Gateway.ConnectAsync();

    internal static JsonSerializer _gatewaySerializer { get; set; } = CreateGatewaySerializer();

    internal static JsonSerializer CreateGatewaySerializer()
    {
        JsonSerializer serializer = new JsonSerializer
        {
            ContractResolver = new FluxerContractResolver(),
            NullValueHandling = NullValueHandling.Ignore
        };
        serializer.Converters.Add(new StringUInt64Converter());
        serializer.Converters.Add(new GuildPermissionsConverter());
        serializer.Converters.Add(new ChannelPermissionsConverter());
        serializer.Converters.Add(new UtcDateTimeOffsetConverter());
        return serializer;
    }


    internal static JsonSerializerSettings _restSerializer { get; set; } = CreateRestSerializer();

    internal static JsonSerializerSettings CreateRestSerializer()
    {
        JsonSerializerSettings serializer = new JsonSerializerSettings
        {
            ContractResolver = new FluxerContractResolver(),
            NullValueHandling = NullValueHandling.Ignore
        };
        serializer.Converters.Add(new GuildPermissionsConverter());
        serializer.Converters.Add(new ChannelPermissionsConverter());
        serializer.Converters.Add(new UtcDateTimeOffsetConverter());
        return serializer;
    }

    /// <summary>
    /// Checks that the token has a recognized prefix for the Fluxer API.
    /// </summary>
    /// <param name="token">The token to check.</param>
    /// <exception cref="ArgumentNullException">The token is null, empty or whitespace.</exception>
    /// <exception cref="ArgumentException">
    /// The token does not begin with <c>Bot </c> (including the trailing space) or <c>flx_</c>.
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
