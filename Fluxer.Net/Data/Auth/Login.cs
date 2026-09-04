namespace Fluxer.Net;

/// <inheritdoc />
public class Login : Entity, ILogin
{
    /// <inheritdoc />
    public string Token { get; private set; }

    /// <inheritdoc />
    public ulong UserId { get; private set; }

    internal Login(FluxerBaseClient client) : base(client)
    {

    }

    /// <summary>
    /// Create a Login object from json.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="json"></param>
    /// <returns></returns>
    public static Login Create(FluxerBaseClient client, LoginJson json)
    {
        Login data = new Login(client);
        data.Update(client, json);
        return data;
    }

    internal void Update(FluxerBaseClient client, LoginJson json)
    {
        Token = json.Token;
        UserId = json.UserId;
    }
}
