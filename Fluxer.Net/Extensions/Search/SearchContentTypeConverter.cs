using Newtonsoft.Json;

namespace Fluxer.Net.Extensions;

internal class SearchContentTypeConverter : JsonConverter
{
    public static readonly SearchContentTypeConverter Instance = new SearchContentTypeConverter();

    public override bool CanConvert(Type objectType) => true;
    public override bool CanRead => true;
    public override bool CanWrite => true;

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        if (reader.Value is string[] array)
        {
            SearchContentType? types = null;
            foreach (var i in array)
            {
                switch (i)
                {
                    case "image":
                        types |= SearchContentType.Image;
                        break;
                    case "sound":
                        types |= SearchContentType.Sound;
                        break;
                    case "video":
                        types |= SearchContentType.Video;
                        break;
                    case "file":
                        types |= SearchContentType.File;
                        break;
                    case "sticker":
                        types |= SearchContentType.Sticker;
                        break;
                    case "embed":
                        types |= SearchContentType.Embed;
                        break;
                    case "link":
                        types |= SearchContentType.Link;
                        break;
                    case "poll":
                        types |= SearchContentType.Poll;
                        break;
                    case "snapshot":
                        types |= SearchContentType.Snapshot;
                        break;
                }
            }
            return types;
        }
        throw new JsonSerializationException("Unknown search scope");
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        if (value is SearchContentType types)
        {
            writer.WriteStartArray();
            if (types.HasFlag(SearchContentType.Image))
                writer.WriteValue("image");

            if (types.HasFlag(SearchContentType.Sound))
                writer.WriteValue("sound");

            if (types.HasFlag(SearchContentType.Video))
                writer.WriteValue("video");

            if (types.HasFlag(SearchContentType.File))
                writer.WriteValue("file");

            if (types.HasFlag(SearchContentType.Sticker))
                writer.WriteValue("sticker");

            if (types.HasFlag(SearchContentType.Embed))
                writer.WriteValue("embed");

            if (types.HasFlag(SearchContentType.Link))
                writer.WriteValue("link");

            if (types.HasFlag(SearchContentType.Poll))
                writer.WriteValue("poll");

            if (types.HasFlag(SearchContentType.Snapshot))
                writer.WriteValue("snapshot");

            writer.WriteEndArray();
        }
    }
}