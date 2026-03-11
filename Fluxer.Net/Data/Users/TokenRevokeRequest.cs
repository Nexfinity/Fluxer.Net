using Newtonsoft.Json;

namespace Fluxer.Net;

public class TokenRevokeRequest
{
	[JsonProperty("token")]
	public string Token { get; set; }
}
