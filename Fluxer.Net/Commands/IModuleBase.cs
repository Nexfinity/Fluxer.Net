namespace Fluxer.Net.Commands;

/// <summary>
/// The members the command system calls on a command module.
/// </summary>
public interface IModuleBase
{
    /// <summary>
    /// Sets the context of the command that is about to run.
    /// </summary>
    /// <param name="context">The context to set.</param>
    void SetContext(ICommandContext context);

    /// <summary>
    /// Runs asynchronously before a command in this module runs.
    /// </summary>
    /// <param name="command">The command that is about to run.</param>
    Task BeforeExecuteAsync(CommandInfo command);

    /// <summary>
    /// Runs before a command in this module runs.
    /// </summary>
    /// <param name="command">The command that is about to run.</param>
    void BeforeExecute(CommandInfo command);

    /// <summary>
    /// Runs asynchronously after a command in this module runs.
    /// </summary>
    /// <param name="command">The command that ran.</param>
    Task AfterExecuteAsync(CommandInfo command);

    /// <summary>
    /// Runs after a command in this module runs, including when the command throws.
    /// </summary>
    /// <param name="command">The command that ran.</param>
    void AfterExecute(CommandInfo command);

    /// <summary>
    /// Runs when this module is being built.
    /// </summary>
    /// <param name="commandService">The command service that is building this module.</param>
    /// <param name="builder">The builder constructing this module.</param>
    void OnModuleBuilding(CommandService commandService, ModuleBuilder builder);
}
