namespace Fluxer.Net;

/// <summary>
/// An activity a user is doing, such as playing a game.
/// </summary>
public interface IActivity
{
    /// <summary>
    /// The name of the activity.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// The activity type, as the API sends it.
    /// </summary>
    int Type { get; }

    /// <summary>
    /// The URL of the activity, or null when it has none.
    /// </summary>
    string? Url { get; }

    /// <summary>
    /// The creation time of the activity, as the API sends it.
    /// </summary>
    long CreatedAt { get; }

    /// <summary>
    /// The start and end times of the activity, or null when it has none.
    /// </summary>
    IActivityTimestamps? Timestamps { get; }

    /// <summary>
    /// The details of the activity, or null when it has none.
    /// </summary>
    string? Details { get; }

    /// <summary>
    /// The state of the activity, or null when it has none.
    /// </summary>
    string? State { get; }

    /// <summary>
    /// The emoji of the activity, or null when it has none.
    /// </summary>
    IEmoji? Emoji { get; }
}

/// <summary>
/// The start and end times of an activity.
/// </summary>
public interface IActivityTimestamps
{
    /// <summary>
    /// The start time of the activity, as the API sends it, or null when it is not set.
    /// </summary>
    long? Start { get; }

    /// <summary>
    /// The end time of the activity, as the API sends it, or null when it is not set.
    /// </summary>
    long? End { get; }
}