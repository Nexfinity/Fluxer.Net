#if NET5_0_OR_GREATER
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace Fluxer.Net.OAuth;

/// <summary>
/// The ASP.NET Core OAuth handler that signs users in with Fluxer.
/// </summary>
public partial class FluxerOAuthHandler : OAuthHandler<FluxerOAuthOptions>
{
    /// <summary>
    /// Creates the OAuth handler for Fluxer sign-in.
    /// </summary>
    /// <param name="options">The sign-in options.</param>
    /// <param name="logger">The logger factory.</param>
    /// <param name="encoder">The URL encoder.</param>
    /// <param name="clock">The system clock.</param>
    public FluxerOAuthHandler(
        IOptionsMonitor<FluxerOAuthOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock) : base(options, logger, encoder, clock)
    {

    }

    /// <inheritdoc />
    protected override string BuildChallengeUrl(
        [NotNull] AuthenticationProperties properties,
        [NotNull] string redirectUri)
    {
        if (Options.EmailScope)
            Options.Scope.Add("email");

        if (Options.GuildsScope)
            Options.Scope.Add("guilds");

        if (Options.ConnectionsScope)
            Options.Scope.Add("connections");

        string challengeUrl = base.BuildChallengeUrl(properties, redirectUri);

        if (!string.IsNullOrEmpty(Options.Prompt))
            challengeUrl = QueryHelpers.AddQueryString(challengeUrl, "prompt", Options.Prompt);

        return challengeUrl;
    }

    /// <inheritdoc />
    protected override async Task<AuthenticationTicket> CreateTicketAsync(
        [NotNull] ClaimsIdentity identity,
        [NotNull] AuthenticationProperties properties,
        [NotNull] OAuthTokenResponse tokens)
    {
        using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, Options.UserInformationEndpoint);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokens.AccessToken);

        using HttpResponseMessage response = await Backchannel.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, Context.RequestAborted);

        //string Headers = response.Headers.ToString();
        //string Body = await response.Content.ReadAsStringAsync();

        //Console.WriteLine("--- Headers ---\n" +
        //    $"{Headers}\n" +
        //    $"--- Body ---\n" +
        //    $"{Body}\n" +
        //    $"--- --- ---");


        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException("An error occurred while retrieving the user profile.");
        }

        using JsonDocument payload = JsonDocument.Parse(await response.Content.ReadAsStringAsync(Context.RequestAborted));

        ClaimsPrincipal principal = new ClaimsPrincipal(identity);
        OAuthCreatingTicketContext context = new OAuthCreatingTicketContext(principal, properties, Context, Scheme, Options, Backchannel, tokens, payload.RootElement);
        context.RunClaimActions();
        await Events.CreatingTicket(context);
        return new AuthenticationTicket(context.Principal!, context.Properties, Scheme.Name);
    }
}
#endif