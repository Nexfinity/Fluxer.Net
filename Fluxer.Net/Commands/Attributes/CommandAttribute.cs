namespace Fluxer.Net.Commands;

/// <summary>
/// Marks a method as a text command.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public class CommandAttribute : Attribute
{
    /// <summary>
    /// Gets the name of the command.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets or sets whether the command can be invoked while another command is running.
    /// </summary>
    public RunMode RunMode { get; set; } = RunMode.Sync;

    /// <summary>
    /// Marks a method as a command with the specified name.
    /// </summary>
    /// <param name="name">The name of the command.</param>
    public CommandAttribute(string name)
    {
        Name = name;
    }
}
