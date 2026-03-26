#if NET5_0_OR_GREATER
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;

namespace Fluxer.Net.OAuth;

/// <Summary>
/// Extension methods for handling fluxer oauth client.
/// </Summary>
public static class FluxerOAuthExtensions
{
    /// <Summary>
    /// Add Fluxer oauth client on dependency injection service.
    /// </Summary>
    public static IServiceCollection AddFluxerClient(this IServiceCollection services, string clientId, string clientSecret, FluxerConfig? config = null)
    {
        return services.AddSingleton(new FluxerOAuthClient(clientId, clientSecret, config));
    }

    /// <Summary>
    /// Get Fluxer specific claims for a claim user.
    /// </Summary>
    public static FluxerOAuthUser GetFluxerClaims(this ClaimsPrincipal principal, FluxerBaseClient client)
    {
        return new FluxerOAuthUser(client, principal);
    }

    /// <Summary>
    /// Add Fluxer oauth client on dependency injection service.
    /// </Summary>
    public static AuthenticationBuilder AddFluxer(
        [NotNull] this AuthenticationBuilder builder)
    {

        return builder.AddFluxer(FluxerOAuthDefaults.AuthenticationScheme, options => { });
    }

    /// <Summary>
    /// Add Fluxer oauth client on dependency injection service.
    /// </Summary>
    public static AuthenticationBuilder AddFluxer(
        [NotNull] this AuthenticationBuilder builder,
        [NotNull] Action<FluxerOAuthOptions> configuration)
    {

        return builder.AddFluxer(FluxerOAuthDefaults.AuthenticationScheme, configuration);
    }

    /// <Summary>
    /// Add Fluxer oauth client on dependency injection service.
    /// </Summary>
    public static AuthenticationBuilder AddFluxer(
        [NotNull] this AuthenticationBuilder builder,
        [NotNull] string scheme,
        [NotNull] Action<FluxerOAuthOptions> configuration)
    {

        return builder.AddFluxer(scheme, FluxerOAuthDefaults.DisplayName, configuration);
    }

    /// <Summary>
    /// Add Fluxer oauth client on dependency injection service.
    /// </Summary>
    public static AuthenticationBuilder AddFluxer(
        [NotNull] this AuthenticationBuilder builder,
        [NotNull] string scheme,
        [MaybeNull] string caption,
        [NotNull] Action<FluxerOAuthOptions> configuration)
    {

        return builder.AddOAuth<FluxerOAuthOptions, FluxerOAuthHandler>(scheme, caption, configuration);
    }
}
#endif