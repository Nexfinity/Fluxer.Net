namespace Fluxer.Net;

public interface ICurrentApplication
{
    /// <summary>
    /// Owner user of this application.
    /// </summary>
    IUser Owner { get; }
}
