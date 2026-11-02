namespace Fluxer.Net;

/// <summary>
/// A user signed in through OAuth2.
/// </summary>
public interface IFluxerOAuthUser
{
    /// <summary>
    /// The email address of the user.
    /// </summary>
    string? Email { get; }

    /// <summary>
    /// Whether the user has verified their email.
    /// </summary>
    bool? IsVerified { get; }
}
