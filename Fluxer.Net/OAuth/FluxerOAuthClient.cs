using Serilog;

namespace Fluxer.Net.OAuth;

public class FluxerOAuthClient : FluxerBaseClient
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

    public ulong ClientId => base.Id;
    public string ClientSecret => base.Token;

    public new ApiClient Rest => base.Rest;

    public Task<FluxerOAuthUser> GetOAuthUser(string accessToken)
        => Rest.GetOAuthUserAsync(accessToken);

    public Task<FluxerOAuthToken> GetOAuthTokenAsync(string accessToken)
        => Rest.GetOAuthTokenAsync(accessToken);

    public Task<IEnumerable<Guild>> GetOAuthGuildsAsync(string accessToken)
        => Rest.GetOAuthGuildsAsync(accessToken);

    public Task<IEnumerable<UserConnection>> GetOAuthConnectionsAsync(string accessToken)
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

    public Task<FluxerOAuthValidTokenJson> GetValidTokenAsync(string accessToken)
        => Rest.GetOAuthValidTokenAsync(ClientId, ClientSecret, accessToken);

    public Task<FluxerOAuthRefreshTokenJson> GetRefreshTokenAsync(string refreshToken)
        => Rest.GetOAuthRefreshTokenAsync(ClientId, ClientSecret, refreshToken);

    public Task RevokeAccessTokenAsync(string accessToken)
        => Rest.RevokeAccessTokenAsync(ClientId, ClientSecret, accessToken);

    public Task RevokeRefreshTokenAsync(string refreshToken)
        => Rest.RevokeRefreshTokenAsync(ClientId, ClientSecret, refreshToken);
}
