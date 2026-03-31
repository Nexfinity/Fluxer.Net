namespace Fluxer.Net;

/// <inheritdoc />
public class Login : Entity, ILogin
{
    /// <inheritdoc />
    public string Token { get; internal set; }

    /// <inheritdoc />
    public ulong UserId { get; internal set; }

    internal Login(FluxerBaseClient client) : base(client)
    {

    }

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
