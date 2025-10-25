namespace Fluxer.Net.Extensions;

[System.Serializable]
public class FluxerApiException : System.Exception
{
    public FluxerApiException(string message, string data) : base(message)
    {
        FluxerData = data;
    }

    public string FluxerData { get; set; }
}
