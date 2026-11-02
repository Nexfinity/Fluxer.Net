namespace Fluxer.Net.Commands;

/// <summary>
/// The workflow a command runs with when it is executed.
/// </summary>
/// <seealso cref="CommandServiceConfig"/>
/// <seealso cref="CommandAttribute"/>
public enum RunMode
{
    /// <summary>
    /// The <see cref="CommandServiceConfig.DefaultRunMode"/> of the command service.
    /// </summary>
    Default,
    /// <summary>
    /// The command runs and is awaited before the execution call returns.
    /// </summary>
    Sync,
    /// <summary>
    /// The command runs on a background task, and the execution call returns before it finishes.
    /// </summary>
    Async
}
