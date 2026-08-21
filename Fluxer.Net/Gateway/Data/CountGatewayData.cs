using Newtonsoft.Json;

namespace Fluxer.Net.Gateway;

public class CountGatewayData<T> where T : class
{
    [JsonProperty("counts")]
    public T[] Counts { get; set; }
}
