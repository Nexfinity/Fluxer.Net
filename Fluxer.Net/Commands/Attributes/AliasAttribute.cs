namespace Fluxer.Net.Commands.Attributes;

/// <summary>
/// Provides alternative names for a command.
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public class AliasAttribute : Attribute
{
	/// <summary>
	/// Gets the aliases for the command or module.
	/// </summary>
	public string[] Aliases { get; }

	/// <summary>
	/// Provides alternative names for a command or module.
	/// </summary>
	/// <param name="aliases">The aliases.</param>
	public AliasAttribute(params string[] aliases)
	{
		Aliases = aliases;
	}
}
