namespace Fluxer.Net.Rest;

public class RequestOptions
{
    /// <summary>
    /// Add audit log reason to this request, only used for certain endpoints.
    /// </summary>
    public string AuditLogReason { get; set; }
}
