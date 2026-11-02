namespace Fluxer.Net.Commands;

/// <summary>
/// The base class for a check on a module or command that runs before the command executes.
/// </summary>
/// <seealso cref="ParameterPreconditionAttribute"/>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
public abstract class PreconditionAttribute : Attribute
{
    /// <summary>
    /// The name of the group this precondition belongs to, or null when it belongs to no group.
    /// </summary>
    /// <remarks>
    /// Preconditions in the same group pass when any one of them passes (A || B). Preconditions without a group must all
    /// pass (A &amp;&amp; B).
    /// </remarks>
    public string Group { get; set; } = null;

    /// <summary>
    /// The message returned when the precondition fails.
    /// </summary>
    /// <remarks>
    /// A derived class that does not override this property ignores the value that is set.
    /// </remarks>
    public virtual string ErrorMessage { get { return null; } set { } }

    /// <summary>
    /// Checks whether the command can be executed in the given context.
    /// </summary>
    /// <param name="context">The context of the command.</param>
    /// <param name="command">The command being executed.</param>
    /// <param name="services">The service collection used for dependency injection.</param>
    public abstract Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services);
}
