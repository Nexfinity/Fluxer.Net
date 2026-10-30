namespace Fluxer.Net.Test;

public class GatewayEventTests
{
    private static (FluxerClient Client, SocketGuild Guild) Create()
    {
        FluxerClient client = new("Bot test");
        SocketGuild guild = new(client);
        guild.Update(new GuildJson { Id = 10, Name = "Test" });
        client.Gateway.Guilds[10] = guild;
        return (client, guild);
    }

    private static string Dispatch(string name, string data) => $"{{\"op\":0,\"t\":\"{name}\",\"s\":1,\"d\":{data}}}";

    [Test]
    public void ThreadListSyncCachesThreadsAndRaisesTheEvent()
    {
        (FluxerClient client, SocketGuild guild) = Create();
        ThreadChannel[]? threads = null;
        ThreadMember[]? members = null;
        ulong[]? parents = null;
        client.Gateway.ThreadListSynced += (_, t, m, p) =>
        {
            threads = t;
            members = m;
            parents = p;
        };

        client.Gateway.GatewayMessageHandler(Dispatch("THREAD_LIST_SYNC",
            "{\"guild_id\":\"10\",\"channel_ids\":[\"4\"],\"threads\":[{\"id\":\"30\",\"type\":11,\"guild_id\":\"10\",\"parent_id\":\"4\",\"name\":\"post\",\"message_count\":3,\"member_count\":2}],"
            + "\"members\":[{\"id\":\"30\",\"user_id\":\"1\",\"join_timestamp\":\"2025-01-01T00:00:00.000Z\",\"flags\":5}]}"));

        Assert.Multiple(() =>
        {
            Assert.That(threads, Has.Length.EqualTo(1));
            Assert.That(threads![0].Name, Is.EqualTo("post"));
            Assert.That(threads[0].MessageCount, Is.EqualTo(3));
            Assert.That(members![0].Flags, Is.EqualTo(5));
            Assert.That(parents, Is.EqualTo(new ulong[] { 4 }));
            Assert.That(client.Gateway.Channels[30], Is.InstanceOf<SocketThreadChannel>());
            Assert.That(guild.Channels.ContainsKey(30), Is.True);
            Assert.That(client.Gateway.Channels[30].ThreadMember!.Flags, Is.EqualTo(5));
        });
    }

    [Test]
    public void ThreadListSyncForUnknownGuildsIsIgnored()
    {
        FluxerClient client = new("Bot test");
        bool raised = false;
        client.Gateway.ThreadListSynced += (_, _, _, _) => raised = true;

        client.Gateway.GatewayMessageHandler(Dispatch("THREAD_LIST_SYNC", "{\"guild_id\":\"99\",\"threads\":[],\"members\":[]}"));

        Assert.That(raised, Is.False);
    }

    [Test]
    public void ThreadMemberUpdateRefreshesTheCachedMembership()
    {
        (FluxerClient client, _) = Create();
        client.Gateway.GatewayMessageHandler(Dispatch("THREAD_LIST_SYNC",
            "{\"guild_id\":\"10\",\"threads\":[{\"id\":\"30\",\"type\":11,\"guild_id\":\"10\",\"parent_id\":\"4\",\"name\":\"post\"}],\"members\":[]}"));
        ThreadMember? updated = null;
        client.Gateway.ThreadMemberUpdated += (_, m) => updated = m;

        client.Gateway.GatewayMessageHandler(Dispatch("THREAD_MEMBER_UPDATE",
            "{\"guild_id\":\"10\",\"id\":\"30\",\"user_id\":\"1\",\"join_timestamp\":\"2025-01-01T00:00:00.000Z\",\"flags\":2,\"muted\":true}"));

        Assert.Multiple(() =>
        {
            Assert.That(updated!.ThreadId, Is.EqualTo(30UL));
            Assert.That(updated.Muted, Is.True);
            Assert.That(client.Gateway.Channels[30].ThreadMember!.Flags, Is.EqualTo(2));
        });
    }

    [Test]
    public void ThreadMembersUpdateKeepsTheCachedMemberCountCurrent()
    {
        (FluxerClient client, _) = Create();
        client.Gateway.GatewayMessageHandler(Dispatch("THREAD_LIST_SYNC",
            "{\"guild_id\":\"10\",\"threads\":[{\"id\":\"30\",\"type\":11,\"guild_id\":\"10\",\"parent_id\":\"4\",\"name\":\"post\",\"member_count\":1}],\"members\":[]}"));

        client.Gateway.GatewayMessageHandler(Dispatch("THREAD_MEMBERS_UPDATE", "{\"id\":\"30\",\"guild_id\":\"10\",\"member_count\":7}"));

        Assert.That(client.Gateway.Channels[30].MemberCount, Is.EqualTo(7));
    }

    [Test]
    public void ReactionAddManyRaisesOneEventWithEveryReaction()
    {
        (FluxerClient client, _) = Create();
        client.Gateway.GatewayMessageHandler(Dispatch("THREAD_LIST_SYNC",
            "{\"guild_id\":\"10\",\"threads\":[{\"id\":\"30\",\"type\":11,\"guild_id\":\"10\",\"parent_id\":\"4\",\"name\":\"post\"}],\"members\":[]}"));
        Channel? channel = null;
        SocketReaction[]? reactions = null;
        ulong messageId = 0;
        client.Gateway.MessageReactionsAdded += (c, m, r) =>
        {
            channel = c;
            messageId = m.Id;
            reactions = r;
        };

        client.Gateway.GatewayMessageHandler(Dispatch("MESSAGE_REACTION_ADD_MANY",
            "{\"guild_id\":\"10\",\"channel_id\":\"30\",\"message_id\":\"77\",\"reactions\":["
            + "{\"user_id\":\"1\",\"emoji\":{\"name\":\"x\"}},{\"user_id\":\"2\",\"emoji\":{\"name\":\"y\"}}]}"));

        Assert.Multiple(() =>
        {
            Assert.That(channel!.Id, Is.EqualTo(30UL));
            Assert.That(messageId, Is.EqualTo(77UL));
            Assert.That(reactions!.Select(x => x.UserId), Is.EqualTo(new ulong[] { 1, 2 }));
            Assert.That(reactions.All(x => x.MessageId == 77 && x.ChannelId == 30), Is.True);
        });
    }

    [Test]
    public void PrivateReactionAddManyRaisesTheDirectMessageEvent()
    {
        FluxerClient client = new("Bot test");
        SocketReaction[]? reactions = null;
        client.Gateway.PrivateMessageReactionsAdded += (_, _, r) => reactions = r;

        client.Gateway.GatewayMessageHandler(Dispatch("MESSAGE_REACTION_ADD_MANY",
            "{\"channel_id\":\"30\",\"message_id\":\"77\",\"reactions\":[{\"user_id\":\"1\",\"emoji\":{\"name\":\"x\"}}]}"));

        Assert.That(reactions, Has.Length.EqualTo(1));
    }

    [Test]
    public void GuildHealthUpdateRaisesTheEvent()
    {
        FluxerClient client = new("Bot test");
        (ulong, bool)? received = null;
        client.Gateway.GuildHealthUpdated += (id, degraded) => received = (id, degraded);

        client.Gateway.GatewayMessageHandler(Dispatch("GUILD_HEALTH_UPDATE", "{\"guild_id\":\"10\",\"degraded\":true}"));

        Assert.That(received, Is.EqualTo((10UL, true)));
    }

    [Test]
    public void IdentifyAsksForReactionBatchesOnlyWhenEnabled()
    {
        FluxerClient client = new("Bot test");
        client.Gateway.Config = new FluxerGatewayConfig();
        Assert.That(client.Gateway.CreateIdentifyPacket(new Dictionary<string, string>()).Data!["flags"], Is.Null);

        client.Gateway.Config = new FluxerGatewayConfig { DebounceMessageReactions = true };
        Assert.That((int)client.Gateway.CreateIdentifyPacket(new Dictionary<string, string>()).Data!["flags"]!, Is.EqualTo(2));
    }

    [Test]
    public void GuildResponsesCarryBadgesThreadsAndVisits()
    {
        GuildJson json = Newtonsoft.Json.JsonConvert.DeserializeObject<GuildJson>(
            "{\"id\":\"10\",\"name\":\"g\",\"owner_id\":\"1\",\"badge_ids\":[\"5\",\"6\"],\"threads_active\":true,\"visiting\":true}")!;
        Guild guild = Guild.Create(new FluxerClient("Bot test"), json);

        Assert.Multiple(() =>
        {
            Assert.That(guild.BadgeIds, Is.EqualTo(new ulong[] { 5, 6 }));
            Assert.That(guild.ThreadsActive, Is.True);
            Assert.That(guild.Visiting, Is.True);
            Assert.That(Guild.Create(new FluxerClient("Bot test"), new GuildJson { Id = 11 }).ThreadsActive, Is.Null);
        });
    }
}
