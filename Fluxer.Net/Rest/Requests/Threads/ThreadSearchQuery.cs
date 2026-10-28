namespace Fluxer.Net.Rest;

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

    public ThreadSearchSortBy? SortBy { get; set; }

    public SortDirection? SortDirection { get; set; }

    /// <summary>
    /// The number of threads to return, from 1 to 25.
    /// </summary>
    public int? Limit { get; set; }

    public int? Offset { get; set; }

    /// <summary>
    /// Only threads created before this thread.
    /// </summary>
    public ulong? BeforeId { get; set; }

    /// <summary>
    /// Only threads created after this thread.
    /// </summary>
    public ulong? AfterId { get; set; }

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

public enum ForumTagSetting
{
    MatchSome,
    MatchAll
}

public enum ThreadSearchSortBy
{
    LastMessageTime,
    ArchiveTime,
    Relevance,
    CreationTime
}

public enum SortDirection
{
    Ascending,
    Descending
}
