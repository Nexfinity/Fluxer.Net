using System.Drawing;
using Newtonsoft.Json;

namespace Fluxer.Net.Objects.Models;

public class UserProfile
{
    [JsonProperty("banner")]
    public string Banner { get; set; }

    [JsonProperty("accent_color")]
    public int? RawAccentColor { get; set; }

    /// <remarks>
    ///     #000 when <see cref="RawAccentColor"/> is null
    /// </remarks> 
    public Color AccentColor
    {
        get => Color.FromArgb(RawAccentColor ?? 0);
        set => RawAccentColor = value.ToArgb();
    }

    [JsonProperty("theme_colors")]
    public List<int>? ThemeColors { get; set; }

    [JsonProperty("biography")]
    public string Biography { get; set; }

    [JsonProperty("location")]
    public string Location { get; set; }

    [JsonProperty("website")]
    public string Website { get; set; }

    [JsonProperty("pronouns")]
    public string Pronouns { get; set; }
}
