namespace Fluxer.Net.Commands;

/// <summary>
/// A command that matched a search, with the alias that matched it.
/// </summary>
public struct CommandMatch
{
    /// <summary>
    /// The command that matched the search.
    /// </summary>
    public CommandInfo Command { get; }
    /// <summary>
    /// The alias the input matched.
    /// </summary>
    public string Alias { get; }

    /// <summary>
    /// Creates a new <see cref="CommandMatch"/>.
    /// </summary>
    /// <param name="command">The matched command.</param>
    /// <param name="alias">The alias the input matched.</param>
    public CommandMatch(CommandInfo command, string alias)
    {
        Command = command;
        Alias = alias;
    }

    /// <summary>
    /// Checks the preconditions of the matched command.
    /// </summary>
    /// <param name="context">The context of the command.</param>
    /// <param name="services">The service provider used for dependency injection.</param>
    /// <returns>The result of the precondition checks.</returns>
    public Task<PreconditionResult> CheckPreconditionsAsync(ICommandContext context, IServiceProvider services = null)
        => Command.CheckPreconditionsAsync(context, services);
    /// <summary>
    /// Parses the input of the matched command.
    /// </summary>
    /// <param name="context">The context of the command.</param>
    /// <param name="searchResult">The search result that contains the input.</param>
    /// <param name="preconditionResult">The result of the precondition checks, if any.</param>
    /// <param name="services">The service provider used for dependency injection.</param>
    /// <returns>The result of parsing the arguments.</returns>
    public Task<ParseResult> ParseAsync(ICommandContext context, SearchResult searchResult, PreconditionResult preconditionResult = null, IServiceProvider services = null)
        => Command.ParseAsync(context, Alias.Length, searchResult, preconditionResult, services);
    /// <summary>
    /// Executes the matched command with the given arguments.
    /// </summary>
    /// <param name="context">The context of the command.</param>
    /// <param name="argList">The parsed arguments.</param>
    /// <param name="paramList">The parsed values of the params parameter.</param>
    /// <param name="services">The service provider used for dependency injection.</param>
    /// <returns>The result of the execution.</returns>
    public Task<IResult> ExecuteAsync(ICommandContext context, IEnumerable<object> argList, IEnumerable<object> paramList, IServiceProvider services)
        => Command.ExecuteAsync(context, argList, paramList, services);
    /// <summary>
    /// Executes the matched command with the parsed input.
    /// </summary>
    /// <param name="context">The context of the command.</param>
    /// <param name="parseResult">The result of parsing the input.</param>
    /// <param name="services">The service provider used for dependency injection.</param>
    /// <returns>The result of the execution.</returns>
    public Task<IResult> ExecuteAsync(ICommandContext context, ParseResult parseResult, IServiceProvider services)
        => Command.ExecuteAsync(context, parseResult, services);
}
