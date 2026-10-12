namespace Fluxer.Net;

/// <inheritdoc />
public class Call : Entity, ICall
{
    /// <inheritdoc />
    public ulong ChannelId { get; private set; }

    /// <inheritdoc />
    public ulong MessageId { get; private set; }

    /// <inheritdoc />
    public string? Region { get; private set; }

    /// <inheritdoc />
    public ulong[] RingingUsers { get; private set; }

    /// <inheritdoc />
    public VoiceState[] VoiceStates { get; private set; }

    /// <inheritdoc />
    public ulong[]? Recipients { get; private set; }

    /// <inheritdoc />
    public DateTimeOffset? CreatedAt { get; private set; }

    IVoiceState[] ICall.VoiceStates => VoiceStates;

    internal Call(FluxerBaseClient client) : base(client)
    {

    }

    /// <summary>
    /// Create a Call object from json.
    /// </summary>
    /// <returns><see cref="Call"/></returns>
    public static Call Create(FluxerBaseClient client, CallJson json)
    {
        Call data = new Call(client);
        data.Update(json);
        return data;
    }

    internal void Update(CallJson json)
    {
        ChannelId = json.ChannelId;
        MessageId = json.MessageId;
        Region = json.Region;
        RingingUsers = json.RingingUsers;
        if (json.VoiceStates != null)
            VoiceStates = json.VoiceStates.Select(x => VoiceState.Create(Client, x)).ToArray();
        Recipients = json.Recipients;
        CreatedAt = json.CreatedAt;
    }
}
