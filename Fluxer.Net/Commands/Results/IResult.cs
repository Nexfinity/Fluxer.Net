namespace Fluxer.Net.Commands;

/// <summary>
/// The result of an operation on a command.
/// </summary>
public interface IResult
{
    /// <summary>
    /// The type of error that occurred, or null when the operation succeeded.
    /// </summary>
    CommandError? Error { get; }
    /// <summary>
    /// The reason for the error.
    /// </summary>
    string ErrorReason { get; }
    /// <summary>
    /// Whether the operation succeeded.
    /// </summary>
    bool IsSuccess { get; }
}
