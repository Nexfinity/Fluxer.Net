namespace Fluxer.Net;

/// <inheritdoc />
public class Login : Entity, ILogin
{
    /// <inheritdoc />
    public string Token { get; internal set; }

    /// <inheritdoc />
    public ulong UserId { get; internal set; }

    internal Login(BaseClient client) : base(client)
    {

    }

    public static Login Create(BaseClient client, LoginJson json)
    {
        var data = new Login(client);
        data.Update(client, json);
        return data;
    }

    internal void Update(BaseClient client, LoginJson json)
    {
        Token = json.Token;
        UserId = json.UserId;
    }
}
