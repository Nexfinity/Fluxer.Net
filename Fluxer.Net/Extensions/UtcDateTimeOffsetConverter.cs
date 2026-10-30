using System.Globalization;
using Newtonsoft.Json;

namespace Fluxer.Net.Extensions;

/// <summary>
/// Writes <see cref="DateTimeOffset"/> values as UTC ISO 8601 timestamps ending in <c>Z</c>.
/// </summary>
public class UtcDateTimeOffsetConverter : JsonConverter
{
    /// <inheritdoc />
    public override bool CanRead => false;

    /// <inheritdoc />
    public override bool CanConvert(Type objectType)
        => objectType == typeof(DateTimeOffset) || objectType == typeof(DateTimeOffset?);

    /// <inheritdoc />
    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        => throw new NotSupportedException();

    /// <inheritdoc />
    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        if (value is not DateTimeOffset date)
        {
            writer.WriteNull();
            return;
        }

        writer.WriteValue(date.UtcDateTime.ToString("yyyy-MM-dd'T'HH:mm:ss.fff'Z'", CultureInfo.InvariantCulture));
    }
}
