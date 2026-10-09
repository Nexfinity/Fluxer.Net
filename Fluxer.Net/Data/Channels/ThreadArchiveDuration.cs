namespace Fluxer.Net;

/// <summary>
/// The duration of time until a thread is locked.
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