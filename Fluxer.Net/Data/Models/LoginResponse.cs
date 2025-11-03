using Newtonsoft.Json;

namespace Fluxer.Net.Data.Models;

public class LoginResponse
{
	[JsonProperty("token")]
	public string Token { get; set; }

	[JsonProperty("user_id")]
	public ulong UserId { get; set; }
}
