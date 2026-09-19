using Newtonsoft.Json;

namespace Fluxer.Net.Extensions;

internal class SearchEmbedTypeConverter : JsonConverter
{
    public static readonly SearchEmbedTypeConverter Instance = new SearchEmbedTypeConverter();

    public override bool CanConvert(Type objectType) => true;
    public override bool CanRead => true;
    public override bool CanWrite => true;

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        if (reader.Value is string[] array)
        {
            SearchEmbedType? types = null;
            foreach (var i in array)
            {
                switch (i)
                {
                    case "image":
                        types |= SearchEmbedType.Image;
                        break;
                    case "sound":
                        types |= SearchEmbedType.Sound;
                        break;
                    case "video":
                        types |= SearchEmbedType.Video;
                        break;
                    case "article":
                        types |= SearchEmbedType.Article;
                        break;
                }
            }
            return types;
        }
        throw new JsonSerializationException("Unknown search scope");
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        if (value is SearchEmbedType types)
        {
            writer.WriteStartArray();
            if (types.HasFlag(SearchEmbedType.Image))
                writer.WriteValue("image");

            if (types.HasFlag(SearchEmbedType.Sound))
                writer.WriteValue("sound");

            if (types.HasFlag(SearchEmbedType.Video))
                writer.WriteValue("video");

            if (types.HasFlag(SearchEmbedType.Article))
                writer.WriteValue("article");

            writer.WriteEndArray();
        }
    }
}