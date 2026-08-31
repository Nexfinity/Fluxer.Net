using System.Globalization;

namespace Fluxer.Net.Commands;

/// <summary>
///     A <see cref="TypeReader"/> for parsing objects implementing <see cref="IRole"/>.
/// </summary>
/// <typeparam name="T">The type to be checked; must implement <see cref="IRole"/>.</typeparam>
public class RoleTypeReader<T> : TypeReader
    where T : class, IRole
{
    /// <inheritdoc />
    public override Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider services)
    {
        if (context.Guild != null && context is CommandContext ctx)
        {
            Dictionary<ulong, TypeReaderValue> results = new Dictionary<ulong, TypeReaderValue>();

            if (input.Length > 10)
            {
                //By Mention (1.0)
                if (ulong.TryParse(input.Substring(2, input.Length - 3), NumberStyles.None, CultureInfo.InvariantCulture, out ulong id))
                    AddResult(results, ctx.Guild.GetRole(id) as T, 1.00f);

                //By Id (0.9)
                if (ulong.TryParse(input, NumberStyles.None, CultureInfo.InvariantCulture, out id))
                    AddResult(results, ctx.Guild.GetRole(id) as T, 0.90f);
            }

            //By Name (0.7-0.8)
            foreach (SocketRole channel in ctx.Guild.Roles.Values.Where(x => string.Equals(input, x.Name, StringComparison.OrdinalIgnoreCase)))
                AddResult(results, channel as T, channel.Name == input ? 0.80f : 0.70f);

            if (results.Count > 0)
                return Task.FromResult(TypeReaderResult.FromSuccess(results.Values.ToReadOnlyCollection()));
        }
        return Task.FromResult(TypeReaderResult.FromError(CommandError.ObjectNotFound, "Role not found."));
    }

    private void AddResult(Dictionary<ulong, TypeReaderValue> results, T role, float score)
    {
        if (role != null && !results.ContainsKey(role.Id))
            results.Add(role.Id, new TypeReaderValue(role, score));
    }
}
