using Newtonsoft.Json;

namespace Fluxer.Net.Data.Users;

public class TokenRevokeRequest
{
	[JsonProperty("token")]
	public string Token { get; set; }
}
