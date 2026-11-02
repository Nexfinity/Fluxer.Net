using System.Diagnostics;

namespace Fluxer.Net.Commands;

/// <summary>
/// The result of executing a command.
/// </summary>
[DebuggerDisplay(@"{DebuggerDisplay,nq}")]
public struct ExecuteResult : IResult
{
    /// <summary>
    /// The exception that caused the failure, or null.
    /// </summary>
    public Exception Exception { get; }

    /// <inheritdoc />
    public CommandError? Error { get; }
    /// <inheritdoc />
    public string ErrorReason { get; }

    /// <inheritdoc />
    public bool IsSuccess => !Error.HasValue;

    private ExecuteResult(Exception exception, CommandError? error, string errorReason)
    {
        Exception = exception;
        Error = error;
        ErrorReason = errorReason;
    }

    /// <summary>
    /// Creates a result for a successful execution.
    /// </summary>
    /// <returns>
    /// An <see cref="ExecuteResult"/> that does not contain any errors.
    /// </returns>
    public static ExecuteResult FromSuccess()
        => new ExecuteResult(null, null, null);
    /// <summary>
    /// Creates a result for an unsuccessful execution with the given error and reason.
    /// </summary>
    /// <param name="error">The type of error.</param>
    /// <param name="reason">The reason behind the error.</param>
    /// <returns>
    /// An <see cref="ExecuteResult"/> that contains the <see cref="CommandError"/> and reason.
    /// </returns>
    public static ExecuteResult FromError(CommandError error, string reason)
        => new ExecuteResult(null, error, reason);
    /// <summary>
    /// Creates a result for an execution that threw an exception.
    /// </summary>
    /// <param name="ex">The exception that caused the command execution to fail.</param>
    /// <returns>
    /// An <see cref="ExecuteResult"/> that contains the exception, a <see cref="CommandError"/> of type
    /// <c>Exception</c>, and the exception message as the reason.
    /// </returns>
    public static ExecuteResult FromError(Exception ex)
        => new ExecuteResult(ex, CommandError.Exception, ex.Message);
    /// <summary>
    /// Creates a result that copies the error and reason of another result.
    /// </summary>
    /// <param name="result">The result to copy from.</param>
    /// <returns>
    /// An <see cref="ExecuteResult"/> with the error type and reason of <paramref name="result"/>.
    /// </returns>
    public static ExecuteResult FromError(IResult result)
        => new ExecuteResult(null, result.Error, result.ErrorReason);

    /// <summary>
    /// Gets a string that indicates the execution result.
    /// </summary>
    /// <returns>
    /// <c>Success</c> if <see cref="IsSuccess"/> is <see langword="true" />. Otherwise the error type and the error
    /// reason, separated by a colon.
    /// </returns>
    public override string ToString() => IsSuccess ? "Success" : $"{Error}: {ErrorReason}";
    private string DebuggerDisplay => IsSuccess ? "Success" : $"{Error}: {ErrorReason}";
}
