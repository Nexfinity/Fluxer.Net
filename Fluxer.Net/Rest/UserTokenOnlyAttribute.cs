namespace Fluxer.Net.Rest;

/// <summary>
/// Marks an API method that Fluxer only serves to a logged-in user. Called with a bot token, the server answers
/// 403 <c>ACCESS_DENIED</c>, and the client throws <see cref="UserTokenRequiredException"/> before sending anything.
/// </summary>
/// <remarks>
/// Fluxer allows user accounts to run automated clients, and these methods work for them. Bot applications have no
/// setting that unlocks them.
/// </remarks>
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
