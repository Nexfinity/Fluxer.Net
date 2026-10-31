namespace Fluxer.Net.Test;

public class InviteTests
{
    private static string Dispatch(string name, string data) => $"{{\"op\":0,\"t\":\"{name}\",\"s\":1,\"d\":{data}}}";

    [Test]
    public void InviteWithoutAnInviterIsRead()
    {
        FluxerClient client = new("Bot test");

        Invite invite = Invite.Create(client, new InviteJson { Code = "vanity" });

        Assert.That(invite.Inviter, Is.Null);
        Assert.That(invite.Channel, Is.Null);
    }

    [Test]
    public void DeletedInviteComesWithTheCachedInvite()
    {
        FluxerClient client = new("Bot test");
        client.Gateway.Channels[3] = Channel.Create(client, new ChannelJson { Id = 3, GuildId = 10, Type = ChannelType.GuildText });
        TestCache.Use(client, invites: 10);
        Invite? removed = null;
        client.Gateway.InviteRemoved += (_, _, _, invite) => removed = invite;

        client.Gateway.GatewayMessageHandler(Dispatch("INVITE_CREATE", "{\"code\":\"abc\",\"guild\":{\"id\":\"10\",\"name\":\"Test\"},\"channel\":{\"id\":\"3\",\"name\":\"general\",\"type\":0},\"uses\":4}"));
        client.Gateway.GatewayMessageHandler(Dispatch("INVITE_DELETE", "{\"code\":\"abc\",\"channel_id\":\"3\",\"guild_id\":\"10\"}"));

        Assert.That(removed?.Code, Is.EqualTo("abc"));
    }

    [Test]
    public async Task InvitesCreatedWithIdsAreDroppedWithTheirChannel()
    {
        FluxerClient client = new("Bot test");
        SocketGuild guild = new(client);
        guild.Update(new GuildJson { Id = 10, Name = "Test" });
        client.Gateway.Guilds[10] = guild;
        client.Gateway.Channels[3] = Channel.Create(client, new ChannelJson { Id = 3, GuildId = 10, Type = ChannelType.GuildText });
        TestCache.Use(client, invites: 10);

        client.Gateway.GatewayMessageHandler(Dispatch("INVITE_CREATE", "{\"code\":\"abc\",\"guild_id\":\"10\",\"channel_id\":\"3\",\"uses\":0}"));
        Assert.That(await client.Gateway.Cache.Invites.GetAsync("abc"), Is.Not.Null);

        client.Gateway.GatewayMessageHandler(Dispatch("CHANNEL_DELETE", "{\"id\":\"3\",\"guild_id\":\"10\",\"type\":0}"));
        Assert.That(await client.Gateway.Cache.Invites.GetAsync("abc"), Is.Null);
    }
}
