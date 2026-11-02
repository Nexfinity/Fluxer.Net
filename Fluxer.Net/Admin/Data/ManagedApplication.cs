namespace Fluxer.Net.Admin;

/// <summary>
/// An application read through a <see cref="FluxerAdminClient"/>.
/// </summary>
public class ManagedApplication : Application
{


    internal ManagedApplication(FluxerBaseClient client) : base(client)
    {

    }

    /// <summary>
    /// Creates an application from its API data.
    /// </summary>
    /// <param name="client">The admin client the application was read with.</param>
    /// <param name="json">The application data.</param>
    public static ManagedApplication Create(FluxerAdminClient client, ApplicationJson json)
    {
        ManagedApplication data = new ManagedApplication(client);
        data.Update(json);
        return data;
    }
}
