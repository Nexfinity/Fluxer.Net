namespace Fluxer.Net;

/// <summary>
/// Snowflake object with unique ID.
/// </summary>
public interface ISnowflake
{
    /// <summary>
    /// Fluxer unique identifier (snowflake) for the object.
    /// </summary>
    ulong Id { get; }

    /// <summary>
    /// Object created at UTC.
    /// </summary>
    DateTimeOffset CreatedAt { get; }
}

/// <summary>
/// Fluxer entity with client attached.
/// </summary>
public abstract class Entity
{
    public Entity(FluxerBaseClient client)
    {
        Client = client;
    }

    internal FluxerBaseClient Client { get; set; }
}