namespace Fluxer.Net.Commands;

/// <summary>
/// Attaches a docs link for the command (Cosmetic)
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public class DocsAttribute : Attribute
{
    /// <summary>
    /// Url that leads to the docs.
    /// </summary>
    public string Url { get; }

    /// <summary>
    /// Create a DocsAttribute with the url given.
    /// </summary>
    /// <param name="url"></param>
    public DocsAttribute(string url)
    {
        Url = url;
    }
}