namespace Fluxer.Net.Commands;

/// <summary>
/// Excludes a module from being loaded automatically.
/// </summary>
/// <remarks>
/// <see cref="CommandService.AddModulesAsync(System.Reflection.Assembly, IServiceProvider)"/> skips a module marked with
/// this attribute. The module can still be added with <see cref="CommandService.AddModuleAsync{T}(IServiceProvider)"/>.
/// </remarks>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public class DontAutoLoadAttribute : Attribute
{
}
