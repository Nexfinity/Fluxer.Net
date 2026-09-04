namespace Fluxer.Net;

/// <inheritdoc />
public class Application : PartialApplication, IApplication
{
    /// <inheritdoc />
    public string[] RedirectUrls { get; private set; }

    /// <inheritdoc />
    public User Bot { get; private set; }

    IUser IApplication.Bot => Bot;

    internal Application(FluxerBaseClient client) : base(client)
    {

    }

    /// <summary>
    /// Create a Application object from json.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="json"></param>
    /// <returns></returns>
    public static Application Create(FluxerBaseClient client, ApplicationJson json)
    {
        Application data = new Application(client);
        data.Update(json);
        return data;
    }

    internal virtual void Update(ApplicationJson json)
    {
        base.Update(json);
        RedirectUrls = json.RedirectUrls;
        Bot = User.Create(Client, json.Bot);
    }
}