namespace Fluxer.Net.Commands;

/// <summary>
/// Sets the alternative names that trigger a command or module.
/// </summary>
/// <remarks>
/// The aliases are combined with the aliases of the enclosing module.
/// </remarks>
/// <example>
/// In the following example, the command is triggered by "stats", "stat" or "info".
/// <code language="cs">
/// [Command("stats")]
/// [Alias("stat", "info")]
/// public async Task GetStatsAsync(IUser user)
/// {
///     // ...pull stats
/// }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public class AliasAttribute : Attribute
{
    /// <summary>
    /// The aliases defined for the command.
    /// </summary>
    public string[] Aliases { get; }

    /// <summary>
    /// Creates a new <see cref="AliasAttribute"/> with the given aliases.
    /// </summary>
    /// <param name="aliases">The alternative names.</param>
    public AliasAttribute(params string[] aliases)
    {
        Aliases = aliases;
    }
}
