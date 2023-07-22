namespace Squll.Net.Extensions;

[System.Serializable]
public class SqullException : System.Exception
{
    public SqullException() { }
    public SqullException(string message) : base(message) { }
    public SqullException(string message, System.Exception inner) : base(message, inner) { }
    protected SqullException(
        System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

    public string SqullData { get; set; }
}
