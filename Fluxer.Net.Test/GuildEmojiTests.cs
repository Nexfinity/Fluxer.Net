namespace Fluxer.Net.Test;

public class GuildEmojiTests
{
    [Test]
    public void GuildEmojiKeepsItsIdAndBuildsTheMediaUrl()
    {
        FluxerClient client = new("Bot test", new FluxerConfig { MediaUrl = "https://media.example" });
        GuildEmoji emoji = GuildEmoji.Create(client, new GuildEmojiJson { Id = 42, Name = "wave" }, 7);

        Assert.Multiple(() =>
        {
            Assert.That(emoji.Id, Is.EqualTo(42UL));
            Assert.That(((Emoji)emoji).Id, Is.EqualTo(42UL));
            Assert.That(emoji.GetEmojiUrl(48), Is.EqualTo("https://media.example/emojis/42.webp?size=48"));
        });
    }
}
