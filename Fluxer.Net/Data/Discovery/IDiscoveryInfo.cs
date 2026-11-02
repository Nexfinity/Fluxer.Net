namespace Fluxer.Net;

/// <summary>
/// A guild's discovery status.
/// </summary>
public interface IDiscoveryInfo
{
    /// <summary>
    /// The guild's discovery application.
    /// </summary>
    IDiscoveryApplication Application { get; }
}

/// <summary>
/// A guild's application to be listed in discovery.
/// </summary>
public interface IDiscoveryApplication
{

}
