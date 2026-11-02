using Newtonsoft.Json;

namespace Fluxer.Net;


/// <summary>
/// Converts the value of an <c>Optional&lt;T&gt;</c> to and from JSON, using an inner converter when one is given.
/// </summary>
/// <typeparam name="T">The type of the wrapped value.</typeparam>
/// <remarks>
/// An empty optional is written as the default value of <typeparamref name="T"/>, not as null.
/// </remarks>
public class OptionalSerializerConverter<T> : JsonConverter
{
    private readonly JsonConverter _innerConverter;

    /// <inheritdoc />
    public override bool CanConvert(Type objectType) => true;

    /// <inheritdoc />
    public override bool CanRead => true;

    /// <inheritdoc />
    public override bool CanWrite => true;

    /// <summary>
    /// Creates a converter for the wrapped value.
    /// </summary>
    /// <param name="innerConverter">The converter for the wrapped value, or null to use the serializer.</param>
    public OptionalSerializerConverter(JsonConverter innerConverter)
    {
        _innerConverter = innerConverter;
    }

    /// <inheritdoc />
    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        T obj;
        if (_innerConverter != null)
        {
            object o = _innerConverter.ReadJson(reader, typeof(T), null, serializer);
            if (o is Optional<T>)
                return o;

            obj = (T)o;
        }
        else
            obj = serializer.Deserialize<T>(reader);

        return Optional.Some(obj);
    }

    /// <inheritdoc />
    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        value = ((Optional<T>)value!).Value;
        if (_innerConverter != null)
            _innerConverter.WriteJson(writer, value, serializer);
        else
            serializer.Serialize(writer, value, typeof(T));
    }
}