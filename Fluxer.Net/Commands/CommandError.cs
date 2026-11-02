namespace Fluxer.Net.Commands;

/// <summary>
/// The kind of error reported by a command result.
/// </summary>
public enum CommandError
{
    //Search
    /// <summary>
    /// No command matched the input.
    /// </summary>
    UnknownCommand = 1,

    //Parse
    /// <summary>
    /// The input could not be parsed.
    /// </summary>
    ParseFailed,
    /// <summary>
    /// The input has too few or too many arguments.
    /// </summary>
    BadArgCount,

    //Parse (Type Reader)
    //CastFailed,
    /// <summary>
    /// A <see cref="TypeReader"/> found no object for the input.
    /// </summary>
    ObjectNotFound,
    /// <summary>
    /// A <see cref="TypeReader"/> matched more than one object for the input.
    /// </summary>
    MultipleMatches,

    //Preconditions
    /// <summary>
    /// A <see cref="PreconditionAttribute"/> was not met.
    /// </summary>
    UnmetPrecondition,

    //Execute
    /// <summary>
    /// An exception was thrown while the command ran.
    /// </summary>
    Exception,

    //Runtime
    /// <summary>
    /// The command finished without success, as reported by its <see cref="RuntimeResult"/>.
    /// </summary>
    Unsuccessful
}
