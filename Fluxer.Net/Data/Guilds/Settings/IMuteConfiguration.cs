namespace Fluxer.Net;

public interface IMuteConfiguration
{
    DateTimeOffset? EndAt { get; }

    int? SelectedTimeSeconds { get; }
}
