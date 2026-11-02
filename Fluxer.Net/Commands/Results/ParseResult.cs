using System.Diagnostics;

namespace Fluxer.Net.Commands;

/// <summary>
/// The result of parsing the input of a command.
/// </summary>
[DebuggerDisplay(@"{DebuggerDisplay,nq}")]
public struct ParseResult : IResult
{
    /// <summary>
    /// The parsed values of the command's parameters, in order.
    /// </summary>
    public IReadOnlyList<TypeReaderResult> ArgValues { get; }
    /// <summary>
    /// The parsed values of the params parameter, one for each value in the input.
    /// </summary>
    public IReadOnlyList<TypeReaderResult> ParamValues { get; }

    /// <inheritdoc/>
    public CommandError? Error { get; }
    /// <inheritdoc/>
    public string ErrorReason { get; }

    /// <summary>
    /// The parameter that caused the parsing error, or null when the error is not specific to one parameter.
    /// </summary>
    public ParameterInfo ErrorParameter { get; }

    /// <inheritdoc/>
    public bool IsSuccess => !Error.HasValue;

    private ParseResult(IReadOnlyList<TypeReaderResult> argValues, IReadOnlyList<TypeReaderResult> paramValues, CommandError? error, string errorReason, ParameterInfo errorParamInfo)
    {
        ArgValues = argValues;
        ParamValues = paramValues;
        Error = error;
        ErrorReason = errorReason;
        ErrorParameter = errorParamInfo;
    }

    /// <summary>
    /// Creates a result for a successful parse. Returns a <see cref="CommandError.MultipleMatches"/> error when a
    /// value matched more than one object.
    /// </summary>
    /// <param name="argValues">The parsed values of the parameters.</param>
    /// <param name="paramValues">The parsed values of the params parameter.</param>
    /// <returns>The parse result.</returns>
    public static ParseResult FromSuccess(IReadOnlyList<TypeReaderResult> argValues, IReadOnlyList<TypeReaderResult> paramValues)
    {
        for (int i = 0; i < argValues.Count; i++)
        {
            if (argValues[i].Values.Count > 1)
                return new ParseResult(argValues, paramValues, CommandError.MultipleMatches, "Multiple matches found.", null);
        }
        for (int i = 0; i < paramValues.Count; i++)
        {
            if (paramValues[i].Values.Count > 1)
                return new ParseResult(argValues, paramValues, CommandError.MultipleMatches, "Multiple matches found.", null);
        }
        return new ParseResult(argValues, paramValues, null, null, null);
    }
    /// <summary>
    /// Creates a result for a successful parse from single values.
    /// </summary>
    /// <param name="argValues">The parsed values of the parameters.</param>
    /// <param name="paramValues">The parsed values of the params parameter, or null when there is none.</param>
    /// <returns>The parse result.</returns>
    public static ParseResult FromSuccess(IReadOnlyList<TypeReaderValue> argValues, IReadOnlyList<TypeReaderValue> paramValues)
    {
        TypeReaderResult[] argList = new TypeReaderResult[argValues.Count];
        for (int i = 0; i < argValues.Count; i++)
            argList[i] = TypeReaderResult.FromSuccess(argValues[i]);
        TypeReaderResult[] paramList = null;
        if (paramValues != null)
        {
            paramList = new TypeReaderResult[paramValues.Count];
            for (int i = 0; i < paramValues.Count; i++)
                paramList[i] = TypeReaderResult.FromSuccess(paramValues[i]);
        }
        return new ParseResult(argList, paramList, null, null, null);
    }

    /// <summary>
    /// Creates a result for a failed parse with the given error and reason.
    /// </summary>
    /// <param name="error">The type of error.</param>
    /// <param name="reason">The reason for the error.</param>
    /// <returns>A failed <see cref="ParseResult"/>.</returns>
    public static ParseResult FromError(CommandError error, string reason)
        => new ParseResult(null, null, error, reason, null);
    /// <summary>
    /// Creates a result for a failed parse with the given error, reason and parameter.
    /// </summary>
    /// <param name="error">The type of error.</param>
    /// <param name="reason">The reason for the error.</param>
    /// <param name="parameterInfo">The parameter that caused the error.</param>
    /// <returns>A failed <see cref="ParseResult"/>.</returns>
    public static ParseResult FromError(CommandError error, string reason, ParameterInfo parameterInfo)
        => new ParseResult(null, null, error, reason, parameterInfo);
    /// <summary>
    /// Creates a result for a parse that threw an exception.
    /// </summary>
    /// <param name="ex">The exception that caused the failure.</param>
    /// <returns>A failed <see cref="ParseResult"/> with <see cref="CommandError.Exception"/>.</returns>
    public static ParseResult FromError(Exception ex)
        => FromError(CommandError.Exception, ex.Message);
    /// <summary>
    /// Creates a result that copies the error and reason of another result.
    /// </summary>
    /// <param name="result">The result to copy from.</param>
    /// <returns>A failed <see cref="ParseResult"/>.</returns>
    public static ParseResult FromError(IResult result)
        => new ParseResult(null, null, result.Error, result.ErrorReason, null);
    /// <summary>
    /// Creates a result that copies the error and reason of another result, for the given parameter.
    /// </summary>
    /// <param name="result">The result to copy from.</param>
    /// <param name="parameterInfo">The parameter that caused the error.</param>
    /// <returns>A failed <see cref="ParseResult"/>.</returns>
    public static ParseResult FromError(IResult result, ParameterInfo parameterInfo)
        => new ParseResult(null, null, result.Error, result.ErrorReason, parameterInfo);

    /// <inheritdoc />
    public override string ToString() => IsSuccess ? "Success" : $"{Error}: {ErrorReason}";
    private string DebuggerDisplay => IsSuccess ? $"Success ({ArgValues.Count}{(ParamValues.Count > 0 ? $" +{ParamValues.Count} Values" : "")})" : $"{Error}: {ErrorReason}";
}
