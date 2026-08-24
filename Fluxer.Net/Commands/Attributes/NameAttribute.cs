namespace Fluxer.Net.Commands;

/// <summary>
/// Override public name of command/module
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
public class NameAttribute : Attribute
{
    /// <summary>
    ///     Gets the name of the command.
    /// </summary>
    public string Text { get; }

    /// <summary>
    ///     Marks the public name of a command, module, or parameter with the provided name.
    /// </summary>
    /// <param name="text">The public name of the object.</param>
    public NameAttribute(string text)
    {
        Text = text;
    }
}
