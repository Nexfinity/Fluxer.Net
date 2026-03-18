#if NET5_0_OR_GREATER
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;

namespace Fluxer.Net.OAuth;

public static class FluxerOAuthExtensions
{
    public static IServiceCollection AddFluxerClient(this IServiceCollection services, string clientId, string clientSecret, FluxerConfig? config = null)
    {
        return services.AddSingleton(new FluxerOAuthClient(clientId, clientSecret, config));
    }


    public static FluxerOAuthClaims GetFluxerClaims(this ClaimsPrincipal principal)
    {
        return new FluxerOAuthClaims(principal);
    }

    public static AuthenticationBuilder AddFluxer(
        [NotNull] this AuthenticationBuilder builder)
    {

        return builder.AddFluxer(FluxerOAuthDefaults.AuthenticationScheme, options => { });
    }

    public static AuthenticationBuilder AddFluxer(
        [NotNull] this AuthenticationBuilder builder,
        [NotNull] Action<FluxerOAuthOptions> configuration)
    {

        return builder.AddFluxer(FluxerOAuthDefaults.AuthenticationScheme, configuration);
    }

    public static AuthenticationBuilder AddFluxer(
        [NotNull] this AuthenticationBuilder builder,
        [NotNull] string scheme,
        [NotNull] Action<FluxerOAuthOptions> configuration)
    {

        return builder.AddFluxer(scheme, FluxerOAuthDefaults.DisplayName, configuration);
    }

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