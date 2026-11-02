using Serilog;

namespace Fluxer.Net.Commands;

/// <summary>
/// The configuration of a <see cref="CommandService"/>.
/// </summary>
public class CommandServiceConfig
{
    /// <summary>
    /// The <see cref="RunMode"/> used by commands that do not set one.
    /// </summary>
    public RunMode DefaultRunMode { get; set; } = RunMode.Sync;

    /// <summary>
    /// The character that separates the words of a command name, such as the space in "guild info".
    /// </summary>
    public char SeparatorChar { get; set; } = ' ';

    /// <summary>
    /// Whether command names are matched case-sensitively.
    /// </summary>
    public bool CaseSensitiveCommands { get; set; } = false;

    /// <summary>
    /// Whether exceptions thrown by a command are rethrown to the caller. When <see langword="false"/>, they are
    /// reported only through the result.
    /// </summary>
    public bool ThrowOnError { get; set; } = true;

    /// <summary>
    /// The pairs of opening and closing quotation marks that start and end a quoted argument. The key is the opening
    /// mark and the value is the closing mark.
    /// </summary>
    /// <remarks>
    /// Defaults to the regional and Unicode pairs in <c>QuotationAliasUtils.GetDefaultAliasMap</c>. When this map is null
    /// or empty, only the double quote <c>"</c> starts a quoted argument. Removing a mark from this map disables it.
    /// </remarks>
    /// <example>
    /// <code language="cs">
    /// QuotationMarkAliasMap = new Dictionary&lt;char, char&gt;()
    /// {
    ///     {'\"', '\"' },
    ///     {'“', '”' },
    ///     {'「', '」' },
    /// }
    /// </code>
    /// </example>
    public Dictionary<char, char> QuotationMarkAliasMap { get; set; } = QuotationAliasUtils.GetDefaultAliasMap;

    /// <summary>
    /// Whether extra arguments are ignored instead of failing the command with <see cref="CommandError.BadArgCount"/>.
    /// </summary>
    public bool IgnoreExtraArgs { get; set; } = false;

    /// <summary>
    /// The logger the command service writes to, or null to write no logs.
    /// </summary>
    public ILogger Logger { get; set; }

    /// <summary>
    /// The user ids that pass the owner preconditions, in addition to the bot owner reported by the gateway.
    /// </summary>
    public ulong[]? OwnerIDs { get; set; }

    /// <summary>
    /// Whether owners pass the user permission and owner preconditions without holding the permission.
    /// </summary>
    public bool OwnerBypassPermissions { get; set; }
}
