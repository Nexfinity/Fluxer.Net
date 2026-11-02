namespace Fluxer.Net;

/// <summary>
/// An object that can be mentioned in a message.
/// </summary>
public interface IMentionable
{
    /// <summary>
    /// The mention string that refers to the object.
    /// </summary>
    string Mention { get; }
}
