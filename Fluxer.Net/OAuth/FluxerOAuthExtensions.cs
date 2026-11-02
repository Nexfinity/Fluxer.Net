#if NET5_0_OR_GREATER
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;

namespace Fluxer.Net.OAuth;

/// <summary>
/// Extension methods that register Fluxer clients and Fluxer sign-in with ASP.NET Core.
/// </summary>
public static class FluxerOAuthExtensions
{
    /// <summary>
    /// Registers a <see cref="FluxerClient"/> as a singleton in the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="token">The token of the client.</param>
    /// <param name="config">The settings to use. Defaults are used when null.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddFluxerClient(this IServiceCollection services, string token, FluxerConfig? config = null)
    {
        return services.AddSingleton(new FluxerClient(token, config));
    }

    /// <summary>
    /// Registers a <see cref="FluxerOAuthClient"/> as a singleton in the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="clientId">The client id of the OAuth app.</param>
    /// <param name="clientSecret">The client secret of the OAuth app.</param>
    /// <param name="config">The settings to use. Defaults are used when null.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddFluxerOAuth(this IServiceCollection services, ulong clientId, string clientSecret, FluxerConfig? config = null)
    {
        return services.AddSingleton(new FluxerOAuthClient(clientId, clientSecret, config));
    }

    /// <summary>
    /// Reads the Fluxer user from the claims of a signed-in principal.
    /// </summary>
    /// <param name="principal">The signed-in user.</param>
    /// <param name="client">The client the user is read with.</param>
    /// <returns>The Fluxer user.</returns>
    public static FluxerOAuthUser GetFluxerClaims(this ClaimsPrincipal principal, FluxerBaseClient client)
    {
        return FluxerOAuthUser.Create(client, principal);
    }

    /// <summary>
    /// Adds Fluxer sign-in under the default scheme name.
    /// </summary>
    /// <param name="builder">The authentication builder.</param>
    /// <param name="configuration">Configures the sign-in options.</param>
    /// <returns>The authentication builder.</returns>
    public static AuthenticationBuilder AddFluxer(
        [NotNull] this AuthenticationBuilder builder,
        [NotNull] Action<FluxerOAuthOptions> configuration)
    {

        return builder.AddFluxer(FluxerOAuthDefaults.AuthenticationScheme, configuration);
    }

    /// <summary>
    /// Adds Fluxer sign-in under the given scheme name.
    /// </summary>
    /// <param name="builder">The authentication builder.</param>
    /// <param name="scheme">The authentication scheme name.</param>
    /// <param name="configuration">Configures the sign-in options.</param>
    /// <returns>The authentication builder.</returns>
    public static AuthenticationBuilder AddFluxer(
        [NotNull] this AuthenticationBuilder builder,
        [NotNull] string scheme,
        [NotNull] Action<FluxerOAuthOptions> configuration)
    {

        return builder.AddFluxer(scheme, FluxerOAuthDefaults.DisplayName, configuration);
    }

    /// <summary>
    /// Adds Fluxer sign-in under the given scheme name and display caption.
    /// </summary>
    /// <param name="builder">The authentication builder.</param>
    /// <param name="scheme">The authentication scheme name.</param>
    /// <param name="caption">The display name of the sign-in.</param>
    /// <param name="configuration">Configures the sign-in options.</param>
    /// <returns>The authentication builder.</returns>
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