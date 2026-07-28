namespace Fluxer.Net.Commands;

/// <summary>
/// Provides a summary description for a command or parameter.
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
public class SummaryAttribute : Attribute
{
    /// <summary>
    /// Gets the summary text.
    /// </summary>
    public string Text { get; }

    /// <summary>
    /// Provides a summary description.
    /// </summary>
    /// <param name="text">The summary text.</param>
    public SummaryAttribute(string text)
    {
        Text = text;
    }
}
