namespace Fluxer.Net.Commands;

/// <summary>
/// Attaches remarks to your commands (Cosmetic)
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public class RemarksAttribute : Attribute
{
    public string Text { get; }

    public RemarksAttribute(string text)
    {
        Text = text;
    }
}