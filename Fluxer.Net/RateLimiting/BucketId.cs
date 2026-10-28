namespace Fluxer.Net.RateLimiting;

/// <summary>
/// Which rate limit bucket a request belongs to. Fluxer keeps one allowance per route and per path resource, so a
/// route is keyed by its method and its path with every id replaced by a placeholder, plus the ids of the channel,
/// guild and webhook it acts on. Once Fluxer names the route's bucket, the name replaces the route.
/// </summary>
public sealed class BucketId : IEquatable<BucketId>
{
    private static readonly HashSet<string> MajorParents = new(StringComparer.Ordinal) { "channels", "guilds", "webhooks" };

    private BucketId(string httpMethod, string endpoint, string majorParameters, string bucketHash, bool isGlobalExempt)
    {
        HttpMethod = httpMethod;
        Endpoint = endpoint;
        MajorParameters = majorParameters;
        BucketHash = bucketHash;
        IsGlobalExempt = isGlobalExempt;
    }

    /// <summary>
    /// Gets the request method, or null for a bucket named by Fluxer.
    /// </summary>
    public string HttpMethod { get; }

    /// <summary>
    /// Gets the route with its ids replaced by placeholders, or null for a bucket named by Fluxer.
    /// </summary>
    public string Endpoint { get; }

    /// <summary>
    /// Gets the channel, guild and webhook ids the request acts on, joined with slashes.
    /// </summary>
    public string MajorParameters { get; }

    /// <summary>
    /// Gets Fluxer's name for the bucket, once a response has carried it.
    /// </summary>
    public string BucketHash { get; }

    /// <summary>
    /// Gets whether the bucket is named by Fluxer.
    /// </summary>
    public bool IsHashBucket => BucketHash != null;

    /// <summary>
    /// Gets whether requests in this bucket are left out of the global limit. Fluxer exempts executing a webhook and
    /// reading, editing or deleting its messages through the webhook token.
    /// </summary>
    public bool IsGlobalExempt { get; }

    /// <summary>
    /// Gets the method and route template that identify the route, whatever bucket it ends up in.
    /// </summary>
    public string RouteKey => HttpMethod != null ? $"{HttpMethod} {Endpoint}" : null;

    /// <summary>
    /// Describes the bucket of a request.
    /// </summary>
    /// <param name="httpMethod">The request method.</param>
    /// <param name="route">The request path, with or without a query string.</param>
    /// <returns>The route's bucket.</returns>
    public static BucketId Create(string httpMethod, string route)
    {
        int query = route.IndexOf('?');
        string path = query >= 0 ? route.Substring(0, query) : route;
        string[] segments = path.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
        List<string> template = new(segments.Length);
        List<string> majors = new();

        for (int i = 0; i < segments.Length; i++)
        {
            string segment = segments[i];
            string parent = i > 0 ? segments[i - 1] : null;
            string grandparent = i > 1 ? segments[i - 2] : null;

            if (parent != null && MajorParents.Contains(parent) && IsId(segment))
            {
                template.Add($"{{{parent}}}");
                majors.Add(segment);
            }
            else if (grandparent == "webhooks" && IsId(parent))
            {
                template.Add("{token}");
                majors.Add(segment);
            }
            else if (parent == "reactions")
            {
                template.Add("{emoji}");
            }
            else if (parent is "invites" or "templates")
            {
                template.Add("{code}");
            }
            else
            {
                template.Add(IsId(segment) ? "{id}" : segment);
            }
        }

        bool webhookToken = segments.Length >= 3 && segments[0] == "webhooks" && IsId(segments[1]);
        return new BucketId(httpMethod.ToUpperInvariant(), "/" + string.Join("/", template), string.Join("/", majors), null,
            webhookToken);
    }

    /// <summary>
    /// Gets the bucket Fluxer named for this route, keeping the same major parameters.
    /// </summary>
    /// <param name="hash">The value of the X-RateLimit-Bucket header.</param>
    /// <returns>The named bucket.</returns>
    public BucketId WithHash(string hash) => new(null, null, MajorParameters, hash, IsGlobalExempt);

    /// <inheritdoc />
    public override string ToString() => IsHashBucket ? $"{BucketHash}:{MajorParameters}" : $"{RouteKey}:{MajorParameters}";

    /// <inheritdoc />
    public bool Equals(BucketId other) => other is not null && ToString() == other.ToString();

    /// <inheritdoc />
    public override bool Equals(object obj) => Equals(obj as BucketId);

    /// <inheritdoc />
    public override int GetHashCode() => StringComparer.Ordinal.GetHashCode(ToString());

    private static bool IsId(string segment)
    {
        if (string.IsNullOrEmpty(segment))
            return false;

        foreach (char c in segment)
        {
            if (c is < '0' or > '9')
                return false;
        }

        return true;
    }
}
