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

/// <Summary>
/// Asp.net oauth handler for Fluxer.
/// </Summary>
public partial class FluxerOAuthHandler : OAuthHandler<FluxerOAuthOptions>
{
    /// <Summary>
    /// Create asp.net oauth handler for Fluxer.
    /// </Summary>
    public FluxerOAuthHandler(
        IOptionsMonitor<FluxerOAuthOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock) : base(options, logger, encoder, clock)
    {

    }

    protected override string BuildChallengeUrl(
        [NotNull] AuthenticationProperties properties,
        [NotNull] string redirectUri)
    {
        var challengeUrl = base.BuildChallengeUrl(properties, redirectUri);

        if (!string.IsNullOrEmpty(Options.Prompt))
        {
            challengeUrl = QueryHelpers.AddQueryString(challengeUrl, "prompt", Options.Prompt);
        }




        return challengeUrl;
    }

    protected override async Task<AuthenticationTicket> CreateTicketAsync(
        [NotNull] ClaimsIdentity identity,
        [NotNull] AuthenticationProperties properties,
        [NotNull] OAuthTokenResponse tokens)
    {
        using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, Options.UserInformationEndpoint);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokens.AccessToken);

        using var response = await Backchannel.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, Context.RequestAborted);

        string Headers = response.Headers.ToString();
        string Body = await response.Content.ReadAsStringAsync();

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