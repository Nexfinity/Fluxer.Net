using System.Globalization;

namespace Fluxer.Net;

/// <summary>
/// An RGB color, as used by embeds and roles.
/// </summary>
public readonly struct Color : IEquatable<Color>
{
    public const uint MaxDecimalValue = 0xFFFFFF;

    public static readonly Color Default = new(0);

    public static readonly Color Teal = new(0x1ABC9C);

    public static readonly Color DarkTeal = new(0x11806A);

    public static readonly Color Green = new(0x2ECC71);

    public static readonly Color DarkGreen = new(0x1F8B4C);

    public static readonly Color Blue = new(0x3498DB);

    public static readonly Color DarkBlue = new(0x206694);

    public static readonly Color Purple = new(0x9B59B6);

    public static readonly Color DarkPurple = new(0x71368A);

    public static readonly Color Magenta = new(0xE91E63);

    public static readonly Color DarkMagenta = new(0xAD1457);

    public static readonly Color Gold = new(0xF1C40F);

    public static readonly Color LightOrange = new(0xC27C0E);

    public static readonly Color Orange = new(0xE67E22);

    public static readonly Color DarkOrange = new(0xA84300);

    public static readonly Color Red = new(0xE74C3C);

    public static readonly Color DarkRed = new(0x992D22);

    public static readonly Color LightGrey = new(0x979C9F);

    public static readonly Color LighterGrey = new(0x95A5A6);

    public static readonly Color DarkGrey = new(0x607D8B);

    public static readonly Color DarkerGrey = new(0x546E7A);

    /// <summary>
    /// Creates a color from a raw 0xRRGGBB value.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">The value is larger than <see cref="MaxDecimalValue" />.</exception>
    public Color(uint rawValue)
    {
        if (rawValue > MaxDecimalValue)
            throw new ArgumentOutOfRangeException(nameof(rawValue), $"A color cannot be larger than 0x{MaxDecimalValue:X6}.");
        RawValue = rawValue;
    }

    /// <summary>
    /// Creates a color from its red, green and blue parts.
    /// </summary>
    public Color(byte r, byte g, byte b)
    {
        RawValue = ((uint)r << 16) | ((uint)g << 8) | b;
    }

    /// <summary>
    /// Creates a color from its red, green and blue parts, each from 0 to 255.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">A part is outside 0 to 255.</exception>
    public Color(int r, int g, int b) : this(ToByte(r, nameof(r)), ToByte(g, nameof(g)), ToByte(b, nameof(b)))
    {
    }

    /// <summary>
    /// Creates a color from its red, green and blue parts, each from 0 to 1.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">A part is outside 0 to 1.</exception>
    public Color(float r, float g, float b) : this(ToByte(r, nameof(r)), ToByte(g, nameof(g)), ToByte(b, nameof(b)))
    {
    }

    /// <summary>
    /// The raw 0xRRGGBB value.
    /// </summary>
    public uint RawValue { get; }

    /// <summary>
    /// The red part, from 0 to 255.
    /// </summary>
    public byte R => (byte)(RawValue >> 16);

    /// <summary>
    /// The green part, from 0 to 255.
    /// </summary>
    public byte G => (byte)(RawValue >> 8);

    /// <summary>
    /// The blue part, from 0 to 255.
    /// </summary>
    public byte B => (byte)RawValue;

    /// <summary>
    /// Reads a color written as hex, with or without a leading # or 0x.
    /// </summary>
    public static bool TryParse(string? text, out Color color)
    {
        color = Default;
        if (string.IsNullOrWhiteSpace(text))
            return false;

        string hex = text!.Trim();
        if (hex.StartsWith("#", StringComparison.Ordinal))
            hex = hex.Substring(1);
        else if (hex.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            hex = hex.Substring(2);

        if (hex.Length != 6 || !uint.TryParse(hex, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out uint raw))
            return false;

        color = new Color(raw);
        return true;
    }

    /// <summary>
    /// Reads a color written as hex, with or without a leading # or 0x.
    /// </summary>
    /// <exception cref="FormatException">The text is not a six digit hex color.</exception>
    public static Color Parse(string text) =>
        TryParse(text, out Color color) ? color : throw new FormatException($"'{text}' is not a hex color.");

    /// <summary>
    /// Returns the color as #RRGGBB.
    /// </summary>
    public override string ToString() => $"#{RawValue:X6}";

    /// <inheritdoc />
    public bool Equals(Color other) => RawValue == other.RawValue;

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is Color other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() => RawValue.GetHashCode();

    public static bool operator ==(Color left, Color right) => left.Equals(right);

    public static bool operator !=(Color left, Color right) => !left.Equals(right);

    public static implicit operator uint(Color color) => color.RawValue;

    public static implicit operator int(Color color) => (int)color.RawValue;

    public static explicit operator Color(uint rawValue) => new(rawValue);

    private static byte ToByte(int value, string name)
    {
        if (value is < 0 or > 255)
            throw new ArgumentOutOfRangeException(name, "A color part must be from 0 to 255.");
        return (byte)value;
    }

    private static byte ToByte(float value, string name)
    {
        if (value is < 0f or > 1f)
            throw new ArgumentOutOfRangeException(name, "A color part must be from 0 to 1.");
        return (byte)Math.Round(value * 255f);
    }
}
