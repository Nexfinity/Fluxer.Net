using Newtonsoft.Json;

namespace Fluxer.Net;

/// <summary>
/// Serializes Fluxer models the same way the library reads them from Fluxer, for example to keep cache entries in an
/// external store.
/// </summary>
public static class FluxerJson
{
    /// <summary>
    /// Serializes a model to JSON.
    /// </summary>
    public static string Serialize<T>(T value)
    {
        using StringWriter writer = new StringWriter();
        FluxerClient._gatewaySerializer.Serialize(writer, value);
        return writer.ToString();
    }

    /// <summary>
    /// Reads a model from JSON written by <see cref="Serialize{T}"/> or sent by Fluxer.
    /// </summary>
    public static T? Deserialize<T>(string json)
    {
        using StringReader reader = new StringReader(json);
        using JsonTextReader jsonReader = new JsonTextReader(reader);
        return FluxerClient._gatewaySerializer.Deserialize<T>(jsonReader);
    }
}
