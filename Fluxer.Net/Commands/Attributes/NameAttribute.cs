namespace Fluxer.Net.Commands;

/// <summary>
/// Sets the public name of a command, module or parameter.
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
public class NameAttribute : Attribute
{
    /// <summary>
    /// The public name of the command, module or parameter.
    /// </summary>
    public string Text { get; }

    /// <summary>
    /// Creates a new <see cref="NameAttribute"/> with the given name.
    /// </summary>
    /// <param name="text">The public name of the object.</param>
    public NameAttribute(string text)
    {
        Text = text;
    }
}
