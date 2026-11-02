using Serilog;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Reflection;

namespace Fluxer.Net.Commands;

/// <summary>
/// The service that holds command modules and TypeReaders, matches input to commands and executes them.
/// </summary>
/// <remarks>
/// Modules can be declared at compile time by deriving from <see cref="ModuleBase"/>, or built at run time with
/// <see cref="ModuleBuilder"/>.
/// </remarks>
public class CommandService : IDisposable
{
    #region CommandService
    // <summary>
    //     Occurs when a command-related information is received.
    // </summary>
    //public event Func<LogMessage, Task> Log { add { _logEvent.Add(value); } remove { _logEvent.Remove(value); } }
    //internal readonly AsyncEvent<Func<LogMessage, Task>> _logEvent = new AsyncEvent<Func<LogMessage, Task>>();

    /// <summary>
    /// Raised when a command is executed, whether it succeeded or not.
    /// </summary>
    /// <remarks>
    /// The command is null when the input did not match a command. Commands that run with <see cref="RunMode.Async"/>
    /// report their outcome only through this event.
    /// </remarks>
    public event Func<CommandInfo?, ICommandContext, IResult, Task>? CommandExecuted;

    internal async Task RaiseCommandExecutedAsync(CommandInfo? command, ICommandContext context, IResult result)
    {
        Func<CommandInfo?, ICommandContext, IResult, Task>? handlers = CommandExecuted;
        if (handlers == null)
            return;

        foreach (Func<CommandInfo?, ICommandContext, IResult, Task> handler in handlers.GetInvocationList()
                     .Cast<Func<CommandInfo?, ICommandContext, IResult, Task>>())
        {
            try
            {
                await handler(command, context, result).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger?.Error(ex, "A CommandExecuted handler threw an exception.");
            }
        }
    }

    private readonly SemaphoreSlim _moduleLock;
    private readonly ConcurrentDictionary<Type, ModuleInfo> _typedModuleDefs;
    private readonly ConcurrentDictionary<Type, ConcurrentDictionary<Type, TypeReader>> _typeReaders;
    private readonly ConcurrentDictionary<Type, TypeReader> _defaultTypeReaders;
    private readonly ImmutableList<(Type EntityType, Type TypeReaderType)> _entityTypeReaders;
    private readonly HashSet<ModuleInfo> _moduleDefs;
    private readonly CommandMap _map;

    internal readonly bool _caseSensitive, _throwOnError, _ignoreExtraArgs, _ownerBypassPermissions;
    internal readonly ulong[]? _ownerIds;
    internal readonly char _separatorChar;
    internal readonly RunMode _defaultRunMode;
    internal readonly ILogger? _logger;
    internal readonly IReadOnlyDictionary<char, char> _quotationMarkAliasMap;

    internal bool _isDisposed;

    /// <summary>
    /// The modules loaded into the service.
    /// </summary>
    public IEnumerable<ModuleInfo> Modules => _moduleDefs.Select(x => x);

    /// <summary>
    /// The commands of all modules loaded into the service.
    /// </summary>
    public IEnumerable<CommandInfo> Commands => _moduleDefs.SelectMany(x => x.Commands);

    /// <summary>
    /// The custom <see cref="TypeReader"/>s added to the service, grouped by the type they read.
    /// </summary>
    public ILookup<Type, TypeReader> TypeReaders => _typeReaders.SelectMany(x => x.Value.Select(y => new { y.Key, y.Value })).ToLookup(x => x.Key, x => x.Value);

    /// <summary>
    /// Creates a command service with the default configuration.
    /// </summary>
    public CommandService() : this(new CommandServiceConfig()) { }

    /// <summary>
    /// Creates a command service with the given configuration.
    /// </summary>
    /// <param name="config">The configuration of the service.</param>
    /// <exception cref="InvalidOperationException">
    /// The <see cref="RunMode"/> cannot be set to <see cref="RunMode.Default"/>.
    /// </exception>
    public CommandService(CommandServiceConfig config)
    {
        _caseSensitive = config.CaseSensitiveCommands;
        _ownerBypassPermissions = config.OwnerBypassPermissions;
        _ownerIds = config.OwnerIDs;
        _throwOnError = config.ThrowOnError;
        _ignoreExtraArgs = config.IgnoreExtraArgs;
        _separatorChar = config.SeparatorChar;
        _defaultRunMode = config.DefaultRunMode;
        _quotationMarkAliasMap = (config.QuotationMarkAliasMap ?? new Dictionary<char, char>()).ToImmutableDictionary();
        if (_defaultRunMode == RunMode.Default)
            throw new InvalidOperationException("The default run mode cannot be set to Default.");

        _logger = config.Logger;
        //_logManager = new LogManager(config.LogLevel);
        //_logManager.Message += async msg => await _logEvent.InvokeAsync(msg).ConfigureAwait(false);
        //_cmdLogger = _logManager.CreateLogger("Command");

        _moduleLock = new SemaphoreSlim(1, 1);
        _typedModuleDefs = new ConcurrentDictionary<Type, ModuleInfo>();
        _moduleDefs = new HashSet<ModuleInfo>();
        _map = new CommandMap(this);
        _typeReaders = new ConcurrentDictionary<Type, ConcurrentDictionary<Type, TypeReader>>();

        _defaultTypeReaders = new ConcurrentDictionary<Type, TypeReader>();
        foreach (Type type in PrimitiveParsers.SupportedTypes)
        {
            _defaultTypeReaders[type] = PrimitiveTypeReader.Create(type);
            _defaultTypeReaders[typeof(Nullable<>).MakeGenericType(type)] = NullableTypeReader.Create(type, _defaultTypeReaders[type]);
        }

        TimeSpanTypeReader tsreader = new TimeSpanTypeReader();
        _defaultTypeReaders[typeof(TimeSpan)] = tsreader;
        _defaultTypeReaders[typeof(TimeSpan?)] = NullableTypeReader.Create(typeof(TimeSpan), tsreader);

        _defaultTypeReaders[typeof(string)] =
            new PrimitiveTypeReader<string>((string x, out string y) => { y = x; return true; }, 0);

        ImmutableList<(Type, Type)>.Builder entityTypeReaders = ImmutableList.CreateBuilder<(Type, Type)>();
        entityTypeReaders.Add((typeof(IMessage), typeof(MessageTypeReader<>)));
        entityTypeReaders.Add((typeof(IChannel), typeof(ChannelTypeReader<>)));
        entityTypeReaders.Add((typeof(IRole), typeof(RoleTypeReader<>)));
        entityTypeReaders.Add((typeof(IUser), typeof(UserTypeReader<>)));
        entityTypeReaders.Add((typeof(IGuildMember), typeof(MemberTypeReader<>)));
        _entityTypeReaders = entityTypeReaders.ToImmutable();
    }
    #endregion

    #region Modules
    /// <summary>
    /// Creates a module with the given primary alias and loads it into the service.
    /// </summary>
    /// <param name="primaryAlias">The primary alias of the module.</param>
    /// <param name="buildFunc">The action that configures the module.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains the created module.
    /// </returns>
    public async Task<ModuleInfo> CreateModuleAsync(string primaryAlias, Action<ModuleBuilder> buildFunc)
    {
        await _moduleLock.WaitAsync().ConfigureAwait(false);
        try
        {
            ModuleBuilder builder = new ModuleBuilder(this, null, primaryAlias);
            buildFunc(builder);

            ModuleInfo module = builder.Build(this, null);

            return LoadModuleInternal(module);
        }
        finally
        {
            _moduleLock.Release();
        }
    }

    /// <summary>
    /// Adds the command module of type <typeparamref name="T"/>.
    /// </summary>
    /// <example>
    /// <para>The following example registers the module <c>MyModule</c> to <c>commandService</c>.</para>
    /// <code language="cs">
    /// await commandService.AddModuleAsync&lt;MyModule&gt;(serviceProvider);
    /// </code>
    /// </example>
    /// <typeparam name="T">The type of module.</typeparam>
    /// <param name="services">The <see cref="IServiceProvider"/> for your dependency injection solution if using one, or <see langword="null"/> otherwise.</param>
    /// <exception cref="ArgumentException">This module has already been added.</exception>
    /// <exception cref="InvalidOperationException">
    /// The <see cref="ModuleInfo"/> could not be built. An invalid type may have been provided.
    /// </exception>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains the added module.
    /// </returns>
    public Task<ModuleInfo> AddModuleAsync<T>(IServiceProvider services = null) => AddModuleAsync(typeof(T), services);

    /// <summary>
    /// Adds a command module from a <see cref="Type"/>.
    /// </summary>
    /// <param name="type">The type of module.</param>
    /// <param name="services">The <see cref="IServiceProvider"/> for your dependency injection solution if using one, or <see langword="null"/> otherwise.</param>
    /// <exception cref="ArgumentException">This module has already been added.</exception>
    /// <exception cref="InvalidOperationException">
    /// The <see cref="ModuleInfo"/> could not be built. An invalid type may have been provided.
    /// </exception>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains the added module.
    /// </returns>
    public async Task<ModuleInfo> AddModuleAsync(Type type, IServiceProvider services = null)
    {
        services ??= EmptyServiceProvider.Instance;

        await _moduleLock.WaitAsync().ConfigureAwait(false);
        try
        {
            TypeInfo typeInfo = type.GetTypeInfo();

            if (_typedModuleDefs.ContainsKey(type))
                throw new ArgumentException("This module has already been added.");

            KeyValuePair<Type, ModuleInfo> module = (await ModuleClassBuilder.BuildAsync(this, services, typeInfo).ConfigureAwait(false)).FirstOrDefault();

            if (module.Value == default(ModuleInfo))
                throw new InvalidOperationException($"Could not build the module {type.FullName}, did you pass an invalid type?");

            _typedModuleDefs[module.Key] = module.Value;

            return LoadModuleInternal(module.Value);
        }
        finally
        {
            _moduleLock.Release();
        }
    }
    /// <summary>
    /// Adds the command modules found in an <see cref="Assembly"/>.
    /// </summary>
    /// <param name="assembly">The <see cref="Assembly"/> containing command modules.</param>
    /// <param name="services">The <see cref="IServiceProvider"/> for your dependency injection solution if using one, or <see langword="null"/> otherwise.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains the modules added.
    /// </returns>
    public async Task<IEnumerable<ModuleInfo>> AddModulesAsync(Assembly assembly, IServiceProvider services = null)
    {
        services ??= EmptyServiceProvider.Instance;

        await _moduleLock.WaitAsync().ConfigureAwait(false);
        try
        {
            IReadOnlyList<TypeInfo> types = await ModuleClassBuilder.SearchAsync(assembly, this).ConfigureAwait(false);
            Dictionary<Type, ModuleInfo> moduleDefs = await ModuleClassBuilder.BuildAsync(types, this, services).ConfigureAwait(false);

            foreach (KeyValuePair<Type, ModuleInfo> info in moduleDefs)
            {
                _typedModuleDefs[info.Key] = info.Value;
                LoadModuleInternal(info.Value);
            }

            return moduleDefs.Select(x => x.Value).ToImmutableArray();
        }
        finally
        {
            _moduleLock.Release();
        }
    }
    private ModuleInfo LoadModuleInternal(ModuleInfo module)
    {
        _moduleDefs.Add(module);

        foreach (CommandInfo command in module.Commands)
            _map.AddCommand(command);

        foreach (ModuleInfo submodule in module.Submodules)
            LoadModuleInternal(submodule);

        return module;
    }
    /// <summary>
    /// Removes a module and its commands from the service.
    /// </summary>
    /// <param name="module">The <see cref="ModuleInfo"/> to be removed from the service.</param>
    /// <returns>
    /// A task that represents the asynchronous removal operation. The task result is <see langword="true"/> if the
    /// <paramref name="module"/> was removed, and <see langword="false"/> otherwise.
    /// </returns>
    public async Task<bool> RemoveModuleAsync(ModuleInfo module)
    {
        await _moduleLock.WaitAsync().ConfigureAwait(false);
        try
        {
            KeyValuePair<Type, ModuleInfo> typeModulePair = _typedModuleDefs.FirstOrDefault(x => x.Value.Equals(module));

            if (!typeModulePair.Equals(default(KeyValuePair<Type, ModuleInfo>)))
                _typedModuleDefs.TryRemove(typeModulePair.Key, out ModuleInfo _);

            return RemoveModuleInternal(module);
        }
        finally
        {
            _moduleLock.Release();
        }
    }
    /// <summary>
    /// Removes the module of type <typeparamref name="T"/> from the service.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type"/> of the module.</typeparam>
    /// <returns>
    /// A task that represents the asynchronous removal operation. The task result is <see langword="true"/> if the
    /// module was removed, and <see langword="false"/> otherwise.
    /// </returns>
    public Task<bool> RemoveModuleAsync<T>() => RemoveModuleAsync(typeof(T));
    /// <summary>
    /// Removes the module of the given type from the service.
    /// </summary>
    /// <param name="type">The <see cref="Type"/> of the module.</param>
    /// <returns>
    /// A task that represents the asynchronous removal operation. The task result is <see langword="true"/> if the
    /// module was removed, and <see langword="false"/> otherwise.
    /// </returns>
    public async Task<bool> RemoveModuleAsync(Type type)
    {
        await _moduleLock.WaitAsync().ConfigureAwait(false);
        try
        {
            if (!_typedModuleDefs.TryRemove(type, out ModuleInfo module))
                return false;

            return RemoveModuleInternal(module);
        }
        finally
        {
            _moduleLock.Release();
        }
    }
    private bool RemoveModuleInternal(ModuleInfo module)
    {
        if (!_moduleDefs.Remove(module))
            return false;

        foreach (CommandInfo cmd in module.Commands)
            _map.RemoveCommand(cmd);

        foreach (ModuleInfo submodule in module.Submodules)
        {
            RemoveModuleInternal(submodule);
        }

        return true;
    }
    #endregion

    #region Type Readers
    /// <summary>
    /// Adds a custom <see cref="TypeReader"/> for <typeparamref name="T"/>.
    /// </summary>
    /// <remarks>
    /// If <typeparamref name="T"/> is a value type, a nullable <see cref="TypeReader"/> is also added. If a default
    /// <see cref="TypeReader"/> exists for <typeparamref name="T"/>, a warning is logged and the default is replaced.
    /// </remarks>
    /// <typeparam name="T">The object type to be read by the <see cref="TypeReader"/>.</typeparam>
    /// <param name="reader">The <see cref="TypeReader"/> to be added.</param>
    public void AddTypeReader<T>(TypeReader reader)
        => AddTypeReader(typeof(T), reader);
    /// <summary>
    /// Adds a custom <see cref="TypeReader"/> for the given type.
    /// </summary>
    /// <remarks>
    /// If <paramref name="type"/> is a value type, a nullable <see cref="TypeReader"/> is also added. If a default
    /// <see cref="TypeReader"/> exists for <paramref name="type"/>, a warning is logged and the default is replaced.
    /// </remarks>
    /// <param name="type">The type to be read.</param>
    /// <param name="reader">The <see cref="TypeReader"/> to be added.</param>
    public void AddTypeReader(Type type, TypeReader reader)
    {
        if (_defaultTypeReaders.ContainsKey(type))
            _logger?.Warning($"The default TypeReader for {type.FullName} was replaced by {reader.GetType().FullName}." +
                "To suppress this message, use AddTypeReader<T>(reader, true).");
        AddTypeReader(type, reader, true);
    }
    /// <summary>
    /// Adds a custom <see cref="TypeReader"/> for <typeparamref name="T"/>.
    /// </summary>
    /// <remarks>
    /// If <typeparamref name="T"/> is a value type, a nullable <see cref="TypeReader"/> is also added.
    /// </remarks>
    /// <typeparam name="T">The object type to be read by the <see cref="TypeReader"/>.</typeparam>
    /// <param name="reader">The <see cref="TypeReader"/> to be added.</param>
    /// <param name="replaceDefault">
    /// Whether the <see cref="TypeReader"/> replaces the default one for <typeparamref name="T"/>, if one exists.
    /// </param>
    public void AddTypeReader<T>(TypeReader reader, bool replaceDefault)
        => AddTypeReader(typeof(T), reader, replaceDefault);
    /// <summary>
    /// Adds a custom <see cref="TypeReader"/> for the given type.
    /// </summary>
    /// <remarks>
    /// If <paramref name="type"/> is a value type, a nullable <see cref="TypeReader"/> for the value type is also added.
    /// </remarks>
    /// <param name="type">The type to be read.</param>
    /// <param name="reader">The <see cref="TypeReader"/> to be added.</param>
    /// <param name="replaceDefault">
    /// Whether the <see cref="TypeReader"/> replaces the default one for <paramref name="type"/>, if one exists.
    /// </param>
    public void AddTypeReader(Type type, TypeReader reader, bool replaceDefault)
    {
        if (replaceDefault && HasDefaultTypeReader(type))
        {
            _defaultTypeReaders.AddOrUpdate(type, reader, (k, v) => reader);
            if (type.GetTypeInfo().IsValueType)
            {
                Type nullableType = typeof(Nullable<>).MakeGenericType(type);
                TypeReader nullableReader = NullableTypeReader.Create(type, reader);
                _defaultTypeReaders.AddOrUpdate(nullableType, nullableReader, (k, v) => nullableReader);
            }
        }
        else
        {
            ConcurrentDictionary<Type, TypeReader> readers = _typeReaders.GetOrAdd(type, x => new ConcurrentDictionary<Type, TypeReader>());
            readers[reader.GetType()] = reader;

            if (type.GetTypeInfo().IsValueType)
                AddNullableTypeReader(type, reader);
        }
    }

    /// <summary>
    /// Removes the <see cref="TypeReader"/>s registered for a type.
    /// </summary>
    /// <remarks>
    /// Removing a <see cref="TypeReader"/> from the <see cref="CommandService"/> does not change the <see cref="TypeReader"/>s
    /// already held by loaded modules and commands. Reload the modules for the change to take effect.
    /// </remarks>
    /// <param name="type">The type to remove the readers from.</param>
    /// <param name="isDefaultTypeReader">Whether the default reader for <paramref name="type"/> is removed instead of the added readers.</param>
    /// <param name="readers">The removed collection of type readers.</param>
    /// <returns><see langword="true"/> if the remove operation was successful, and <see langword="false"/> otherwise.</returns>
    public bool TryRemoveTypeReader(Type type, bool isDefaultTypeReader, out IDictionary<Type, TypeReader> readers)
    {
        readers = new Dictionary<Type, TypeReader>();

        if (isDefaultTypeReader)
        {
            var isSuccess = _defaultTypeReaders.TryRemove(type, out TypeReader result);
            if (isSuccess)
                readers.Add(result?.GetType(), result);

            return isSuccess;
        }
        else
        {
            var isSuccess = _typeReaders.TryRemove(type, out ConcurrentDictionary<Type, TypeReader> result);

            if (isSuccess)
                readers = result;

            return isSuccess;
        }
    }

    internal bool HasDefaultTypeReader(Type type)
    {
        if (_defaultTypeReaders.ContainsKey(type))
            return true;

        TypeInfo typeInfo = type.GetTypeInfo();
        if (typeInfo.IsEnum)
            return true;
        return _entityTypeReaders.Any(x => type == x.EntityType || typeInfo.ImplementedInterfaces.Contains(x.EntityType));
    }
    internal void AddNullableTypeReader(Type valueType, TypeReader valueTypeReader)
    {
        ConcurrentDictionary<Type, TypeReader> readers = _typeReaders.GetOrAdd(typeof(Nullable<>).MakeGenericType(valueType), x => new ConcurrentDictionary<Type, TypeReader>());
        TypeReader nullableReader = NullableTypeReader.Create(valueType, valueTypeReader);
        readers[nullableReader.GetType()] = nullableReader;
    }
    internal IDictionary<Type, TypeReader> GetTypeReaders(Type type)
    {
        if (_typeReaders.TryGetValue(type, out ConcurrentDictionary<Type, TypeReader> definedTypeReaders))
            return definedTypeReaders;
        return null;
    }
    internal TypeReader GetDefaultTypeReader(Type type)
    {
        if (_defaultTypeReaders.TryGetValue(type, out TypeReader reader))
            return reader;
        TypeInfo typeInfo = type.GetTypeInfo();

        //Is this an enum?
        if (typeInfo.IsEnum)
        {
            reader = EnumTypeReader.GetReader(type);
            _defaultTypeReaders[type] = reader;
            return reader;
        }
        Type underlyingType = Nullable.GetUnderlyingType(type);
        if (underlyingType != null && underlyingType.IsEnum)
        {
            reader = NullableTypeReader.Create(underlyingType, EnumTypeReader.GetReader(underlyingType));
            _defaultTypeReaders[type] = reader;
            return reader;
        }

        //Is this an entity?
        for (int i = 0; i < _entityTypeReaders.Count; i++)
        {
            if (type == _entityTypeReaders[i].EntityType || typeInfo.ImplementedInterfaces.Contains(_entityTypeReaders[i].EntityType))
            {
                reader = Activator.CreateInstance(_entityTypeReaders[i].TypeReaderType.MakeGenericType(type)) as TypeReader;
                _defaultTypeReaders[type] = reader;
                return reader;
            }
        }
        return null;
    }
    #endregion

    #region Execution
    /// <summary>
    /// Finds the commands that match the message content from <paramref name="argPos"/> onward.
    /// </summary>
    /// <param name="context">The context of the command.</param>
    /// <param name="argPos">The index in the message content where the command text starts.</param>
    /// <returns>The result containing the matching commands.</returns>
    public SearchResult Search(ICommandContext context, int argPos)
        => Search(context.Message.Content.Substring(argPos));
    /// <summary>
    /// Finds the commands that match the given input.
    /// </summary>
    /// <param name="context">The context of the command.</param>
    /// <param name="input">The command string.</param>
    /// <returns>The result containing the matching commands.</returns>
    public SearchResult Search(ICommandContext context, string input)
        => Search(input);
    /// <summary>
    /// Finds the commands that match the given input.
    /// </summary>
    /// <param name="input">The command string.</param>
    /// <returns>The result containing the matching commands.</returns>
    public SearchResult Search(string input)
    {
        string searchInput = _caseSensitive ? input : input.ToLowerInvariant();
        ImmutableArray<CommandMatch> matches = _map.GetCommands(searchInput).OrderByDescending(x => x.Command.Priority).ToImmutableArray();

        if (matches.Length > 0)
            return SearchResult.FromSuccess(input, matches);
        else
            return SearchResult.FromError(CommandError.UnknownCommand, "Unknown command.");
    }

    /// <summary>
    /// Finds, checks and executes the command that matches the message content from <paramref name="argPos"/> onward.
    /// </summary>
    /// <param name="context">The context of the command.</param>
    /// <param name="argPos">The index in the message content where the command text starts.</param>
    /// <param name="services">The service to be used in the command's dependency injection.</param>
    /// <param name="multiMatchHandling">The handling mode when multiple command matches are found.</param>
    /// <returns>
    /// A task that represents the asynchronous execution operation. The task result contains the result of the
    /// command execution.
    /// </returns>
    public Task<IResult> ExecuteAsync(ICommandContext context, int argPos, IServiceProvider services = null, MultiMatchHandling multiMatchHandling = MultiMatchHandling.Exception)
        => ExecuteAsync(context, context.Message.Content.Substring(argPos), services, multiMatchHandling);

    /// <summary>
    /// Finds, checks and executes the command that matches the given input.
    /// </summary>
    /// <param name="context">The context of the command.</param>
    /// <param name="input">The command string.</param>
    /// <param name="services">The service to be used in the command's dependency injection.</param>
    /// <param name="multiMatchHandling">The handling mode when multiple command matches are found.</param>
    /// <returns>
    /// A task that represents the asynchronous execution operation. The task result contains the result of the
    /// command execution.
    /// </returns>
    public async Task<IResult> ExecuteAsync(ICommandContext context, string input, IServiceProvider services = null, MultiMatchHandling multiMatchHandling = MultiMatchHandling.Exception)
    {
        context.CommandService = this;
        services ??= EmptyServiceProvider.Instance;

        if (context.Channel == null && context.Message is SocketPrivateMessage privateMessage)
        {
            Channel? channel = await privateMessage.Channel.GetOrDownloadAsync();
            if (channel == null)
                return PreconditionResult.FromError("Failed to get DM or Group channel.");
        }

        SearchResult searchResult = Search(input);

        IResult validationResult = await ValidateAndGetBestMatch(searchResult, context, services, multiMatchHandling);

        if (validationResult is SearchResult result)
        {
            await RaiseCommandExecutedAsync(null, context, result).ConfigureAwait(false);
            return result;
        }

        if (validationResult is MatchResult matchResult)
        {
            return await HandleCommandPipeline(matchResult, context, services);
        }

        return validationResult;
    }

    private async Task<IResult> HandleCommandPipeline(MatchResult matchResult, ICommandContext context, IServiceProvider services = null)
    {
        if (!matchResult.IsSuccess)
            return matchResult;

        if (matchResult.Pipeline is ParseResult parseResult)
        {
            if (!parseResult.IsSuccess)
            {
                await RaiseCommandExecutedAsync(matchResult.Match.Value.Command, context, parseResult).ConfigureAwait(false);
                return parseResult;
            }

            IResult executeResult = await matchResult.Match.Value.ExecuteAsync(context, parseResult, services);

            if (!executeResult.IsSuccess && !(executeResult is RuntimeResult || executeResult is ExecuteResult))
                await RaiseCommandExecutedAsync(matchResult.Match.Value.Command, context, executeResult).ConfigureAwait(false);
            return executeResult;
        }

        if (matchResult.Pipeline is PreconditionResult preconditionResult)
        {
            await RaiseCommandExecutedAsync(matchResult.Match.Value.Command, context, preconditionResult).ConfigureAwait(false);
            return preconditionResult;
        }

        return matchResult;
    }

    // Calculates the 'score' of a command given a parse result
    float CalculateScore(CommandMatch match, ParseResult parseResult)
    {
        float argValuesScore = 0, paramValuesScore = 0;

        if (match.Command.Parameters.Count > 0)
        {
            var argValuesSum = parseResult.ArgValues?.Sum(x => x.Values.OrderByDescending(y => y.Score).FirstOrDefault().Score) ?? 0;
            var paramValuesSum = parseResult.ParamValues?.Sum(x => x.Values.OrderByDescending(y => y.Score).FirstOrDefault().Score) ?? 0;

            argValuesScore = argValuesSum / match.Command.Parameters.Count;
            paramValuesScore = paramValuesSum / match.Command.Parameters.Count;
        }

        var totalArgsScore = (argValuesScore + paramValuesScore) / 2;
        return match.Command.Priority + totalArgsScore * 0.99f;
    }

    /// <summary>
    /// Validates and gets the best <see cref="CommandMatch"/> from a specified <see cref="SearchResult"/>
    /// </summary>
    /// <param name="matches">The SearchResult.</param>
    /// <param name="context">The context of the command.</param>
    /// <param name="provider">The service provider to be used on the command's dependency injection.</param>
    /// <param name="multiMatchHandling">The handling mode when multiple command matches are found.</param>
    /// <returns>
    /// A task that represents the asynchronous validation operation. The task result contains the result of the
    /// command validation as a <see cref="MatchResult"/>, or a <see cref="SearchResult"/> if no matches were found.
    /// </returns>
    public async Task<IResult> ValidateAndGetBestMatch(SearchResult matches, ICommandContext context, IServiceProvider provider, MultiMatchHandling multiMatchHandling = MultiMatchHandling.Exception)
    {
        if (!matches.IsSuccess)
            return matches;

        IReadOnlyList<CommandMatch> commands = matches.Commands;
        Dictionary<CommandMatch, PreconditionResult> preconditionResults = new Dictionary<CommandMatch, PreconditionResult>();

        foreach (CommandMatch command in commands)
        {
            preconditionResults[command] = await command.CheckPreconditionsAsync(context, provider);
        }

        KeyValuePair<CommandMatch, PreconditionResult>[] successfulPreconditions = preconditionResults
            .Where(x => x.Value.IsSuccess)
            .ToArray();

        if (successfulPreconditions.Length == 0)
        {
            //All preconditions failed, return the one from the highest priority command
            KeyValuePair<CommandMatch, PreconditionResult> bestCandidate = preconditionResults
               .OrderByDescending(x => x.Key.Command.Priority)
               .FirstOrDefault(x => !x.Value.IsSuccess);

            context.Command = bestCandidate.Key.Command;
            return MatchResult.FromSuccess(bestCandidate.Key, bestCandidate.Value);
        }

        Dictionary<CommandMatch, ParseResult> parseResults = new Dictionary<CommandMatch, ParseResult>();

        foreach (KeyValuePair<CommandMatch, PreconditionResult> pair in successfulPreconditions)
        {
            ParseResult parseResult = await pair.Key.ParseAsync(context, matches, pair.Value, provider).ConfigureAwait(false);

            if (parseResult.Error == CommandError.MultipleMatches)
            {
                IReadOnlyList<TypeReaderValue> argList, paramList;
                switch (multiMatchHandling)
                {
                    case MultiMatchHandling.Best:
                        argList = parseResult.ArgValues.Select(x => x.Values.OrderByDescending(y => y.Score).First()).ToImmutableArray();
                        paramList = parseResult.ParamValues.Select(x => x.Values.OrderByDescending(y => y.Score).First()).ToImmutableArray();
                        parseResult = ParseResult.FromSuccess(argList, paramList);
                        break;
                }
            }

            parseResults[pair.Key] = parseResult;
        }

        IOrderedEnumerable<KeyValuePair<CommandMatch, ParseResult>> weightedParseResults = parseResults
           .OrderByDescending(x => CalculateScore(x.Key, x.Value));

        KeyValuePair<CommandMatch, ParseResult>[] successfulParses = weightedParseResults
            .Where(x => x.Value.IsSuccess)
            .ToArray();

        if (successfulParses.Length == 0)
        {
            KeyValuePair<CommandMatch, ParseResult> bestMatch = parseResults
                .FirstOrDefault(x => !x.Value.IsSuccess);

            context.Command = bestMatch.Key.Command;
            return MatchResult.FromSuccess(bestMatch.Key, bestMatch.Value);
        }

        KeyValuePair<CommandMatch, ParseResult> chosenOverload = successfulParses[0];

        context.Command = chosenOverload.Key.Command;
        return MatchResult.FromSuccess(chosenOverload.Key, chosenOverload.Value);
    }
    #endregion

    #region Dispose
    /// <summary>
    /// Releases the resources used by the service.
    /// </summary>
    /// <param name="disposing">Whether managed resources are released.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!_isDisposed)
        {
            if (disposing)
            {
                _moduleLock?.Dispose();
            }

            _isDisposed = true;
        }
    }

    void IDisposable.Dispose()
    {
        Dispose(true);
    }
    #endregion
}
