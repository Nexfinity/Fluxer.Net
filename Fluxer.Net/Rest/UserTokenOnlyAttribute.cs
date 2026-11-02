namespace Fluxer.Net.Rest;

/// <summary>
/// Marks an API method that Fluxer only serves to user accounts. The client does not read this attribute. Bot token
/// requests are checked against <see cref="UserOnlyRoutes"/> instead.
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public sealed class UserTokenOnlyAttribute : Attribute
{
}

/// <summary>
/// Thrown when a bot token is used for an API route Fluxer only serves to users. Nothing was sent to the server.
/// </summary>
public class UserTokenRequiredException : InvalidOperationException
{
    /// <summary>
    /// Creates the exception for a route.
    /// </summary>
    public UserTokenRequiredException(HttpMethod method, string route)
        : base($"{method} {route} is only available to user accounts. Fluxer answers bot tokens with 403 ACCESS_DENIED. " +
               "Use a user token (flx_...) for this call, or set FluxerConfig.EnforceUserOnlyRoutes to false to send it anyway.")
    {
        Method = method;
        Route = route;
    }

    /// <summary>
    /// The HTTP method of the refused call.
    /// </summary>
    public HttpMethod Method { get; }

    /// <summary>
    /// The route of the refused call.
    /// </summary>
    public string Route { get; }
}
