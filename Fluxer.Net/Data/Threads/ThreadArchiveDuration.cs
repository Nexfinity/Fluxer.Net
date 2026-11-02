namespace Fluxer.Net;

/// <summary>
/// The time of inactivity after which a thread is archived.
/// </summary>
public enum ThreadArchiveDuration
{
    /// <summary>
    /// 1 hour (60 minutes).
    /// </summary>
    OneHour = 60,

    /// <summary>
    /// 1 day (1440 minutes).
    /// </summary>
    OneDay = 1440,

    /// <summary>
    /// 3 days (4320 minutes).
    /// </summary>
    ThreeDays = 4320,

    /// <summary>
    /// 1 week (10080 minutes).
    /// </summary>
    OneWeek = 10080
}