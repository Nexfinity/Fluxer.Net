using Fluxer.Net.Rest;

namespace Fluxer.Net.Admin;

public class FluxerAdminClient : FluxerBaseClient
{
    public FluxerAdminClient(string token, FluxerConfig? config = null)
    {
        if (config == null)
            config = new FluxerConfig();

        Token = token;
        Config = config;
        Rest = new FluxerApiClient(this);
        Rest.Initialize();
    }

    public async Task<CurrentUser> GetCurrentUserAsync()
    {
        CurrentUserJson json = await Rest.SendRequestAsync<CurrentUserJson>(HttpMethod.Get, "/users/@me", true);
        return CurrentUser.Create(this, json);
    }
}
