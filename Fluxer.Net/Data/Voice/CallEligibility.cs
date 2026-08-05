namespace Fluxer.Net;

/// <inheritdoc />
public class CallEligibility : Entity, ICallEligibility
{

    /// <inheritdoc />
    public bool IsRingable { get; internal set; }


    /// <inheritdoc />
    public bool IsSilent { get; internal set; }

    internal CallEligibility(FluxerBaseClient client) : base(client)
    {

    }

    /// <summary>
    /// Create a CallEligibility object from json.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="json"></param>
    /// <returns></returns>
    public static CallEligibility Create(FluxerBaseClient client, CallEligibilityJson json)
    {
        CallEligibility data = new CallEligibility(client);
        data.Update(client, json);
        return data;
    }

    internal void Update(FluxerBaseClient client, CallEligibilityJson json)
    {
        IsRingable = json.IsRingable;
        IsSilent = json.IsSilent;
    }
}

