using Fluxer.Net.Rest;
using Serilog;

namespace Fluxer.Net.OAuth;

/// <summary>
/// A client for the Fluxer OAuth API. It reads a user's profile, guilds and connections and manages their tokens.
/// </summary>
public class FluxerOAuthClient : FluxerBaseClient
{
    /// <summary>
    /// Creates an OAuth client for an app's client id and secret.
    /// </summary>
    /// <param name="clientId">The client id of the OAuth app.</param>
    /// <param name="clientSecret">The client secret of the OAuth app.</param>
    /// <param name="config">The settings to use. Defaults are used when null.</param>
    /// <exception cref="ArgumentNullException">The client id is 0, or the client secret is null or empty.</exception>
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
    /// The client id of the OAuth app.
    /// </summary>
    public ulong ClientId => base.Id;

    /// <summary>
    /// The client secret of the OAuth app.
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

    /// <summary>
    /// Checks whether an access token is active.
    /// </summary>
    /// <param name="accessToken">The access token to check.</param>
    /// <param name="options">The request options.</param>
    /// <returns>Whether the token is active. False when the check fails for any reason.</returns>
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
