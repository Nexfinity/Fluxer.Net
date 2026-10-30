using Fluxer.Net.Rest;
using Newtonsoft.Json;

namespace Fluxer.Net.Test;

public class MessageFieldTests
{
    private static FluxerClient Client() => new("Bot test", RecordingHandler.Config(new HttpClient(new RecordingHandler())));

    private static T Read<T>(string json) => JsonConvert.DeserializeObject<T>(json, FluxerClient._restSerializer)!;

    private static string Serialize(object value) => JsonConvert.SerializeObject(value, FluxerClient._restSerializer);

    private const string Author = "{\"id\":\"1\",\"username\":\"a\"}";

    [Test]
    public void ReplyCarriesTheRepliedToMessageAndChannelMentions()
    {
        MessageJson json = Read<MessageJson>(
            $"{{\"id\":\"20\",\"channel_id\":\"3\",\"author\":{Author},\"content\":\"reply\",\"timestamp\":\"2025-01-01T00:00:00Z\"," +
            "\"message_reference\":{\"channel_id\":\"3\",\"message_id\":\"10\",\"type\":0}," +
            $"\"referenced_message\":{{\"id\":\"10\",\"channel_id\":\"3\",\"author\":{Author},\"content\":\"original\",\"timestamp\":\"2025-01-01T00:00:00Z\"}}," +
            "\"mention_channels\":[{\"id\":\"7\",\"name\":\"general\",\"type\":0}]}");

        Message message = Message.Create(Client(), json);

        Assert.Multiple(() =>
        {
            Assert.That(message.ReferencedMessage?.Id, Is.EqualTo(10UL));
            Assert.That(message.ReferencedMessage?.Content, Is.EqualTo("original"));
            Assert.That(message.MentionChannels!.Single().Name, Is.EqualTo("general"));
        });
    }

    [Test]
    public void ReplyToADeletedMessageHasAReferenceButNoMessage()
    {
        MessageJson json = Read<MessageJson>(
            $"{{\"id\":\"20\",\"channel_id\":\"3\",\"author\":{Author},\"content\":\"reply\",\"timestamp\":\"2025-01-01T00:00:00Z\"," +
            "\"message_reference\":{\"channel_id\":\"3\",\"message_id\":\"10\",\"type\":0},\"referenced_message\":null}");

        Message message = Message.Create(Client(), json);

        Assert.Multiple(() =>
        {
            Assert.That(message.ReferencedMessage, Is.Null);
            Assert.That(message.MessageReference?.MessageId, Is.EqualTo(10UL));
        });
    }

    [Test]
    public void ForwardingSelectedMediaSendsTheChosenItems()
    {
        string json = Serialize(new MessageReferenceRequest
        {
            MessageId = 10,
            ChannelId = 3,
            Type = MessageReferenceType.Forward,
            AttachmentIds = new ulong[] { 5 },
            EmbedIndices = new[] { 0 }
        });

        Assert.That(json, Does.Contain("\"attachment_ids\":[\"5\"]").Or.Contain("\"attachment_ids\":[5]"));
        Assert.That(json, Does.Contain("\"embed_indices\":[0]"));
    }

    [Test]
    public void ChannelUpdateSendsForumAndVoiceSettings()
    {
        string json = Serialize(new UpdateChannelRequest
        {
            RtcP2P = true,
            DefaultTagSetting = ForumTagMatchSetting.MatchAll,
            DefaultLayout = ForumLayout.List,
            ForumTags = new[] { new ForumTagUpdateRequest { Id = 4UL, Name = "bug" }, new ForumTagUpdateRequest { Name = "new" } }
        });

        Assert.Multiple(() =>
        {
            Assert.That(json, Does.Contain("\"rtc_p2p\":true"));
            Assert.That(json, Does.Contain("\"default_tag_setting\":\"match_all\""));
            Assert.That(json, Does.Contain("\"default_forum_layout\":1"));
            Assert.That(json, Does.Contain("{\"id\":\"4\",\"name\":\"bug\"}").Or.Contain("{\"id\":4,\"name\":\"bug\"}"));
            Assert.That(json, Does.Contain("{\"name\":\"new\"}"));
        });
    }

    [Test]
    public void ChannelReadsTagSettingAndPeerToPeerVoice()
    {
        Channel channel = Channel.Create(Client(), Read<ChannelJson>("{\"id\":\"9\",\"type\":15,\"default_tag_setting\":\"match_some\",\"rtc_p2p\":true}"));

        Assert.Multiple(() =>
        {
            Assert.That(channel.DefaultTagSetting, Is.EqualTo(ForumTagMatchSetting.MatchSome));
            Assert.That(channel.RtcP2P, Is.True);
        });
    }

    [Test]
    public void SearchResultsKeepTheNextPageCursor()
    {
        GlobalSearch search = GlobalSearch.Create(Client(), Read<GlobalSearchJson>("{\"messages\":[],\"channels\":[],\"total\":30,\"hits_per_page\":25,\"page\":1,\"cursor\":[\"a\",\"b\"]}"));

        Assert.That(search.Cursor, Is.EqualTo(new[] { "a", "b" }));
        Assert.That(Serialize(new MessageSearchRequest { Cursor = search.Cursor }), Does.Contain("\"cursor\":[\"a\",\"b\"]"));
    }
}
