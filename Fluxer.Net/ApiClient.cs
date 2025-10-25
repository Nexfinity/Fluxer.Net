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
using Fluxer.Net.Extensions;
using Fluxer.Net.Objects;
namespace Fluxer.Net;

public class ApiClient
{
    #region Declares
    public string Token { get; set; }
    public HttpClient HttpClient { get; set; }

    private readonly FluxerConfig _config;
#pragma warning disable CS0169
    private readonly Logger _logger;
#pragma warning restore CS0169
    #endregion

    #region Meta
    public ApiClient(string token, FluxerConfig config)
    {
        Token = token;
        _config = config;
        HttpClient = _config.HttpClient ?? new();
        Log.Logger = _config.Serilog ?? new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Console()
                .CreateLogger();
        Log.Information("Initialized Fluxer.Net api client ({AssemblyVersion}) (API {ApiVersion})", Assembly.GetExecutingAssembly().GetName().Version, _config.Version);
        Log.Verbose("Loaded with config {@Config}", _config);
    }

    public async Task<TResponse> MakeFluxerApiRequestRS<TResponse, TSend>(HttpMethod method, string route, TSend data, bool throwOnNonSuccess = false, bool authorize = true)
    {
        var rawContent = JsonConvert.SerializeObject(data, new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Ignore
        });
        Log.Verbose("Sending {@Data} to {Route}", rawContent, route);
        var req = new HttpRequestMessage()
        {
            Method = method,
            Content = new StringContent(rawContent, new MediaTypeHeaderValue("application/json")),
            RequestUri = new(_config.RealApiBaseUrl + route)
        };
        if (authorize)
            req.Headers.Add("Authorization", Token);
        var result = await HttpClient.SendAsync(req);

        Log.Debug("Made {Method} request to {Route}", method, route);
        var resp = await result.Content.ReadAsStringAsync();
        Log.Verbose("Received {Code}:{Result} from {Route}", result.StatusCode, resp, route);

        if (throwOnNonSuccess && !result.IsSuccessStatusCode)
            throw new FluxerApiException($"Fluxer returned a non-success code {result.StatusCode}", resp);

        return JsonConvert.DeserializeObject<TResponse>(resp);
    }

    public async Task<HttpStatusCode> MakeFluxerApiRequestS<TSend>(HttpMethod method, string route, TSend data, bool throwOnNonSuccess = false, bool authorize = true)
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
        if (authorize)
            req.Headers.Add("Authorization", Token);
        var result = await HttpClient.SendAsync(req);

        Log.Debug("Made {Method} request to {Route}", method, route);
        var resp = await result.Content.ReadAsStringAsync();
        Log.Verbose("Received {Code}:{Result} from {Route}", result.StatusCode, resp, route);

        if (throwOnNonSuccess && !result.IsSuccessStatusCode)
            throw new FluxerApiException($"Fluxer returned a non-success code {result.StatusCode}", resp);

        return result.StatusCode;
    }

    public async Task<TResponse> MakeFluxerApiRequestR<TResponse>(HttpMethod method, string route, bool throwOnNonSuccess = false, bool authorize = true)
    {
        var req = new HttpRequestMessage()
        {
            Method = method,
            RequestUri = new(_config.RealApiBaseUrl + route)
        };
        if (authorize)
            req.Headers.Add("Authorization", Token);
        var result = await HttpClient.SendAsync(req);

        Log.Debug("Made {Method} request to {Route}", method, route);
        var resp = await result.Content.ReadAsStringAsync();
        Log.Verbose("Received {Code}:{Result} from {Route}", result.StatusCode, resp, route);

        if (throwOnNonSuccess && !result.IsSuccessStatusCode)
            throw new FluxerApiException($"Fluxer returned a non-success code {result.StatusCode}", resp);

        return JsonConvert.DeserializeObject<TResponse>(resp);
    }

    public async Task<HttpStatusCode> MakeFluxerApiRequest(HttpMethod method, string route, bool throwOnNonSuccess = false, bool authorize = true)
    {
        var req = new HttpRequestMessage()
        {
            Method = method,
            RequestUri = new(_config.RealApiBaseUrl + route)
        };
        if (authorize)
            req.Headers.Add("Authorization", Token);
        var result = await HttpClient.SendAsync(req);

        Log.Debug("Made {Method} request to {Route} with response code {Code}", method, route, result.StatusCode);
        if (throwOnNonSuccess && !result.IsSuccessStatusCode)
            throw new FluxerApiException($"Fluxer returned a non-success code {result.StatusCode}", await result.Content.ReadAsStringAsync());

        return result.StatusCode;
    }
    #endregion

    #region Channels API

    public async Task<Message> PostChannelMessage(ulong channelId, Message message)
        => await MakeFluxerApiRequestRS<Message, Message>(HttpMethod.Post, $"channels/{channelId}/messages", message, true);

    public async Task PostChannelMessageAck(ulong channelId, ulong messageId, MessageAck details)
        => await MakeFluxerApiRequestS<MessageAck>(HttpMethod.Post, $"channels/{channelId}/messages/{messageId}/ack", details, true);

    public async Task DeleteChannel(ulong channelId)
        => await MakeFluxerApiRequest(HttpMethod.Delete, $"channels/{channelId}", true);

    public async Task DeleteChannelMessage(ulong channelId, ulong messageId)
        => await MakeFluxerApiRequest(HttpMethod.Delete, $"channels/{channelId}/messages/{messageId}", true);

    public async Task DeleteTyping(ulong channelId)
        => await MakeFluxerApiRequest(HttpMethod.Delete, $"channels/{channelId}/typing", true);

    public async Task<Channel> GetChannel(ulong channelId)
        => await MakeFluxerApiRequestR<Channel>(HttpMethod.Get, $"channels/{channelId}", true);

    public async Task<List<Invite>> GetChannelInvites(ulong channelId)
        => await MakeFluxerApiRequestR<List<Invite>>(HttpMethod.Get, $"channels/{channelId}/invites", true);

    public async Task<List<Message>> GetChannelMessages(ulong channelId)
        => await MakeFluxerApiRequestR<List<Message>>(HttpMethod.Get, $"channels/{channelId}/messages", true);

    public async Task<Message> GetChannelMessage(ulong channelId, ulong messageId)
        => await MakeFluxerApiRequestR<Message>(HttpMethod.Get, $"channels/{channelId}/messages/{messageId}", true);


    public async Task<Channel> PatchChannel(ulong channelId, Channel channel)
        => await MakeFluxerApiRequestRS<Channel, Channel>(HttpMethod.Patch, $"channels/{channelId}", channel, true);

    public async Task<Message> PatchChannelMessage(ulong channelId, ulong messageId, Message message)
        => await MakeFluxerApiRequestRS<Message, Message>(HttpMethod.Patch, $"channels/{channelId}/messages/{messageId}", message, true);

    public async Task PostInvite(ulong channelId)
        => await MakeFluxerApiRequest(HttpMethod.Post, $"channels/{channelId}/invites", true);

    public async Task PostTyping(ulong channelId)
        => await MakeFluxerApiRequest(HttpMethod.Post, $"channels/{channelId}/typing", true);

    #endregion

    #region Communities API

    public async Task<CommunityProperties> GetCommunity(ulong communityId)
        => await MakeFluxerApiRequestR<CommunityProperties>(HttpMethod.Get, $"communities/{communityId}", true);

    public async Task DeleteCommunity(ulong communityId)
        => await MakeFluxerApiRequest(HttpMethod.Delete, $"communities/{communityId}", true);

    public async Task DeleteCommunityUser(ulong communityId, ulong userId)
        => await MakeFluxerApiRequest(HttpMethod.Delete, $"communities/{communityId}/members/{userId}", true);

    public async Task<List<Invite>> GetCommunityInvites(ulong communityId)
        => await MakeFluxerApiRequestR<List<Invite>>(HttpMethod.Get, $"communities/{communityId}/invites", true);

    public async Task<List<User>> GetCommunityUsers(ulong communityId)
        => await MakeFluxerApiRequestR<List<User>>(HttpMethod.Get, $"communities/{communityId}/members", true);

    public async Task<List<Role>> GetCommunityRoles(ulong communityId)
        => await MakeFluxerApiRequestR<List<Role>>(HttpMethod.Get, $"communities/{communityId}/roles", true);

    public async Task<List<Channel>> GetCommunityChannels(ulong communityId)
        => await MakeFluxerApiRequestR<List<Channel>>(HttpMethod.Get, $"communities/{communityId}/channels", true);

    public async Task<CommunityProperties> PatchCommunity(ulong communityId, CommunityProperties community)
        => await MakeFluxerApiRequestRS<CommunityProperties, CommunityProperties>(HttpMethod.Patch, $"communities/{communityId}", community, true);

    public async Task<CommunityMember> PatchCommunityUser(ulong communityId, ulong userId, CommunityMember member)
        => await MakeFluxerApiRequestRS<CommunityMember, CommunityMember>(HttpMethod.Patch, $"communities/{communityId}/members/{userId}", member, true);

    public async Task<CommunityMember> PatchCommunitySelfUser(ulong communityId, CommunityMember member)
        => await MakeFluxerApiRequestRS<CommunityMember, CommunityMember>(HttpMethod.Patch, $"communities/{communityId}/members/@me", member, true);

    public async Task<CommunityProperties> PostCommunity()
        => await MakeFluxerApiRequestR<CommunityProperties>(HttpMethod.Post, $"communities", true);

    public async Task<Role> PostCommunityRole(ulong communityId)
        => await MakeFluxerApiRequestR<Role>(HttpMethod.Post, $"communities/{communityId}/roles", true);

    public async Task<Channel> PostCommunityChannel(ulong communityId)
        => await MakeFluxerApiRequestR<Channel>(HttpMethod.Post, $"communities/{communityId}/channels", true);

    public async Task<CommunityProperties> PostCommunityVanityUrl(ulong communityId, string vanityUrl)
        => await MakeFluxerApiRequestRS<CommunityProperties, string>(HttpMethod.Post, $"communities/{communityId}/vanity-url", "{code: \"" + vanityUrl + "\"}", true);

    #endregion

    #region Invites API

    public async Task<CommunityProperties> PostCommunity(string invite)
        => await MakeFluxerApiRequestR<CommunityProperties>(HttpMethod.Post, $"invites/{invite}", true);

    #endregion

    #region Users API

    public async Task DeleteCommunity(ulong communityId)
        => await MakeFluxerApiRequest(HttpMethod.Delete, $"users/@me/communities/{communityId}", true, true);

    public async Task<User> GetUser(ulong userId)
        => await MakeFluxerApiRequestR<User>(HttpMethod.Get, $"users/{userId}", true);

    public async Task<User> GetCurrentUser()
        => await MakeFluxerApiRequestR<User>(HttpMethod.Get, $"users/@me", true);

    public async Task<UserSettings> GetCurrentUserSettings()
        => await MakeFluxerApiRequestR<UserSettings>(HttpMethod.Get, $"users/@me/settings", true);

    public async Task<List<CommunityProperties>> GetCurrentUserCommunities()
        => await MakeFluxerApiRequestR<List<CommunityProperties>>(HttpMethod.Get, $"users/@me/communities", true);

    public async Task<User> PatchCurrentUser(User user)
        => await MakeFluxerApiRequestRS<User, User>(HttpMethod.Patch, $"users/@me", user, true);

    public async Task<UserProfile> PatchCurrentUserProfile(UserProfile profile)
        => await MakeFluxerApiRequestRS<UserProfile, UserProfile>(HttpMethod.Patch, $"users/@me/profile", profile, true);

    public async Task<LoginResponse> PostLogin(LoginRequest data)
        => await MakeFluxerApiRequestRS<LoginResponse, LoginRequest>(HttpMethod.Post, "auth/login", data, true);

    #endregion

    #region Tokens API
    public async Task PostTokenRevoke(TokenRevokeRequest data)
        => await MakeFluxerApiRequestS<TokenRevokeRequest>(HttpMethod.Post, "tokens/revoke", data, true);
    #endregion
}
