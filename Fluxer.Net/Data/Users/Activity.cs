namespace Fluxer.Net;

/// <summary>
/// An activity a user is doing, such as playing a game, with its name and details.
/// </summary>
public class Activity : Entity, IActivity
{
    /// <inheritdoc />
    public string Name { get; private set; }

    /// <inheritdoc />
    public int Type { get; private set; }

    /// <inheritdoc />
    public string? Url { get; private set; }

    /// <inheritdoc />
    public long CreatedAt { get; private set; }

    /// <inheritdoc />
    public ActivityTimestamps? Timestamps { get; private set; }

    /// <inheritdoc />
    public string? Details { get; private set; }

    /// <inheritdoc />
    public string? State { get; private set; }

    /// <inheritdoc />
    public Emoji? Emoji { get; private set; }

    IActivityTimestamps? IActivity.Timestamps => Timestamps;

    IEmoji? IActivity.Emoji => Emoji;

    internal Activity(FluxerBaseClient client) : base(client)
    {

    }

    /// <summary>
    /// Creates an activity from its JSON data.
    /// </summary>
    /// <param name="client">The client the activity belongs to.</param>
    /// <param name="json">The activity data.</param>
    /// <returns>The activity.</returns>
    public static Activity Create(FluxerBaseClient client, ActivityJson json)
    {
        Activity data = new Activity(client);
        data.Update(json);
        return data;
    }

    internal void Update(ActivityJson json)
    {
        Name = json.Name;
        Type = json.Type;
        Url = json.Url;
        CreatedAt = json.CreatedAt;
        Timestamps = ActivityTimestamps.Create(Client, json.Timestamps);
        Details = json.Details;
        State = json.State;
        Emoji = Emoji.Create(Client, json.Emoji);
    }
}

/// <summary>
/// The start and end times of an activity.
/// </summary>
public class ActivityTimestamps : IActivityTimestamps
{
    /// <inheritdoc />
    public long? Start { get; internal set; }

    /// <inheritdoc />
    public long? End { get; internal set; }

    internal ActivityTimestamps(FluxerBaseClient client)
    {

    }

    /// <summary>
    /// Creates the timestamps from their JSON data.
    /// </summary>
    /// <param name="client">The client the timestamps belong to.</param>
    /// <param name="json">The timestamps data, or null when the activity has none.</param>
    /// <returns>The timestamps, or null when <paramref name="json"/> is null.</returns>
    public static ActivityTimestamps? Create(FluxerBaseClient client, ActivityTimestampsJson? json)
    {
        if (json == null)
            return null;

        ActivityTimestamps data = new ActivityTimestamps(client);
        data.Update(json);
        return data;
    }

    internal void Update(ActivityTimestampsJson json)
    {
        Start = json.Start;
        End = json.End;
    }
}