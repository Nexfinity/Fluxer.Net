namespace Fluxer.Net;

public interface IClientStatus
{
    string? Desktop { get; }

    string? Mobile { get; }

    string? Web { get; }
}
