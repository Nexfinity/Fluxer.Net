using Newtonsoft.Json;

namespace Fluxer.Net.Data.Models;

public class TokenRevokeRequest
{
	[JsonProperty("token")]
	public string Token { get; set; }
}
