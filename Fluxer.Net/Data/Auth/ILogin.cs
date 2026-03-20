namespace Fluxer.Net;

public interface ILogin
{
    /// <summary>
    /// Authentication token for API requests.
    /// </summary>
    string Token { get; }

    /// <summary>
    /// ID of the authenticated user.
    /// </summary>
    ulong UserId { get; }
}
