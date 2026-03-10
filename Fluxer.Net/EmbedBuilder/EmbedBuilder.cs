using Fluxer.Net.Data.Messages;
using Fluxer.Net.Data.Models;

namespace Fluxer.Net.EmbedBuilder;

/// <summary>
/// A builder for creating rich embeds with validation and a fluent API.
/// Based on Discord.Net's EmbedBuilder implementation.
/// </summary>
public class EmbedBuilder
{
	/// <summary>
	/// Maximum number of fields allowed in an embed.
	/// </summary>
	public const int MaxFieldCount = 25;

	/// <summary>
	/// Maximum length of the title.
	/// </summary>
	public const int MaxTitleLength = 256;

	/// <summary>
	/// Maximum length of the description.
	/// </summary>
	public const int MaxDescriptionLength = 4096;

	/// <summary>
	/// Maximum total length of all embed content.
	/// </summary>
	public const int MaxEmbedLength = 6000;

	private string? _title;
	private string? _description;
	private string? _url;
	private string? _imageUrl;
	private string? _thumbnailUrl;
	private int? _color;
	private DateTime? _timestamp;

	/// <summary>
	/// Gets or sets the title of the embed.
	/// </summary>
	/// <exception cref="ArgumentException">Title length exceeds <see cref="MaxTitleLength"/>.</exception>
	public string? Title
	{
		get => _title;
		set
		{
			if (value?.Length > MaxTitleLength)
				throw new ArgumentException($"Title length must be less than or equal to {MaxTitleLength}.", nameof(Title));
			_title = value;
		}
	}

	/// <summary>
	/// Gets or sets the description of the embed.
	/// </summary>
	/// <exception cref="ArgumentException">Description length exceeds <see cref="MaxDescriptionLength"/>.</exception>
	public string? Description
	{
		get => _description;
		set
		{
			if (value?.Length > MaxDescriptionLength)
				throw new ArgumentException($"Description length must be less than or equal to {MaxDescriptionLength}.", nameof(Description));
			_description = value;
		}
	}

	/// <summary>
	/// Gets or sets the URL of the embed.
	/// </summary>
	public string? Url
	{
		get => _url;
		set => _url = value;
	}

	/// <summary>
	/// Gets or sets the thumbnail URL of the embed.
	/// </summary>
	public string? ThumbnailUrl
	{
		get => _thumbnailUrl;
		set => _thumbnailUrl = value;
	}

	/// <summary>
	/// Gets or sets the image URL of the embed.
	/// </summary>
	public string? ImageUrl
	{
		get => _imageUrl;
		set => _imageUrl = value;
	}

	/// <summary>
	/// Gets or sets the color of the embed.
	/// </summary>
	public int? Color
	{
		get => _color;
		set => _color = value;
	}

	/// <summary>
	/// Gets or sets the timestamp of the embed.
	/// </summary>
	public DateTime? Timestamp
	{
		get => _timestamp;
		set => _timestamp = value;
	}

	/// <summary>
	/// Gets or sets the author field of the embed.
	/// </summary>
	public EmbedAuthorBuilder? Author { get; set; }

	/// <summary>
	/// Gets or sets the footer field of the embed.
	/// </summary>
	public EmbedFooterBuilder? Footer { get; set; }

	/// <summary>
	/// Gets the list of field builders for the embed.
	/// </summary>
	public List<EmbedFieldBuilder> Fields { get; set; } = new List<EmbedFieldBuilder>();

	/// <summary>
	/// Gets the total length of all embed content.
	/// </summary>
	public int Length
	{
		get
		{
			int length = Title?.Length ?? 0;
			length += Description?.Length ?? 0;
			length += Author?.Name?.Length ?? 0;
			length += Footer?.Text?.Length ?? 0;

			foreach (var field in Fields)
			{
				length += field.Name?.Length ?? 0;
				length += field.Value?.Length ?? 0;
			}

			return length;
		}
	}

	/// <summary>
	/// Sets the title of the embed.
	/// </summary>
	/// <param name="title">The title to set.</param>
	/// <returns>The current builder.</returns>
	public EmbedBuilder WithTitle(string? title)
	{
		Title = title;
		return this;
	}

	/// <summary>
	/// Sets the description of the embed.
	/// </summary>
	/// <param name="description">The description to set.</param>
	/// <returns>The current builder.</returns>
	public EmbedBuilder WithDescription(string? description)
	{
		Description = description;
		return this;
	}

	/// <summary>
	/// Sets the URL of the embed.
	/// </summary>
	/// <param name="url">The URL to set.</param>
	/// <returns>The current builder.</returns>
	public EmbedBuilder WithUrl(string? url)
	{
		Url = url;
		return this;
	}

	/// <summary>
	/// Sets the thumbnail URL of the embed.
	/// </summary>
	/// <param name="thumbnailUrl">The thumbnail URL to set.</param>
	/// <returns>The current builder.</returns>
	public EmbedBuilder WithThumbnailUrl(string? thumbnailUrl)
	{
		ThumbnailUrl = thumbnailUrl;
		return this;
	}

	/// <summary>
	/// Sets the image URL of the embed.
	/// </summary>
	/// <param name="imageUrl">The image URL to set.</param>
	/// <returns>The current builder.</returns>
	public EmbedBuilder WithImageUrl(string? imageUrl)
	{
		ImageUrl = imageUrl;
		return this;
	}

	/// <summary>
	/// Sets the color of the embed.
	/// </summary>
	/// <param name="color">The color to set as an integer (RGB).</param>
	/// <returns>The current builder.</returns>
	public EmbedBuilder WithColor(int color)
	{
		Color = color;
		return this;
	}

	/// <summary>
	/// Sets the color of the embed using RGB values.
	/// </summary>
	/// <param name="r">Red component (0-255).</param>
	/// <param name="g">Green component (0-255).</param>
	/// <param name="b">Blue component (0-255).</param>
	/// <returns>The current builder.</returns>
	public EmbedBuilder WithColor(byte r, byte g, byte b)
	{
		Color = (r << 16) | (g << 8) | b;
		return this;
	}

	/// <summary>
	/// Sets the timestamp of the embed.
	/// </summary>
	/// <param name="timestamp">The timestamp to set.</param>
	/// <returns>The current builder.</returns>
	public EmbedBuilder WithTimestamp(DateTime timestamp)
	{
		Timestamp = timestamp;
		return this;
	}

	/// <summary>
	/// Sets the timestamp of the embed to the current UTC time.
	/// </summary>
	/// <returns>The current builder.</returns>
	public EmbedBuilder WithCurrentTimestamp()
	{
		Timestamp = DateTime.UtcNow;
		return this;
	}

	/// <summary>
	/// Sets the author field of the embed.
	/// </summary>
	/// <param name="author">The author builder to set.</param>
	/// <returns>The current builder.</returns>
	public EmbedBuilder WithAuthor(EmbedAuthorBuilder author)
	{
		Author = author;
		return this;
	}

	/// <summary>
	/// Sets the author field of the embed.
	/// </summary>
	/// <param name="name">The name of the author.</param>
	/// <param name="iconUrl">The icon URL of the author.</param>
	/// <param name="url">The URL of the author.</param>
	/// <returns>The current builder.</returns>
	public EmbedBuilder WithAuthor(string name, string? iconUrl = null, string? url = null)
	{
		var author = new EmbedAuthorBuilder
		{
			Name = name,
			IconUrl = iconUrl,
			Url = url
		};
		Author = author;
		return this;
	}

	/// <summary>
	/// Sets the author field of the embed using an action.
	/// </summary>
	/// <param name="action">The action to configure the author builder.</param>
	/// <returns>The current builder.</returns>
	public EmbedBuilder WithAuthor(Action<EmbedAuthorBuilder> action)
	{
		var author = new EmbedAuthorBuilder();
		action(author);
		Author = author;
		return this;
	}

	/// <summary>
	/// Sets the footer field of the embed.
	/// </summary>
	/// <param name="footer">The footer builder to set.</param>
	/// <returns>The current builder.</returns>
	public EmbedBuilder WithFooter(EmbedFooterBuilder footer)
	{
		Footer = footer;
		return this;
	}

	/// <summary>
	/// Sets the footer field of the embed.
	/// </summary>
	/// <param name="text">The text of the footer.</param>
	/// <param name="iconUrl">The icon URL of the footer.</param>
	/// <returns>The current builder.</returns>
	public EmbedBuilder WithFooter(string text, string? iconUrl = null)
	{
		var footer = new EmbedFooterBuilder
		{
			Text = text,
			IconUrl = iconUrl
		};
		Footer = footer;
		return this;
	}

	/// <summary>
	/// Sets the footer field of the embed using an action.
	/// </summary>
	/// <param name="action">The action to configure the footer builder.</param>
	/// <returns>The current builder.</returns>
	public EmbedBuilder WithFooter(Action<EmbedFooterBuilder> action)
	{
		var footer = new EmbedFooterBuilder();
		action(footer);
		Footer = footer;
		return this;
	}

	/// <summary>
	/// Adds a field to the embed.
	/// </summary>
	/// <param name="field">The field builder to add.</param>
	/// <returns>The current builder.</returns>
	/// <exception cref="ArgumentException">Field count exceeds <see cref="MaxFieldCount"/>.</exception>
	public EmbedBuilder AddField(EmbedFieldBuilder field)
	{
		if (Fields.Count >= MaxFieldCount)
			throw new ArgumentException($"Field count must be less than or equal to {MaxFieldCount}.", nameof(field));

		Fields.Add(field);
		return this;
	}

	/// <summary>
	/// Adds a field to the embed.
	/// </summary>
	/// <param name="name">The name of the field.</param>
	/// <param name="value">The value of the field.</param>
	/// <param name="inline">Whether the field should be inline.</param>
	/// <returns>The current builder.</returns>
	/// <exception cref="ArgumentException">Field count exceeds <see cref="MaxFieldCount"/>.</exception>
	public EmbedBuilder AddField(string name, object value, bool inline = false)
	{
		var field = new EmbedFieldBuilder
		{
			Name = name,
			Value = value?.ToString() ?? "",
			IsInline = inline
		};
		return AddField(field);
	}

	/// <summary>
	/// Adds a field to the embed using an action.
	/// </summary>
	/// <param name="action">The action to configure the field builder.</param>
	/// <returns>The current builder.</returns>
	/// <exception cref="ArgumentException">Field count exceeds <see cref="MaxFieldCount"/>.</exception>
	public EmbedBuilder AddField(Action<EmbedFieldBuilder> action)
	{
		var field = new EmbedFieldBuilder();
		action(field);
		return AddField(field);
	}

	/// <summary>
	/// Builds the embed.
	/// </summary>
	/// <returns>A new <see cref="Embed"/> object.</returns>
	/// <exception cref="InvalidOperationException">The embed exceeds <see cref="MaxEmbedLength"/>.</exception>
	public Embed Build()
	{
		// Validate total length
		if (Length > MaxEmbedLength)
			throw new InvalidOperationException($"Total embed length must be less than or equal to {MaxEmbedLength}.");

		// Validate URLs
		if (!string.IsNullOrEmpty(Url) && !IsValidUrl(Url))
			throw new InvalidOperationException("Embed URL must include a protocol (http:// or https://).");
		if (!string.IsNullOrEmpty(ThumbnailUrl) && !IsValidUrl(ThumbnailUrl))
			throw new InvalidOperationException("Thumbnail URL must include a protocol (http:// or https://).");
		if (!string.IsNullOrEmpty(ImageUrl) && !IsValidUrl(ImageUrl))
			throw new InvalidOperationException("Image URL must include a protocol (http:// or https://).");

		var embed = new Embed
		{
			Type = "rich",
			Title = Title,
			Description = Description,
			Url = Url,
			Color = Color,
			Timestamp = Timestamp,
			Author = Author?.Build(),
			Footer = Footer?.Build(),
			Fields = Fields.Count > 0 ? Fields.Select(f => f.Build()).ToList() : null
		};

		// Set thumbnail
		if (!string.IsNullOrEmpty(ThumbnailUrl))
		{
			embed.Thumbnail = new EmbedMedia { Url = ThumbnailUrl };
		}

		// Set image
		if (!string.IsNullOrEmpty(ImageUrl))
		{
			embed.Image = new EmbedMedia { Url = ImageUrl };
		}

		return embed;
	}

	private static bool IsValidUrl(string url)
	{
		return url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
		       url.StartsWith("https://", StringComparison.OrdinalIgnoreCase) ||
		       url.StartsWith("attachment://", StringComparison.OrdinalIgnoreCase);
	}
}
