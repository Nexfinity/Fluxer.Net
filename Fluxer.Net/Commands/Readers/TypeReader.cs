namespace Fluxer.Net.Commands;

/// <summary>
/// The base class for a reader that parses raw command input into an object of a specific type.
/// </summary>
public abstract class TypeReader
{
    /// <summary>
    /// Parses the <paramref name="input"/> into the desired type.
    /// </summary>
    /// <param name="context">The context of the command.</param>
    /// <param name="input">The raw input of the command.</param>
    /// <param name="services">The service collection used for dependency injection.</param>
    /// <returns>
    /// A task that represents the asynchronous parsing operation. The task result contains the parsing result.
    /// </returns>
    public abstract Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider services);
}
