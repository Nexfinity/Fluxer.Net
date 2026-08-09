namespace Fluxer.Net;

/// <summary>
/// Represents an optional value.
/// </summary>
public struct Optional<T>
{
    private readonly bool hasValue;
    private readonly T value;

    /// <summary>
    /// Checks if a value is present.
    /// </summary>
    public bool HasValue => hasValue;

    public T Value => value;

    internal Optional(T value, bool hasValue)
    {
        this.value = value;
        this.hasValue = hasValue;
    }

    /// <summary>
    /// Returns the existing value if present, and otherwise an alternative value.
    /// </summary>
    /// <param name="alternative">The alternative value.</param>
    /// <returns>The existing or alternative value.</returns>
    public T ValueOr(T alternative) => hasValue ? value : alternative;
}

/// <summary>
/// Provides a set of functions for creating optional values.
/// </summary>
public static class Optional
{
    /// <summary>
    /// Wraps an existing value in an Option&lt;T&gt; instance.
    /// </summary>
    /// <param name="value">The value to be wrapped.</param>
    /// <returns>An optional containing the specified value.</returns>
    public static Optional<T> Some<T>(T value) => new Optional<T>(value, true);


    /// <summary>
    /// Creates an empty Option&lt;T&gt; instance.
    /// </summary>
    /// <returns>An empty optional.</returns>
    public static Optional<T> None<T>() => new Optional<T>(default(T), false);
}