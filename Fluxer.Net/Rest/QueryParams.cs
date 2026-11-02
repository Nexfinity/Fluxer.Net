namespace Fluxer.Net.Rest;

/// <summary>
/// The names of the query parameters the REST client sends.
/// </summary>
public static class QueryParams
{
    /// <summary>
    /// The query parameter for the maximum number of results.
    /// </summary>
    public const string Limit = "limit";

    /// <summary>
    /// The query parameter for the ID to return results after.
    /// </summary>
    public const string After = "after";

    /// <summary>
    /// The query parameter for the ID to return results before.
    /// </summary>
    public const string Before = "before";

    /// <summary>
    /// The query parameter for the ID to return results around.
    /// </summary>
    public const string Around = "around";

    /// <summary>
    /// The query parameter for a user ID.
    /// </summary>
    public const string UserId = "user_id";

    /// <summary>
    /// The query parameter for the audit log action type.
    /// </summary>
    public const string ActionType = "action_type";
}