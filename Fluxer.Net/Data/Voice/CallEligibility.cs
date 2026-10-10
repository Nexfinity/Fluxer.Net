namespace Fluxer.Net;

/// <inheritdoc />
public class CallEligibility : Entity, ICallEligibility
{

    /// <inheritdoc />
    public bool IsRingable { get; private set; }


    /// <inheritdoc />
    public bool IsSilent { get; private set; }

    internal CallEligibility(FluxerBaseClient client) : base(client)
    {

    }

    /// <summary>
    /// Create a CallEligibility object from json.
    /// </summary>
    /// <returns><see cref="CallEligibility"/></returns>
    public static CallEligibility Create(FluxerBaseClient client, CallEligibilityJson json)
    {
        CallEligibility data = new CallEligibility(client);
        data.Update(json);
        return data;
    }

    internal void Update(CallEligibilityJson json)
    {
        IsRingable = json.IsRingable;
        IsSilent = json.IsSilent;
    }
}

