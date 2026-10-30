using Fluxer.Net.Rest;
using Newtonsoft.Json;

namespace Fluxer.Net.Test;

public class MemberUpdateTests
{
    private const string MemberJson =
        "{\"user\":{\"id\":\"5\",\"username\":\"bot\"},\"joined_at\":\"2025-01-01T00:00:00Z\",\"roles\":[],\"profile_flags\":3,\"mention_flags\":2}";

    private static string Serialize(object value) => JsonConvert.SerializeObject(value, FluxerClient._restSerializer);

    [Test]
    public async Task CurrentMemberUpdateSendsOnlyTheSetProfileFields()
    {
        RecordingHandler handler = new(MemberJson);
        FluxerClient client = new("Bot test", RecordingHandler.Config(new HttpClient(handler)));

        await client.Rest.UpdateCurrentMemberAsync(10, new UpdateCurrentMemberRequest
        {
            Bio = "hello",
            Pronouns = "they/them",
            AvatarBase64 = null
        });

        var request = handler.Requests.Single();
        Assert.Multiple(() =>
        {
            Assert.That(request.Method, Is.EqualTo(HttpMethod.Patch));
            Assert.That(request.Url, Does.EndWith("/guilds/10/members/@me"));
            Assert.That(request.Body, Is.EqualTo("{\"avatar\":null,\"bio\":\"hello\",\"pronouns\":\"they/them\"}"));
        });
    }

    [Test]
    public void ProfileFlagsAndMentionPreferenceAreSentAsNumbers()
    {
        string json = Serialize(new UpdateGuildMemberRequest
        {
            ProfileFlags = GuildMemberProfileFlags.AvatarUnset | GuildMemberProfileFlags.BannerUnset,
            MentionFlags = MentionReplyPreference.PreferMention
        });

        Assert.That(json, Is.EqualTo("{\"profile_flags\":3,\"mention_flags\":1}"));
    }

    [Test]
    public void MemberReadsProfileFlagsAndMentionPreference()
    {
        GuildMemberJson json = JsonConvert.DeserializeObject<GuildMemberJson>(MemberJson, FluxerClient._restSerializer)!;

        Assert.Multiple(() =>
        {
            Assert.That(json.ProfileFlags, Is.EqualTo(GuildMemberProfileFlags.AvatarUnset | GuildMemberProfileFlags.BannerUnset));
            Assert.That(json.MentionFlags, Is.EqualTo(MentionReplyPreference.PreferNoMention));
        });
    }
}
