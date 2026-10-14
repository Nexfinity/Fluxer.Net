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
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    public FluxerOAuthClient(ulong clientId, string clientSecret, FluxerConfig? config = null)
    {
        if (clientId == 0)
            throw new ArgumentNullException("Missing client id.");

        if (string.IsNullOrEmpty(clientSecret))
            throw new ArgumentNullException("Missing client secret.");

        base.Id = clientId;
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

    /// <inheritdoc cref="FluxerApiClient.GetOAuthUserAsync(string, RequestOptions?)" />
    public Task<FluxerOAuthUser?> GetUserAsync(string accessToken, RequestOptions? options = null)
        => Rest.GetOAuthUserAsync(accessToken, options);

    /// <inheritdoc cref="FluxerApiClient.GetOAuthTokenAsync(string, RequestOptions?)" />
    public Task<FluxerOAuthToken?> GetTokenAsync(string accessToken, RequestOptions? options = null)
        => Rest.GetOAuthTokenAsync(accessToken, options);

    /// <inheritdoc cref="FluxerApiClient.GetOAuthGuildsAsync(string, RequestOptions?)" />
    public Task<IEnumerable<Guild>> GetGuildsAsync(string accessToken, RequestOptions? options = null)
        => Rest.GetOAuthGuildsAsync(accessToken, options);

    /// <inheritdoc cref="FluxerApiClient.GetOAuthConnectionsAsync(string, RequestOptions?)" />
    public Task<IEnumerable<UserConnection>> GetConnectionsAsync(string accessToken, RequestOptions? options = null)
        => Rest.GetOAuthConnectionsAsync(accessToken, options);

    /// <inheritdoc cref="FluxerApiClient.GetOAuthValidTokenAsync(ulong, string, string, RequestOptions?)" />
    public async Task<bool> CheckValidTokenAsync(string accessToken, RequestOptions? options = null)
    {
        try
        {
            FluxerOAuthValidToken token = await Rest.GetOAuthValidTokenAsync(ClientId, ClientSecret, accessToken, options);
            return token.IsActive;
        }
        catch
        {
            return false;
        }
    }

    /// <inheritdoc cref="FluxerApiClient.GetOAuthValidTokenAsync(ulong, string, string, RequestOptions?)" />
    public Task<FluxerOAuthValidToken?> GetValidTokenAsync(string accessToken, RequestOptions? options = null)
        => Rest.GetOAuthValidTokenAsync(ClientId, ClientSecret, accessToken, options);

    /// <inheritdoc cref="FluxerApiClient.ExchangeOAuthRefreshTokenAsync(ulong, string, string, RequestOptions?)" />
    public Task<FluxerOAuthRefreshToken> ExchangeRefreshTokenAsync(string refreshToken, RequestOptions? options = null)
        => Rest.ExchangeOAuthRefreshTokenAsync(ClientId, ClientSecret, refreshToken, options);

    /// <inheritdoc cref="FluxerApiClient.RevokeOAuthAccessTokenAsync(ulong, string, string, RequestOptions?)" />
    public Task RevokeAccessTokenAsync(string accessToken, RequestOptions? options = null)
        => Rest.RevokeOAuthAccessTokenAsync(ClientId, ClientSecret, accessToken, options);

    /// <inheritdoc cref="FluxerApiClient.RevokeOAuthRefreshTokenAsync(ulong, string, string, RequestOptions?)" />
    public Task RevokeRefreshTokenAsync(string refreshToken, RequestOptions? options = null)
        => Rest.RevokeOAuthRefreshTokenAsync(ClientId, ClientSecret, refreshToken, options);
}
