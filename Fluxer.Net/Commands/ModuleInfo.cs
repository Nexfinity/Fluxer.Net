using System.Reflection;

namespace Fluxer.Net.Commands;

/// <summary>
/// Represents information about a command module.
/// </summary>
public class ModuleInfo
{
    /// <summary>
    /// Gets the module's name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the module's command group.
    /// </summary>
    public string? Group { get; }

    /// <summary>
    /// Gets the module's aliases.
    /// </summary>
    public IReadOnlyList<string> Aliases { get; }

    /// <summary>
    /// Gets the commands in this module.
    /// </summary>
    public IReadOnlyList<CommandInfo> Commands { get; internal set; } = Array.Empty<CommandInfo>();

    /// <summary>
    /// Gets all registered modules.
    /// </summary>
    public IReadOnlyList<ModuleInfo> Modules { get; internal set; } = Array.Empty<ModuleInfo>();

    /// <summary>
    /// Gets the module type.
    /// </summary>
    internal Type Type { get; }

    internal ModuleInfo(Type type)
    {
        Type = type;
        Name = type.Name;

        AliasAttribute aliasAttr = type.GetCustomAttribute<AliasAttribute>();
        Aliases = aliasAttr?.Aliases ?? Array.Empty<string>();

        GroupAttribute groupAttr = type.GetCustomAttribute<GroupAttribute>();
        if (groupAttr != null)
            Group = groupAttr.Prefix;
    }

    internal void Build(CommandService service)
    {
        List<CommandInfo> commands = new List<CommandInfo>();

        foreach (MethodInfo method in Type.GetMethods(BindingFlags.Public | BindingFlags.Instance))
        {
            CommandAttribute cmdAttr = method.GetCustomAttribute<CommandAttribute>();
            if (cmdAttr != null)
            {
                commands.Add(new CommandInfo(method, this, cmdAttr));
            }
        }

        //List<ModuleInfo> modules = new List<ModuleInfo>();

        //foreach (var i in Type.GetNestedTypes())
        //{
        //    if (i.IsClass && i.DeclaringType == Type && !i.IsAbstract && i.IsSubclassOf(typeof(ModuleBase)))
        //    {
        //        ModuleInfo module = new ModuleInfo(i);
        //        module.Build(service);
        //        modules.Add(module);

        //        service._logger?.Information("Registered sub command module {ModuleName} from {ParentModule} with {CommandCount} commands",
        //            module.Name, Name, module.Commands.Count);
        //    }
        //}

        Commands = commands.AsReadOnly();
        //Modules = modules.AsReadOnly();
    }
}
