using System.Globalization;

namespace Fluxer.Net.Commands;

/// <summary>
/// A <see cref="TypeReader"/> that parses an <see cref="IChannel"/> from a mention, an id or a name.
/// </summary>
/// <remarks>
/// The reader is used by default for <see cref="IChannel"/> parameters. Matches are scored: a mention scores 1.0, an
/// id 0.9, an exact name 0.8 and a name that differs only in case 0.7. When several channels match, all of them are
/// returned. Mentions and ids are read only from input longer than 10 characters, and names are matched only in a
/// <see cref="CommandContext"/>. Outside a guild, the reader returns an <see cref="CommandError.ObjectNotFound"/> error.
/// </remarks>
/// <typeparam name="T">The type to be checked. It must implement <see cref="IChannel"/>.</typeparam>
public class ChannelTypeReader<T> : TypeReader
    where T : class, IChannel
{
    /// <inheritdoc />
    public override async Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider services)
    {
        if (context.Guild != null)
        {
            Dictionary<ulong, TypeReaderValue> results = new Dictionary<ulong, TypeReaderValue>();

            if (input.Length > 10)
            {
                //By Mention (1.0)
                if (ulong.TryParse(input.Substring(2, input.Length - 3), NumberStyles.None, CultureInfo.InvariantCulture, out ulong id))
                    AddResult(results, context.Gateway.GetChannel(id), 1.00f);

                //By Id (0.9)
                if (ulong.TryParse(input, NumberStyles.None, CultureInfo.InvariantCulture, out id))
                    AddResult(results, context.Gateway.GetChannel(id), 0.90f);
            }

            //By Name (0.7-0.8)
            if (context is CommandContext ctx)
            {
                foreach (Channel channel in ctx.Guild.Channels.Values.Where(x => string.Equals(input, x.Name, StringComparison.OrdinalIgnoreCase)))
                    AddResult(results, channel, channel.Name == input ? 0.80f : 0.70f);
            }

            if (results.Count > 0)
                return TypeReaderResult.FromSuccess(results.Values.ToReadOnlyCollection());
        }

        return TypeReaderResult.FromError(CommandError.ObjectNotFound, "Channel not found.");
    }

    internal static bool Accepts(IChannel? channel) => channel is T;

    private static void AddResult(Dictionary<ulong, TypeReaderValue> results, IChannel? channel, float score)
    {
        if (channel is T typed && !results.ContainsKey(channel.Id))
            results.Add(channel.Id, new TypeReaderValue(typed, score));
    }
}
