namespace Fluxer.Net.Commands;

/// <summary>
/// Marks the last parameter of a command to receive the rest of the input as one unparsed value.
/// </summary>
/// <remarks>
/// Only the last parameter of a command can be a remainder parameter.
/// </remarks>
[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
public class RemainderAttribute : Attribute
{
}
