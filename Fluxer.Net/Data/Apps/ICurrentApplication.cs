namespace Fluxer.Net;

/// <summary>
/// The application of the authenticated bot.
/// </summary>
public interface ICurrentApplication
{
    /// <summary>
    /// The user that owns this application.
    /// </summary>
    IUser Owner { get; }
}
