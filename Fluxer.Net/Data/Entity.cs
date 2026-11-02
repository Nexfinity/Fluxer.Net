namespace Fluxer.Net;

/// <summary>
/// An object with a snowflake id.
/// </summary>
public interface ISnowflake
{
    /// <summary>
    /// The unique identifier (snowflake) of the object.
    /// </summary>
    ulong Id { get; }

    /// <summary>
    /// When the object was created, in UTC.
    /// </summary>
    DateTimeOffset CreatedAt { get; }
}

/// <summary>
/// A Fluxer object that is attached to the client it was received from.
/// </summary>
public abstract class Entity
{
    /// <summary>
    /// Creates an entity attached to a client.
    /// </summary>
    /// <param name="client">The client the entity belongs to.</param>
    public Entity(FluxerBaseClient client)
    {
        Client = client;
    }

    internal FluxerBaseClient Client { get; set; }
}