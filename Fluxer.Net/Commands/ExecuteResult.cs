namespace Fluxer.Net.Commands;

/// <summary>
/// Represents the result of a command execution.
/// </summary>
public class ExecuteResult : IResult
{
	/// <inheritdoc />
	public bool IsSuccess { get; }

	/// <inheritdoc />
	public string? Error { get; }

	/// <inheritdoc />
	public CommandError? ErrorType { get; }

	/// <summary>
	/// Gets the exception that occurred during execution, if any.
	/// </summary>
	public Exception? Exception { get; }

	private ExecuteResult(bool isSuccess, string? error, CommandError? errorType, Exception? exception = null)
	{
		IsSuccess = isSuccess;
		Error = error;
		ErrorType = errorType;
		Exception = exception;
	}

	/// <summary>
	/// Creates a successful result.
	/// </summary>
	public static ExecuteResult FromSuccess() => new(true, null, null);

	/// <summary>
	/// Creates an error result.
	/// </summary>
	public static ExecuteResult FromError(CommandError error, string reason) => new(false, reason, error);

	/// <summary>
	/// Creates an error result from an exception.
	/// </summary>
	public static ExecuteResult FromError(Exception exception) => new(false, exception.Message, CommandError.Exception, exception);

	/// <inheritdoc />
	public override string ToString() => IsSuccess ? "Success" : $"{ErrorType}: {Error}";
}
