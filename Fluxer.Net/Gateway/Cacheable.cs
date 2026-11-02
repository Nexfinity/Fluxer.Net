namespace Fluxer.Net;

/// <summary>
/// An entity an event refers to, with a function that downloads it when it is not cached.
/// </summary>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
public class Cacheable<TEntity> where TEntity : ISnowflake
{
    /// <summary>
    /// The id of the entity.
    /// </summary>
    public ulong Id { get; private set; }

    /// <summary>
    /// The cached entity, or null when it was not cached.
    /// </summary>
    public TEntity? Value { get; private set; }

    private Func<Task<TEntity?>> DownloadFunc { get; }

    /// <summary>
    /// Creates the reference from the entity's id, its cached value and the function that downloads it.
    /// </summary>
    /// <param name="id">The id of the entity.</param>
    /// <param name="value">The cached entity, or null when it was not cached.</param>
    /// <param name="downloadFunc">Downloads the entity when it is not cached.</param>
    public Cacheable(ulong id, TEntity value, Func<Task<TEntity>> downloadFunc)
    {
        Id = id;
        Value = value;
        DownloadFunc = downloadFunc;
    }

    /// <summary>
    /// Gets the cached entity, or downloads it when it is not cached.
    /// </summary>
    /// <returns>The entity, or null when it could not be downloaded.</returns>
    public async Task<TEntity?> GetOrDownloadAsync()
    {
        if (Value != null)
            return Value;

        Task<TEntity?>? download = DownloadFunc?.Invoke();
        return download is null ? default : await download;
    }
}
