using Newtonsoft.Json;

namespace Fluxer.Net;

public class InviteUserJson
{
    [JsonProperty("id")]
    public ulong Id { get; set; }

    [JsonProperty("username")]
    public string Username { get; set; }

    [JsonProperty("discriminator")]
    public string Discriminator { get; set; }

    [JsonProperty("global_name")]
    public string DisplayName { get; set; }

    [JsonProperty("avatar")]
    public string AvatarId { get; set; }

    [JsonProperty("avatar_color")]
    public int AvatarColor { get; set; }

    [JsonProperty("flags")]
    public ulong Flags { get; set; }
}
