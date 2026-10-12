using Newtonsoft.Json;

namespace Fluxer.Net.Extensions;

public class UnixDateConverter : JsonConverter<DateTimeOffset>
{
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

    public override void WriteJson(JsonWriter writer, DateTimeOffset value, JsonSerializer serializer)
        => writer.WriteValue(value.ToUnixTimeMilliseconds());
}