namespace Fluxer.Net.Commands;

/// <summary>
/// Attaches a documentation URL to a command or module.
/// </summary>
/// <remarks>
/// The library stores this value in the attributes of the command or module and does not read it.
/// </remarks>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public class DocsAttribute : Attribute
{
    /// <summary>
    /// The documentation URL.
    /// </summary>
    public string Url { get; }

    /// <summary>
    /// Creates a new <see cref="DocsAttribute"/> with the given URL.
    /// </summary>
    /// <param name="url">The documentation URL.</param>
    public DocsAttribute(string url)
    {
        Url = url;
    }
}