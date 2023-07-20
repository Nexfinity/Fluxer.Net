namespace Squll.Net.Gateway;

public class ReadyGatewayPacket : IGatewayPacket
{
    public SqullOpCode OpCode => SqullOpCode.Ready;
    public IGatewayData Data => _data;

    public int? Sequence { get; set; }

    private ReadyGatewayData _data;

    public ReadyGatewayPacket(ReadyGatewayData data, int sequence)
    {
        _data = data;
        Sequence = sequence;
    }
}
