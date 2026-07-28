namespace Fluxer.Net.Commands;

/// <summary>
/// Marks a parameter to capture all remaining text as a single string.
/// </summary>
[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
public class RemainderAttribute : Attribute
{
}
