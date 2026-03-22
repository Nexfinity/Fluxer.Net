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

    IActivityEmoji? Emoji { get; }
}

public interface IActivityTimestamps
{
    long? Start { get; }

    long? End { get; }
}

public interface IActivityEmoji
{
    ulong? Id { get; }

    string Name { get; }

    bool Animated { get; }
}