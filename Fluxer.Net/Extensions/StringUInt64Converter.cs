using Newtonsoft.Json;

namespace Fluxer.Net.Extensions;

/// <summary>
/// Converts a <see cref="ulong"/> to and from JSON. Reading accepts a quoted numeric string or a number, and writing
/// always writes a quoted string.
/// </summary>
public class StringUInt64Converter : JsonConverter<ulong>
{
    /// <inheritdoc />
    public override ulong ReadJson(JsonReader reader, Type objectType, ulong existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.String)
            return ulong.Parse((string)reader.Value!);

        if (reader.TokenType == JsonToken.Integer)
            return Convert.ToUInt64(reader.Value);

        throw new JsonSerializationException(
            $"Unexpected token type '{reader.TokenType}' when deserializing ulong at path '{reader.Path}'.");
    }

    /// <inheritdoc />
    public override void WriteJson(JsonWriter writer, ulong value, JsonSerializer serializer)
        => writer.WriteValue(value.ToString());
}
