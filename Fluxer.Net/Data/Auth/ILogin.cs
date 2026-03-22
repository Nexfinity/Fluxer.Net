namespace Fluxer.Net;

/// <summary>
/// Login response data when successful.
/// </summary>
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
