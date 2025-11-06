namespace Fluxer.Net.Commands;

/// <summary>
/// Specifies the execution mode of a command.
/// </summary>
public enum RunMode
{
	/// <summary>
	/// The command will run synchronously on the gateway thread.
	/// </summary>
	Sync,

	/// <summary>
	/// The command will run asynchronously on a separate thread.
	/// </summary>
	Async
}
