namespace Fluxer.Net.Commands;

/// <summary>
/// Specifies a custom name for a command parameter.
/// </summary>
[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
public class NameAttribute : Attribute
{
    /// <summary>
    /// Gets the parameter name.
    /// </summary>
    public string Text { get; }

    /// <summary>
    /// Specifies a custom name for a parameter.
    /// </summary>
    /// <param name="text">The parameter name.</param>
    public NameAttribute(string text)
    {
        Text = text;
    }
}
