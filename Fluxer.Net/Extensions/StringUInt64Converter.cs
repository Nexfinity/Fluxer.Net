using Newtonsoft.Json;

namespace Fluxer.Net.Extensions;

/// <summary>
/// Deserializes a JSON value as <see cref="ulong"/>, accepting both a raw JSON number
/// and a quoted numeric string (e.g. <c>"8933636165184"</c>).
/// The Fluxer gateway sends permission bitfields as quoted strings to avoid JavaScript
/// integer precision loss, but the REST API may return them as plain numbers.
/// </summary>
internal class StringUInt64Converter : JsonConverter<ulong>
{
    public override ulong ReadJson(JsonReader reader, Type objectType, ulong existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.String)
            return ulong.Parse((string)reader.Value!);

        if (reader.TokenType == JsonToken.Integer)
            return Convert.ToUInt64(reader.Value);

        throw new JsonSerializationException(
            $"Unexpected token type '{reader.TokenType}' when deserializing ulong at path '{reader.Path}'.");
    }

    public override void WriteJson(JsonWriter writer, ulong value, JsonSerializer serializer)
        => writer.WriteValue(value.ToString());
}
