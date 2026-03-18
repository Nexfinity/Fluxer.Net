using Newtonsoft.Json;

namespace Fluxer.Net.Data.Apps;

public class Application : Entity
{
    [JsonProperty("id")]
    public ulong Id { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("icon")]
    public string Icon { get; set; }

    [JsonProperty("description")]
    public string Description { get; set; }

    [JsonProperty("bot_public")]
    public bool IsPublic { get; set; }

    [JsonProperty("bot_requires_code_grant")]
    public bool RequiresCodeGrant { get; set; }

    [JsonProperty("flags")]
    public ulong Flags { get; set; }
}
