namespace Fluxer.Net.Commands;

/// <summary>
/// Marks a method as a command and sets its text, run mode and descriptions.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public class CommandAttribute : Attribute
{
    /// <summary>
    /// The text that triggers the command, or null when the attribute has no text.
    /// </summary>
    public string Text { get; }
    /// <summary>
    /// The <see cref="RunMode"/> of the command. A value of <see cref="RunMode.Default"/> uses the run mode of the
    /// command service.
    /// </summary>
    public RunMode RunMode { get; set; } = RunMode.Default;
    /// <summary>
    /// Whether extra arguments are ignored, or null to use <see cref="CommandServiceConfig.IgnoreExtraArgs"/>.
    /// </summary>
    public bool? IgnoreExtraArgs { get; }

    /// <summary>
    /// The summary of the command.
    /// </summary>
    /// <remarks>
    /// <see cref="SummaryAttribute"/> overrides the value of this property if present.
    /// </remarks>
    public string Summary { get; set; }

    /// <summary>
    /// The aliases of the command, added to its text.
    /// </summary>
    /// <remarks>
    /// <see cref="AliasAttribute"/> adds to the value of this property if present.
    /// </remarks>
    public string[] Aliases { get; set; }

    /// <summary>
    /// The remarks of the command.
    /// </summary>
    /// <remarks>
    /// <see cref="RemarksAttribute"/> overrides the value of this property if present.
    /// </remarks>
    public string Remarks { get; set; }

    /// <summary>
    /// Creates a new <see cref="CommandAttribute"/> without text.
    /// </summary>
    public CommandAttribute()
    {
        Text = null;
    }

    /// <summary>
    /// Creates a new <see cref="CommandAttribute"/> with the given text.
    /// </summary>
    /// <param name="text">The text that triggers the command.</param>
    public CommandAttribute(string text)
    {
        Text = text;
    }

    /// <summary>
    /// Creates a new <see cref="CommandAttribute"/> with the given text and extra argument handling.
    /// </summary>
    /// <param name="text">The text that triggers the command.</param>
    /// <param name="ignoreExtraArgs">Whether extra arguments are ignored.</param>
    public CommandAttribute(string text, bool ignoreExtraArgs)
    {
        Text = text;
        IgnoreExtraArgs = ignoreExtraArgs;
    }

    /// <summary>
    /// Creates a new <see cref="CommandAttribute"/> with the given values.
    /// </summary>
    /// <param name="text">The text that triggers the command.</param>
    /// <param name="ignoreExtraArgs">Whether extra arguments are ignored.</param>
    /// <param name="summary">The summary of the command.</param>
    /// <param name="aliases">The aliases of the command.</param>
    /// <param name="remarks">The remarks of the command.</param>
    public CommandAttribute(string text, bool ignoreExtraArgs, string summary = default, string[] aliases = default, string remarks = default)
    {
        Text = text;
        IgnoreExtraArgs = ignoreExtraArgs;
        Summary = summary;
        Aliases = aliases;
        Remarks = remarks;
    }
}
