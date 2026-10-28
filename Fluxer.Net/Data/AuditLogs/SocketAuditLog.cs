using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;
using System.Reflection;

namespace Fluxer.Net;

public class SocketAuditLog : Entity, ISnowflake
{
    public SocketGuild Guild { get; private set; }

    public ulong Id { get; private set; }

    [JsonIgnore]
    public DateTimeOffset CreatedAt => SnowflakeUtils.FromSnowflake(Id);

    public ActionType Action { get; private set; }

    public ulong? UserId { get; private set; }

    public ulong? TargetId { get; private set; }

    public string? Reason { get; private set; }

    public AuditLogRawDataJson Raw { get; private set; }

    public IAuditLogData? OldData { get; private set; }

    public IAuditLogData NewData { get; private set; }

    internal SocketAuditLog(FluxerBaseClient client) : base(client)
    {

    }

    /// <summary>
    /// Create a Login object from json.
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
