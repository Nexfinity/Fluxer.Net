namespace Fluxer.Net.Commands;

/// <summary>
/// Helper utilities for activating instances with dependency injection.
/// </summary>
internal static class ActivatorUtilities
{
	/// <summary>
	/// Creates an instance of the specified type using the service provider.
	/// </summary>
	public static object CreateInstance(IServiceProvider provider, Type instanceType, params object[] parameters)
	{
        // Try to get from service provider first
        object service = provider.GetService(instanceType);
		if (service != null)
			return service;

		// Otherwise create with Activator
		return Activator.CreateInstance(instanceType, parameters)
			?? throw new InvalidOperationException($"Failed to create instance of {instanceType.Name}");
	}
}
