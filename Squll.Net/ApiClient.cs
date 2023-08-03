using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Serilog;
using Serilog.Core;
using Squll.Net.Extensions;
using Squll.Net.Objects;
namespace Squll.Net;

public class ApiClient
{
    #region Declares
    public string Token { get; set; }
    public HttpClient HttpClient { get; set; }
    
    private readonly SqullConfig _config;
    private readonly Logger _logger;
    #endregion

    #region Meta
    public ApiClient(string token, SqullConfig config)
    {
        Token = token;
        _config = config;
        HttpClient = _config.HttpClient ?? new();
        Log.Logger = _config.Serilog ??  new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Console()
                .CreateLogger();
        Log.Information("Initialized Squll.Net api client ({AssemblyVersion}) (API {ApiVersion})", Assembly.GetExecutingAssembly().GetName().Version, _config.Version);
        Log.Verbose("Loaded with config {@Config}", _config);
    }

    public async Task<TResponse> MakeSqullApiRequest<TResponse, TSend>(HttpMethod method, string route, TSend data, bool throwOnNonSuccess = false)
    {
        Log.Verbose("Sending {@Data} to {Route}", data, route);
        var req = new HttpRequestMessage()
        {
            Method = method,
            Content = new StringContent(JsonConvert.SerializeObject(data, new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore
            }), new MediaTypeHeaderValue("application/json")),
            RequestUri = new(_config.RealApiBaseUrl + route)
        };
        req.Headers.Add("Authorization", Token);
        var result = await HttpClient.SendAsync(req);

        Log.Debug("Made {Method} request to {Route}", method, route);
        var resp = await result.Content.ReadAsStringAsync();
        Log.Verbose("Received {Code}:{Result} from {Route}", result.StatusCode, resp, route);

        if (throwOnNonSuccess && !result.IsSuccessStatusCode)
            throw new SqullApiException($"Squll returned a non-success code {result.StatusCode}", resp);

        return JsonConvert.DeserializeObject<TResponse>(resp);
    }

    public async Task<TResponse> MakeSqullApiRequest<TResponse>(HttpMethod method, string route, bool throwOnNonSuccess = false)
    {
        var req = new HttpRequestMessage()
        {
            Method = method,
            RequestUri = new(_config.RealApiBaseUrl + route)
        };
        req.Headers.Add("Authorization", Token);
        var result = await HttpClient.SendAsync(req);

        Log.Debug("Made {Method} request to {Route}", method, route);
        var resp = await result.Content.ReadAsStringAsync();
        Log.Verbose("Received {Code}:{Result} from {Route}", result.StatusCode, resp, route);

        if (throwOnNonSuccess && !result.IsSuccessStatusCode)
            throw new SqullApiException($"Squll returned a non-success code {result.StatusCode}", resp);

        return JsonConvert.DeserializeObject<TResponse>(resp);
    }

    public async Task<HttpStatusCode> MakeSqullApiRequest(HttpMethod method, string route, bool throwOnNonSuccess = false)
    {
        var req = new HttpRequestMessage()
        {
            Method = method,
            RequestUri = new(_config.RealApiBaseUrl + route)
        };
        req.Headers.Add("Authorization", Token);
        var result = await HttpClient.SendAsync(req);

        Log.Debug("Made {Method} request to {Route} with response code {Code}", method, route, result.StatusCode);
        if (throwOnNonSuccess && !result.IsSuccessStatusCode)
            throw new SqullApiException($"Squll returned a non-success code {result.StatusCode}", await result.Content.ReadAsStringAsync());

        return result.StatusCode;
    }
    #endregion

    #region Spaces API
    
    public async Task<Message> SendMessage(ulong spaceId, Message message)
        => await MakeSqullApiRequest<Message, Message>(HttpMethod.Post, $"spaces/{spaceId}/messages", message, true);
    
    public async Task DeleteSpace(ulong spaceId)
	    => await MakeSqullApiRequest(HttpMethod.Delete, $"spaces/{spaceId}", true);
    
    public async Task DeleteMessage(ulong spaceId, ulong messageId)
	    => await MakeSqullApiRequest(HttpMethod.Delete, $"spaces/{spaceId}/messages/{messageId}", true);
    
    public async Task StopTyping(ulong spaceId)
	    => await MakeSqullApiRequest(HttpMethod.Delete, $"spaces/{spaceId}/typing", true);
    
	public async Task<Space> GetSpace(ulong spaceId)
		=> await MakeSqullApiRequest<Space>(HttpMethod.Get, $"spaces/{spaceId}", true);

    public async Task<List<Invite>> GetSpaceInvites(ulong spaceId)
	    => await MakeSqullApiRequest<List<Invite>>(HttpMethod.Get, $"spaces/{spaceId}/invites", true);

	public async Task<List<Message>> GetSpaceMessages(ulong spaceId)
		=> await MakeSqullApiRequest<List<Message>>(HttpMethod.Get, $"spaces/{spaceId}/messages", true);

	public async Task<Message> GetSpaceMessage(ulong spaceId, ulong messageId)
		=> await MakeSqullApiRequest<Message>(HttpMethod.Get, $"spaces/{spaceId}/messages/{messageId}", true);
	
    //PATCH /v1/spaces/{spaceId}
    //PATCH /v1/spaces/{spaceId}/messages/{message_id}

    public async Task CreateInvite(ulong spaceId)
	    => await MakeSqullApiRequest(HttpMethod.Post, $"spaces/{spaceId}/invites", true);

    public async Task StartTyping(ulong spaceId)
	    => await MakeSqullApiRequest(HttpMethod.Post, $"spaces/{spaceId}/typing", true);

    #endregion

    #region Squads API

    public async Task<SquadProperties> GetSquad(ulong squadId)
	    => await MakeSqullApiRequest<SquadProperties>(HttpMethod.Get, $"squads/{squadId}", true);
    
    public async Task DeleteSquad(ulong squadId)
	    => await MakeSqullApiRequest(HttpMethod.Delete, $"squads/{squadId}", true);
    
    public async Task KickUser(ulong squadId, ulong userId)
	    => await MakeSqullApiRequest(HttpMethod.Delete, $"squads/{squadId}/members/{userId}", true);
    
    public async Task<List<Invite>> GetSquadInvites(ulong squadId)
	    => await MakeSqullApiRequest<List<Invite>>(HttpMethod.Get, $"squads/{squadId}/invites", true);
    
	public async Task<List<User>> GetSquadUsers(ulong squadId)
		=> await MakeSqullApiRequest<List<User>>(HttpMethod.Get, $"squads/{squadId}/members", true);
	
	public async Task<User> GetSquadUser(ulong squadId, ulong userId)
		=> await MakeSqullApiRequest<User>(HttpMethod.Get, $"squads/{squadId}/members/{userId}", true);
	
    public async Task<List<Role>> GetSquadRoles(ulong squadId)
	    => await MakeSqullApiRequest<List<Role>>(HttpMethod.Get, $"squads/{squadId}/roles", true);
    
	public async Task<List<Space>> GetSquadSpaces(ulong squadId)
		=> await MakeSqullApiRequest<List<Space>>(HttpMethod.Get, $"squads/{squadId}/spaces", true);
	
	//PATCH /v1/squads/{squadId}
    //PATCH /v1/squads/{squadId}/members/{userId}
    //PATCH /v1/squads/{squadId}/members/@me
    public async Task<SquadProperties> CreateSquad()
		=> await MakeSqullApiRequest<SquadProperties>(HttpMethod.Post, $"squads", true);
    
	public async Task<Role> CreateSquadRole(ulong squadId)
		=> await MakeSqullApiRequest<Role>(HttpMethod.Post, $"squads/{squadId}/roles", true);

	public async Task<Space> CreateSquadSpace(ulong squadId)
		=> await MakeSqullApiRequest<Space>(HttpMethod.Post, $"squads/{squadId}/spaces", true);
	//POST /v1/squads/{squadId}/vanity-url

    #endregion

    #region Invites API

    public async Task<SquadProperties> JoinSquad(string invite)
	    => await MakeSqullApiRequest<SquadProperties>(HttpMethod.Post, $"invites/{invite}", true);

    #endregion

    #region Users API

    public async Task LeaveSquad(ulong squadId)
	    => await MakeSqullApiRequest(HttpMethod.Delete, $"users/@me/squads/{squadId}", true);
    
    public async Task<User> GetUser(ulong userId)
	    => await MakeSqullApiRequest<User>(HttpMethod.Get, $"users/{userId}", true);
    
    public async Task<User> GetCurrentUser()
	    => await MakeSqullApiRequest<User>(HttpMethod.Get, $"users/@me", true);
    
    public async Task<UserSettings> GetCurrentUserSettings()
	    => await MakeSqullApiRequest<UserSettings>(HttpMethod.Get, $"users/@me/settings", true);
    
    public async Task<List<SquadProperties>> GetCurrentUserSquads()
	    => await MakeSqullApiRequest<List<SquadProperties>>(HttpMethod.Get, $"users/@me/squads", true); 
    
    //PATCH /v1/users/@me/email
	//PATCH /v1/users/@me/password
	//PATCH /v1/users/@me/profile
	//PATCH /v1/users/@me/settings
	//PATCH /v1/users/@me/username
	
    #endregion
}
