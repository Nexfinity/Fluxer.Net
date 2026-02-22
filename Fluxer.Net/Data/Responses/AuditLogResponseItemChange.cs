using Fluxer.Net.Extensions;
using System.Text.Json.Serialization;

namespace Fluxer.Net.Data.Responses;

/// <remarks>
/// <see href="https://github.com/fluxerapp/fluxer/blob/c2b69be17d1877c5bb82d10c77fa67cbe4e882d7/packages/schema/src/domains/guild/GuildAuditLogSchemas.tsx#L46"/>
/// </remarks>
[JsonConverter(typeof(AuditLogResponseItemChangeConverter))]
public class AuditLogResponseItemChangeBase
{
    [JsonRequired]
    [JsonPropertyName("key")]
    public string Key { get; set; }

    [JsonPropertyName("old_value")]
    public object? OldValue { get; set; }

    [JsonPropertyName("new_value")]
    public object? NewValue { get; set; }
}

[JsonConverter(typeof(AuditLogResponseItemChangeConverter))]
public class AuditLogResponseItemChange<TValue> : AuditLogResponseItemChangeBase
{
    private TValue _oldValue;
    private TValue _newValue;
    [JsonPropertyName("old_value")]
    public new TValue OldValue
    {
        get => _oldValue;
        set
        {
            base.OldValue = value;
            _oldValue = value;
        }
    }

    [JsonPropertyName("new_value")]
    public new TValue NewValue
    {
        get => _newValue;
        set
        {
            base.NewValue = value;
            _newValue = value;
        }
    }
}

public class PermissionDiffSchema
{
    [JsonPropertyName("added")]
    public HashSet<string> Added { get; set; }

    [JsonPropertyName("removed")]
    public HashSet<string> Removed { get; set; }
}
