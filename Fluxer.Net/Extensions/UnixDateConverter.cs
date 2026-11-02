using Newtonsoft.Json;

namespace Fluxer.Net.Extensions;

/// <summary>
/// Converts <see cref="DateTimeOffset"/> to and from a Unix timestamp in milliseconds. Reading accepts a string or a number.
/// </summary>
public class UnixDateConverter : JsonConverter<DateTimeOffset>
{
    /// <inheritdoc />
    public override DateTimeOffset ReadJson(JsonReader reader, Type objectType, DateTimeOffset date, bool hasExistingValue, JsonSerializer serializer)
    {
        try
        {
            if (reader.TokenType == JsonToken.String)
                return DateTimeOffset.FromUnixTimeMilliseconds(long.Parse((string)reader.Value));

            if (reader.TokenType == JsonToken.Integer)
                return DateTimeOffset.FromUnixTimeMilliseconds(Convert.ToInt64(reader.Value));
        }
        catch { }

        throw new JsonSerializationException(
            $"Unexpected token type '{reader.TokenType}' when deserializing DateTimeOffset at path '{reader.Path}'.");
    }

    /// <inheritdoc />
    public override void WriteJson(JsonWriter writer, DateTimeOffset value, JsonSerializer serializer)
        => writer.WriteValue(value.ToUnixTimeMilliseconds());
}