using Fluxer.Net.Rest;

namespace Fluxer.Net;

/// <summary>
/// A builder for creating embed fields with validation.
/// Based on Fluxer.Net's EmbedFieldBuilder implementation.
/// </summary>
public class EmbedFieldBuilder
{
    /// <summary>
    /// Maximum length of the field name.
    /// </summary>
    public static int MaxFieldNameLength { get; } = 256;

    /// <summary>
    /// Maximum length of the field value.
    /// </summary>
    public static int MaxFieldValueLength { get; } = 1024;

    private string? _name;
    private string? _value;

    /// <summary>
    /// Gets or sets the name of the field.
    /// </summary>
    /// <exception cref="ArgumentException">Name length exceeds <see cref="MaxFieldNameLength"/>.</exception>
    public string? Name
    {
        get => _name;
        set
        {
            if (value?.Length > MaxFieldNameLength)
                throw new ArgumentException($"Field name length must be less than or equal to {MaxFieldNameLength}.", nameof(Name));
            _name = value;
        }
    }

    /// <summary>
    /// Gets or sets the value of the field.
    /// </summary>
    /// <exception cref="ArgumentException">Value length exceeds <see cref="MaxFieldValueLength"/>.</exception>
    public string? Value
    {
        get => _value;
        set
        {
            if (value?.Length > MaxFieldValueLength)
                throw new ArgumentException($"Field value length must be less than or equal to {MaxFieldValueLength}.", nameof(Value));
            _value = value;
        }
    }

    /// <summary>
    /// Gets or sets whether the field should be inline.
    /// </summary>
    public bool IsInline { get; set; }

    /// <summary>
    /// Sets the name of the field.
    /// </summary>
    /// <param name="name">The name to set.</param>
    /// <returns>The current builder.</returns>
    public EmbedFieldBuilder WithName(string name)
    {
        Name = name;
        return this;
    }

    /// <summary>
    /// Sets the value of the field.
    /// </summary>
    /// <param name="value">The value to set.</param>
    /// <returns>The current builder.</returns>
    public EmbedFieldBuilder WithValue(object value)
    {
        Value = value?.ToString();
        return this;
    }

    /// <summary>
    /// Sets whether the field should be inline.
    /// </summary>
    /// <param name="isInline">Whether the field should be inline.</param>
    /// <returns>The current builder.</returns>
    public EmbedFieldBuilder WithIsInline(bool isInline)
    {
        IsInline = isInline;
        return this;
    }

    /// <summary>
    /// Builds the embed field.
    /// </summary>
    /// <returns>A new <see cref="EmbedFieldJson"/> object.</returns>
    /// <exception cref="InvalidOperationException">Name or Value is null or empty.</exception>
    public EmbedFieldRequest Build()
    {
        if (string.IsNullOrWhiteSpace(Name))
            throw new InvalidOperationException("Field name cannot be null or empty.");
        if (string.IsNullOrWhiteSpace(Value))
            throw new InvalidOperationException("Field value cannot be null or empty.");

        return new EmbedFieldRequest
        {
            Name = Name,
            Value = Value,
            IsInline = IsInline
        };
    }
}
