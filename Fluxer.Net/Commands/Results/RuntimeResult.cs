using System.Diagnostics;

namespace Fluxer.Net.Commands;

/// <summary>
/// The base class for the result a command returns from a method that returns <see cref="Task{RuntimeResult}"/>.
/// </summary>
[DebuggerDisplay(@"{DebuggerDisplay,nq}")]
public abstract class RuntimeResult : IResult
{
    /// <summary>
    /// Creates a new <see cref="RuntimeResult"/> with the given error type and reason.
    /// </summary>
    /// <param name="error">The type of failure, or <see langword="null" /> if none.</param>
    /// <param name="reason">The reason of failure.</param>
    protected RuntimeResult(CommandError? error, string reason)
    {
        Error = error;
        Reason = reason;
    }

    /// <inheritdoc/>
    public CommandError? Error { get; }
    /// <summary>
    /// The reason the command finished, or null when none was given.
    /// </summary>
    public string Reason { get; }

    /// <inheritdoc/>
    public bool IsSuccess => !Error.HasValue;

    /// <inheritdoc/>
    string IResult.ErrorReason => Reason;

    /// <inheritdoc />
    public override string ToString() => Reason ?? (IsSuccess ? "Successful" : "Unsuccessful");
    private string DebuggerDisplay => IsSuccess ? $"Success: {Reason ?? "No Reason"}" : $"{Error}: {Reason}";
}
