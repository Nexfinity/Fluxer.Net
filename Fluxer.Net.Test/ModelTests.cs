using Fluxer.Net.Commands;

namespace Fluxer.Net.Test;

public class ModelTests
{
    [Test]
    public void EmojiParsesCustomAndUnicode()
    {
        Assert.Multiple(() =>
        {
            Assert.That(Emoji.TryParse("<:trolley:1435375026174558456>", out Emoji custom));
            Assert.That(custom.Name, Is.EqualTo("trolley"));
            Assert.That(custom.Id, Is.EqualTo(1435375026174558456UL));
            Assert.That(custom.IsAnimated, Is.False);
            Assert.That(custom.ToString(), Is.EqualTo("<:trolley:1435375026174558456>"));
            Assert.That(Emoji.TryParse("<a:dance:42>", out Emoji animated) && animated.IsAnimated);
            Assert.That(animated.ToString(), Is.EqualTo("<a:dance:42>"));
            Assert.That(Emoji.TryParse("👍", out Emoji thumbs) && !thumbs.IsCustom);
            Assert.That(thumbs.ToString(), Is.EqualTo("👍"));
            Assert.That(Emoji.TryParse("thumbsup", out _), Is.False);
            Assert.That(Emoji.TryParse("<:broken>", out _), Is.False);
        });
    }

    [Test]
    public void PermissionsHasAndList()
    {
        GuildPermissions perms = new(GuildPermission.KickMembers | GuildPermission.BanMembers);
        Assert.Multiple(() =>
        {
            Assert.That(perms.Has(GuildPermission.KickMembers));
            Assert.That(perms.Has(GuildPermission.KickMembers | GuildPermission.BanMembers));
            Assert.That(perms.Has(GuildPermission.KickMembers | GuildPermission.Administrator), Is.False);
            Assert.That(perms.ToList(), Is.EquivalentTo(new[] { GuildPermission.KickMembers, GuildPermission.BanMembers }));
        });
    }

    [Test]
    public void UserToStringShowsTheDiscriminatorOnlyWhenSet()
    {
        Assert.Multiple(() =>
        {
            Assert.That(User.Create(null!, new UserJson { Id = 1, Username = "sylv", Discriminator = "0" }).ToString(), Is.EqualTo("sylv"));
            Assert.That(User.Create(null!, new UserJson { Id = 1, Username = "sylv", Discriminator = "0420" }).ToString(), Is.EqualTo("sylv#0420"));
        });
    }

    [Test]
    public void GuildAvatarIsNullWithoutAServerAvatar()
    {
        GuildMemberJson member = new() { GuildId = 2, User = new UserJson { Id = 1, AvatarHash = "account" } };
        GuildMemberJson withServerAvatar = new() { GuildId = 2, AvatarHash = "server", User = new UserJson { Id = 1, AvatarHash = "account" } };

        Assert.Multiple(() =>
        {
            Assert.That(member.GetGuildAvatarUrl(), Is.Null);
            Assert.That(member.GetAvatarUrl(), Does.Contain("/avatars/1/account."));
            Assert.That(withServerAvatar.GetGuildAvatarUrl(), Does.Contain("/guilds/2/users/1/avatars/server."));
            Assert.That(withServerAvatar.GetAvatarUrl(), Is.EqualTo(withServerAvatar.GetGuildAvatarUrl()));
        });
    }

    [Test]
    public void ContextTypesDoNotOverlap()
    {
        Assert.Multiple(() =>
        {
            Assert.That(ContextType.Community & ContextType.DM, Is.EqualTo((ContextType)0));
            Assert.That(ContextType.Community & ContextType.Group, Is.EqualTo((ContextType)0));
            Assert.That(ContextType.Guild, Is.EqualTo(ContextType.Community));
        });
    }

    [Test]
    public void EmojiMatchIgnoresVariationSelectorsAndUsesIdsForCustomEmojis()
    {
        Assert.Multiple(() =>
        {
            Assert.That(Emoji.FromUnicode("\u23EE\uFE0F").Matches(Emoji.FromUnicode("\u23EE")), Is.True);
            Assert.That(Emoji.FromUnicode("\u2705").Matches(Emoji.FromUnicode("\u274C")), Is.False);
            Assert.That(Emoji.FromCustom("a", 1).Matches(Emoji.FromCustom("b", 1)), Is.True);
            Assert.That(Emoji.FromCustom("a", 1).Matches(Emoji.FromUnicode("a")), Is.False);
            Assert.That(Emoji.From(Emoji.FromCustom("a", 5)).ToString(), Is.EqualTo("<:a:5>"));
        });
    }
}
