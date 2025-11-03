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
using Fluxer.Net.Objects.Models;
using Fluxer.Net.Objects.Models;

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
        Log.Verbose("Sending {@Enums} to {Route}", rawContent, route);
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
        Log.Verbose("Sending {@Enums} to {Route}", data, route);
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

    #region Guilds API

    public async Task<GuildProperties> GetGuild(ulong guildId)
        => await MakeFluxerApiRequestR<GuildProperties>(HttpMethod.Get, $"guilds/{guildId}", true);

    public async Task DeleteGuild(ulong guildId)
        => await MakeFluxerApiRequest(HttpMethod.Delete, $"guilds/{guildId}", true);

    public async Task DeleteGuildUser(ulong guildId, ulong userId)
        => await MakeFluxerApiRequest(HttpMethod.Delete, $"guilds/{guildId}/members/{userId}", true);

    public async Task<List<Invite>> GetGuildInvites(ulong guildId)
        => await MakeFluxerApiRequestR<List<Invite>>(HttpMethod.Get, $"guilds/{guildId}/invites", true);

    public async Task<List<User>> GetGuildUsers(ulong guildId)
        => await MakeFluxerApiRequestR<List<User>>(HttpMethod.Get, $"guilds/{guildId}/members", true);

    public async Task<List<Role>> GetGuildRoles(ulong guildId)
        => await MakeFluxerApiRequestR<List<Role>>(HttpMethod.Get, $"guilds/{guildId}/roles", true);

    public async Task<List<Channel>> GetGuildChannels(ulong guildId)
        => await MakeFluxerApiRequestR<List<Channel>>(HttpMethod.Get, $"guilds/{guildId}/channels", true);

    public async Task<GuildProperties> PatchGuild(ulong guildId, GuildProperties guild)
        => await MakeFluxerApiRequestRS<GuildProperties, GuildProperties>(HttpMethod.Patch, $"guilds/{guildId}", guild, true);

    public async Task<GuildMember> PatchGuildUser(ulong guildId, ulong userId, GuildMember member)
        => await MakeFluxerApiRequestRS<GuildMember, GuildMember>(HttpMethod.Patch, $"guilds/{guildId}/members/{userId}", member, true);

    public async Task<GuildMember> PatchGuildSelfUser(ulong guildId, GuildMember member)
        => await MakeFluxerApiRequestRS<GuildMember, GuildMember>(HttpMethod.Patch, $"guilds/{guildId}/members/@me", member, true);

    public async Task<GuildProperties> PostGuild()
        => await MakeFluxerApiRequestR<GuildProperties>(HttpMethod.Post, $"guilds", true);

    public async Task<Role> PostGuildRole(ulong guildId)
        => await MakeFluxerApiRequestR<Role>(HttpMethod.Post, $"guilds/{guildId}/roles", true);

    public async Task<Channel> PostGuildChannel(ulong guildId)
        => await MakeFluxerApiRequestR<Channel>(HttpMethod.Post, $"guilds/{guildId}/channels", true);

    public async Task<GuildProperties> PostGuildVanityUrl(ulong guildId, string vanityUrl)
        => await MakeFluxerApiRequestRS<GuildProperties, string>(HttpMethod.Post, $"guilds/{guildId}/vanity-url", "{code: \"" + vanityUrl + "\"}", true);

    #endregion

    #region Invites API

    public async Task<GuildProperties> PostGuildJoin(string invite)
        => await MakeFluxerApiRequestR<GuildProperties>(HttpMethod.Post, $"invites/{invite}", true);

    #endregion

    #region Users API

    // public async Task DeleteGuild(ulong guildId)
    //     => await MakeFluxerApiRequest(HttpMethod.Delete, $"users/@me/guilds/{guildId}", true, true);

    public async Task<User> GetUser(ulong userId)
        => await MakeFluxerApiRequestR<User>(HttpMethod.Get, $"users/{userId}", true);

    public async Task<User> GetCurrentUser()
        => await MakeFluxerApiRequestR<User>(HttpMethod.Get, $"users/@me", true);

    public async Task<UserSettings> GetCurrentUserSettings()
        => await MakeFluxerApiRequestR<UserSettings>(HttpMethod.Get, $"users/@me/settings", true);

    public async Task<List<GuildProperties>> GetCurrentUserGuilds()
        => await MakeFluxerApiRequestR<List<GuildProperties>>(HttpMethod.Get, $"users/@me/guilds", true);

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
