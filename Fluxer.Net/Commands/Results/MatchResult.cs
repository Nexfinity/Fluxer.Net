using System.Diagnostics;

namespace Fluxer.Net.Commands;

/// <summary>
/// The result of matching input to a command, with the result of the stage it reached.
/// </summary>
[DebuggerDisplay(@"{DebuggerDisplay,nq}")]
public class MatchResult : IResult
{
    /// <summary>
    /// The command that matched, or null when no command matched.
    /// </summary>
    public CommandMatch? Match { get; }

    /// <summary>
    /// The result of the stage the command reached, such as a <see cref="ParseResult"/> or a
    /// <see cref="PreconditionResult"/>.
    /// </summary>
    public IResult Pipeline { get; }

    /// <inheritdoc />
    public CommandError? Error { get; }
    /// <inheritdoc />
    public string ErrorReason { get; }
    /// <inheritdoc />
    public bool IsSuccess => !Error.HasValue;

    private MatchResult(CommandMatch? match, IResult pipeline, CommandError? error, string errorReason)
    {
        Match = match;
        Error = error;
        Pipeline = pipeline;
        ErrorReason = errorReason;
    }

    /// <summary>
    /// Creates a result for a successful match.
    /// </summary>
    /// <param name="match">The matched command.</param>
    /// <param name="pipeline">The result of the stage the command reached.</param>
    /// <returns>A successful <see cref="MatchResult"/>.</returns>
    public static MatchResult FromSuccess(CommandMatch match, IResult pipeline)
        => new MatchResult(match, pipeline, null, null);
    /// <summary>
    /// Creates a result for a failed match with the given error and reason.
    /// </summary>
    /// <param name="error">The type of error.</param>
    /// <param name="reason">The reason for the error.</param>
    /// <returns>A failed <see cref="MatchResult"/>.</returns>
    public static MatchResult FromError(CommandError error, string reason)
        => new MatchResult(null, null, error, reason);
    /// <summary>
    /// Creates a result for a match that threw an exception.
    /// </summary>
    /// <param name="ex">The exception that caused the failure.</param>
    /// <returns>A failed <see cref="MatchResult"/> with <see cref="CommandError.Exception"/>.</returns>
    public static MatchResult FromError(Exception ex)
        => FromError(CommandError.Exception, ex.Message);
    /// <summary>
    /// Creates a result that copies the error and reason of another result.
    /// </summary>
    /// <param name="result">The result to copy from.</param>
    /// <returns>A failed <see cref="MatchResult"/>.</returns>
    public static MatchResult FromError(IResult result)
        => new MatchResult(null, null, result.Error, result.ErrorReason);
    /// <summary>
    /// Creates a result for a failure at a given pipeline stage.
    /// </summary>
    /// <param name="pipeline">The result of the stage that failed.</param>
    /// <param name="error">The type of error.</param>
    /// <param name="reason">The reason for the error.</param>
    /// <returns>A failed <see cref="MatchResult"/>.</returns>
    public static MatchResult FromError(IResult pipeline, CommandError error, string reason)
        => new MatchResult(null, pipeline, error, reason);

    /// <inheritdoc />
    public override string ToString() => IsSuccess ? "Success" : $"{Error}: {ErrorReason}";
    private string DebuggerDisplay => IsSuccess ? "Success" : $"{Error}: {ErrorReason}";

}
