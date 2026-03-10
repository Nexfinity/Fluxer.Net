using Newtonsoft.Json;

namespace Fluxer.Net.Data.Channels;

/// <remarks>
/// <see href="https://github.com/fluxerapp/fluxer/blob/4f5704fa1f6426d65a12ee5fef13c0104669d08e/packages/schema/src/domains/channel/ChannelSchemas.tsx#L27"/>
/// </remarks>
public class ChannelPermissionOverwrite
{
	[JsonProperty("id")]
	public ulong Id { get; set; }

	[JsonProperty("type")]
	public int Type { get; set; }

	[JsonProperty("allow")]
	public ulong Allow { get; set; }

	[JsonProperty("deny")]
	public ulong Deny { get; set; }
}
