namespace Fluxer.Net;

/// <inheritdoc />
public class Application : PartialApplication, IApplication
{
    /// <inheritdoc />
    public string[] RedirectUrls { get; internal set; }

    /// <inheritdoc />
    public User Bot { get; internal set; }

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
        data.Update(client, json);
        return data;
    }

    internal virtual void Update(FluxerBaseClient client, ApplicationJson json)
    {
        base.Update(client, json);
        RedirectUrls = json.RedirectUrls;
        Bot = User.Create(client, json.Bot);
    }
}
