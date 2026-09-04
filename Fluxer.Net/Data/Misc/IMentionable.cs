namespace Fluxer.Net;

/// <summary>
/// Object is mentionable.
/// </summary>
public interface IMentionable
{
    /// <summary>
    /// Formatted mention string for this object.
    /// </summary>
    string Mention { get; }
}
