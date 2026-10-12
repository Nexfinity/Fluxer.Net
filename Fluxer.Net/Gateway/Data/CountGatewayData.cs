using Newtonsoft.Json;

namespace Fluxer.Net.Gateway;

internal class CountGatewayData<T> where T : class
{
    [JsonProperty("counts")]
    public T[] Counts { get; set; }
}