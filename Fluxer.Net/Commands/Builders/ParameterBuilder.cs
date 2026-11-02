using System.Reflection;

namespace Fluxer.Net.Commands;

/// <summary>
/// The builder for a parameter of a <see cref="CommandBuilder"/>.
/// </summary>
public class ParameterBuilder
{
    #region ParameterBuilder
    private readonly List<ParameterPreconditionAttribute> _preconditions;
    private readonly List<Attribute> _attributes;

    /// <summary>
    /// The command the parameter belongs to.
    /// </summary>
    public CommandBuilder Command { get; }
    /// <summary>
    /// The name of the parameter.
    /// </summary>
    public string Name { get; internal set; }
    /// <summary>
    /// The type of the parameter.
    /// </summary>
    public Type ParameterType { get; internal set; }

    /// <summary>
    /// The <see cref="TypeReader"/> that reads the parameter from the input.
    /// </summary>
    public TypeReader TypeReader { get; internal set; }
    /// <summary>
    /// Whether the parameter is optional.
    /// </summary>
    public bool IsOptional { get; internal set; }
    /// <summary>
    /// Whether the parameter receives the rest of the input.
    /// </summary>
    public bool IsRemainder { get; internal set; }
    /// <summary>
    /// Whether the parameter is a params array that receives the remaining values.
    /// </summary>
    public bool IsMultiple { get; internal set; }
    /// <summary>
    /// The default value of the parameter.
    /// </summary>
    public object DefaultValue { get; internal set; }
    /// <summary>
    /// The summary of the parameter.
    /// </summary>
    public string Summary { get; internal set; }

    /// <summary>
    /// The preconditions of the parameter.
    /// </summary>
    public IReadOnlyList<ParameterPreconditionAttribute> Preconditions => _preconditions;
    /// <summary>
    /// The attributes of the parameter.
    /// </summary>
    public IReadOnlyList<Attribute> Attributes => _attributes;
    #endregion

    #region Automatic
    internal ParameterBuilder(CommandBuilder command)
    {
        _preconditions = new List<ParameterPreconditionAttribute>();
        _attributes = new List<Attribute>();

        Command = command;
    }
    #endregion

    #region User-defined
    internal ParameterBuilder(CommandBuilder command, string name, Type type)
        : this(command)
    {
        if (string.IsNullOrEmpty(name))
            throw new ArgumentException(null, nameof(name));

        Name = name;
        SetType(type);
    }

    internal void SetType(Type type)
    {
        TypeReader = GetReader(type);

        if (type.GetTypeInfo().IsValueType)
            DefaultValue = Activator.CreateInstance(type);
        else if (type.IsArray)
            DefaultValue = Array.CreateInstance(type.GetElementType(), 0);
        ParameterType = type;
    }

    private TypeReader GetReader(Type type)
    {
        CommandService commands = Command.Module.Service;
        if (type.GetTypeInfo().GetCustomAttribute<NamedArgumentTypeAttribute>() != null)
        {
            IsRemainder = true;
            TypeReader reader = commands.GetTypeReaders(type)?.FirstOrDefault().Value;
            if (reader == null)
            {
                Type readerType;
                try
                {
                    readerType = typeof(NamedArgumentTypeReader<>).MakeGenericType(new[] { type });
                }
                catch (ArgumentException ex)
                {
                    throw new InvalidOperationException($"Parameter type '{type.Name}' for command '{Command.Name}' must be a class with a public parameterless constructor to use as a NamedArgumentType.", ex);
                }

                reader = (TypeReader)Activator.CreateInstance(readerType, new[] { commands });
                commands.AddTypeReader(type, reader);
            }

            return reader;
        }


        IDictionary<Type, TypeReader> readers = commands.GetTypeReaders(type);
        if (readers != null)
            return readers.FirstOrDefault().Value;
        else
            return commands.GetDefaultTypeReader(type);
    }

    /// <summary>
    /// Sets the summary of the parameter.
    /// </summary>
    /// <param name="summary">The summary of the parameter.</param>
    public ParameterBuilder WithSummary(string summary)
    {
        Summary = summary;
        return this;
    }
    /// <summary>
    /// Sets the default value of the parameter.
    /// </summary>
    /// <param name="defaultValue">The default value.</param>
    public ParameterBuilder WithDefault(object defaultValue)
    {
        DefaultValue = defaultValue;
        return this;
    }
    /// <summary>
    /// Sets whether the parameter is optional.
    /// </summary>
    /// <param name="isOptional">Whether the parameter is optional.</param>
    public ParameterBuilder WithIsOptional(bool isOptional)
    {
        IsOptional = isOptional;
        return this;
    }
    /// <summary>
    /// Sets whether the parameter receives the rest of the input.
    /// </summary>
    /// <param name="isRemainder">Whether the parameter is a remainder parameter.</param>
    public ParameterBuilder WithIsRemainder(bool isRemainder)
    {
        IsRemainder = isRemainder;
        return this;
    }
    /// <summary>
    /// Sets whether the parameter is a params array.
    /// </summary>
    /// <param name="isMultiple">Whether the parameter is a params array.</param>
    public ParameterBuilder WithIsMultiple(bool isMultiple)
    {
        IsMultiple = isMultiple;
        return this;
    }

    /// <summary>
    /// Adds attributes to the parameter.
    /// </summary>
    /// <param name="attributes">The attributes to add.</param>
    public ParameterBuilder AddAttributes(params Attribute[] attributes)
    {
        _attributes.AddRange(attributes);
        return this;
    }
    /// <summary>
    /// Adds a precondition that the argument of the parameter must pass.
    /// </summary>
    /// <param name="precondition">The precondition to add.</param>
    public ParameterBuilder AddPrecondition(ParameterPreconditionAttribute precondition)
    {
        _preconditions.Add(precondition);
        return this;
    }

    internal ParameterInfo Build(CommandInfo info)
    {
        if ((TypeReader ??= GetReader(ParameterType)) == null)
            throw new InvalidOperationException($"No type reader found for type {ParameterType.Name}, one must be specified");

        return new ParameterInfo(this, info, Command.Module.Service);
    }
    #endregion
}
