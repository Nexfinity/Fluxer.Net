namespace Fluxer.Net;

public class PartialChannel : Entity, IPartialChannel
{
    /// <inheritdoc />
    public ulong Id { get; private set; }

    /// <inheritdoc />
    public DateTimeOffset CreatedAt => SnowflakeUtils.FromSnowflake(Id);

    /// <inheritdoc />
    public string Mention => $"<#{Id}>";

    /// <inheritdoc />
    public string? Name { get; private set; }

    /// <inheritdoc />
    public ChannelType Type { get; private set; }

    /// <inheritdoc/>
    public bool IsTextable => Channel.TextableTypes(Type);

    internal PartialChannel(FluxerBaseClient client) : base(client)
    {

    }

    public static PartialChannel Create(FluxerBaseClient client, PartialChannelJson json)
    {
        PartialChannel data = new PartialChannel(client);
        data.Update(json);
        return data;
    }

    internal virtual void Update(PartialChannelJson json)
    {
        Id = json.Id;
        Type = json.Type;
        Name = json.Name;
    }
}
