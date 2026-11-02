namespace Fluxer.Net.Rest;

/// <summary>
/// Filters and paging options for searching the threads of a channel.
/// </summary>
public class ThreadSearchQuery
{
    /// <summary>
    /// Text to look for in thread names, up to 100 characters.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Forum tags the threads must have, up to 20.
    /// </summary>
    public IEnumerable<ulong>? TagIds { get; set; }

    /// <summary>
    /// Whether a thread needs one of <see cref="TagIds"/> or all of them.
    /// </summary>
    public ForumTagSetting? TagSetting { get; set; }

    /// <summary>
    /// True for archived threads only, false for active threads only, or null for both.
    /// </summary>
    public bool? IsArchived { get; set; }

    /// <summary>
    /// The field to sort the threads by.
    /// </summary>
    public ThreadSearchSortBy? SortBy { get; set; }

    /// <summary>
    /// The order to sort the threads in.
    /// </summary>
    public SortDirection? SortDirection { get; set; }

    /// <summary>
    /// The number of threads to return, from 1 to 25.
    /// </summary>
    public int? Limit { get; set; }

    /// <summary>
    /// The number of threads to skip before the results start.
    /// </summary>
    public int? Offset { get; set; }

    /// <summary>
    /// Only threads created before this thread.
    /// </summary>
    public ulong? BeforeId { get; set; }

    /// <summary>
    /// Only threads created after this thread.
    /// </summary>
    public ulong? AfterId { get; set; }

    /// <summary>
    /// Builds the query string for the thread search from the filters that are set.
    /// </summary>
    public string BuildQuery()
    {
        List<string> list = new List<string>();
        if (!string.IsNullOrEmpty(Name))
            list.Add("name=" + Uri.EscapeDataString(Name));
        if (TagIds != null)
            list.AddRange(TagIds.Select(x => $"tag={x}"));
        if (TagSetting.HasValue)
            list.Add("tag_setting=" + (TagSetting.Value == ForumTagSetting.MatchAll ? "match_all" : "match_some"));
        if (IsArchived.HasValue)
            list.Add("archived=" + (IsArchived.Value ? "true" : "false"));
        if (SortBy.HasValue)
        {
            list.Add("sort_by=" + SortBy.Value switch
            {
                ThreadSearchSortBy.ArchiveTime => "archive_time",
                ThreadSearchSortBy.Relevance => "relevance",
                ThreadSearchSortBy.CreationTime => "creation_time",
                _ => "last_message_time"
            });
        }
        if (SortDirection.HasValue)
            list.Add("sort_order=" + (SortDirection.Value == Rest.SortDirection.Ascending ? "asc" : "desc"));
        if (Limit.HasValue)
            list.Add($"limit={Limit.Value}");
        if (Offset.HasValue)
            list.Add($"offset={Offset.Value}");
        if (BeforeId.HasValue)
            list.Add($"max_id={BeforeId.Value}");
        if (AfterId.HasValue)
            list.Add($"min_id={AfterId.Value}");
        return string.Join("&", list);
    }
}

/// <summary>
/// How a thread's forum tags are matched against the tags in a thread search.
/// </summary>
public enum ForumTagSetting
{
    /// <summary>
    /// A thread needs at least one of the tags.
    /// </summary>
    MatchSome,

    /// <summary>
    /// A thread needs all of the tags.
    /// </summary>
    MatchAll
}

/// <summary>
/// The field that thread search results are sorted by.
/// </summary>
public enum ThreadSearchSortBy
{
    /// <summary>
    /// Sort by the time of the last message.
    /// </summary>
    LastMessageTime,

    /// <summary>
    /// Sort by the time the thread was archived.
    /// </summary>
    ArchiveTime,

    /// <summary>
    /// Sort by how closely the thread matches the search.
    /// </summary>
    Relevance,

    /// <summary>
    /// Sort by the time the thread was created.
    /// </summary>
    CreationTime
}

/// <summary>
/// The order that thread search results are sorted in.
/// </summary>
public enum SortDirection
{
    /// <summary>
    /// Sort from the lowest value to the highest.
    /// </summary>
    Ascending,

    /// <summary>
    /// Sort from the highest value to the lowest.
    /// </summary>
    Descending
}
