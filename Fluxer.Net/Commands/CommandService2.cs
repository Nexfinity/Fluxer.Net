using Serilog;
using System.Reflection;

namespace Fluxer.Net.Commands;

/// <summary>
/// Provides a framework for creating and executing text-based commands.
/// </summary>
public class CommandService2
{
    private readonly List<ModuleInfo> _modules = new();
    internal readonly ILogger? _logger;
    private readonly IServiceProvider? _services;

    /// <summary>
    /// Gets all registered modules.
    /// </summary>
    public IReadOnlyList<ModuleInfo> Modules => _modules.AsReadOnly();

    /// <summary>
    /// Gets all registered commands across all modules.
    /// </summary>
    public IEnumerable<CommandInfo> Commands => _modules.SelectMany(m => m.Commands);

    /// <summary>
    /// Creates a new command service.
    /// </summary>
    /// <param name="prefixChar">The prefix character for commands (e.g., '!' or '/').</param>
    /// <param name="logger">Optional logger for command execution.</param>
    /// <param name="services">Optional service provider for dependency injection.</param>
    public CommandService2(ILogger? logger = null, IServiceProvider? services = null)
    {
        _logger = logger;
        _services = services;
    }

    /// <summary>
    /// Registers all command modules from the specified assembly.
    /// </summary>
    /// <param name="assembly">The assembly to search for modules.</param>
    public async Task AddModulesAsync(Assembly assembly)
    {
        IEnumerable<Type> moduleTypes = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsNested && !t.IsAbstract && t.IsSubclassOf(typeof(ModuleBase)));

        foreach (Type type in moduleTypes)
        {
            await AddModuleAsync(type);
        }
    }

    /// <summary>
    /// Registers a specific command module type.
    /// </summary>
    /// <typeparam name="T">The module type.</typeparam>
    public Task<ModuleInfo> AddModuleAsync<T>() where T : ModuleBase
    {
        return AddModuleAsync(typeof(T));
    }

    /// <summary>
    /// Registers a specific command module type.
    /// </summary>
    /// <param name="type">The module type.</param>
    public Task<ModuleInfo> AddModuleAsync(Type type)
    {
        if (!type.IsSubclassOf(typeof(ModuleBase)))
            throw new ArgumentException($"Type {type.Name} must inherit from ModuleBase", nameof(type));

        ModuleInfo module = new ModuleInfo(type);

        //
        //
        //module.Build(this);
        //
        //

        _modules.Add(module);
        _logger?.Information("Registered command module {ModuleName} with {CommandCount} commands",
            module.Name, module.Commands.Count);

        return Task.FromResult(module);
    }

    /// <summary>
    /// Executes a command from a message.
    /// </summary>
    /// <param name="context">The command context.</param>
    /// <param name="argPos">The position in the message where arguments begin.</param>
    public async Task<IResult> ExecuteAsync(CommandContext context, int argPos)
    {
        string input = context.Message.Content?.Substring(argPos);
        if (string.IsNullOrWhiteSpace(input))
            return ExecuteResult.FromError(CommandError.ParseFailed, "No input provided");

        // Split input into command name and arguments
        string[] parts = input.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0)
            return ExecuteResult.FromError(CommandError.ParseFailed, "No command specified");

        string commandName = parts[0].ToLowerInvariant();
        int argIndex = 1;

        Console.WriteLine($"Parts: " + string.Join(", ", parts));

        // Find matching command
        CommandInfo? matchedCommand = MatchCommand(ref commandName, parts, ref argIndex);
        List<string> argList = parts.Skip(argIndex).ToList();
        Console.WriteLine("Use index: " + argIndex);

        if (matchedCommand == null)
        {
            _logger?.Debug("Command not found: {CommandName}", commandName);
            return ExecuteResult.FromError(CommandError.UnknownCommand, $"Unknown command: {commandName}");
        }

        // Parse arguments
        object parseResult = ParseArguments(matchedCommand, argList);
        if (parseResult is IResult result && !result.IsSuccess)
            return result;

        object[] args = (object[])parseResult;

        // Execute command
        _logger?.Debug("Executing command {CommandName} with {ArgCount} arguments", commandName, args.Length);

        if (matchedCommand.RunMode == RunMode.Async)
        {
            _ = Task.Run(async () => await matchedCommand.ExecuteAsync(context, args, _services));
            return ExecuteResult.FromSuccess();
        }
        else
        {
            return await matchedCommand.ExecuteAsync(context, args, _services);
        }
    }

    private CommandInfo? MatchCommand(ref string commandName, string[] parts, ref int argIndex)
    {
        Console.WriteLine($"Match: {commandName} - {argIndex}");


        foreach (ModuleInfo module in _modules)
        {
            if (!string.IsNullOrEmpty(module.Group) && module.Group.Equals(commandName, StringComparison.OrdinalIgnoreCase))
            {
                if (argIndex != parts.Length)
                {
                    commandName = parts[argIndex].ToLowerInvariant();
                    Console.WriteLine("Find sub command: " + commandName);
                    CommandInfo? FoundCommand = SearchCommand(module, commandName);
                    if (FoundCommand != null)
                    {
                        argIndex += 1;
                        Console.WriteLine("Use sub command");
                        return FoundCommand;
                    }

                    CommandInfo Match = null;
                    if (parts.Length != argIndex)
                    {
                        Console.WriteLine("Find other match");
                        Console.WriteLine("+1");
                        argIndex += 1;
                        Match = MatchCommand(ref commandName, parts, ref argIndex);
                    }

                    if (Match != null)
                    {
                        Console.WriteLine("Use Match");
                        return Match;
                    }
                }

                CommandInfo? GroupCommand = module.Commands.FirstOrDefault(x => string.IsNullOrEmpty(x.Name));
                if (GroupCommand != null)
                {
                    Console.WriteLine("Use Group Command");
                    return GroupCommand;
                }
            }
            else
            {
                string cmd = commandName;
                foreach (CommandInfo command in module.Commands)
                {
                    if (command.Name.Equals(cmd, StringComparison.OrdinalIgnoreCase) ||
                        command.Aliases.Any(a => a.Equals(cmd, StringComparison.OrdinalIgnoreCase)))
                    {
                        return command;
                    }
                }
            }
        }
        return null;
    }

    /// <summary>
    /// Searches for a command by name or alias.
    /// </summary>
    /// <param name="name">The command name or alias.</param>
    private CommandInfo? SearchCommand(ModuleInfo module, string name)
    {
        foreach (CommandInfo command in module.Commands)
        {
            if (command.Name.Equals(name, StringComparison.OrdinalIgnoreCase) ||
                command.Aliases.Any(a => a.Equals(name, StringComparison.OrdinalIgnoreCase)))
            {
                return command;
            }
        }

        return null;
    }

    private object ParseArguments(CommandInfo command, List<string> argList)
    {
        IReadOnlyList<ParameterInfo> parameters = command.Parameters;
        object[] args = new object[parameters.Count];

        int argIndex = 0;
        for (int i = 0; i < parameters.Count; i++)
        {
            ParameterInfo param = parameters[i];

            // Handle remainder parameter
            if (param.IsRemainder && argIndex < argList.Count)
            {
                args[i] = string.Join(" ", argList.Skip(argIndex));
                continue;
            }

            // Handle optional parameter
            if (argIndex >= argList.Count)
            {
                if (param.IsOptional)
                {
                    args[i] = param.DefaultValue ?? GetDefaultValue(param.Type);
                    continue;
                }
                else
                {
                    return ExecuteResult.FromError(CommandError.BadArgCount,
                        $"Missing required parameter: {param.Name}");
                }
            }

            // Parse argument
            string argString = argList[argIndex];
            try
            {
                args[i] = ParseArgument(argString, param.Type);
                argIndex++;
            }
            catch (Exception ex)
            {
                return ExecuteResult.FromError(CommandError.ParseFailed,
                    $"Failed to parse parameter '{param.Name}': {ex.Message}");
            }
        }

        return args;
    }

    private object ParseArgument(string input, Type targetType)
    {
        // Handle nullable types
        Type underlyingType = Nullable.GetUnderlyingType(targetType);
        if (underlyingType != null)
            targetType = underlyingType;

        // String - return as is
        if (targetType == typeof(string))
            return input;

        // Boolean
        if (targetType == typeof(bool))
            return bool.Parse(input);

        // Numeric types
        if (targetType == typeof(int))
            return int.Parse(input);
        if (targetType == typeof(long))
            return long.Parse(input);
        if (targetType == typeof(ulong))
            return ulong.Parse(input);
        if (targetType == typeof(uint))
            return uint.Parse(input);
        if (targetType == typeof(short))
            return short.Parse(input);
        if (targetType == typeof(ushort))
            return ushort.Parse(input);
        if (targetType == typeof(byte))
            return byte.Parse(input);
        if (targetType == typeof(sbyte))
            return sbyte.Parse(input);
        if (targetType == typeof(float))
            return float.Parse(input);
        if (targetType == typeof(double))
            return double.Parse(input);
        if (targetType == typeof(decimal))
            return decimal.Parse(input);

        // DateTime
        if (targetType == typeof(DateTime))
            return DateTime.Parse(input);

        // TimeSpan
        if (targetType == typeof(TimeSpan))
            return TimeSpan.Parse(input);

        // Enum
        if (targetType.IsEnum)
            return Enum.Parse(targetType, input, true);

        throw new ArgumentException($"Unsupported parameter type: {targetType.Name}");
    }

    private static object? GetDefaultValue(Type type)
    {
        return type.IsValueType ? Activator.CreateInstance(type) : null;
    }
}
