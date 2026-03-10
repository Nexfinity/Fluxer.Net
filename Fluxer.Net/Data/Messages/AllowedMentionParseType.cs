using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Fluxer.Net.Data.Messages;

/// <remarks>
/// https://github.com/fluxerapp/fluxer/blob/848269a4d4df7349acfc861ff926b17fe4c4a548/packages/schema/src/primitives/MessageValidators.tsx#L76
/// </remarks>
[JsonConverter(typeof(StringEnumConverter))]
public enum AllowedMentionParseType
{
    USERS,
    ROLES,
    EVERYONE
}
