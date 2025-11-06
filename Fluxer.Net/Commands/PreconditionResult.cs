namespace Fluxer.Net.Commands;

/// <summary>
/// Represents the result of a precondition check.
/// </summary>
public class PreconditionResult : IResult
{
	/// <summary>
	/// Gets whether the precondition check was successful.
	/// </summary>
	public bool IsSuccess { get; }

	/// <summary>
	/// Gets the error message if the precondition failed.
	/// </summary>
	public string? Error { get; }

	/// <summary>
	/// Gets the error type if the precondition failed.
	/// </summary>
	public CommandError? ErrorType { get; }

	private PreconditionResult(bool isSuccess, string? error, CommandError? errorType)
	{
		IsSuccess = isSuccess;
		Error = error;
		ErrorType = errorType;
	}

	/// <summary>
	/// Creates a successful precondition result.
	/// </summary>
	public static PreconditionResult FromSuccess()
		=> new(true, null, null);

	/// <summary>
	/// Creates a failed precondition result.
	/// </summary>
	public static PreconditionResult FromError(string reason)
		=> new(false, reason, CommandError.UnmetPrecondition);

	/// <summary>
	/// Creates a failed precondition result from an exception.
	/// </summary>
	public static PreconditionResult FromError(Exception exception)
		=> new(false, exception.Message, CommandError.Exception);
}
