using System.Globalization;

namespace Fluxer.Net.Commands;

/// <summary>
///     A <see cref="TypeReader"/> for parsing objects implementing <see cref="IMessage"/>.
/// </summary>
/// <typeparam name="T">The type to be checked; must implement <see cref="IMessage"/>.</typeparam>
public class MessageTypeReader<T> : TypeReader
    where T : class, IMessage
{
    /// <inheritdoc />
    public override async Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider services)
    {
        if (context is not CommandContext ctx)
            return TypeReaderResult.FromError(CommandError.ObjectNotFound, "Message not found.");

        //By Link
        if (Uri.TryCreate(input, UriKind.Absolute, out Uri? messageUrl))
        {
            if (messageUrl.Segments.Length == 5)
            {
                string ChannelId = messageUrl.Segments[3].Substring(0, messageUrl.Segments[3].Length - 1);
                string MessageId = messageUrl.Segments[4];
                if (ulong.TryParse(ChannelId, out ulong chanId))
                {
                    Channel? Channel = ctx.Gateway.GetChannel(chanId);
                    if (Channel == null)
                        return TypeReaderResult.FromError(CommandError.ObjectNotFound, "Channel not found.");

                    if (ulong.TryParse(MessageId, out ulong msgId))
                    {
                        Message? Message = await Channel.GetMessageAsync(msgId);
                        if (Message != null)
                            return TypeReaderResult.FromSuccess(Message);
                    }
                }
            }
        }

        //By Id
        if (ulong.TryParse(input, NumberStyles.None, CultureInfo.InvariantCulture, out ulong id))
        {
            var message = await ctx.Channel.GetMessageAsync(id);
            if (message != null)
                return TypeReaderResult.FromSuccess(message);
        }

        return TypeReaderResult.FromError(CommandError.ObjectNotFound, "Message not found.");
    }
}
