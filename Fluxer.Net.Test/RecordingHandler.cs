using System.Net;
using System.Text;

namespace Fluxer.Net.Test;

internal sealed class RecordingHandler : HttpMessageHandler
{
    private readonly string _response;
    private readonly HttpStatusCode _status;

    public RecordingHandler(string response = "{}", HttpStatusCode status = HttpStatusCode.OK)
    {
        _response = response;
        _status = status;
    }

    public List<(HttpMethod Method, string Url, string Body)> Requests { get; } = new();

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        string body = request.Content == null ? "" : await request.Content.ReadAsStringAsync();
        Requests.Add((request.Method, request.RequestUri!.ToString(), body));
        return new HttpResponseMessage(_status)
        {
            Content = new StringContent(_response, Encoding.UTF8, "application/json")
        };
    }

    public static FluxerConfig Config(HttpClient http) => new()
    {
        HttpClient = http,
        RestSerilog = Serilog.Core.Logger.None,
        GatewaySerilog = Serilog.Core.Logger.None
    };
}
