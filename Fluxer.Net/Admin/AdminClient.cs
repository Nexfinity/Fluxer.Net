using Fluxer.Net.Rest;

namespace Fluxer.Net.Admin;

/// <summary>
/// A REST client that authenticates with a token. It does not connect to the gateway.
/// </summary>
public class FluxerAdminClient : FluxerBaseClient
{
    /// <summary>
    /// Creates an admin client for the given token.
    /// </summary>
    /// <param name="token">The token sent with each request.</param>
    /// <param name="config">The settings to use, with <see cref="FluxerConfig.RestSerilog"/> set. Defaults are used when null.</param>
    /// <remarks>
    /// Unlike <see cref="FluxerClient"/>, this client does not create a logger when <see cref="FluxerConfig.RestSerilog"/> is null.
    /// </remarks>
    public FluxerAdminClient(string token, FluxerConfig? config = null)
    {
        if (config == null)
            config = new FluxerConfig();

        Token = token;
        Config = config;
        Rest = new FluxerApiClient(this);
        Rest.Initialize();
    }

    /// <summary>
    /// Gets the user the token belongs to.
    /// </summary>
    /// <returns>The current user.</returns>
    public async Task<CurrentUser> GetCurrentUserAsync()
    {
        CurrentUserJson json = await Rest.SendRequestAsync<CurrentUserJson>(HttpMethod.Get, "/users/@me", true);
        return CurrentUser.Create(this, json);
    }
}
