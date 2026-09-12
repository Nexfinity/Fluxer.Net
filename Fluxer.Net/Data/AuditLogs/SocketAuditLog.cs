

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Fluxer.Net;

public class SocketAuditLog : Entity, ISnowflake
{
    public SocketGuild Guild { get; private set; }

    public ulong Id { get; private set; }

    public DateTimeOffset CreatedAt => SnowflakeUtils.FromSnowflake(Id);

    public ActionType Action { get; private set; }

    public ulong? UserId { get; private set; }

    public ulong? TargetId { get; private set; }

    public string? Reason { get; private set; }
    public AuditRawDataJson Raw { get; private set; }
    public IAuditLogData? OldData { get; private set; }
    public IAuditLogData NewData { get; private set; }

    internal SocketAuditLog(FluxerBaseClient client) : base(client)
    {

    }

    /// <summary>
    /// Create a Login object from json.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="json"></param>
    /// <returns></returns>
    public static SocketAuditLog Create(FluxerBaseClient client, GuildAuditLogJson json)
    {
        SocketAuditLog data = new SocketAuditLog(client)
        {
            Id = json.Id,
            Action = json.Action,
            UserId = json.UserId,
            TargetId = json.TargetId,
            Reason = json.Reason,
            Raw = new AuditRawDataJson
            {
                OldData = new JObject(),
                NewData = new JObject()
            },
            Guild = (client as FluxerClient).Gateway.GetGuild(json.GuildId)
        };
        foreach (var c in json.Changes)
        {
            if (c.OldValue != null)
                data.Raw.OldData.Add(c.Key, c.OldValue);

            data.Raw.NewData.Add(c.Key, c.NewValue);
        }
        Type? type = null;
        switch (data.Action)
        {
            case ActionType.GuildUpdated:
                data.OldData = new AuditGuildDataJson();
                data.NewData = new AuditGuildDataJson();
                type = typeof(AuditGuildDataJson);
                break;
        }
        if (type != null)
        {
            var props = type.GetProperties();
            foreach (var p in props)
            {
                if (p.GetCustomAttributes(typeof(JsonPropertyAttribute), true).FirstOrDefault() is not JsonPropertyAttribute jsonAttribute)
                    continue;

                AuditLogChangeJson? change = json.Changes.FirstOrDefault(x => x.Key == jsonAttribute.PropertyName);
                if (change == null)
                    continue;

                if (data.OldData != null)
                    p.SetValue(data.OldData, change.OldValue?.ToObject(p.PropertyType, FluxerClient._gatewaySerializer));
                p.SetValue(data.NewData, change.NewValue?.ToObject(p.PropertyType, FluxerClient._gatewaySerializer));
            }

        }
        return data;
    }
}
