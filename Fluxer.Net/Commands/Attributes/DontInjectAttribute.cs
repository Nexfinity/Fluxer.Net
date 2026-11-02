namespace Fluxer.Net.Commands;

/// <summary>
/// Excludes a property from dependency injection into a module.
/// </summary>
/// <remarks>
/// A property marked with this attribute is not set by the dependency injection service when the module is built.
/// </remarks>
/// <example>
/// In the following example, <c>DatabaseService</c> is not injected into the module, so a failure to resolve it does
/// not stop the module from being built.
/// <code language="cs">
/// public class MyModule : ModuleBase
/// {
///     [DontInject]
///     public DatabaseService DatabaseService { get; set; }
///     public MyModule()
///     {
///         DatabaseService = DatabaseFactory.Generate();
///     }
/// }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public class DontInjectAttribute : Attribute
{
}
