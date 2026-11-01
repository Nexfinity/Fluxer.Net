namespace Fluxer.Net.Test;

public class CdnUrlTests
{
    [Test]
    public void StaticHashKeepsPlainPngUrl()
    {
        Assert.That(CdnUrl.Png("https://media.example/avatars/1/c83846f7", "c83846f7", 128),
            Is.EqualTo("https://media.example/avatars/1/c83846f7.png?size=128"));
    }

    [Test]
    public void AnimatedHashAsksForFirstFrame()
    {
        Assert.That(CdnUrl.Png("https://media.example/avatars/1/a_02777747", "a_02777747", 128),
            Is.EqualTo("https://media.example/avatars/1/a_02777747.png?size=128&animated=false"));
    }

    [Test]
    public void AnimatedUserAvatarUrlAsksForFirstFrame()
    {
        UserJson animated = new UserJson { Id = 1, AvatarHash = "a_02777747" };
        UserJson still = new UserJson { Id = 1, AvatarHash = "c83846f7" };

        Assert.Multiple(() =>
        {
            Assert.That(animated.GetAvatarUrl(128), Does.EndWith(".png?size=128&animated=false"));
            Assert.That(still.GetAvatarUrl(128), Does.EndWith(".png?size=128"));
        });
    }
}
