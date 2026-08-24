namespace Fluxer.Net.Commands;

/// <summary>
/// Attaches a summary to your command (Cosmetic)
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
public class SummaryAttribute : Attribute
{
    public string Text { get; }

    public SummaryAttribute(string text)
    {
        Text = text;
    }
}
