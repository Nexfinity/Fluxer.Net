namespace Fluxer.Net;

public class Cacheable<TEntity> where TEntity : ISnowflake
{
    public ulong Id { get; private set; }

    public TEntity? Value { get; private set; }

    private Func<Task<TEntity?>> DownloadFunc { get; }

    public Cacheable(ulong id, TEntity value, Func<Task<TEntity>> downloadFunc)
    {
        Id = id;
        Value = value;
        DownloadFunc = downloadFunc;
    }

    public async Task<TEntity?> GetOrDownloadAsync() => Value != null ? Value : await DownloadFunc();
}
