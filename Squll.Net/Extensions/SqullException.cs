namespace Squll.Net.Extensions;

[System.Serializable]
public class SqullApiException : System.Exception
{
    public SqullApiException(string message, string data) : base(message)
    {
        SqullData = data;
    }

    public string SqullData { get; set; }
}
