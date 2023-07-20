namespace Squll.Net.Gateway;

public class StatusUpdateGatewayPacket : IGatewayPacket
{
    public SqullOpCode OpCode => SqullOpCode.SetStatus;
    public IGatewayData Data => _data;

    public int? Sequence { get; set; }

    private StatusUpdateGatewayData _data;

    public StatusUpdateGatewayPacket(StatusUpdateGatewayData data, int sequence)
    {
        _data = data;
        Sequence = sequence;
    }
}
