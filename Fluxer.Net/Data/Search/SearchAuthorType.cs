using Fluxer.Net.Extensions;

namespace Fluxer.Net;

// Any changes needs to be updated with json converters too!
/// <summary>
/// The author types a search can filter by. Serialized by <see cref="SearchAuthorTypeConverter"/>.
/// </summary>
/// <remarks>
/// The members are 0, 1 and 2, not single bits. User is 0, so the converter writes user for every value.
/// </remarks>
[Flags]
public enum SearchAuthorType
{
    /// <summary>
    /// Messages sent by users.
    /// </summary>
    User,

    /// <summary>
    /// Messages sent by bots.
    /// </summary>
    Bot,

    /// <summary>
    /// Messages sent by webhooks.
    /// </summary>
    Webhook
}
