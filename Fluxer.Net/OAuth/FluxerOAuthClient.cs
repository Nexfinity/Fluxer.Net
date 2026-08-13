using Fluxer.Net.Rest;
using Serilog;

namespace Fluxer.Net.OAuth;

/// <summary>
/// OAuth client used for Fluxer.
/// </summary>
public class FluxerOAuthClient : FluxerBaseClient
{
    /// <summary>
    /// Create an OAuth client with id and secret then use access or refresh tokens.
    /// </summary>
    /// <param name="clientId"></param>
    /// <param name="clientSecret"></param>
    /// <param name="config"></param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    public FluxerOAuthClient(string clientId, string clientSecret, FluxerConfig? config = null)
    {
        if (string.IsNullOrEmpty(clientId))
            throw new ArgumentNullException("Missing client id.");

        if (string.IsNullOrEmpty(clientSecret))
            throw new ArgumentNullException("Missing client secret.");

        if (!ulong.TryParse(clientId, out ulong id))
            throw new ArgumentException("Invalid client id.");


        base.Id = id;
        base.Token = clientSecret;
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

        base.Rest = new FluxerApiClient(this);
    }

    /// <summary>
    /// Client id for the OAuth app.
    /// </summary>
    public ulong ClientId => base.Id;

    /// <summary>
    /// Client secret for the OAuth app.
    /// </summary>
    public string ClientSecret => base.Token;

    /// <inheritdoc cref="FluxerBaseClient.Rest" />
    public new FluxerApiClient Rest => base.Rest;

    /// <inheritdoc cref="FluxerApiClient.GetOAuthUserAsync(string)" />
    public Task<FluxerOAuthUser> GetOAuthUser(string accessToken)
        => Rest.GetOAuthUserAsync(accessToken);

    /// <inheritdoc cref="FluxerApiClient.GetOAuthTokenAsync(string)" />
    public Task<FluxerOAuthToken> GetOAuthTokenAsync(string accessToken)
        => Rest.GetOAuthTokenAsync(accessToken);

    /// <inheritdoc cref="FluxerApiClient.GetOAuthGuildsAsync(string)" />
    public Task<IEnumerable<Guild>> GetOAuthGuildsAsync(string accessToken)
        => Rest.GetOAuthGuildsAsync(accessToken);

    /// <inheritdoc cref="FluxerApiClient.GetOAuthConnectionsAsync(string)" />
    public Task<IEnumerable<UserConnection>> GetOAuthConnectionsAsync(string accessToken)
        => Rest.GetOAuthConnectionsAsync(accessToken);

    /// <inheritdoc cref="FluxerApiClient.GetOAuthValidTokenAsync(ulong, string, string)" />
    public async Task<bool> CheckValidTokenAsync(string accessToken)
    {
        try
        {
            FluxerOAuthValidTokenJson token = await Rest.GetOAuthValidTokenAsync(ClientId, ClientSecret, accessToken);
            return token.IsActive;
        }
        catch
        {
            return false;
        }
    }

    /// <inheritdoc cref="FluxerApiClient.GetOAuthValidTokenAsync(ulong, string, string)" />
    public Task<FluxerOAuthValidTokenJson> GetValidTokenAsync(string accessToken)
        => Rest.GetOAuthValidTokenAsync(ClientId, ClientSecret, accessToken);

    /// <inheritdoc cref="FluxerApiClient.GetOAuthRefreshTokenAsync(ulong, string, string)" />
    public Task<FluxerOAuthRefreshTokenJson> GetRefreshTokenAsync(string refreshToken)
        => Rest.GetOAuthRefreshTokenAsync(ClientId, ClientSecret, refreshToken);

    /// <inheritdoc cref="FluxerApiClient.RevokeOAuthAccessTokenAsync(ulong, string, string)" />
    public Task RevokeAccessTokenAsync(string accessToken)
        => Rest.RevokeOAuthAccessTokenAsync(ClientId, ClientSecret, accessToken);

    /// <inheritdoc cref="FluxerApiClient.RevokeOAuthRefreshTokenAsync(ulong, string, string)" />
    public Task RevokeRefreshTokenAsync(string refreshToken)
        => Rest.RevokeOAuthRefreshTokenAsync(ClientId, ClientSecret, refreshToken);
}
