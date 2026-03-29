namespace Fluxer.Net;

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
