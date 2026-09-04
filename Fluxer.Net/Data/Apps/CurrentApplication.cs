namespace Fluxer.Net;

/// <inheritdoc />
public class CurrentApplication : Application, ICurrentApplication
{
    /// <inheritdoc />
    public User Owner { get; private set; }

    IUser ICurrentApplication.Owner => Owner;

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
        data.Update(json);
        return data;
    }

    internal virtual void Update(CurrentApplicationJson json)
    {
        base.Update(json);
        Owner = User.Create(Client, json.Owner);
    }
}