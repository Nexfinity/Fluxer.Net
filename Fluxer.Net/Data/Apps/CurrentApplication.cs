namespace Fluxer.Net;

/// <summary>
/// The application of the authenticated bot, with its owner user.
/// </summary>
public class CurrentApplication : Application, ICurrentApplication
{
    /// <inheritdoc />
    public User Owner { get; private set; }

    /// <summary>
    /// The public key used to verify requests signed by Fluxer for this application.
    /// </summary>
    public string? VerifyKey { get; private set; }

    IUser ICurrentApplication.Owner => Owner;

    internal CurrentApplication(FluxerBaseClient client) : base(client)
    {

    }

    /// <summary>
    /// Creates the current application from its JSON.
    /// </summary>
    /// <returns><see cref="CurrentApplication"/></returns>
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
        VerifyKey = json.VerifyKey;
    }
}