namespace Fluxer.Net.Commands.Attributes;

/// <summary>
/// Base class for command preconditions that must be satisfied before execution.
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
public abstract class PreconditionAttribute : Attribute
{
	/// <summary>
	/// Checks if the precondition is met.
	/// </summary>
	/// <param name="context">The command context.</param>
	/// <param name="command">The command being executed.</param>
	/// <param name="services">The service provider.</param>
	public abstract Task<PreconditionResult> CheckPermissionsAsync(
		CommandContext context,
		CommandInfo command,
		IServiceProvider? services);
}
