namespace Fluxer.Net;

public class Login : Entity
{
    public string Token { get; set; }

    public ulong UserId { get; set; }

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
