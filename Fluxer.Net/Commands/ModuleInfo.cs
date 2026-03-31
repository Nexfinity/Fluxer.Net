using System.Reflection;
using Fluxer.Net.Commands.Attributes;

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
	/// Gets the module's aliases.
	/// </summary>
	public IReadOnlyList<string> Aliases { get; }

	/// <summary>
	/// Gets the commands in this module.
	/// </summary>
	public IReadOnlyList<CommandInfo> Commands { get; internal set; } = Array.Empty<CommandInfo>();

	/// <summary>
	/// Gets the module type.
	/// </summary>
	internal Type Type { get; }

	internal ModuleInfo(Type type)
	{
		Type = type;
		Name = type.Name;

		var aliasAttr = type.GetCustomAttribute<AliasAttribute>();
		Aliases = aliasAttr?.Aliases ?? Array.Empty<string>();
	}

	internal void Build()
	{
        List<CommandInfo> commands = new List<CommandInfo>();

		foreach (var method in Type.GetMethods(BindingFlags.Public | BindingFlags.Instance))
		{
			var cmdAttr = method.GetCustomAttribute<CommandAttribute>();
			if (cmdAttr != null)
			{
				commands.Add(new CommandInfo(method, this, cmdAttr));
			}
		}

		Commands = commands.AsReadOnly();
	}
}
