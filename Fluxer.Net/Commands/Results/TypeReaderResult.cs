using System.Collections.Immutable;
using System.Diagnostics;

namespace Fluxer.Net.Commands;

/// <summary>
/// A value read by a <see cref="TypeReader"/>, with a score for how well the input matched it.
/// </summary>
[DebuggerDisplay(@"{DebuggerDisplay,nq}")]
public struct TypeReaderValue
{
    /// <summary>
    /// The value that was read.
    /// </summary>
    public object Value { get; }
    /// <summary>
    /// The score of the match. A higher score is preferred when several values match.
    /// </summary>
    public float Score { get; }

    /// <summary>
    /// Creates a new <see cref="TypeReaderValue"/> with the given value and score.
    /// </summary>
    /// <param name="value">The value that was read.</param>
    /// <param name="score">The score of the match.</param>
    public TypeReaderValue(object value, float score)
    {
        Value = value;
        Score = score;
    }

    /// <inheritdoc />
    public override string ToString() => Value?.ToString();
    private string DebuggerDisplay => $"[{Value}, {Math.Round(Score, 2)}]";
}

/// <summary>
/// The result of a <see cref="TypeReader"/> reading an input.
/// </summary>
[DebuggerDisplay(@"{DebuggerDisplay,nq}")]
public struct TypeReaderResult : IResult
{
    /// <summary>
    /// The values read from the input. Contains more than one value when several objects matched.
    /// </summary>
    public IReadOnlyCollection<TypeReaderValue> Values { get; }

    /// <inheritdoc/>
    public CommandError? Error { get; }
    /// <inheritdoc/>
    public string ErrorReason { get; }

    /// <inheritdoc/>
    public bool IsSuccess => !Error.HasValue;

    /// <exception cref="InvalidOperationException">TypeReaderResult was not successful.</exception>
    public object BestMatch => IsSuccess
        ? (Values.Count == 1 ? Values.Single().Value : Values.OrderByDescending(v => v.Score).First().Value)
        : throw new InvalidOperationException("TypeReaderResult was not successful.");

    private TypeReaderResult(IReadOnlyCollection<TypeReaderValue> values, CommandError? error, string errorReason)
    {
        Values = values;
        Error = error;
        ErrorReason = errorReason;
    }

    /// <summary>
    /// Creates a result for a successful read with a single value and a score of 1.
    /// </summary>
    /// <param name="value">The value that was read.</param>
    /// <returns>A successful <see cref="TypeReaderResult"/>.</returns>
    public static TypeReaderResult FromSuccess(object value)
        => new TypeReaderResult(ImmutableArray.Create(new TypeReaderValue(value, 1.0f)), null, null);
    /// <summary>
    /// Creates a result for a successful read with a single value.
    /// </summary>
    /// <param name="value">The value that was read, with its score.</param>
    /// <returns>A successful <see cref="TypeReaderResult"/>.</returns>
    public static TypeReaderResult FromSuccess(TypeReaderValue value)
        => new TypeReaderResult(ImmutableArray.Create(value), null, null);
    /// <summary>
    /// Creates a result for a successful read with one or more values.
    /// </summary>
    /// <param name="values">The values that were read.</param>
    /// <returns>A successful <see cref="TypeReaderResult"/>.</returns>
    public static TypeReaderResult FromSuccess(IReadOnlyCollection<TypeReaderValue> values)
        => new TypeReaderResult(values, null, null);
    /// <summary>
    /// Creates a result for a read that failed with the given error and reason.
    /// </summary>
    /// <param name="error">The type of error.</param>
    /// <param name="reason">The reason for the error.</param>
    /// <returns>A failed <see cref="TypeReaderResult"/>.</returns>
    public static TypeReaderResult FromError(CommandError error, string reason)
        => new TypeReaderResult(null, error, reason);
    /// <summary>
    /// Creates a result for a read that threw an exception.
    /// </summary>
    /// <param name="ex">The exception that caused the failure.</param>
    /// <returns>A failed <see cref="TypeReaderResult"/> with <see cref="CommandError.Exception"/>.</returns>
    public static TypeReaderResult FromError(Exception ex)
        => FromError(CommandError.Exception, ex.Message);
    /// <summary>
    /// Creates a result that copies the error and reason of another result.
    /// </summary>
    /// <param name="result">The result to copy from.</param>
    /// <returns>A failed <see cref="TypeReaderResult"/>.</returns>
    public static TypeReaderResult FromError(IResult result)
        => new TypeReaderResult(null, result.Error, result.ErrorReason);

    /// <inheritdoc />
    public override string ToString() => IsSuccess ? "Success" : $"{Error}: {ErrorReason}";
    private string DebuggerDisplay => IsSuccess ? $"Success ({string.Join(", ", Values)})" : $"{Error}: {ErrorReason}";
}
