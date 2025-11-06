using System.Reflection;
using Fluxer.Net.Commands.Attributes;

namespace Fluxer.Net.Commands;

/// <summary>
/// Represents information about a command parameter.
/// </summary>
public class ParameterInfo
{
	/// <summary>
	/// Gets the parameter's name.
	/// </summary>
	public string Name { get; }

	/// <summary>
	/// Gets the parameter's type.
	/// </summary>
	public Type Type { get; }

	/// <summary>
	/// Gets the parameter's summary.
	/// </summary>
	public string? Summary { get; }

	/// <summary>
	/// Gets whether this parameter is optional.
	/// </summary>
	public bool IsOptional { get; }

	/// <summary>
	/// Gets whether this parameter captures the remainder of the input.
	/// </summary>
	public bool IsRemainder { get; }

	/// <summary>
	/// Gets the default value for this parameter if it's optional.
	/// </summary>
	public object? DefaultValue { get; }

	internal ParameterInfo(ParameterInfo parameter)
	{
		Name = parameter.Name;
		Type = parameter.Type;
		Summary = parameter.Summary;
		IsOptional = parameter.IsOptional;
		IsRemainder = parameter.IsRemainder;
		DefaultValue = parameter.DefaultValue;
	}

	internal ParameterInfo(System.Reflection.ParameterInfo paramInfo)
	{
		Name = paramInfo.GetCustomAttribute<NameAttribute>()?.Text ?? paramInfo.Name ?? "unknown";
		Type = paramInfo.ParameterType;
		Summary = paramInfo.GetCustomAttribute<SummaryAttribute>()?.Text;
		IsOptional = paramInfo.IsOptional;
		IsRemainder = paramInfo.GetCustomAttribute<RemainderAttribute>() != null;
		DefaultValue = paramInfo.DefaultValue;
	}
}
