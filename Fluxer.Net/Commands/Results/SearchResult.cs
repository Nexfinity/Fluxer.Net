using System.Diagnostics;

namespace Fluxer.Net.Commands;

/// <summary>
/// The result of searching for commands that match an input.
/// </summary>
[DebuggerDisplay(@"{DebuggerDisplay,nq}")]
public struct SearchResult : IResult
{
    /// <summary>
    /// The input that was searched.
    /// </summary>
    public string Text { get; }
    /// <summary>
    /// The commands whose names match the input.
    /// </summary>
    public IReadOnlyList<CommandMatch> Commands { get; }

    /// <inheritdoc/>
    public CommandError? Error { get; }
    /// <inheritdoc/>
    public string ErrorReason { get; }

    /// <inheritdoc/>
    public bool IsSuccess => !Error.HasValue;

    private SearchResult(string text, IReadOnlyList<CommandMatch> commands, CommandError? error, string errorReason)
    {
        Text = text;
        Commands = commands;
        Error = error;
        ErrorReason = errorReason;
    }

    /// <summary>
    /// Creates a result for a search that found commands.
    /// </summary>
    /// <param name="text">The input that was searched.</param>
    /// <param name="commands">The commands that match the input.</param>
    /// <returns>A successful <see cref="SearchResult"/>.</returns>
    public static SearchResult FromSuccess(string text, IReadOnlyList<CommandMatch> commands)
        => new SearchResult(text, commands, null, null);
    /// <summary>
    /// Creates a result for a search that failed with the given error and reason.
    /// </summary>
    /// <param name="error">The type of error.</param>
    /// <param name="reason">The reason for the error.</param>
    /// <returns>A failed <see cref="SearchResult"/>.</returns>
    public static SearchResult FromError(CommandError error, string reason)
        => new SearchResult(null, null, error, reason);
    /// <summary>
    /// Creates a result for a search that threw an exception.
    /// </summary>
    /// <param name="ex">The exception that caused the failure.</param>
    /// <returns>A failed <see cref="SearchResult"/> with <see cref="CommandError.Exception"/>.</returns>
    public static SearchResult FromError(Exception ex)
        => FromError(CommandError.Exception, ex.Message);
    /// <summary>
    /// Creates a result that copies the error and reason of another result.
    /// </summary>
    /// <param name="result">The result to copy from.</param>
    /// <returns>A failed <see cref="SearchResult"/>.</returns>
    public static SearchResult FromError(IResult result)
        => new SearchResult(null, null, result.Error, result.ErrorReason);

    /// <inheritdoc />
    public override string ToString() => IsSuccess ? "Success" : $"{Error}: {ErrorReason}";
    private string DebuggerDisplay => IsSuccess ? $"Success ({Commands.Count} Results)" : $"{Error}: {ErrorReason}";
}
