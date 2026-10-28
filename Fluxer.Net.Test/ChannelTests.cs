using Fluxer.Net.Commands;

namespace Fluxer.Net.Test;

public class ChannelTests
{
    [Test]
    public void SocketVoiceChannelSatisfiesItsKindInterfaces()
    {
        SocketVoiceChannel channel = new(null!);
        Assert.Multiple(() =>
        {
            Assert.That(channel, Is.InstanceOf<SocketTextChannel>());
            Assert.That(channel, Is.InstanceOf<IVoiceChannel>());
            Assert.That(channel, Is.InstanceOf<IAudioChannel>());
            Assert.That(channel, Is.InstanceOf<ITextChannel>());
            Assert.That(channel, Is.InstanceOf<IGuildChannel>());
            Assert.That(channel, Is.InstanceOf<ISocketGuildChannel>());
            Assert.That(channel, Is.Not.InstanceOf<INewsChannel>());
            Assert.That(channel, Is.Not.InstanceOf<IThreadChannel>());
        });
    }

    [Test]
    public void SocketNewsChannelSatisfiesItsKindInterfaces()
    {
        SocketNewsChannel channel = new(null!);
        Assert.Multiple(() =>
        {
            Assert.That(channel, Is.InstanceOf<SocketTextChannel>());
            Assert.That(channel, Is.InstanceOf<INewsChannel>());
            Assert.That(channel, Is.InstanceOf<ITextChannel>());
            Assert.That(channel, Is.InstanceOf<IGuildChannel>());
            Assert.That(channel, Is.Not.InstanceOf<IVoiceChannel>());
        });
    }

    [Test]
    public void SocketThreadChannelSatisfiesItsKindInterfaces()
    {
        SocketThreadChannel channel = new(null!) { ThreadType = ThreadType.PrivateThread };
        Assert.Multiple(() =>
        {
            Assert.That(channel, Is.InstanceOf<SocketTextChannel>());
            Assert.That(channel, Is.InstanceOf<IThreadChannel>());
            Assert.That(channel, Is.InstanceOf<ITextChannel>());
            Assert.That(channel, Is.InstanceOf<IGuildChannel>());
            Assert.That(channel.IsPrivate, Is.True);
            Assert.That(channel.CategoryId, Is.Null);
        });
    }

    [Test]
    public void RestChannelsSatisfyTheSameKindInterfaces()
    {
        Assert.Multiple(() =>
        {
            Assert.That(new VoiceChannel(null!), Is.InstanceOf<IVoiceChannel>().And.InstanceOf<ITextChannel>().And.InstanceOf<IGuildChannel>());
            Assert.That(new NewsChannel(null!), Is.InstanceOf<INewsChannel>().And.InstanceOf<ITextChannel>());
            Assert.That(new ThreadChannel(null!), Is.InstanceOf<IThreadChannel>().And.InstanceOf<ITextChannel>());
            Assert.That(new CategoryChannel(null!), Is.InstanceOf<ICategoryChannel>().And.InstanceOf<IGuildChannel>());
            Assert.That(new MediaChannel(null!), Is.InstanceOf<IMediaChannel>().And.InstanceOf<IForumChannel>().And.InstanceOf<INestedChannel>());
            Assert.That(new DMChannel(null!), Is.InstanceOf<IDMChannel>().And.InstanceOf<IPrivateChannel>().And.InstanceOf<IMessageChannel>().And.InstanceOf<ITextable>());
            Assert.That(new GroupChannel(null!), Is.InstanceOf<IGroupChannel>().And.InstanceOf<IPrivateChannel>());
            Assert.That(new TextChannel(null!), Is.Not.InstanceOf<IVoiceChannel>());
        });
    }

    [Test]
    public void ChannelTypeReaderAcceptsInterfaceParameterTypes()
    {
        SocketVoiceChannel voice = new(null!);
        SocketNewsChannel news = new(null!);
        Assert.Multiple(() =>
        {
            Assert.That(ChannelTypeReader<IVoiceChannel>.Accepts(voice), Is.True);
            Assert.That(ChannelTypeReader<IVoiceChannel>.Accepts(news), Is.False);
            Assert.That(ChannelTypeReader<ITextChannel>.Accepts(voice), Is.True);
            Assert.That(ChannelTypeReader<INewsChannel>.Accepts(news), Is.True);
            Assert.That(ChannelTypeReader<SocketTextChannel>.Accepts(voice), Is.True);
            Assert.That(ChannelTypeReader<IChannel>.Accepts(null), Is.False);
        });
    }

    [Test]
    public void CommandServiceUsesTheChannelReaderForInterfaceTypes()
    {
        CommandService service = new();
        Assert.Multiple(() =>
        {
            Assert.That(service.GetDefaultTypeReader(typeof(IVoiceChannel)), Is.InstanceOf<ChannelTypeReader<IVoiceChannel>>());
            Assert.That(service.GetDefaultTypeReader(typeof(IThreadChannel)), Is.InstanceOf<ChannelTypeReader<IThreadChannel>>());
            Assert.That(service.GetDefaultTypeReader(typeof(ICategoryChannel)), Is.InstanceOf<ChannelTypeReader<ICategoryChannel>>());
            Assert.That(service.GetDefaultTypeReader(typeof(SocketVoiceChannel)), Is.InstanceOf<ChannelTypeReader<SocketVoiceChannel>>());
        });
    }

    [Test]
    public void SocketChannelFactoryBuildsSocketKindsWithTheirGuild()
    {
        SocketGuild guild = new(null!);
        Channel voice = SocketChannel.Create(null!, new ChannelJson { Id = 1, Type = ChannelType.GuildVoice }, guild);
        Channel news = SocketChannel.Create(null!, new ChannelJson { Id = 2, Type = ChannelType.GuildNews }, guild);
        Channel thread = SocketChannel.Create(null!, new ChannelJson { Id = 3, Type = ChannelType.PublicThread, ParentId = 2 }, guild);
        Channel category = SocketChannel.Create(null!, new ChannelJson { Id = 4, Type = ChannelType.GuildCategory }, guild);
        Assert.Multiple(() =>
        {
            Assert.That(voice, Is.InstanceOf<SocketVoiceChannel>().And.InstanceOf<IVoiceChannel>());
            Assert.That(news, Is.InstanceOf<SocketNewsChannel>().And.InstanceOf<INewsChannel>());
            Assert.That(thread, Is.InstanceOf<SocketThreadChannel>().And.InstanceOf<IThreadChannel>());
            Assert.That(category, Is.InstanceOf<SocketCategoryChannel>().And.InstanceOf<ICategoryChannel>());
            Assert.That(((ISocketGuildChannel)voice).Guild, Is.SameAs(guild));
            Assert.That(((ISocketGuildChannel)thread).Guild, Is.SameAs(guild));
            Assert.That(((ISocketGuildChannel)category).Guild, Is.SameAs(guild));
        });
    }
}
