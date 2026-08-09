using Newtonsoft.Json;

namespace Fluxer.Net.Extensions;

public class GuildPermissionsConverter : JsonConverter<GuildPermissions>
{
    public override GuildPermissions ReadJson(JsonReader reader, Type objectType, GuildPermissions GuildPermissions, bool hasExistingValue, JsonSerializer serializer)
    {
        if (reader.Value == null)
            return new GuildPermissions(0);

        if (reader.TokenType == JsonToken.String)
            return new GuildPermissions((GuildPermission)ulong.Parse((string)reader.Value));

        if (reader.TokenType == JsonToken.Integer)
            return new GuildPermissions((GuildPermission)Convert.ToUInt64(reader.Value));

        throw new JsonSerializationException(
            $"Unexpected token type '{reader.TokenType}' when deserializing GuildPermissions at path '{reader.Path}'.");
    }

    public override void WriteJson(JsonWriter writer, GuildPermissions value, JsonSerializer serializer)
        => writer.WriteValue((ulong)value.RawValue);
}
