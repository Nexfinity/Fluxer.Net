using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;
using System.Reflection;

namespace Fluxer.Net;

/// <summary>
/// An audit log entry of a guild, with the values before and after the change.
/// </summary>
public class SocketAuditLog : Entity, ISnowflake
{
    /// <summary>
    /// The guild the entry belongs to.
    /// </summary>
    public SocketGuild Guild { get; private set; }

    /// <inheritdoc />
    public ulong Id { get; private set; }

    /// <inheritdoc />
    [JsonIgnore]
    public DateTimeOffset CreatedAt => SnowflakeUtils.FromSnowflake(Id);

    /// <summary>
    /// The type of action the entry records.
    /// </summary>
    public ActionType Action { get; private set; }

    /// <summary>
    /// The id of the user that made the change.
    /// </summary>
    public ulong? UserId { get; private set; }

    /// <summary>
    /// The id of the entity that was changed.
    /// </summary>
    public ulong? TargetId { get; private set; }

    /// <summary>
    /// The reason given for the change.
    /// </summary>
    public string? Reason { get; private set; }

    /// <summary>
    /// The raw old and new values, keyed by field name.
    /// </summary>
    public AuditLogRawDataJson Raw { get; private set; }

    /// <summary>
    /// The values before the change, or null when the action has no typed data.
    /// </summary>
    public IAuditLogData? OldData { get; private set; }

    /// <summary>
    /// The values after the change, or null when the action has no typed data.
    /// </summary>
    public IAuditLogData NewData { get; private set; }

    internal SocketAuditLog(FluxerBaseClient client) : base(client)
    {

    }

    /// <summary>
    /// Creates an audit log entry from its JSON, with typed values when the action has a known data type.
    /// </summary>
    /// <returns><see cref="SocketAuditLog"/></returns>
    public static SocketAuditLog Create(FluxerBaseClient client, GuildAuditLogJson json)
    {
        SocketAuditLog data = new SocketAuditLog(client)
        {
            Id = json.Id,
            Action = json.Action,
            UserId = json.UserId,
            TargetId = json.TargetId,
            Reason = json.Reason,
            Raw = new AuditLogRawDataJson
            {
                OldData = new JObject(),
                NewData = new JObject()
            },
            Guild = (client as FluxerClient).Gateway.GetGuild(json.GuildId)
        };
        if (json.Changes != null)
        {
            foreach (AuditLogChangeJson c in json.Changes)
            {
                if (c.OldValue != null)
                    data.Raw.OldData.Add(c.Key, c.OldValue);

                data.Raw.NewData.Add(c.Key, c.NewValue);
            }
        }
        Type? type = GetChangeDataType(data.Action);
        if (type != null)
        {
            data.OldData = (IAuditLogData)Activator.CreateInstance(type)!;
            data.NewData = (IAuditLogData)Activator.CreateInstance(type)!;

            ILogger? logger = (client as FluxerClient)?.Config.GatewaySerilog;
            PropertyInfo[] props = type.GetProperties();
            foreach (PropertyInfo p in props)
            {
                if (p.GetCustomAttributes(typeof(JsonPropertyAttribute), true).FirstOrDefault() is not JsonPropertyAttribute jsonAttribute)
                    continue;

                AuditLogChangeJson? change = json.Changes?.FirstOrDefault(x => x.Key == jsonAttribute.PropertyName);
                if (change == null)
                    continue;

                SetChangeValue(p, data.OldData, change.OldValue, logger);
                SetChangeValue(p, data.NewData, change.NewValue, logger);
            }

        }
        return data;
    }

    private static Type? GetChangeDataType(ActionType action) => action switch
    {
        ActionType.GuildUpdated => typeof(GuildChangeDataJson),
        ActionType.ChannelCreated or ActionType.ChannelUpdated or ActionType.ChannelDeleted => typeof(ChannelChangeDataJson),
        ActionType.Ban or ActionType.Unban => typeof(BanChangeDataJson),
        ActionType.MemberUpdated or ActionType.MemberRoleUpdated => typeof(MemberChangeDataJson),
        ActionType.MemberMoved or ActionType.MemberDisconnected => typeof(VoiceChannelChangeDataJson),
        ActionType.RoleCreated or ActionType.RoleUpdated or ActionType.RoleDeleted => typeof(RoleChangeDataJson),
        ActionType.InviteCreated or ActionType.InviteUpdated or ActionType.InviteDeleted => typeof(InviteChangeDataJson),
        ActionType.WebhookCreated or ActionType.WebhookUpdated or ActionType.WebhookDeleted => typeof(WebhookChangeDataJson),
        ActionType.EmojiCreated or ActionType.EmojiUpdated or ActionType.EmojiDeleted => typeof(EmojiChangeDataJson),
        ActionType.StickerCreated or ActionType.StickerUpdated or ActionType.StickerDeleted => typeof(StickerChangeDataJson),
        _ => null
    };

    private static void SetChangeValue(PropertyInfo property, IAuditLogData target, JToken? value, ILogger? logger)
    {
        if (value == null || value.Type == JTokenType.Null)
            return;

        try
        {
            property.SetValue(target, value.ToObject(property.PropertyType, FluxerClient._gatewaySerializer));
        }
        catch (Exception ex) when (ex is JsonException or ArgumentException or FormatException or InvalidCastException or OverflowException)
        {
            logger?.Debug(ex, "Could not convert audit log change {Property} to {Type}", property.Name, property.PropertyType);
        }
    }
}
