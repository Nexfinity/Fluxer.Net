using Newtonsoft.Json;

namespace Fluxer.Net.Test;

public class AuditLogTests
{
    private static SocketAuditLog Parse(string json) =>
        SocketAuditLog.Create(new FluxerClient("Bot test"), JsonConvert.DeserializeObject<GuildAuditLogJson>(json)!);

    [Test]
    public void MemberRoleUpdatesCarryTypedRoleIds()
    {
        SocketAuditLog entry = Parse("""
            {"id":"10","guild_id":"1","action_type":25,"user_id":"2","target_id":"3",
             "changes":[{"key":"roles","old_value":["100"],"new_value":["100","200"]}]}
            """);

        Assert.Multiple(() =>
        {
            Assert.That(entry.OldData, Is.InstanceOf<MemberChangeDataJson>());
            Assert.That(((MemberChangeDataJson)entry.OldData!).RoleIds, Is.EquivalentTo(new ulong[] { 100 }));
            Assert.That(((MemberChangeDataJson)entry.NewData).RoleIds, Is.EquivalentTo(new ulong[] { 100, 200 }));
        });
    }

    [Test]
    public void RoleCreationsCarryTypedRoleData()
    {
        SocketAuditLog entry = Parse("""
            {"id":"11","guild_id":"1","action_type":30,"user_id":"2","target_id":"300",
             "changes":[{"key":"role_id","old_value":null,"new_value":"300"},
                        {"key":"name","old_value":null,"new_value":"Helpers"},
                        {"key":"permissions","old_value":null,"new_value":"8"},
                        {"key":"color","old_value":null,"new_value":16711680},
                        {"key":"hoist","old_value":null,"new_value":true}]}
            """);

        RoleChangeDataJson role = (RoleChangeDataJson)entry.NewData;
        Assert.Multiple(() =>
        {
            Assert.That(role.RoleId, Is.EqualTo(300UL));
            Assert.That(role.Name, Is.EqualTo("Helpers"));
            Assert.That(role.Color, Is.EqualTo(16711680));
            Assert.That(role.IsHoisted, Is.True);
            Assert.That(role.Permissions, Is.Not.Null);
            Assert.That(((RoleChangeDataJson)entry.OldData!).Name, Is.Null);
        });
    }

    [Test]
    public void EntriesWithoutChangesDoNotThrow()
    {
        SocketAuditLog entry = Parse("""{"id":"12","guild_id":"1","action_type":32,"user_id":"2","target_id":"300"}""");

        Assert.That(entry.NewData, Is.InstanceOf<RoleChangeDataJson>());
    }
}
