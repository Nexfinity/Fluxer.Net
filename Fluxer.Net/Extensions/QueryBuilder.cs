using System.Text;

namespace Fluxer.Net.Extensions;

internal class QueryBuilder
{
    private StringBuilder builder;

    public QueryBuilder(string route)
    {
        builder = new StringBuilder(route);
    }

    private bool HasStart = false;

    public QueryBuilder With(string name, bool value)
    {
        if (!value)
            return this;

        builder.Append((HasStart ? "?" : "&") + $"{name}={value}");
        HasStart = true;
        return this;
    }

    public QueryBuilder With(string name, string value)
    {
        if (string.IsNullOrEmpty(value))
            return this;

        builder.Append((HasStart ? "?" : "&") + $"{name}={value}");
        HasStart = true;
        return this;
    }

    public string Build()
    {
        return builder.ToString();
    }
}
