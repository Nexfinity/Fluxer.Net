namespace Fluxer.Net.Commands;

/// <summary>
/// Represents a result from executing a command.
/// </summary>
public interface IResult
{
	/// <summary>
	/// Gets whether the command execution was successful.
	/// </summary>
	bool IsSuccess { get; }

	/// <summary>
	/// Gets the error reason if the command failed.
	/// </summary>
	string? Error { get; }

	/// <summary>
	/// Gets the error type.
	/// </summary>
	CommandError? ErrorType { get; }
}
