using Newtonsoft.Json;

namespace Fluxer.Net.Extensions;

internal class SearchScopeConverter : JsonConverter
{
    public static readonly SearchScopeConverter Instance = new SearchScopeConverter();

    public override bool CanConvert(Type objectType) => true;
    public override bool CanRead => true;
    public override bool CanWrite => true;

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        return (string)reader.Value switch
        {
            "current" => SearchScope.Current,
            "open_dms" => SearchScope.OpenDMs,
            "all_dms" => SearchScope.AllDMs,
            "all_guilds" => SearchScope.AllGuilds,
            "all" => SearchScope.All,
            "open_dms_and_all_guilds" => SearchScope.OpenDMsAndAllGuilds,
            _ => throw new JsonSerializationException("Unknown search scope"),
        };
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        switch ((SearchScope)value)
        {
            case SearchScope.Current:
                writer.WriteValue("current");
                break;
            case SearchScope.OpenDMs:
                writer.WriteValue("open_dms");
                break;
            case SearchScope.AllDMs:
                writer.WriteValue("all_dms");
                break;
            case SearchScope.AllGuilds:
                writer.WriteValue("all_guilds");
                break;
            case SearchScope.All:
                writer.WriteValue("all");
                break;
            case SearchScope.OpenDMsAndAllGuilds:
                writer.WriteValue("open_dms_and_all_guilds");
                break;
            default:
                throw new JsonSerializationException("Invalid search scope");
        }
    }
}