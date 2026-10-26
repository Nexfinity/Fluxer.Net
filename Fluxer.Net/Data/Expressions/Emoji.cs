namespace Fluxer.Net;

/// <inheritdoc />
public class Emoji : Entity, IEmoji
{
    /// <inheritdoc />
    public ulong? Id { get; private set; }

    /// <inheritdoc />
    public DateTimeOffset? CreatedAt => Id.HasValue ? SnowflakeUtils.FromSnowflake(Id.Value) : null;

    /// <inheritdoc />
    public string Name { get; private set; }

    /// <inheritdoc />
    public bool IsAnimated { get; private set; }

    /// <inheritdoc />
    public bool AllowCloning { get; private set; }

    internal string ToRequest()
    {
        if (Id.HasValue)
            return $"{Name}:{Id}";
        return Name;
    }

    /// <inheritdoc />
    public Emoji(string name) : base(null)
    {
        Name = name;
    }

    /// <inheritdoc />
    public Emoji(ulong id, string name, bool isAnimated = false) : base(null)
    {
        Name = name;
        Id = id;
        IsAnimated = isAnimated;
    }

    public static Emoji Parse(string text)
    {
        text = text.Replace("<", "").Replace(">", "");
        bool animated = text.StartsWith("a:");
        if (animated)
            text = text.Substring(1);

        string[] split = text.Split(":");
        if (split.Length == 1)
            return new Emoji(split[0]);
        else if (split.Length == 3 && ulong.TryParse(split[2], out ulong id))
            return new Emoji(id, split[1]);

        throw new FormatException("Invalid emoji");
    }

    public static bool TryParse(string text, out Emoji? result)
    {
        try
        {
            result = Parse(text);
            return true;
        }
        catch { }

        result = null;
        return false;
    }

    internal Emoji(FluxerBaseClient client) : base(client)
    {

    }

    /// <summary>
    /// Create a Emoji object from json.
    /// </summary>
    /// <returns><see cref="Emoji"/></returns>
    public static Emoji Create(FluxerBaseClient client, EmojiJson json)
    {
        Emoji data = new Emoji(client);
        data.Update(json);
        return data;
    }

    internal void Update(EmojiJson json)
    {
        Id = json.Id;
        Name = json.Name;
        IsAnimated = json.IsAnimated;
        AllowCloning = json.AllowCloning;
    }
}
