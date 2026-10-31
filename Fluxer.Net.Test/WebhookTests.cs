using System.Net;
using Fluxer.Net.Rest;

namespace Fluxer.Net.Test;

public class WebhookTests
{
    private const string MessageJson = "{\"id\":\"5\",\"channel_id\":\"2\",\"content\":\"hi\",\"timestamp\":\"2025-01-01T00:00:00Z\",\"author\":{\"id\":\"9\",\"username\":\"hook\",\"discriminator\":\"0\"},\"type\":0,\"flags\":0,\"pinned\":false,\"mention_everyone\":false,\"tts\":false,\"mentions\":[],\"mention_roles\":[],\"attachments\":[],\"embeds\":[]}";

    private static (FluxerWebhookClient Client, RecordingHandler Handler) Create(string response = "{}", HttpStatusCode status = HttpStatusCode.OK)
    {
        RecordingHandler handler = new(response, status);
        HttpClient http = new(handler);
        return (new FluxerWebhookClient("https://api.fluxer.app/v1/webhooks/12/tok", RecordingHandler.Config(http)), handler);
    }

    [Test]
    public async Task ExecuteTargetsAThreadAndCreatesForumPosts()
    {
        (FluxerWebhookClient client, RecordingHandler handler) = Create();

        await client.SendMessageAsync("hello", threadId: 77, threadName: "Bug", appliedTags: new List<ulong> { 3, 4 });

        Assert.Multiple(() =>
        {
            Assert.That(handler.Requests[0].Method, Is.EqualTo(HttpMethod.Post));
            Assert.That(handler.Requests[0].Url, Does.EndWith("/webhooks/12/tok?thread_id=77"));
            Assert.That(handler.Requests[0].Body, Does.Contain("\"thread_name\":\"Bug\"").And.Contain("\"applied_tags\":[\"3\",\"4\"]").Or.Contain("\"applied_tags\":[3,4]"));
        });
    }

    [Test]
    public async Task ExecuteWaitKeepsWaitAndAddsThreadId()
    {
        (FluxerWebhookClient client, RecordingHandler handler) = Create(MessageJson);

        await client.SendMessageWaitAsync("hello", threadId: 77);
        await client.SendMessageWaitAsync("hello");

        Assert.Multiple(() =>
        {
            Assert.That(handler.Requests[0].Url, Does.EndWith("/webhooks/12/tok?wait=true&thread_id=77"));
            Assert.That(handler.Requests[1].Url, Does.EndWith("/webhooks/12/tok?wait=true"));
            Assert.That(handler.Requests[1].Body, Does.Not.Contain("thread_name"));
        });
    }

    [Test]
    public async Task MessageEndpointsCarryTheThreadId()
    {
        (FluxerWebhookClient client, RecordingHandler handler) = Create(MessageJson);

        await client.GetMessageAsync(5, 77);
        await client.EditMessageAsync(5, "edited", threadId: 77);
        await client.DeleteMessageAsync(5, threadId: 77);
        await client.GetMessageAsync(5);

        Assert.Multiple(() =>
        {
            Assert.That(handler.Requests[0].Method, Is.EqualTo(HttpMethod.Get));
            Assert.That(handler.Requests[0].Url, Does.EndWith("/webhooks/12/tok/messages/5?thread_id=77"));
            Assert.That(handler.Requests[1].Method, Is.EqualTo(HttpMethod.Patch));
            Assert.That(handler.Requests[1].Url, Does.EndWith("/webhooks/12/tok/messages/5?thread_id=77"));
            Assert.That(handler.Requests[2].Method, Is.EqualTo(HttpMethod.Delete));
            Assert.That(handler.Requests[2].Url, Does.EndWith("/webhooks/12/tok/messages/5?thread_id=77"));
            Assert.That(handler.Requests[3].Url, Does.EndWith("/webhooks/12/tok/messages/5"));
        });
    }

    [Test]
    public async Task FilesAreSentAsMultipart()
    {
        (FluxerWebhookClient client, RecordingHandler handler) = Create(MessageJson);

        await client.Rest.ExecuteWebhookWaitAsync(12, "tok", "hi",
            attachments: new List<AttachmentRequest> { new(new MemoryStream(System.Text.Encoding.ASCII.GetBytes("data")), "a.txt") });

        string body = handler.Requests.Single().Body;
        Assert.Multiple(() =>
        {
            Assert.That(body, Does.Contain("name=\"files[0]\"").Or.Contain("name=files[0]"));
            Assert.That(body, Does.Contain("payload_json"));
            Assert.That(body, Does.Contain("data"));
        });
    }

    [Test]
    public async Task EditingAMessageSendsItsFiles()
    {
        RecordingHandler handler = new(MessageJson);
        FluxerClient client = new("Bot test", RecordingHandler.Config(new HttpClient(handler)));

        await client.Rest.EditMessageAsync(2, 5, "edited",
            attachments: new List<AttachmentRequest> { new(new MemoryStream(System.Text.Encoding.ASCII.GetBytes("data")), "a.txt") });

        string body = handler.Requests.Single().Body;
        Assert.Multiple(() =>
        {
            Assert.That(body, Does.Contain("name=\"files[0]\"").Or.Contain("name=files[0]"));
            Assert.That(body, Does.Contain("payload_json"));
            Assert.That(body, Does.Contain("data"));
        });
    }

    [Test]
    public async Task WebhookEntityDeleteMessageDeletes()
    {
        RecordingHandler handler = new();
        FluxerClient client = new("Bot test", RecordingHandler.Config(new HttpClient(handler)));
        Webhook webhook = Webhook.Create(client, Newtonsoft.Json.JsonConvert.DeserializeObject<WebhookJson>("{\"id\":\"1\",\"token\":\"tok\"}", FluxerClient._restSerializer)!);

        await webhook.DeleteMessageAsync(9);

        Assert.That(handler.Requests.Single().Method, Is.EqualTo(HttpMethod.Delete));
        Assert.That(handler.Requests.Single().Url, Does.EndWith("/webhooks/1/tok/messages/9"));
    }
}
