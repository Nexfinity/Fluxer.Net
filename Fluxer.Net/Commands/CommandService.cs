using Serilog;
using System.Reflection;

namespace Fluxer.Net.Commands;

/// <summary>
/// Provides a framework for creating and executing text-based commands.
/// </summary>
public class CommandService
{
    private readonly List<ModuleInfo> _modules = new();
    private readonly ILogger? _logger;
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
    public CommandService(ILogger? logger = null, IServiceProvider? services = null)
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
            .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(ModuleBase)));

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
        module.Build();

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
        var input = context.Message.Content?.Substring(argPos);
        if (string.IsNullOrWhiteSpace(input))
            return ExecuteResult.FromError(CommandError.ParseFailed, "No input provided");

        // Split input into command name and arguments
        var parts = input.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0)
            return ExecuteResult.FromError(CommandError.ParseFailed, "No command specified");

        var commandName = parts[0].ToLowerInvariant();
        List<string> argList = parts.Skip(1).ToList();

        // Find matching command
        CommandInfo? matchedCommand = null;

        foreach (ModuleInfo module in _modules)
        {
            if (!string.IsNullOrEmpty(module.Group) && module.Group.Equals(commandName, StringComparison.OrdinalIgnoreCase))
            {
                CommandInfo? GroupCommand = module.Commands.FirstOrDefault(x => string.IsNullOrEmpty(x.Name));
                if (GroupCommand != null)
                    matchedCommand = GroupCommand;
                else
                {
                    if (parts.Length < 2)
                        continue;

                    commandName = parts[1].ToLowerInvariant();
                    argList = parts.Skip(2).ToList();

                    foreach (CommandInfo command in module.Commands)
                    {
                        if (command.Name.Equals(commandName, StringComparison.OrdinalIgnoreCase) ||
                            command.Aliases.Any(a => a.Equals(commandName, StringComparison.OrdinalIgnoreCase)))
                        {
                            matchedCommand = command;
                            break;
                        }
                    }
                }
            }
            else
            {
                foreach (CommandInfo command in module.Commands)
                {
                    if (command.Name.Equals(commandName, StringComparison.OrdinalIgnoreCase) ||
                        command.Aliases.Any(a => a.Equals(commandName, StringComparison.OrdinalIgnoreCase)))
                    {
                        matchedCommand = command;
                        break;
                    }
                }
            }

            if (matchedCommand != null) break;
        }

        if (matchedCommand == null)
        {
            _logger?.Debug("Command not found: {CommandName}", commandName);
            return ExecuteResult.FromError(CommandError.UnknownCommand, $"Unknown command: {commandName}");
        }

        // Parse arguments
        var parseResult = ParseArguments(matchedCommand, argList);
        if (parseResult is IResult result && !result.IsSuccess)
            return result;

        var args = (object[])parseResult;

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

    /// <summary>
    /// Searches for a command by name or alias.
    /// </summary>
    /// <param name="name">The command name or alias.</param>
    public CommandInfo? Search(string name)
    {
        var lowerName = name.ToLowerInvariant();

        foreach (ModuleInfo module in _modules)
        {
            foreach (CommandInfo command in module.Commands)
            {
                if (command.Name.Equals(lowerName, StringComparison.OrdinalIgnoreCase) ||
                    command.Aliases.Any(a => a.Equals(lowerName, StringComparison.OrdinalIgnoreCase)))
                {
                    return command;
                }
            }
        }

        return null;
    }

    private object ParseArguments(CommandInfo command, List<string> argList)
    {
        IReadOnlyList<ParameterInfo> parameters = command.Parameters;
        var args = new object[parameters.Count];

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
            var argString = argList[argIndex];
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
