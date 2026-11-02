namespace Fluxer.Net;

/// <summary>
/// A value that is either present or absent, for request properties that can be left unset.
/// </summary>
/// <typeparam name="T">The type of the wrapped value.</typeparam>
public struct Optional<T>
{
    private readonly bool hasValue;
    private readonly T value;

    /// <summary>
    /// Whether a value is present.
    /// </summary>
    public bool HasValue => hasValue;

    /// <summary>
    /// The wrapped value, or the default value of <typeparamref name="T"/> when no value is present.
    /// </summary>
    public T Value => value;

    internal Optional(T value, bool hasValue)
    {
        this.value = value;
        this.hasValue = hasValue;
    }

    /// <summary>
    /// Returns the wrapped value if present, and otherwise the alternative value.
    /// </summary>
    /// <param name="alternative">The value returned when no value is present.</param>
    /// <returns>The wrapped or alternative value.</returns>
    public T ValueOr(T alternative) => hasValue ? value : alternative;

    /// <summary>
    /// Wraps a value as a present optional, so request properties can be assigned the value directly.
    /// </summary>
    /// <param name="value">The value to wrap.</param>
    public static implicit operator Optional<T>(T value) => new Optional<T>(value, true);
}

/// <summary>
/// Creates values of the <c>Optional&lt;T&gt;</c> type.
/// </summary>
public static class Optional
{
    /// <summary>
    /// Creates an optional that holds the value.
    /// </summary>
    /// <param name="value">The value to wrap.</param>
    /// <returns>An optional containing the specified value.</returns>
    public static Optional<T> Some<T>(T value) => new Optional<T>(value, true);


    /// <summary>
    /// Creates an optional with no value.
    /// </summary>
    /// <returns>An empty optional.</returns>
    public static Optional<T> None<T>() => new Optional<T>(default(T), false);
}