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

    private QueryBuilder Append(string name, string value)
    {
        builder.Append(HasStart ? "&" : "?").Append(name).Append('=').Append(Uri.EscapeDataString(value));
        HasStart = true;
        return this;
    }

    public QueryBuilder With(string name, bool value)
        => value ? Append(name, "true") : this;

    public QueryBuilder WithIf(string name, ulong value, bool check)
        => check ? Append(name, value.ToString()) : this;

    public QueryBuilder With(string name, string value)
        => string.IsNullOrEmpty(value) ? this : Append(name, value);

    public string Build()
    {
        return builder.ToString();
    }
}
