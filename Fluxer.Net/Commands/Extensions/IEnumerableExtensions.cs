namespace Fluxer.Net.Commands;

/// <summary>
/// Extension methods for sequences that the command system uses to combine names.
/// </summary>
public static class IEnumerableExtensions
{
    /// <summary>
    /// Combines every element of <paramref name="set"/> with every element of <paramref name="others"/>.
    /// </summary>
    /// <typeparam name="TFirst">The element type of <paramref name="set"/>.</typeparam>
    /// <typeparam name="TSecond">The element type of <paramref name="others"/>.</typeparam>
    /// <typeparam name="TResult">The type returned by <paramref name="func"/>.</typeparam>
    /// <param name="set">The elements that form the first half of each pair.</param>
    /// <param name="others">The elements that form the second half of each pair.</param>
    /// <param name="func">The function that combines a pair.</param>
    /// <returns>The results of <paramref name="func"/> for each pair, in the order of <paramref name="set"/>.</returns>
    public static IEnumerable<TResult> Permutate<TFirst, TSecond, TResult>(
        this IEnumerable<TFirst> set,
        IEnumerable<TSecond> others,
        Func<TFirst, TSecond, TResult> func)
    {
        foreach (TFirst elem in set)
        {
            foreach (TSecond elem2 in others)
            {
                yield return func(elem, elem2);
            }
        }
    }
}
