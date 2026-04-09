namespace Fluxer.Net;

public interface IActivity
{
    string Name { get; }

    int Type { get; }

    string? Url { get; }

    long CreatedAt { get; }

    IActivityTimestamps? Timestamps { get; }

    string? Details { get; }

    string? State { get; }

    IEmoji? Emoji { get; }
}

public interface IActivityTimestamps
{
    long? Start { get; }

    long? End { get; }
}