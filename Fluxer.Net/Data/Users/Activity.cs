namespace Fluxer.Net;

/// <inheritdoc />
public class Activity : Entity, IActivity
{
    /// <inheritdoc />
    public string Name { get; internal set; }

    /// <inheritdoc />
    public int Type { get; internal set; }

    /// <inheritdoc />
    public string? Url { get; internal set; }

    /// <inheritdoc />
    public long CreatedAt { get; internal set; }

    /// <inheritdoc />
    public ActivityTimestamps? Timestamps { get; internal set; }

    /// <inheritdoc />
    public string? Details { get; internal set; }

    /// <inheritdoc />
    public string? State { get; internal set; }

    /// <inheritdoc />
    public ActivityEmoji? Emoji { get; internal set; }

    IActivityTimestamps? IActivity.Timestamps => Timestamps;

    IActivityEmoji? IActivity.Emoji => Emoji;

    internal Activity(BaseClient client) : base(client)
    {

    }

    public static Activity Create(BaseClient client, ActivityJson json)
    {
        var data = new Activity(client);
        data.Update(client, json);
        return data;
    }

    internal void Update(BaseClient client, ActivityJson json)
    {
        Name = json.Name;
        Type = json.Type;
        Url = json.Url;
        CreatedAt = json.CreatedAt;
        Timestamps = ActivityTimestamps.Create(client, json.Timestamps);
        Details = json.Details;
        State = json.State;
        Emoji = ActivityEmoji.Create(client, json.Emoji);
    }
}

/// <inheritdoc />
public class ActivityTimestamps : IActivityTimestamps
{
    /// <inheritdoc />
    public long? Start { get; internal set; }

    /// <inheritdoc />
    public long? End { get; internal set; }

    internal ActivityTimestamps(BaseClient client)
    {

    }

    public static ActivityTimestamps? Create(BaseClient client, ActivityTimestampsJson? json)
    {
        if (json == null)
            return null;

        var data = new ActivityTimestamps(client);
        data.Update(client, json);
        return data;
    }

    internal void Update(BaseClient client, ActivityTimestampsJson json)
    {
        Start = json.Start;
        End = json.End;
    }
}

/// <inheritdoc />
public class ActivityEmoji : IActivityEmoji
{
    /// <inheritdoc />
    public ulong? Id { get; internal set; }

    /// <inheritdoc />
    public string Name { get; internal set; }

    /// <inheritdoc />
    public bool Animated { get; internal set; }

    internal ActivityEmoji(BaseClient client)
    {

    }

    public static ActivityEmoji? Create(BaseClient client, ActivityEmojiJson? json)
    {
        if (json == null)
            return null;

        var data = new ActivityEmoji(client);
        data.Update(client, json);
        return data;
    }

    internal void Update(BaseClient client, ActivityEmojiJson json)
    {
        Id = json.Id;
        Name = json.Name;
        Animated = json.Animated;
    }
}