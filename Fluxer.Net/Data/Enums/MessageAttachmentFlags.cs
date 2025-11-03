namespace Fluxer.Net.Objects.Data;

[Flags]
public enum MessageAttachmentFlags
{
	None = 0,
	IsSpoiler = 1 << 3,
	ContainsExplicitMedia = 1 << 4,
	IsAnimated = 1 << 5,
}
