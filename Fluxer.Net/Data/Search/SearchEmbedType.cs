using Fluxer.Net.Extensions;

namespace Fluxer.Net;

// Any changes needs to be updated with json converters too!
/// <see cref="SearchEmbedTypeConverter"/>
[Flags]
public enum SearchEmbedType
{
    Image,
    Video,
    Sound,
    Article
}
