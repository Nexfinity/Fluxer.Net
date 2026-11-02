namespace Fluxer.Net.Commands;

/// <summary>
/// Marks a type whose parameters are read as named arguments that match its public properties.
/// </summary>
/// <remarks>
/// A parameter of this type is treated as a remainder parameter, so it receives the rest of the input.
/// </remarks>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class NamedArgumentTypeAttribute : Attribute { }
