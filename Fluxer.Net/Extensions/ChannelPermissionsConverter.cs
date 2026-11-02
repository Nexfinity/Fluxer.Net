using Newtonsoft.Json;

namespace Fluxer.Net.Extensions;

/// <summary>
/// Converts <see cref="ChannelPermissions"/> to and from JSON. Reading accepts a string or a number, and writing
/// always writes a string.
/// </summary>
public class ChannelPermissionsConverter : JsonConverter<ChannelPermissions>
{
    /// <inheritdoc />
    public override ChannelPermissions ReadJson(JsonReader reader, Type objectType, ChannelPermissions GuildPermissions, bool hasExistingValue, JsonSerializer serializer)
    {
        if (reader.Value == null)
            return new ChannelPermissions((ChannelPermission)0);

        if (reader.TokenType == JsonToken.String)
            return new ChannelPermissions((ChannelPermission)ulong.Parse((string)reader.Value));

        if (reader.TokenType == JsonToken.Integer)
            return new ChannelPermissions((ChannelPermission)Convert.ToUInt64(reader.Value));

        throw new JsonSerializationException(
            $"Unexpected token type '{reader.TokenType}' when deserializing ChannelPermissions at path '{reader.Path}'.");
    }

    /// <inheritdoc />
    public override void WriteJson(JsonWriter writer, ChannelPermissions value, JsonSerializer serializer)
        => writer.WriteValue(((ulong)value.RawValue).ToString());
}