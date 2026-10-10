namespace Fluxer.Net;

/// <inheritdoc />
public class MuteConfiguration : Entity, IMuteConfiguration
{
    /// <inheritdoc />
    public DateTimeOffset? EndAt { get; private set; }

    /// <inheritdoc />
    public int? SelectedTimeSeconds { get; private set; }

    internal MuteConfiguration(FluxerBaseClient client) : base(client)
    {

    }

    /// <summary>
    /// Create a GuildChannelOverride object from json.
    /// </summary>
    /// <returns><see cref="MuteConfiguration"/></returns>
    public static MuteConfiguration Create(FluxerBaseClient client, MuteConfigurationJson json)
    {
        MuteConfiguration data = new MuteConfiguration(client);
        data.Update(json);
        return data;
    }

    internal void Update(MuteConfigurationJson json)
    {
        EndAt = json.EndAt;
        SelectedTimeSeconds = json.SelectedTimeSeconds;
    }
}
