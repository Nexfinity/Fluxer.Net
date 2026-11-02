using System.Collections.Immutable;
using System.Globalization;

namespace Fluxer.Net.Commands;

/// <summary>
/// A <see cref="TypeReader"/> that parses an <see cref="IUser"/> from a mention or an id.
/// </summary>
/// <remarks>
/// The user is looked up as a member of the guild, so the reader finds users only in a guild and in a
/// <see cref="CommandContext"/>. Input of 10 characters or fewer is not read.
/// </remarks>
/// <typeparam name="T">The type to be checked. It must implement <see cref="IUser"/>.</typeparam>
public class UserTypeReader<T> : TypeReader
    where T : class, IUser
{
    /// <inheritdoc />
    public override async Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider services)
    {
        var results = new Dictionary<ulong, TypeReaderValue>();
        if (context is CommandContext ctx)
        {
            if (input.Length > 10 && ctx.Guild != null)
            {
                //By Mention (1.0)
                if (ulong.TryParse(input.Substring(2, input.Length - 3), NumberStyles.None, CultureInfo.InvariantCulture, out ulong id))
                    AddResult(results, (await ctx.Guild.GetMemberAsync(id))?.User as T, 1.00f);

                //By Id (0.9)
                if (ulong.TryParse(input, NumberStyles.None, CultureInfo.InvariantCulture, out id))
                    AddResult(results, (await ctx.Guild.GetMemberAsync(id))?.User as T, 0.90f);
            }

            if (results.Count > 0)
                return TypeReaderResult.FromSuccess(results.Values.ToImmutableArray());
        }


        return TypeReaderResult.FromError(CommandError.ObjectNotFound, "User not found.");
    }

    private void AddResult(Dictionary<ulong, TypeReaderValue> results, T user, float score)
    {
        if (user != null && !results.ContainsKey(user.Id))
            results.Add(user.Id, new TypeReaderValue(user, score));
    }
}