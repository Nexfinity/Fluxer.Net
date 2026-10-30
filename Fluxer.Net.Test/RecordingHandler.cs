using System.Net;
using System.Text;

namespace Fluxer.Net.Test;

internal sealed class RecordingHandler : HttpMessageHandler
{
    private readonly string[] _responses;
    private readonly HttpStatusCode _status;

    public RecordingHandler(string response = "{}", HttpStatusCode status = HttpStatusCode.OK)
    {
        _responses = new[] { response };
        _status = status;
    }

    public RecordingHandler(params string[] responses)
    {
        _responses = responses;
        _status = HttpStatusCode.OK;
    }

    public List<(HttpMethod Method, string Url, string Body)> Requests { get; } = new();

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        string body = request.Content == null ? "" : await request.Content.ReadAsStringAsync();
        string response = _responses[Math.Min(Requests.Count, _responses.Length - 1)];
        Requests.Add((request.Method, request.RequestUri!.ToString(), body));
        return new HttpResponseMessage(_status)
        {
            Content = new StringContent(response, Encoding.UTF8, "application/json")
        };
    }

    public static FluxerConfig Config(HttpClient http) => new()
    {
        HttpClient = http,
        RestSerilog = Serilog.Core.Logger.None,
        GatewaySerilog = Serilog.Core.Logger.None
    };
}
