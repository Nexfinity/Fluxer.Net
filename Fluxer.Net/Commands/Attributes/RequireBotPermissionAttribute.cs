using Fluxer.Net.Data.Enums;

namespace Fluxer.Net.Commands.Attributes;

/// <summary>
/// Requires the bot to have specific permissions to execute the command.
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public class RequireBotPermissionAttribute : PreconditionAttribute
{
	/// <summary>
	/// Gets the required permissions.
	/// </summary>
	public Permissions Permissions { get; }

	/// <summary>
	/// Creates a new bot permission precondition.
	/// </summary>
	/// <param name="permissions">The permissions required.</param>
	public RequireBotPermissionAttribute(Permissions permissions)
	{
		Permissions = permissions;
	}

	/// <summary>
	/// Checks if the bot has the required permissions.
	/// </summary>
	public override Task<PreconditionResult> CheckPermissionsAsync(
		CommandContext context,
		CommandInfo command,
		IServiceProvider? services)
	{
		// Must be in a guild context
		if (!context.GuildId.HasValue)
			return Task.FromResult(PreconditionResult.FromError("This command must be used in a guild."));

		// TODO: The Fluxer API doesn't currently expose permissions for guild members
		// This would require fetching guild roles and computing permissions based on role hierarchy
		// For now, this is a placeholder that always succeeds
		// Implementers should extend this to compute permissions from roles when needed

		return Task.FromResult(PreconditionResult.FromSuccess());
	}
}
