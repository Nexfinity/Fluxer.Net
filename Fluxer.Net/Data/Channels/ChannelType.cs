namespace Fluxer.Net;

/// <summary>
/// Represents the different types of channels available in Fluxer.
/// </summary>
public enum ChannelType
{
	/// <summary>
	/// A text channel within a guild/server.
	/// </summary>
	GuildText = 0,

	/// <summary>
	/// A direct message channel between two users.
	/// </summary>
	Dm = 1,

	/// <summary>
	/// A voice channel within a guild/server.
	/// </summary>
	GuildVoice = 2,

	/// <summary>
	/// A direct message channel between multiple users (group DM).
	/// </summary>
	GroupDm = 3,

	/// <summary>
	/// A channel category used to organize channels within a guild.
	/// </summary>
	GuildCategory = 4,

	/// <summary>
	/// A news/announcement channel within a guild.
	/// </summary>
	GuildNews = 5,

	/// <summary>
	/// A store channel within a guild (deprecated in some platforms).
	/// </summary>
	GuildStore = 6,

	/// <summary>
	/// A thread channel created from a news/announcement post.
	/// </summary>
	NewsThread = 10,

	/// <summary>
	/// A public thread channel that anyone can view and join.
	/// </summary>
	PublicThread = 11,

	/// <summary>
	/// A private thread channel with restricted access.
	/// </summary>
	PrivateThread = 12,

	/// <summary>
	/// A stage voice channel for hosting events with speakers and audience.
	/// </summary>
	GuildStageVoice = 13,

	/// <summary>
	/// A directory channel for guild/server discovery.
	/// </summary>
	GuildDirectory = 14,

	/// <summary>
	/// A forum channel for topic-based discussions.
	/// </summary>
	GuildForum = 15,

	/// <summary>
	/// A media channel for sharing images and videos.
	/// </summary>
	GuildMedia = 16,

	/// <summary>
	/// A guild link channel (custom Fluxer channel type).
	/// </summary>
	GuildLink = 998,

	/// <summary>
	/// A personal notes DM channel for storing private messages.
	/// </summary>
	DmPersonalNotes = 999,
}
