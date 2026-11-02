namespace Fluxer.Net.Commands;

/// <summary>
/// The builder for a command of a <see cref="ModuleBuilder"/>.
/// </summary>
public class CommandBuilder
{
    #region CommandBuilder
    private readonly List<PreconditionAttribute> _preconditions;
    private readonly List<ParameterBuilder> _parameters;
    private readonly List<Attribute> _attributes;
    private readonly List<string> _aliases;

    /// <summary>
    /// The module the command belongs to.
    /// </summary>
    public ModuleBuilder Module { get; }
    internal Func<ICommandContext, object[], IServiceProvider, CommandInfo, Task> Callback { get; set; }

    /// <summary>
    /// The name of the command. Defaults to the primary alias when not set.
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// The summary of the command.
    /// </summary>
    public string Summary { get; set; }
    /// <summary>
    /// The remarks of the command.
    /// </summary>
    public string Remarks { get; set; }
    /// <summary>
    /// The alias the command was created with, or null when it was created from a method attribute.
    /// </summary>
    public string PrimaryAlias { get; set; }
    /// <summary>
    /// The <see cref="RunMode"/> of the command.
    /// </summary>
    public RunMode RunMode { get; set; }
    /// <summary>
    /// The priority of the command when several overloads match the same input.
    /// </summary>
    public int Priority { get; set; }
    /// <summary>
    /// Whether extra arguments are ignored for the command.
    /// </summary>
    public bool IgnoreExtraArgs { get; set; }

    /// <summary>
    /// The preconditions of the command.
    /// </summary>
    public IReadOnlyList<PreconditionAttribute> Preconditions => _preconditions;
    /// <summary>
    /// The parameters of the command, in order.
    /// </summary>
    public IReadOnlyList<ParameterBuilder> Parameters => _parameters;
    /// <summary>
    /// The attributes of the command.
    /// </summary>
    public IReadOnlyList<Attribute> Attributes => _attributes;
    /// <summary>
    /// The aliases of the command.
    /// </summary>
    public IReadOnlyList<string> Aliases => _aliases;
    #endregion

    #region Automatic
    internal CommandBuilder(ModuleBuilder module)
    {
        Module = module;

        _preconditions = new List<PreconditionAttribute>();
        _parameters = new List<ParameterBuilder>();
        _attributes = new List<Attribute>();
        _aliases = new List<string>();
    }
    #endregion

    #region User-defined
    internal CommandBuilder(ModuleBuilder module, string primaryAlias, Func<ICommandContext, object[], IServiceProvider, CommandInfo, Task> callback)
        : this(module)
    {
        if (string.IsNullOrEmpty(primaryAlias))
            throw new ArgumentException(nameof(primaryAlias));

        if (callback == null)
            throw new ArgumentException(nameof(callback));

        Callback = callback;
        PrimaryAlias = primaryAlias;
        _aliases.Add(primaryAlias);
    }

    /// <summary>
    /// Sets the name of the command.
    /// </summary>
    /// <param name="name">The name of the command.</param>
    public CommandBuilder WithName(string name)
    {
        Name = name;
        return this;
    }
    /// <summary>
    /// Sets the summary of the command.
    /// </summary>
    /// <param name="summary">The summary of the command.</param>
    public CommandBuilder WithSummary(string summary)
    {
        Summary = summary;
        return this;
    }
    /// <summary>
    /// Sets the remarks of the command.
    /// </summary>
    /// <param name="remarks">The remarks of the command.</param>
    public CommandBuilder WithRemarks(string remarks)
    {
        Remarks = remarks;
        return this;
    }
    /// <summary>
    /// Sets the <see cref="RunMode"/> of the command.
    /// </summary>
    /// <param name="runMode">The run mode of the command.</param>
    public CommandBuilder WithRunMode(RunMode runMode)
    {
        RunMode = runMode;
        return this;
    }
    /// <summary>
    /// Sets the priority of the command.
    /// </summary>
    /// <param name="priority">The priority of the command.</param>
    public CommandBuilder WithPriority(int priority)
    {
        Priority = priority;
        return this;
    }

    /// <summary>
    /// Adds aliases to the command, skipping duplicates. A null alias is added as an empty string.
    /// </summary>
    /// <param name="aliases">The aliases to add.</param>
    public CommandBuilder AddAliases(params string[] aliases)
    {
        for (int i = 0; i < aliases.Length; i++)
        {
            string alias = aliases[i] ?? "";
            if (!_aliases.Contains(alias))
                _aliases.Add(alias);
        }
        return this;
    }
    /// <summary>
    /// Adds attributes to the command.
    /// </summary>
    /// <param name="attributes">The attributes to add.</param>
    public CommandBuilder AddAttributes(params Attribute[] attributes)
    {
        _attributes.AddRange(attributes);
        return this;
    }
    /// <summary>
    /// Adds a precondition that the command must pass.
    /// </summary>
    /// <param name="precondition">The precondition to add.</param>
    public CommandBuilder AddPrecondition(PreconditionAttribute precondition)
    {
        _preconditions.Add(precondition);
        return this;
    }
    /// <summary>
    /// Adds a parameter of type <typeparamref name="T"/> and configures it with <paramref name="createFunc"/>.
    /// </summary>
    /// <typeparam name="T">The type of the parameter.</typeparam>
    /// <param name="name">The name of the parameter.</param>
    /// <param name="createFunc">The action that configures the parameter.</param>
    public CommandBuilder AddParameter<T>(string name, Action<ParameterBuilder> createFunc)
    {
        ParameterBuilder param = new ParameterBuilder(this, name, typeof(T));
        createFunc(param);
        _parameters.Add(param);
        return this;
    }
    /// <summary>
    /// Adds a parameter of the given type and configures it with <paramref name="createFunc"/>.
    /// </summary>
    /// <param name="name">The name of the parameter.</param>
    /// <param name="type">The type of the parameter.</param>
    /// <param name="createFunc">The action that configures the parameter.</param>
    public CommandBuilder AddParameter(string name, Type type, Action<ParameterBuilder> createFunc)
    {
        ParameterBuilder param = new ParameterBuilder(this, name, type);
        createFunc(param);
        _parameters.Add(param);
        return this;
    }
    internal CommandBuilder AddParameter(Action<ParameterBuilder> createFunc)
    {
        ParameterBuilder param = new ParameterBuilder(this);
        createFunc(param);
        _parameters.Add(param);
        return this;
    }

    /// <exception cref="InvalidOperationException">Only the last parameter in a command may have the Remainder or Multiple flag.</exception>
    internal CommandInfo Build(ModuleInfo info, CommandService service)
    {
        //Default name to primary alias
        if (Name == null)
            Name = PrimaryAlias;

        if (_parameters.Count > 0)
        {
            ParameterBuilder lastParam = _parameters[_parameters.Count - 1];

            ParameterBuilder firstMultipleParam = _parameters.FirstOrDefault(x => x.IsMultiple);
            if ((firstMultipleParam != null) && (firstMultipleParam != lastParam))
                throw new InvalidOperationException($"Only the last parameter in a command may have the Multiple flag. Parameter: {firstMultipleParam.Name} in {PrimaryAlias}");

            ParameterBuilder firstRemainderParam = _parameters.FirstOrDefault(x => x.IsRemainder);
            if ((firstRemainderParam != null) && (firstRemainderParam != lastParam))
                throw new InvalidOperationException($"Only the last parameter in a command may have the Remainder flag. Parameter: {firstRemainderParam.Name} in {PrimaryAlias}");
        }

        return new CommandInfo(this, info, service);
    }
    #endregion
}
