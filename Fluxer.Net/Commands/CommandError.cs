namespace Fluxer.Net.Commands;

/// <summary>
/// Defines the types of errors that can occur during command execution.
/// </summary>
public enum CommandError
{
	/// <summary>
	/// The command failed to parse.
	/// </summary>
	ParseFailed,

	/// <summary>
	/// The command was not found.
	/// </summary>
	UnknownCommand,

	/// <summary>
	/// The user provided too few parameters.
	/// </summary>
	BadArgCount,

	/// <summary>
	/// A precondition failed.
	/// </summary>
	UnmetPrecondition,

	/// <summary>
	/// An exception occurred during execution.
	/// </summary>
	Exception,

	/// <summary>
	/// The command execution was unsuccessful for an unspecified reason.
	/// </summary>
	Unsuccessful
}
