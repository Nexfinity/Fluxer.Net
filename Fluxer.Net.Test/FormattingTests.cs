namespace Fluxer.Net.Test;

public class FormattingTests
{
    [Test]
    public void MentionsRoundTrip()
    {
        Assert.Multiple(() =>
        {
            Assert.That(MentionUtils.MentionUser(123), Is.EqualTo("<@123>"));
            Assert.That(MentionUtils.MentionRole(456), Is.EqualTo("<@&456>"));
            Assert.That(MentionUtils.MentionChannel(789), Is.EqualTo("<#789>"));
            Assert.That(MentionUtils.TryParseUser("<@123>", out ulong user) && user == 123);
            Assert.That(MentionUtils.TryParseUser("<@!123>", out ulong legacy) && legacy == 123);
            Assert.That(MentionUtils.TryParseUser("<@&123>", out _), Is.False);
            Assert.That(MentionUtils.TryParseRole("<@&456>", out ulong role) && role == 456);
            Assert.That(MentionUtils.TryParseChannel("<#789>", out ulong channel) && channel == 789);
            Assert.That(MentionUtils.TryParseChannel("<#abc>", out _), Is.False);
        });
    }

    [Test]
    public void TimestampTagWritesUnixSecondsAndStyle()
    {
        DateTimeOffset time = DateTimeOffset.FromUnixTimeSeconds(1618953630);
        Assert.Multiple(() =>
        {
            Assert.That(TimestampTag.FromDateTimeOffset(time, TimestampTagStyles.Relative).ToString(), Is.EqualTo("<t:1618953630:R>"));
            Assert.That(new TimestampTag(time).ToString(), Is.EqualTo("<t:1618953630:f>"));
            Assert.That(new TimestampTag(time).ToString(TimestampTagStyles.ShortDateMediumTime), Is.EqualTo("<t:1618953630:S>"));
        });
    }

    [Test]
    public void FormatEscapesAndQuotes()
    {
        Assert.Multiple(() =>
        {
            Assert.That(Format.Bold("hi"), Is.EqualTo("**hi**"));
            Assert.That(Format.Code("x"), Is.EqualTo("`x`"));
            Assert.That(Format.Code("x", "cs"), Is.EqualTo("```cs\nx\n```"));
            Assert.That(Format.Quote("a\nb"), Is.EqualTo("> a\n> b"));
            Assert.That(Format.Sanitize("*a_b*"), Is.EqualTo("\\*a\\_b\\*"));
            Assert.That(Format.Sanitize("a - b (c)"), Is.EqualTo("a - b (c)"));
        });
    }

    [Test]
    public void ColorParsesHexAndExposesParts()
    {
        Assert.Multiple(() =>
        {
            Assert.That(Color.TryParse("#2ECC71", out Color green) && green == Color.Green);
            Assert.That(Color.Parse("0x123456").R, Is.EqualTo(0x12));
            Assert.That(Color.TryParse("12345", out _), Is.False);
            Assert.That((int)new Color(255, 0, 128), Is.EqualTo(0xFF0080));
            Assert.That(Color.Blue.ToString(), Is.EqualTo("#3498DB"));
        });
        Assert.Throws<ArgumentOutOfRangeException>(() => _ = new Color(0x1000000));
    }
}
