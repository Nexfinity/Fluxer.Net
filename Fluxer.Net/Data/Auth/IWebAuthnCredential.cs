namespace Fluxer.Net;

/// <summary>
/// Passkey or device based authentication credential.
/// </summary>
public interface IWebAuthnCredential
{
    /// <summary>
    /// ID of the credential.
    /// </summary>
    string Id { get; }

    /// <summary>
    /// User given name of the credential.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Credential created at date.
    /// </summary>
    DateTimeOffset CreatedAt { get; }

    /// <summary>
    /// Credential last used at date.
    /// </summary>
    DateTimeOffset? LastUsedAt { get; }
}