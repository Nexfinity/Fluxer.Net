using System.Net;
using System.Text;

namespace Fluxer.Net.Test;

public class RestErrorTests
{
    [Test]
    public async Task FailedRequestReturnsNullInsteadOfParsingTheErrorBody()
    {
        using HttpClient http = new(new ErrorHandler());
        FluxerClient client = new("Bot test", new FluxerConfig
        {
            HttpClient = http,
            RestSerilog = Serilog.Core.Logger.None,
            GatewaySerilog = Serilog.Core.Logger.None
        });

        GuildMember? member = await client.Rest.GetCurrentMemberAsync(1);

        Assert.That(member, Is.Null);
    }

    private sealed class ErrorHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound)
            {
                Content = new StringContent("{\"code\":\"UNKNOWN_GUILD\",\"message\":\"Unknown Guild\"}", Encoding.UTF8, "application/json")
            });
        }
    }
}
