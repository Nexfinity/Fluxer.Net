using Fluxer.Net.Extensions;

namespace Fluxer.Net;

// Any changes needs to be updated with json converters too!
/// <see cref="SearchContentTypeConverter"/>
public enum SearchContentType
{
    Image,
    Sound,
    Video,
    File,
    Sticker,
    Embed,
    Link,
    Poll,
    Snapshot
}
