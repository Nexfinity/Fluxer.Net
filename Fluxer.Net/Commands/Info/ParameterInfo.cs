using System.Collections.Immutable;
using System.Diagnostics;

namespace Fluxer.Net.Commands;

/// <summary>
/// The information of a command parameter.
/// </summary>
[DebuggerDisplay(@"{DebuggerDisplay,nq}")]
public class ParameterInfo
{
    private readonly TypeReader _reader;

    /// <summary>
    /// The command the parameter belongs to.
    /// </summary>
    public CommandInfo Command { get; }
    /// <summary>
    /// The name of the parameter.
    /// </summary>
    public string Name { get; }
    /// <summary>
    /// The summary of the parameter.
    /// </summary>
    public string Summary { get; }
    /// <summary>
    /// Whether the parameter is optional.
    /// </summary>
    public bool IsOptional { get; }
    /// <summary>
    /// Whether the parameter receives the rest of the input.
    /// </summary>
    public bool IsRemainder { get; }
    /// <summary>
    /// Whether the parameter is a params array that receives the remaining values.
    /// </summary>
    public bool IsMultiple { get; }
    /// <summary>
    /// The type of the parameter.
    /// </summary>
    public Type Type { get; }
    /// <summary>
    /// The default value of an optional parameter, or null.
    /// </summary>
    public object DefaultValue { get; }

    /// <summary>
    /// The preconditions that apply to the parameter.
    /// </summary>
    public IReadOnlyList<ParameterPreconditionAttribute> Preconditions { get; }
    /// <summary>
    /// The attributes of the parameter.
    /// </summary>
    public IReadOnlyList<Attribute> Attributes { get; }

    internal ParameterInfo(ParameterBuilder builder, CommandInfo command, CommandService service)
    {
        Command = command;

        Name = builder.Name;
        Summary = builder.Summary;
        IsOptional = builder.IsOptional;
        IsRemainder = builder.IsRemainder;
        IsMultiple = builder.IsMultiple;

        Type = builder.ParameterType;
        DefaultValue = builder.DefaultValue;

        Preconditions = builder.Preconditions.ToImmutableArray();
        Attributes = builder.Attributes.ToImmutableArray();

        _reader = builder.TypeReader;
    }

    /// <summary>
    /// Checks the preconditions of the parameter against its argument.
    /// </summary>
    /// <param name="context">The context of the command.</param>
    /// <param name="arg">The parsed argument of the parameter.</param>
    /// <param name="services">The service provider used for dependency injection.</param>
    /// <returns>The first failed precondition result, or a success result when all pass.</returns>
    public async Task<PreconditionResult> CheckPreconditionsAsync(ICommandContext context, object arg, IServiceProvider services = null)
    {
        services ??= EmptyServiceProvider.Instance;

        foreach (ParameterPreconditionAttribute precondition in Preconditions)
        {
            PreconditionResult result = await precondition.CheckPermissionsAsync(context, this, arg, services).ConfigureAwait(false);
            if (!result.IsSuccess)
                return result;
        }

        return PreconditionResult.FromSuccess();
    }

    /// <summary>
    /// Reads the parameter from the given input.
    /// </summary>
    /// <param name="context">The context of the command.</param>
    /// <param name="input">The raw input of the parameter.</param>
    /// <param name="services">The service provider used for dependency injection.</param>
    /// <returns>A task that represents the asynchronous read. The task result contains the read result.</returns>
    public Task<TypeReaderResult> ParseAsync(ICommandContext context, string input, IServiceProvider services = null)
    {
        services ??= EmptyServiceProvider.Instance;
        return _reader.ReadAsync(context, input, services);
    }

    /// <inheritdoc />
    public override string ToString() => Name;
    private string DebuggerDisplay => $"{Name}{(IsOptional ? " (Optional)" : "")}{(IsRemainder ? " (Remainder)" : "")}";
}
