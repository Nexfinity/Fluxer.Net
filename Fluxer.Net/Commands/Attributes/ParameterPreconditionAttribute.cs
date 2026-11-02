namespace Fluxer.Net.Commands;

/// <summary>
/// The base class for a check on a command parameter that runs before the command executes.
/// </summary>
/// <seealso cref="PreconditionAttribute"/>
[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = true, Inherited = true)]
public abstract class ParameterPreconditionAttribute : Attribute
{
    /// <summary>
    /// Checks whether the argument of a parameter passes the check.
    /// </summary>
    /// <param name="context">The context of the command.</param>
    /// <param name="parameter">The parameter being checked.</param>
    /// <param name="value">The parsed value of the parameter.</param>
    /// <param name="services">The service collection used for dependency injection.</param>
    public abstract Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, ParameterInfo parameter, object value, IServiceProvider services);
}
