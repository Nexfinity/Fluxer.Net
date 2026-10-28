namespace Fluxer.Net;

/// <summary>
/// How a timestamp tag is shown to the reader, in their own time zone and locale.
/// </summary>
public enum TimestampTagStyles
{
    /// <summary>
    /// Hours and minutes, for example 16:20.
    /// </summary>
    ShortTime = 't',

    /// <summary>
    /// Hours, minutes and seconds, for example 16:20:30.
    /// </summary>
    LongTime = 'T',

    /// <summary>
    /// A numeric date, for example 20/04/2021.
    /// </summary>
    ShortDate = 'd',

    /// <summary>
    /// A written date, for example 20 April 2021.
    /// </summary>
    LongDate = 'D',

    /// <summary>
    /// A written date with the time, for example 20 April 2021 16:20.
    /// </summary>
    ShortDateTime = 'f',

    /// <summary>
    /// The weekday, written date and time, for example Tuesday, 20 April 2021 16:20.
    /// </summary>
    LongDateTime = 'F',

    /// <summary>
    /// A numeric date with hours and minutes, for example 20/04/2021 16:20.
    /// </summary>
    ShortDateShortTime = 's',

    /// <summary>
    /// A numeric date with hours, minutes and seconds, for example 20/04/2021 16:20:30.
    /// </summary>
    ShortDateMediumTime = 'S',

    /// <summary>
    /// The distance from now, for example in 2 hours or 3 days ago.
    /// </summary>
    Relative = 'R'
}

/// <summary>
/// A timestamp that every reader sees in their own time zone.
/// </summary>
public readonly struct TimestampTag
{
    /// <summary>
    /// Creates a tag for a moment in time.
    /// </summary>
    public TimestampTag(DateTimeOffset time, TimestampTagStyles style = TimestampTagStyles.ShortDateTime)
    {
        Time = time;
        Style = style;
    }

    /// <summary>
    /// The moment the tag shows.
    /// </summary>
    public DateTimeOffset Time { get; }

    /// <summary>
    /// How the tag is shown.
    /// </summary>
    public TimestampTagStyles Style { get; }

    /// <summary>
    /// Creates a tag for a moment in time.
    /// </summary>
    public static TimestampTag FromDateTimeOffset(DateTimeOffset time,
        TimestampTagStyles style = TimestampTagStyles.ShortDateTime) => new(time, style);

    /// <summary>
    /// Creates a tag for a moment in time, treating an unspecified kind as UTC.
    /// </summary>
    public static TimestampTag FromDateTime(DateTime time, TimestampTagStyles style = TimestampTagStyles.ShortDateTime) =>
        new(time.Kind == DateTimeKind.Unspecified ? new DateTimeOffset(time, TimeSpan.Zero) : new DateTimeOffset(time), style);

    /// <summary>
    /// Returns the markup for the tag, for example <c>&lt;t:1618953630:R&gt;</c>.
    /// </summary>
    public override string ToString() => ToString(Style);

    /// <summary>
    /// Returns the markup for the tag in another style.
    /// </summary>
    public string ToString(TimestampTagStyles style) => $"<t:{Time.ToUnixTimeSeconds()}:{(char)style}>";
}
