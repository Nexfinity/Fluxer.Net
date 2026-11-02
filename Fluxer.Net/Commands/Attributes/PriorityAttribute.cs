namespace Fluxer.Net.Commands;

/// <summary>
/// Sets the priority of a command that has several overloads matching the same input.
/// </summary>
/// <remarks>
/// Commands with a higher priority are tried first.
/// </remarks>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public class PriorityAttribute : Attribute
{
    /// <summary>
    /// The priority of the command.
    /// </summary>
    public int Priority { get; }

    /// <summary>
    /// Creates a new <see cref="PriorityAttribute"/> with the given priority.
    /// </summary>
    /// <param name="priority">The priority of the command.</param>
    public PriorityAttribute(int priority)
    {
        Priority = priority;
    }
}
