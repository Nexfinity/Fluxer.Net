using System.Diagnostics;

namespace Fluxer.Net.Commands;

/// <summary>
/// The result of a precondition group, with the results of each precondition in the group.
/// </summary>
[DebuggerDisplay(@"{DebuggerDisplay,nq}")]
public class PreconditionGroupResult : PreconditionResult
{
    /// <summary>
    /// The results of the preconditions in the group.
    /// </summary>
    public IReadOnlyCollection<PreconditionResult> PreconditionResults { get; }

    /// <summary>
    /// Creates a new <see cref="PreconditionGroupResult"/>.
    /// </summary>
    /// <param name="error">The type of error, or null when the group passed.</param>
    /// <param name="errorReason">The reason for the error.</param>
    /// <param name="preconditions">The results of the preconditions in the group.</param>
    protected PreconditionGroupResult(CommandError? error, string errorReason, ICollection<PreconditionResult> preconditions)
        : base(error, errorReason)
    {
        PreconditionResults = (preconditions ?? new List<PreconditionResult>(0)).ToReadOnlyCollection();
    }

    /// <summary>
    /// Creates a result for a passed precondition group.
    /// </summary>
    /// <returns>A successful <see cref="PreconditionGroupResult"/>.</returns>
    public new static PreconditionGroupResult FromSuccess()
        => new PreconditionGroupResult(null, null, null);
    /// <summary>
    /// Creates a result for a failed precondition group.
    /// </summary>
    /// <param name="reason">The reason for the failure.</param>
    /// <param name="preconditions">The results of the preconditions in the group.</param>
    /// <returns>A failed <see cref="PreconditionGroupResult"/> with <see cref="CommandError.UnmetPrecondition"/>.</returns>
    public static PreconditionGroupResult FromError(string reason, ICollection<PreconditionResult> preconditions)
        => new PreconditionGroupResult(CommandError.UnmetPrecondition, reason, preconditions);
    /// <summary>
    /// Creates a result for a precondition group that threw an exception.
    /// </summary>
    /// <param name="ex">The exception that caused the failure.</param>
    /// <returns>A failed <see cref="PreconditionGroupResult"/> with <see cref="CommandError.Exception"/>.</returns>
    public static new PreconditionGroupResult FromError(Exception ex)
        => new PreconditionGroupResult(CommandError.Exception, ex.Message, null);
    /// <summary>
    /// Creates a result that copies the error and reason of another result.
    /// </summary>
    /// <param name="result">The result to copy from.</param>
    /// <returns>A failed <see cref="PreconditionGroupResult"/>.</returns>
    public static new PreconditionGroupResult FromError(IResult result) //needed?
        => new PreconditionGroupResult(result.Error, result.ErrorReason, null);

    /// <inheritdoc />
    public override string ToString() => IsSuccess ? "Success" : $"{Error}: {ErrorReason}";
    private string DebuggerDisplay => IsSuccess ? "Success" : $"{Error}: {ErrorReason}";
}
