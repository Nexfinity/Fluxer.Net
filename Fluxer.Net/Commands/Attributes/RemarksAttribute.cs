namespace Fluxer.Net.Commands;

/// <summary>
/// Sets the remarks of a command or module.
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public class RemarksAttribute : Attribute
{
    /// <summary>
    /// The remarks of the command or module.
    /// </summary>
    public string Text { get; }

    /// <summary>
    /// Creates a new <see cref="RemarksAttribute"/> with the given remarks.
    /// </summary>
    /// <param name="text">The remarks.</param>
    public RemarksAttribute(string text)
    {
        Text = text;
    }
}