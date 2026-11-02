using System.Globalization;

namespace Fluxer.Net.Commands;

/// <summary>
/// A <see cref="TypeReader"/> that parses an <see cref="IMessage"/> from a message link or an id.
/// </summary>
/// <remarks>
/// The reader finds messages only in a <see cref="CommandContext"/>. An id is looked up in the channel the command was
/// sent in.
/// </remarks>
/// <typeparam name="T">The type to be checked. It must implement <see cref="IMessage"/>.</typeparam>
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
            Message message = await ctx.Channel.GetMessageAsync(id);
            if (message != null)
                return TypeReaderResult.FromSuccess(message);
        }

        return TypeReaderResult.FromError(CommandError.ObjectNotFound, "Message not found.");
    }
}
