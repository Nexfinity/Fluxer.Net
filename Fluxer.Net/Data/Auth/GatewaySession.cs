namespace Fluxer.Net;

/// <inheritdoc />
public class GatewaySession : Entity, IGatewaySession
{
    /// <inheritdoc />
    public string SessionId { get; private set; }

    /// <inheritdoc />
    public string Status { get; private set; }

    /// <inheritdoc />
    public bool IsMobile { get; private set; }

    /// <inheritdoc />
    public bool IsAfk { get; private set; }

    internal GatewaySession(FluxerBaseClient client) : base(client)
    {

    }

    /// <summary>
    /// Create a AuthSession object from json.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="json"></param>
    /// <returns></returns>
    public static GatewaySession Create(FluxerBaseClient client, GatewaySessionJson json)
    {
        GatewaySession data = new GatewaySession(client);
        data.Update(json);
        return data;
    }

    internal void Update(GatewaySessionJson json)
    {
        SessionId = json.SessionId;
        Status = json.Status;
        IsMobile = json.IsMobile;
        IsAfk = json.IsAfk;
    }
}