using System.Collections;

namespace Fluxer.Net;

internal static class CollectionExtensions
{
    public static IReadOnlyCollection<TValue> ToReadOnlyCollection<TValue>(this ICollection<TValue> source)
        => new CollectionWrapper<TValue>(source, () => source.Count);
}
internal struct CollectionWrapper<TValue> : IReadOnlyCollection<TValue>
{
    private readonly IEnumerable<TValue> _query;
    private readonly Func<int> _countFunc;

    //It's okay that this count is affected by race conditions - we're wrapping a concurrent collection and that's to be expected
    public int Count => _countFunc();

    public CollectionWrapper(IEnumerable<TValue> query, Func<int> countFunc)
    {
        _query = query;
        _countFunc = countFunc;
    }

    private string DebuggerDisplay => $"Count = {Count}";

    public IEnumerator<TValue> GetEnumerator() => _query.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => _query.GetEnumerator();
}