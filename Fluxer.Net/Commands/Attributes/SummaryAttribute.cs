namespace Fluxer.Net.Commands;

/// <summary>
/// Sets the summary of a command, module or parameter.
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
public class SummaryAttribute : Attribute
{
    /// <summary>
    /// The summary of the command, module or parameter.
    /// </summary>
    public string Text { get; }

    /// <summary>
    /// Creates a new <see cref="SummaryAttribute"/> with the given summary.
    /// </summary>
    /// <param name="text">The summary.</param>
    public SummaryAttribute(string text)
    {
        Text = text;
    }
}
