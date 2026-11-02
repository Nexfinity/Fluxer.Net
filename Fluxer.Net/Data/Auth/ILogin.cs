namespace Fluxer.Net;

/// <summary>
/// The result of a successful login.
/// </summary>
public interface ILogin
{
    /// <summary>
    /// The token for authenticated API requests.
    /// </summary>
    string Token { get; }

    /// <summary>
    /// The id of the logged-in user.
    /// </summary>
    ulong UserId { get; }
}
