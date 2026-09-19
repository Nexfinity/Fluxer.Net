using Newtonsoft.Json;

namespace Fluxer.Net.Extensions;

internal class SearchAuthorTypeConverter : JsonConverter
{
    public static readonly SearchAuthorTypeConverter Instance = new SearchAuthorTypeConverter();

    public override bool CanConvert(Type objectType) => true;
    public override bool CanRead => true;
    public override bool CanWrite => true;

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        if (reader.Value is string[] array)
        {
            SearchAuthorType? types = null;
            foreach (var i in array)
            {
                switch (i)
                {
                    case "user":
                        types |= SearchAuthorType.User;
                        break;
                    case "bot":
                        types |= SearchAuthorType.Bot;
                        break;
                    case "webhook":
                        types |= SearchAuthorType.Webhook;
                        break;
                }
            }
            return types;
        }
        throw new JsonSerializationException("Unknown search scope");
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        if (value is SearchAuthorType types)
        {
            writer.WriteStartArray();
            if (types.HasFlag(SearchAuthorType.User))
                writer.WriteValue("user");
            if (types.HasFlag(SearchAuthorType.Bot))
                writer.WriteValue("bot");
            if (types.HasFlag(SearchAuthorType.Webhook))
                writer.WriteValue("webhook");
            writer.WriteEndArray();
        }
    }
}