namespace Fluxer.Net.Commands;

/// <summary>
/// Marks a module as a command group that is triggered by a prefix.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public class GroupAttribute : Attribute
{
    /// <summary>
    /// The prefix of the module group, or null when the attribute has no prefix.
    /// </summary>
    public string Prefix { get; }

    /// <summary>
    /// Creates a new <see cref="GroupAttribute"/> without a prefix.
    /// </summary>
    public GroupAttribute()
    {
        Prefix = null;
    }

    /// <summary>
    /// Creates a new <see cref="GroupAttribute"/> with the given prefix.
    /// </summary>
    /// <param name="prefix">The prefix of the module group.</param>
    public GroupAttribute(string prefix)
    {
        Prefix = prefix;
    }
}
