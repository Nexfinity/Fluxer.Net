using System.Text;
using Fluxer.Net.Rest;
using Newtonsoft.Json;

namespace Fluxer.Net.Test;

public class UploadTests
{
    private static (FluxerClient Client, RecordingHandler Handler) Create(params string[] responses)
    {
        RecordingHandler handler = new(responses);
        return (new FluxerClient("Bot test", RecordingHandler.Config(new HttpClient(handler))), handler);
    }

    [Test]
    public async Task LargeFileIsUploadedInPartsThenFinished()
    {
        (FluxerClient client, RecordingHandler handler) = Create(
            "{\"attachments\":[{\"id\":0,\"filename\":\"big.bin\",\"file_size\":10,\"content_type\":\"application/octet-stream\",\"upload_filename\":\"up/1\"," +
            "\"upload_mode\":\"multipart\",\"upload_id\":\"u1\",\"part_size\":6,\"parts\":[{\"part_number\":2,\"upload_url\":\"https://store.test/p2\"},{\"part_number\":1,\"upload_url\":\"https://store.test/p1\"}]}]}",
            "{}",
            "{}",
            "{\"uploads\":[{\"upload_filename\":\"up/1\"}]}");

        UploadedAttachment upload = await client.Rest.UploadAttachmentAsync(3, new MemoryStream(Encoding.ASCII.GetBytes("0123456789")), "big.bin");

        Assert.Multiple(() =>
        {
            Assert.That(handler.Requests[0].Url, Does.EndWith("/channels/3/attachments"));
            Assert.That(handler.Requests[0].Body, Does.Contain("\"file_size\":10"));
            Assert.That(handler.Requests[1], Is.EqualTo((HttpMethod.Put, "https://store.test/p1", "012345")));
            Assert.That(handler.Requests[2], Is.EqualTo((HttpMethod.Put, "https://store.test/p2", "6789")));
            Assert.That(handler.Requests[3].Url, Does.EndWith("/channels/3/attachments/complete"));
            Assert.That(handler.Requests[3].Body, Is.EqualTo("{\"uploads\":[{\"upload_filename\":\"up/1\",\"upload_id\":\"u1\"}]}"));
            Assert.That(upload.UploadFilename, Is.EqualTo("up/1"));
            Assert.That(upload.FileSize, Is.EqualTo(10));
        });
    }

    [Test]
    public async Task SmallFileIsUploadedWithOnePut()
    {
        (FluxerClient client, RecordingHandler handler) = Create(
            "{\"attachments\":[{\"id\":0,\"filename\":\"a.txt\",\"file_size\":2,\"content_type\":\"text/plain\",\"upload_filename\":\"up/2\",\"upload_mode\":\"singlepart\",\"upload_url\":\"https://store.test/one\"}]}",
            "{}");

        await client.Rest.UploadAttachmentAsync(3, new MemoryStream(Encoding.ASCII.GetBytes("hi")), "a.txt", "text/plain");

        Assert.That(handler.Requests, Has.Count.EqualTo(2));
        Assert.That(handler.Requests[1], Is.EqualTo((HttpMethod.Put, "https://store.test/one", "hi")));
    }

    [Test]
    public async Task UploadedFilesAreSentByTheirUploadKey()
    {
        (FluxerClient client, RecordingHandler handler) = Create(
            "{\"id\":\"9\",\"channel_id\":\"3\",\"author\":{\"id\":\"1\",\"username\":\"a\"},\"content\":\"\",\"timestamp\":\"2025-01-01T00:00:00Z\"}");

        await client.Rest.SendMessageAsync(3, new[] { new UploadedAttachment { Filename = "big.bin", UploadFilename = "up/1", FileSize = 10 } }, "here");

        string body = handler.Requests.Single().Body;
        Assert.Multiple(() =>
        {
            Assert.That(body, Does.Contain("\"upload_filename\":\"up/1\""));
            Assert.That(body, Does.Contain("\"file_size\":10"));
            Assert.That(body, Does.Contain("\"content\":\"here\""));
        });
    }

    [Test]
    public async Task BadgesCanBeLookedUpById()
    {
        (FluxerClient client, RecordingHandler handler) = Create(
            "{\"version\":\"v1\",\"badges\":[{\"id\":\"5\",\"name\":\"Partner\",\"asset_url\":\"https://x.test/b.webp\",\"position\":0}],\"next\":null}");

        InstanceBadges? badges = await client.Rest.GetInstanceBadgesAsync(new ulong[] { 5, 6 });

        Assert.Multiple(() =>
        {
            Assert.That(handler.Requests.Single().Url, Does.EndWith("/instance/badges?ids=5%2C6"));
            Assert.That(badges!.Badges.Single().Name, Is.EqualTo("Partner"));
        });
    }

    [Test]
    public async Task ProfileQueryStartsWithAQuestionMarkAndUsesLowercaseBooleans()
    {
        (FluxerClient client, RecordingHandler handler) = Create("{}");

        try { await client.Rest.GetUserProfileAsync(5, 10, true, true); } catch { }

        Assert.That(handler.Requests.Single().Url, Does.EndWith("/users/5/profile?guild_id=10&with_mutual_friends=true&with_mutual_guilds=true"));
    }

    [Test]
    public void ApplicationReadsRedirectUrisCodeGrantAndVerifyKey()
    {
        CurrentApplicationJson json = JsonConvert.DeserializeObject<CurrentApplicationJson>(
            "{\"id\":\"1\",\"name\":\"bot\",\"redirect_uris\":[\"https://x.test/cb\"],\"bot_require_code_grant\":true,\"verify_key\":\"abc\",\"owner\":{\"id\":\"2\",\"username\":\"o\"}}",
            FluxerClient._restSerializer)!;

        Assert.Multiple(() =>
        {
            Assert.That(json.RedirectUrls, Is.EqualTo(new[] { "https://x.test/cb" }));
            Assert.That(json.RequiresCodeGrant, Is.True);
            Assert.That(json.VerifyKey, Is.EqualTo("abc"));
        });
    }
}
