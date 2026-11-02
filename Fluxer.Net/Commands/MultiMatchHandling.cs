namespace Fluxer.Net.Commands;

/// <summary>
/// The behavior when a parameter matches more than one object during the parsing stage.
/// </summary>
public enum MultiMatchHandling
{
    /// <summary>
    /// The parse fails with <see cref="CommandError.MultipleMatches"/>.
    /// </summary>
    Exception,
    /// <summary>
    /// The highest scoring match is chosen.
    /// </summary>
    Best
}
