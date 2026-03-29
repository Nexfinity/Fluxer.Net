namespace Fluxer.Net;

public interface IFluxerOAuthToken
{
    IPartialApplication Application { get; }

    string[] Scopes { get; }

    DateTime Expires { get; }

    IFluxerOAuthUser User { get; }
}
