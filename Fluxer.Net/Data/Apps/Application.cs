namespace Fluxer.Net;

/// <inheritdoc />
public class Application : PartialApplication, IApplication
{
    public string[] RedirectUrls { get; set; }

    public User Bot { get; set; }

    IUser IApplication.Bot => Bot;

    internal Application(BaseClient client) : base(client)
    {

    }

    public static Application Create(BaseClient client, ApplicationJson json)
    {
        var data = new Application(client);
        data.Update(client, json);
        return data;
    }

    internal void Update(BaseClient client, ApplicationJson json)
    {
        base.Update(client, json);
        RedirectUrls = json.RedirectUrls;
        Bot = User.Create(client, json.Bot);
    }
}
