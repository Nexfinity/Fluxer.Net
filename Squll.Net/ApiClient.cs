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

    #region API
    public async Task<Message> SendMessage(ulong spaceId, Message message)
        => await MakeSqullApiRequest<Message, Message>(HttpMethod.Post, $"spaces/{spaceId}/messages", message, true);

    public async Task<SquadProperties> JoinSquad(string invite)
        => await MakeSqullApiRequest<SquadProperties>(HttpMethod.Post, $"invites/{invite}", true);

    public async Task LeaveSquad(ulong Id)
        => await MakeSqullApiRequest(HttpMethod.Delete, $"users/@me/squads/{Id}", true);

    public async Task<SquadProperties> GetSquad(ulong squadId)
        => await MakeSqullApiRequest<SquadProperties>(HttpMethod.Get, $"squads/{squadId}", true);
    #endregion
}
