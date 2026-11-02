namespace Fluxer.Net;

/// <summary>
/// A WebAuthn credential, such as a passkey.
/// </summary>
public interface IWebAuthnCredential
{
    /// <summary>
    /// The id of the credential.
    /// </summary>
    string Id { get; }

    /// <summary>
    /// The name the user gave the credential.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// The time the credential was created.
    /// </summary>
    DateTimeOffset CreatedAt { get; }

    /// <summary>
    /// The time the credential was last used.
    /// </summary>
    DateTimeOffset? LastUsedAt { get; }
}