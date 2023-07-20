namespace Squll.Net.Gateway;

public class LoginGatewayPacket : IGatewayPacket
{
    public SqullOpCode OpCode => SqullOpCode.Login;
    public IGatewayData Data => _data;

    public int? Sequence { get; set; }

    private LoginGatewayData _data;

    public LoginGatewayPacket(LoginGatewayData data, int sequence)
    {
        _data = data;
        Sequence = sequence;
    }
}
