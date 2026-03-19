namespace Fluxer.Net;

public class Login : Entity
{
    internal Login(BaseClient client) : base(client)
    {

    }

    public static Login Create(BaseClient client, LoginJson json)
    {
        return new Login(client);
    }

    internal void Update(LoginJson json)
    {

    }
}
