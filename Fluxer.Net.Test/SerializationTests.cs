using Fluxer.Net.Rest;
using Newtonsoft.Json;

namespace Fluxer.Net.Test;

public class SerializationTests
{
    [Test]
    public void AllowedMentionParseValuesAreLowercase()
    {
        string json = JsonConvert.SerializeObject(new AllowedMentionsRequest
        {
            Parse = new HashSet<AllowedMentionParseType> { AllowedMentionParseType.USERS, AllowedMentionParseType.EVERYONE }
        });

        Assert.Multiple(() =>
        {
            Assert.That(json, Does.Contain("\"users\""));
            Assert.That(json, Does.Contain("\"everyone\""));
            Assert.That(json, Does.Not.Contain("USERS"));
        });
    }

    [Test]
    public void OverwritePermissionsAreSentAsStrings()
    {
        string json = JsonConvert.SerializeObject(new PermissionOverwriteRequest
        {
            Type = PermissionOverwriteType.Role,
            Allow = (ulong)ChannelPermission.ViewChannelMembers,
            Deny = 0
        });

        Assert.That(json, Does.Contain($"\"allow\":\"{(ulong)ChannelPermission.ViewChannelMembers}\""));
    }

    private static string SerializeForRest(object value) => JsonConvert.SerializeObject(value, FluxerClient._restSerializer);

    [Test]
    public void MemberUpdateOnlySendsWhatWasSet()
    {
        Assert.Multiple(() =>
        {
            Assert.That(SerializeForRest(new UpdateGuildMemberRequest { Nickname = "Sylveon" }),
                Is.EqualTo("{\"nick\":\"Sylveon\"}"));
            Assert.That(SerializeForRest(new UpdateGuildMemberRequest { Nickname = null }),
                Is.EqualTo("{\"nick\":null}"));
            Assert.That(SerializeForRest(new UpdateGuildMemberRequest { CommunicationDisabledUntil = null }),
                Is.EqualTo("{\"communication_disabled_until\":null}"));
            Assert.That(SerializeForRest(new UpdateGuildMemberRequest()), Is.EqualTo("{}"));
        });
    }

    [Test]
    public void ChannelAndGuildUpdatesOnlySendWhatWasSet()
    {
        Assert.Multiple(() =>
        {
            Assert.That(SerializeForRest(new UpdateChannelRequest { Name = "general" }),
                Is.EqualTo("{\"name\":\"general\"}"));
            Assert.That(SerializeForRest(new UpdateChannelRequest { ParentId = null, Topic = null }),
                Is.EqualTo("{\"topic\":null,\"parent_id\":null}"));
            Assert.That(SerializeForRest(new UpdateGuildRequest { Name = "Mewdeko" }),
                Is.EqualTo("{\"name\":\"Mewdeko\"}"));
            Assert.That(SerializeForRest(new UpdateCurrentUserRequest { Username = "Mewdeko" }),
                Is.EqualTo("{\"username\":\"Mewdeko\"}"));
        });
    }

    [Test]
    public void LogsNeverCarryCredentials()
    {
        Assert.Multiple(() =>
        {
            Assert.That(LogRedaction.Redact("{\"op\":2,\"d\":{\"token\":\"Bot abc.def\"}}"), Does.Not.Contain("abc.def"));
            Assert.That(LogRedaction.Redact("{\"access_token\":\"x1\",\"refresh_token\":\"x2\"}"), Does.Not.Contain("x1").And.Not.Contain("x2"));
            Assert.That(LogRedaction.Redact("/webhooks/123/secret-token/messages/4"), Is.EqualTo("/webhooks/123/[redacted]/messages/4"));
            Assert.That(LogRedaction.Redact("{\"content\":\"hello\"}"), Is.EqualTo("{\"content\":\"hello\"}"));
            Assert.That(LogRedaction.Redact("{\"code\":\"UNKNOWN_CHANNEL\"}"), Is.EqualTo("{\"code\":\"UNKNOWN_CHANNEL\"}"));
        });
    }

    [Test]
    public void GatewayErrorsAreRaisedWithTheirCode()
    {
        FluxerClient client = new FluxerClient("Bot test");
        Gateway.GatewayErrorGatewayData? received = null;
        client.Gateway.GatewayErrorReceived += error => received = error;

        client.Gateway.GatewayMessageHandler("{\"op\":12,\"d\":{\"code\":\"VOICE_CHANNEL_FULL\",\"message\":\"The channel is full\"}}");

        Assert.That(received?.Code, Is.EqualTo(FluxerErrorCodes.VoiceChannelFull));
    }
}
