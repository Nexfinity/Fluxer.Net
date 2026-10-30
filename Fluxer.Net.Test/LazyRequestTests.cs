using Fluxer.Net.Gateway;
using Newtonsoft.Json.Linq;

namespace Fluxer.Net.Test;

public class LazyRequestTests
{
    private static string Dispatch(string name, string data) => $"{{\"op\":0,\"t\":\"{name}\",\"s\":1,\"d\":{data}}}";

    [Test]
    public void LazyRequestSendsIdsAsStringsAndLeavesOutUnsetOptions()
    {
        JToken json = JToken.FromObject(new LazyRequestPacket
        {
            Subscriptions = new Dictionary<ulong, GuildSubscription>
            {
                [1189375284394692608] = new GuildSubscription
                {
                    Typing = true,
                    Members = new ulong[] { 1189375284394692700 },
                    MemberListChannels = new Dictionary<ulong, IEnumerable<int[]>> { [1189375284394692610] = new[] { new[] { 0, 99 } } }
                }
            }
        }, FluxerClient._gatewaySerializer);

        JToken guild = json["subscriptions"]!["1189375284394692608"]!;
        Assert.Multiple(() =>
        {
            Assert.That(guild["typing"]!.Value<bool>(), Is.True);
            Assert.That(guild["members"]![0]!.Type, Is.EqualTo(JTokenType.String));
            Assert.That(guild["members"]![0]!.Value<string>(), Is.EqualTo("1189375284394692700"));
            Assert.That(guild["member_list_channels"]!["1189375284394692610"]![0]!.ToObject<int[]>(), Is.EqualTo(new[] { 0, 99 }));
            Assert.That(guild["active"], Is.Null);
            Assert.That(guild["threads"], Is.Null);
        });
    }

    [Test]
    public void ForumUnreadsRequestSendsThePosts()
    {
        JToken json = JToken.FromObject(new ForumUnreadsRequestPacket
        {
            GuildId = 1,
            ChannelId = 2,
            Threads = new[] { new ForumUnreadRequest { ThreadId = 3, AckMessageId = 4 }, new ForumUnreadRequest { ThreadId = 5 } }
        }, FluxerClient._gatewaySerializer);

        Assert.Multiple(() =>
        {
            Assert.That(json["channel_id"]!.Value<string>(), Is.EqualTo("2"));
            Assert.That(json["threads"]![0]!["ack_message_id"]!.Value<string>(), Is.EqualTo("4"));
            Assert.That(json["threads"]![1]!["ack_message_id"], Is.Null);
        });
    }

    [Test]
    public void OldOpcodeNamesStillMeanTheSameOpcodes()
    {
#pragma warning disable CS0618
        Assert.That(FluxerOpCode.CallConnect, Is.EqualTo(FluxerOpCode.VoiceSignal));
        Assert.That(FluxerOpCode.GuildSubscriptions, Is.EqualTo(FluxerOpCode.LazyRequest));
#pragma warning restore CS0618
        Assert.That((int)FluxerOpCode.RequestForumUnreads, Is.EqualTo(28));
    }

    [Test]
    public void MemberListThreadMemberListAndForumUnreadsAreRaised()
    {
        FluxerClient client = new("Bot test");
        GuildMemberListUpdateGatewayData? list = null;
        ThreadMemberListUpdateGatewayData? threadList = null;
        ForumUnreadsGatewayData? unreads = null;
        client.Gateway.GuildMemberListUpdated += d => list = d;
        client.Gateway.ThreadMemberListUpdated += d => threadList = d;
        client.Gateway.ForumUnreadsReceived += d => unreads = d;
        const string member = "{\"user\":{\"id\":\"5\",\"username\":\"u\"},\"roles\":[],\"joined_at\":\"2025-01-01T00:00:00Z\",\"presence\":{\"status\":\"online\",\"mobile\":false,\"afk\":false}}";

        client.Gateway.GatewayMessageHandler(Dispatch("GUILD_MEMBER_LIST_UPDATE",
            "{\"guild_id\":\"10\",\"id\":\"3\",\"member_count\":2,\"online_count\":1,\"groups\":[{\"id\":\"online\",\"count\":1}]," +
            $"\"ops\":[{{\"op\":\"SYNC\",\"range\":[0,1],\"items\":[{{\"group\":{{\"id\":\"online\",\"count\":1}}}},{{\"member\":{member}}}]}}]}}"));
        client.Gateway.GatewayMessageHandler(Dispatch("THREAD_MEMBER_LIST_UPDATE",
            $"{{\"guild_id\":\"10\",\"thread_id\":\"30\",\"members\":[{{\"user_id\":\"5\",\"join_timestamp\":\"2025-01-01T00:00:00Z\",\"flags\":0,\"presence\":{{\"status\":\"idle\"}}}}]}}"));
        client.Gateway.GatewayMessageHandler(Dispatch("FORUM_UNREADS",
            "{\"guild_id\":\"10\",\"channel_id\":\"20\",\"threads\":[{\"thread_id\":\"30\",\"count\":4},{\"thread_id\":\"31\",\"missing\":true}]}"));

        Assert.Multiple(() =>
        {
            Assert.That(list?.Operations.Single().Items[1].Member?.Presence?.Status, Is.EqualTo("online"));
            Assert.That(list?.Groups.Single().Id, Is.EqualTo("online"));
            Assert.That(threadList?.Members.Single().Presence?.Status, Is.EqualTo("idle"));
            Assert.That(unreads?.Threads[0].Count, Is.EqualTo(4));
            Assert.That(unreads?.Threads[1].Missing, Is.True);
        });
    }
}
