using Serilog;

namespace Fluxer.Net.OAuth;

public class FluxerOAuthClient : BaseClient
{
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

        base.Rest = new ApiClient(this);
    }

    public FluxerConfig Config { get; }

    public ulong ClientId => base.Id;
    public string ClientSecret => base.Token;

    public new ApiClient Rest => base.Rest;

    public Task<User> GetOAuthUser(string accessToken)
        => Rest.GetOAuthUserAsync(accessToken);

    public Task<OAuthToken> GetOAuthTokenAsync(string accessToken)
        => Rest.GetOAuthTokenAsync(accessToken);

    public Task<List<Guild>> GetOAuthGuildsAsync(string accessToken)
        => Rest.GetOAuthGuildsAsync(accessToken);

    public Task<List<UserConnection>> GetOAuthConnectionsAsync(string accessToken)
        => Rest.GetOAuthConnectionsAsync(accessToken);

    public async Task<bool> CheckValidTokenAsync(string accessToken)
    {
        try
        {
            var token = await Rest.GetOAuthValidTokenAsync(ClientId, ClientSecret, accessToken);
            return token.IsActive;
        }
        catch
        {
            return false;
        }
    }

    public Task<OAuthValidToken> GetValidTokenAsync(string accessToken)
        => Rest.GetOAuthValidTokenAsync(ClientId, ClientSecret, accessToken);

    public Task<OAuthRefreshToken> GetRefreshTokenAsync(string refreshToken)
        => Rest.GetOAuthRefreshTokenAsync(ClientId, ClientSecret, refreshToken);

    public Task RevokeAccessTokenAsync(string accessToken)
        => Rest.RevokeAccessTokenAsync(ClientId, ClientSecret, accessToken);

    public Task RevokeRefreshTokenAsync(string refreshToken)
        => Rest.RevokeRefreshTokenAsync(ClientId, ClientSecret, refreshToken);
}
