using System.Reflection;
using Fluxer.Net.Commands.Attributes;

namespace Fluxer.Net.Commands;

/// <summary>
/// Represents information about a command.
/// </summary>
public class CommandInfo
{
	/// <summary>
	/// Gets the command's name.
	/// </summary>
	public string Name { get; }

	/// <summary>
	/// Gets the command's aliases.
	/// </summary>
	public IReadOnlyList<string> Aliases { get; }

	/// <summary>
	/// Gets the command's summary.
	/// </summary>
	public string? Summary { get; }

	/// <summary>
	/// Gets the command's remarks.
	/// </summary>
	public string? Remarks { get; }

	/// <summary>
	/// Gets the command's run mode.
	/// </summary>
	public RunMode RunMode { get; }

	/// <summary>
	/// Gets the command's parameters.
	/// </summary>
	public IReadOnlyList<ParameterInfo> Parameters { get; }

	/// <summary>
	/// Gets the module this command belongs to.
	/// </summary>
	public ModuleInfo Module { get; }

	internal MethodInfo Method { get; }

	internal CommandInfo(MethodInfo method, ModuleInfo module, CommandAttribute cmdAttr)
	{
		Method = method;
		Module = module;
		Name = cmdAttr.Name;
		RunMode = cmdAttr.RunMode;

        AliasAttribute aliasAttr = method.GetCustomAttribute<AliasAttribute>();
		Aliases = aliasAttr?.Aliases ?? Array.Empty<string>();

		Summary = method.GetCustomAttribute<SummaryAttribute>()?.Text;
		Remarks = method.GetCustomAttribute<RemarksAttribute>()?.Text;

		Parameters = method.GetParameters()
			.Select(p => new ParameterInfo(p))
			.ToList()
			.AsReadOnly();

		Preconditions = method.GetCustomAttributes<PreconditionAttribute>(true)
			.ToList()
			.AsReadOnly();
	}

	/// <summary>
	/// Gets the preconditions that must be satisfied before executing the command.
	/// </summary>
	public IReadOnlyList<PreconditionAttribute> Preconditions { get; }

	/// <summary>
	/// Executes the command.
	/// </summary>
	public async Task<IResult> ExecuteAsync(CommandContext context, object[] args, IServiceProvider? services = null)
	{
		// Check preconditions first
		foreach (PreconditionAttribute precondition in Preconditions)
		{
            PreconditionResult result = await precondition.CheckPermissionsAsync(context, this, services);
			if (!result.IsSuccess)
				return result;
		}

        // Check module-level preconditions
        IEnumerable<PreconditionAttribute> modulePreconditions = Module.Type.GetCustomAttributes<PreconditionAttribute>(true);
		foreach (PreconditionAttribute precondition in modulePreconditions)
		{
            PreconditionResult result = await precondition.CheckPermissionsAsync(context, this, services);
			if (!result.IsSuccess)
				return result;
		}

		try
		{
			var instance = services != null
				? ActivatorUtilities.CreateInstance(services, Module.Type)
				: Activator.CreateInstance(Module.Type);

			if (instance is ModuleBase moduleBase)
			{
				moduleBase.Context = context;
				moduleBase.BeforeExecute(this);
			}

			var result = Method.Invoke(instance, args);

			if (result is Task<IResult> taskResult)
			{
                IResult execResult = await taskResult;
				if (instance is ModuleBase mb) mb.AfterExecute(this);
				return execResult;
			}
			else if (result is Task task)
			{
				await task;
				if (instance is ModuleBase mb) mb.AfterExecute(this);
				return ExecuteResult.FromSuccess();
			}
			else
			{
				if (instance is ModuleBase mb) mb.AfterExecute(this);
				return ExecuteResult.FromSuccess();
			}
		}
		catch (Exception ex)
		{
			return ExecuteResult.FromError(ex.InnerException ?? ex);
		}
	}
}
