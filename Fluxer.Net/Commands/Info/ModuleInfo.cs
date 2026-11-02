using System.Collections.Immutable;

namespace Fluxer.Net.Commands;

/// <summary>
/// The information of a command module.
/// </summary>
public class ModuleInfo
{
    /// <summary>
    /// The command service the module belongs to.
    /// </summary>
    public CommandService Service { get; }
    /// <summary>
    /// The name of the module.
    /// </summary>
    public string Name { get; }
    /// <summary>
    /// The summary of the module.
    /// </summary>
    public string Summary { get; }
    /// <summary>
    /// The remarks of the module.
    /// </summary>
    public string Remarks { get; }
    /// <summary>
    /// The group name, or main prefix, of the module.
    /// </summary>
    public string Group { get; }

    /// <summary>
    /// The aliases of the module.
    /// </summary>
    public IReadOnlyList<string> Aliases { get; }
    /// <summary>
    /// The commands of the module.
    /// </summary>
    public IReadOnlyList<CommandInfo> Commands { get; }
    /// <summary>
    /// The preconditions that apply to the module, including those of its parent modules.
    /// </summary>
    public IReadOnlyList<PreconditionAttribute> Preconditions { get; }
    /// <summary>
    /// The attributes of the module, including those of its parent modules.
    /// </summary>
    public IReadOnlyList<Attribute> Attributes { get; }
    /// <summary>
    /// The submodules of the module.
    /// </summary>
    public IReadOnlyList<ModuleInfo> Submodules { get; }
    /// <summary>
    /// The parent module, or null for a top-level module.
    /// </summary>
    public ModuleInfo Parent { get; }
    /// <summary>
    /// Whether the module has a parent module.
    /// </summary>
    public bool IsSubmodule => Parent != null;

    internal ModuleInfo(ModuleBuilder builder, CommandService service, IServiceProvider services, ModuleInfo parent = null)
    {
        Service = service;

        Name = builder.Name;
        Summary = builder.Summary;
        Remarks = builder.Remarks;
        Group = builder.Group;
        Parent = parent;

        Aliases = BuildAliases(builder, service).ToImmutableArray();
        Commands = builder.Commands.Select(x => x.Build(this, service)).ToImmutableArray();
        Preconditions = BuildPreconditions(builder).ToImmutableArray();
        Attributes = BuildAttributes(builder).ToImmutableArray();

        Submodules = BuildSubmodules(builder, service, services).ToImmutableArray();
    }

    private static IEnumerable<string> BuildAliases(ModuleBuilder builder, CommandService service)
    {
        List<string> result = builder.Aliases.ToList();
        Queue<ModuleBuilder> builderQueue = new Queue<ModuleBuilder>();

        ModuleBuilder parent = builder;
        while ((parent = parent.Parent) != null)
            builderQueue.Enqueue(parent);

        while (builderQueue.Count > 0)
        {
            ModuleBuilder level = builderQueue.Dequeue();
            // permute in reverse because we want to *prefix* our aliases
            result = level.Aliases.Permutate(result, (first, second) =>
            {
                if (first == "")
                    return second;
                else if (second == "")
                    return first;
                else
                    return first + service._separatorChar + second;
            }).ToList();
        }

        return result;
    }

    private List<ModuleInfo> BuildSubmodules(ModuleBuilder parent, CommandService service, IServiceProvider services)
    {
        List<ModuleInfo> result = new List<ModuleInfo>();

        foreach (ModuleBuilder submodule in parent.Modules)
            result.Add(submodule.Build(service, services, this));

        return result;
    }

    private static List<PreconditionAttribute> BuildPreconditions(ModuleBuilder builder)
    {
        List<PreconditionAttribute> result = new List<PreconditionAttribute>();

        ModuleBuilder parent = builder;
        while (parent != null)
        {
            result.AddRange(parent.Preconditions);
            parent = parent.Parent;
        }

        return result;
    }

    private static List<Attribute> BuildAttributes(ModuleBuilder builder)
    {
        List<Attribute> result = new List<Attribute>();

        ModuleBuilder parent = builder;
        while (parent != null)
        {
            result.AddRange(parent.Attributes);
            parent = parent.Parent;
        }

        return result;
    }
}
