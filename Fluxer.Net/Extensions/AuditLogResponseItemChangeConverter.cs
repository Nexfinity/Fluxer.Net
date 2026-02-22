using Fluxer.Net.Data.Responses;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Fluxer.Net.Extensions;

// TODO this REALLY should be refactored into some sort of polymorphic stuff with System.Text.Json,
// but this passes the (new) tests fine
public class AuditLogResponseItemChangeConverter : JsonConverter<AuditLogResponseItemChangeBase>
{
    public override AuditLogResponseItemChangeBase? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException();
        }

        if (JsonDocument.TryParseValue(ref reader, out var tJsonDoc))
        {
            Debugger.Break();
            var objDto = new AuditLogResponseItemChangeBase();
            if (tJsonDoc.RootElement.TryGetProperty("key", out var keyProp))
            {
                objDto.Key = keyProp.GetString();
                if (string.IsNullOrEmpty(objDto.Key))
                {
                    throw new InvalidOperationException("Value \"key\" cannot be null or empty");
                }
            }
            if (tJsonDoc.RootElement.TryGetProperty("new_value", out var newValueProp))
            {
                objDto.NewValue = TryParseProp(newValueProp);
            }
            if (tJsonDoc.RootElement.TryGetProperty("old_value", out var oldValueProp))
            {
                objDto.OldValue = TryParseProp(oldValueProp);
            }
            return Convert(objDto);
        }
        throw new JsonException();
    }
    private static AuditLogResponseItemChangeBase Convert(AuditLogResponseItemChangeBase dto)
    {
        if (dto.NewValue is string[] || dto.OldValue is string[])
        {
            return new AuditLogResponseItemChange<string[]?>
            {
                Key = dto.Key,
                NewValue = (string[]?)dto.NewValue,
                OldValue = (string[]?)dto.OldValue,
            };
        }
        else if (dto.NewValue is ulong[] || dto.OldValue is ulong[])
        {
            return new AuditLogResponseItemChange<ulong[]?>
            {
                Key = dto.Key,
                NewValue = (ulong[]?)dto.NewValue,
                OldValue = (ulong[]?)dto.OldValue,
            };
        }
        else if (dto.NewValue is long[] || dto.OldValue is long[])
        {
            return new AuditLogResponseItemChange<long[]?>
            {
                Key = dto.Key,
                NewValue = (long[]?)dto.NewValue,
                OldValue = (long[]?)dto.OldValue,
            };
        }
        else if (dto.NewValue is double[] || dto.OldValue is double[])
        {
            return new AuditLogResponseItemChange<double[]?>
            {
                Key = dto.Key,
                NewValue = (double[]?)dto.NewValue,
                OldValue = (double[]?)dto.OldValue,
            };
        }
        else if (dto.NewValue is string || dto.OldValue is string)
        {
            return new AuditLogResponseItemChange<string?>
            {
                Key = dto.Key,
                NewValue = (string?)dto.NewValue,
                OldValue = (string?)dto.OldValue,
            };
        }
        else if (dto.NewValue is double || dto.OldValue is double)
        {
            return new AuditLogResponseItemChange<double?>
            {
                Key = dto.Key,
                NewValue = (double?)dto.NewValue,
                OldValue = (double?)dto.OldValue,
            };
        }
        else if (dto.NewValue is PermissionDiffSchema || dto.OldValue is PermissionDiffSchema)
        {
            return new AuditLogResponseItemChange<PermissionDiffSchema?>
            {
                Key = dto.Key,
                NewValue = (PermissionDiffSchema?)dto.NewValue,
                OldValue = (PermissionDiffSchema?)dto.OldValue,
            };
        }
        else if (dto.NewValue is bool || dto.OldValue is bool)
        {
            return new AuditLogResponseItemChange<bool?>
            {
                Key = dto.Key,
                NewValue = (bool?)dto.NewValue,
                OldValue = (bool?)dto.OldValue,
            };
        }
        return dto;
    }
    private static object? TryParseProp(JsonElement prop)
    {
        switch (prop.ValueKind)
        {
            case JsonValueKind.Array:
                try
                {
                    return prop.Deserialize<string[]>();
                }
                catch { }
                try
                {
                    return prop.Deserialize<ulong[]>();
                }
                catch { }
                try
                {
                    return prop.Deserialize<double[]>();
                }
                catch { }
                break;
            case JsonValueKind.Object:
                return prop.Deserialize<PermissionDiffSchema>();
            case JsonValueKind.False:
                return false;
            case JsonValueKind.True:
                return false;
            case JsonValueKind.Number:
                if (prop.TryGetUInt64(out var vulong)) return vulong;
                if (prop.TryGetInt64(out var vlong)) return vlong;
                if (prop.TryGetDouble(out var vdouble)) return vdouble;
                break;
            case JsonValueKind.String:
                return prop.GetString();
        }
        return null;
    }
    public override void Write(Utf8JsonWriter writer, AuditLogResponseItemChangeBase value, JsonSerializerOptions options)
    {
        // nothing
    }
}
