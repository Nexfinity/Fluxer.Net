using Newtonsoft.Json;

namespace Fluxer.Net.Extensions;

internal class ChannelPermissionsConverter : JsonConverter<ChannelPermissions>
{
    public override ChannelPermissions ReadJson(JsonReader reader, Type objectType, ChannelPermissions GuildPermissions, bool hasExistingValue, JsonSerializer serializer)
    {
        if (reader.Value == null)
            return new ChannelPermissions(0);

        if (reader.TokenType == JsonToken.String)
            return new ChannelPermissions((GuildPermission)ulong.Parse((string)reader.Value));

        if (reader.TokenType == JsonToken.Integer)
            return new ChannelPermissions((GuildPermission)Convert.ToUInt64(reader.Value));

        throw new JsonSerializationException(
            $"Unexpected token type '{reader.TokenType}' when deserializing GuildPermissions at path '{reader.Path}'.");
    }

    public override void WriteJson(JsonWriter writer, ChannelPermissions value, JsonSerializer serializer)
        => writer.WriteValue((ulong)value.RawValue);
}