using System.Reflection;

namespace Fluxer.Net.Commands;

/// <summary>
/// The builder for a command module that is turned into a <see cref="ModuleInfo"/>.
/// </summary>
public class ModuleBuilder
{
    #region ModuleBuilder
    private string _group;
    private readonly List<CommandBuilder> _commands;
    private readonly List<ModuleBuilder> _submodules;
    private readonly List<PreconditionAttribute> _preconditions;
    private readonly List<Attribute> _attributes;
    private readonly List<string> _aliases;

    /// <summary>
    /// The command service the module is built for.
    /// </summary>
    public CommandService Service { get; }
    /// <summary>
    /// The module that contains this module, or null for a top-level module.
    /// </summary>
    public ModuleBuilder Parent { get; }
    /// <summary>
    /// The name of the module. Defaults to the first alias when not set.
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// The summary of the module.
    /// </summary>
    public string Summary { get; set; }
    /// <summary>
    /// The remarks of the module.
    /// </summary>
    public string Remarks { get; set; }
    /// <summary>
    /// The group prefix of the module. Setting it replaces the previous group prefix in the aliases.
    /// </summary>
    public string Group
    {
        get => _group;
        set
        {
            _aliases.Remove(_group);
            _group = value;
            AddAliases(value);
        }
    }

    /// <summary>
    /// The commands of the module.
    /// </summary>
    public IReadOnlyList<CommandBuilder> Commands => _commands;
    /// <summary>
    /// The submodules of the module.
    /// </summary>
    public IReadOnlyList<ModuleBuilder> Modules => _submodules;
    /// <summary>
    /// The preconditions of the module.
    /// </summary>
    public IReadOnlyList<PreconditionAttribute> Preconditions => _preconditions;
    /// <summary>
    /// The attributes of the module.
    /// </summary>
    public IReadOnlyList<Attribute> Attributes => _attributes;
    /// <summary>
    /// The aliases of the module.
    /// </summary>
    public IReadOnlyList<string> Aliases => _aliases;

    internal TypeInfo TypeInfo { get; set; }
    #endregion

    #region Automatic
    internal ModuleBuilder(CommandService service, ModuleBuilder parent)
    {
        Service = service;
        Parent = parent;

        _commands = new List<CommandBuilder>();
        _submodules = new List<ModuleBuilder>();
        _preconditions = new List<PreconditionAttribute>();
        _attributes = new List<Attribute>();
        _aliases = new List<string>();
    }
    #endregion

    #region User-defined
    internal ModuleBuilder(CommandService service, ModuleBuilder parent, string primaryAlias)
        : this(service, parent)
    {
        if (string.IsNullOrEmpty(primaryAlias))
            throw new ArgumentException(nameof(primaryAlias));

        _aliases = new List<string> { primaryAlias };
    }

    /// <summary>
    /// Sets the name of the module.
    /// </summary>
    /// <param name="name">The name of the module.</param>
    public ModuleBuilder WithName(string name)
    {
        Name = name;
        return this;
    }
    /// <summary>
    /// Sets the summary of the module.
    /// </summary>
    /// <param name="summary">The summary of the module.</param>
    public ModuleBuilder WithSummary(string summary)
    {
        Summary = summary;
        return this;
    }
    /// <summary>
    /// Sets the remarks of the module.
    /// </summary>
    /// <param name="remarks">The remarks of the module.</param>
    public ModuleBuilder WithRemarks(string remarks)
    {
        Remarks = remarks;
        return this;
    }

    /// <summary>
    /// Adds aliases to the module, skipping duplicates. A null alias is added as an empty string.
    /// </summary>
    /// <param name="aliases">The aliases to add.</param>
    public ModuleBuilder AddAliases(params string[] aliases)
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
    /// Adds attributes to the module.
    /// </summary>
    /// <param name="attributes">The attributes to add.</param>
    public ModuleBuilder AddAttributes(params Attribute[] attributes)
    {
        _attributes.AddRange(attributes);
        return this;
    }
    /// <summary>
    /// Adds a precondition that commands in the module must pass.
    /// </summary>
    /// <param name="precondition">The precondition to add.</param>
    public ModuleBuilder AddPrecondition(PreconditionAttribute precondition)
    {
        _preconditions.Add(precondition);
        return this;
    }
    /// <summary>
    /// Adds a command with the given primary alias and configures it with <paramref name="createFunc"/>.
    /// </summary>
    /// <param name="primaryAlias">The alias that triggers the command.</param>
    /// <param name="callback">The method that runs the command.</param>
    /// <param name="createFunc">The action that configures the command.</param>
    public ModuleBuilder AddCommand(string primaryAlias, Func<ICommandContext, object[], IServiceProvider, CommandInfo, Task> callback, Action<CommandBuilder> createFunc)
    {
        CommandBuilder builder = new CommandBuilder(this, primaryAlias, callback);
        createFunc(builder);
        _commands.Add(builder);
        return this;
    }
    internal ModuleBuilder AddCommand(Action<CommandBuilder> createFunc)
    {
        CommandBuilder builder = new CommandBuilder(this);
        createFunc(builder);
        _commands.Add(builder);
        return this;
    }
    /// <summary>
    /// Adds a submodule with the given primary alias and configures it with <paramref name="createFunc"/>.
    /// </summary>
    /// <param name="primaryAlias">The alias of the submodule.</param>
    /// <param name="createFunc">The action that configures the submodule.</param>
    public ModuleBuilder AddModule(string primaryAlias, Action<ModuleBuilder> createFunc)
    {
        ModuleBuilder builder = new ModuleBuilder(Service, this, primaryAlias);
        createFunc(builder);
        _submodules.Add(builder);
        return this;
    }
    internal ModuleBuilder AddModule(Action<ModuleBuilder> createFunc)
    {
        ModuleBuilder builder = new ModuleBuilder(Service, this);
        createFunc(builder);
        _submodules.Add(builder);
        return this;
    }

    private ModuleInfo BuildImpl(CommandService service, IServiceProvider services, ModuleInfo parent = null)
    {
        //Default name to first alias
        if (Name == null)
            Name = _aliases[0];

        if (TypeInfo != null && !TypeInfo.IsAbstract)
        {
            IModuleBase moduleInstance = ReflectionUtils.CreateObject<IModuleBase>(TypeInfo, service, services);
            moduleInstance.OnModuleBuilding(service, this);
        }

        return new ModuleInfo(this, service, services, parent);
    }

    /// <summary>
    /// Builds the module. A module class is instantiated so that its <c>OnModuleBuilding</c> method runs.
    /// </summary>
    /// <param name="service">The command service the module is built for.</param>
    /// <param name="services">The service provider used to create the module class.</param>
    /// <returns>The built module.</returns>
    public ModuleInfo Build(CommandService service, IServiceProvider services) => BuildImpl(service, services);

    internal ModuleInfo Build(CommandService service, IServiceProvider services, ModuleInfo parent) => BuildImpl(service, services, parent);
    #endregion
}
