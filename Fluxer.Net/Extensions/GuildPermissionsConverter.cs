using Newtonsoft.Json;

namespace Fluxer.Net.Extensions;

/// <summary>
/// Converts <see cref="GuildPermissions"/> to and from JSON. Reading accepts a string or a number, and writing
/// always writes a string.
/// </summary>
public class GuildPermissionsConverter : JsonConverter<GuildPermissions>
{
    /// <inheritdoc />
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

    /// <inheritdoc />
    public override void WriteJson(JsonWriter writer, GuildPermissions value, JsonSerializer serializer)
        => writer.WriteValue(((ulong)value.RawValue).ToString());
}