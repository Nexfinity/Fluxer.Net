namespace Fluxer.Net;

/// <inheritdoc />
public class CurrentApplication : Application
{
    public User Owner { get; internal set; }

    internal CurrentApplication(FluxerBaseClient client) : base(client)
    {

    }

    /// <summary>
    /// Create a Application object from json.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="json"></param>
    /// <returns></returns>
    public static CurrentApplication Create(FluxerBaseClient client, CurrentApplicationJson json)
    {
        CurrentApplication data = new CurrentApplication(client);
        data.Update(client, json);
        return data;
    }

    internal virtual void Update(FluxerBaseClient client, CurrentApplicationJson json)
    {
        base.Update(client, json);
        Owner = User.Create(client, json.Owner);
    }
}