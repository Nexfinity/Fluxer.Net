using Fluxer.Net.Rest;
using Newtonsoft.Json;

namespace Fluxer.Net.Test;

public class RoleIconTests
{
    private static string Serialize(object value) => JsonConvert.SerializeObject(value, FluxerClient._restSerializer);

    [Test]
    public void RoleRequestsSendIconFieldsOnlyWhenSet()
    {
        Assert.Multiple(() =>
        {
            Assert.That(Serialize(new UpdateGuildRoleRequest { Name = "x" }), Is.EqualTo("{\"name\":\"x\"}"));
            Assert.That(Serialize(new UpdateGuildRoleRequest { IconBase64 = null, UnicodeEmoji = "x" }),
                Is.EqualTo("{\"icon\":null,\"unicode_emoji\":\"x\"}"));
            Assert.That(Serialize(new CreateGuildRoleRequest { Name = "x", IconSourceRoleId = 5 }),
                Is.EqualTo("{\"name\":\"x\",\"icon_source_role_id\":5}"));
            Assert.That(Serialize(new CreateGuildRoleRequest { Name = "x", IconBase64 = "data:image/png;base64,AA" }),
                Does.Contain("\"icon\":\"data:image/png;base64,AA\""));
        });
    }

    [Test]
    public void RoleIconFlowsFromJsonToTheModelAndBuildsTheMediaUrl()
    {
        RoleJson json = JsonConvert.DeserializeObject<RoleJson>("{\"id\":\"42\",\"name\":\"Mod\",\"icon\":\"abc123\",\"unicode_emoji\":null}")!;
        FluxerClient client = new("Bot test", new FluxerConfig { MediaUrl = "https://media.example" });
        Role role = Role.Create(client, json, 1);

        Assert.Multiple(() =>
        {
            Assert.That(json.GetIconUrl(32), Is.EqualTo("https://fluxerusercontent.com/role-icons/42/abc123.webp?size=32"));
            Assert.That(role.IconHash, Is.EqualTo("abc123"));
            Assert.That(role.GetIconUrl(), Is.EqualTo("https://media.example/role-icons/42/abc123.webp?size=64"));
            Assert.That(Role.Create(client, new RoleJson { Id = 43 }, 1).GetIconUrl(), Is.Null);
        });
    }
}
