using Fluxer.Net.Extensions;

namespace Fluxer.Net;

// Any changes needs to be updated with json converters too!
/// <see cref="SearchAuthorTypeConverter"/>
[Flags]
public enum SearchAuthorType
{
    User,
    Bot,
    Webhook
}
