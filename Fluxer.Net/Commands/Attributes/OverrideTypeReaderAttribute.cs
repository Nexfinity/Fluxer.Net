using System.Reflection;

namespace Fluxer.Net.Commands;

/// <summary>
/// Sets the <see cref="TypeReader"/> that reads a command parameter or property.
/// </summary>
/// <remarks>
/// The <see cref="TypeReader"/> applies only to the marked parameter or property. Other commands that read the same
/// type keep their own <see cref="TypeReader"/>.
/// <note type="warning">
/// If the given type reader does not inherit from <see cref="TypeReader"/>, an <see cref="ArgumentException"/> is
/// thrown.
/// </note>
/// </remarks>
/// <example>
/// In this example, the <see cref="TimeSpan"/> is read by a custom <see cref="TypeReader"/>,
/// <c>FriendlyTimeSpanTypeReader</c>, instead of the default <see cref="TimeSpan"/> reader.
/// <code language="cs">
/// [Command("time")]
/// public Task GetTimeAsync([OverrideTypeReader(typeof(FriendlyTimeSpanTypeReader))]TimeSpan time)
///     => ReplyAsync(time);
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class OverrideTypeReaderAttribute : Attribute
{
    private static readonly TypeInfo TypeReaderTypeInfo = typeof(TypeReader).GetTypeInfo();

    /// <summary>
    /// The type of the <see cref="TypeReader"/> that reads the parameter or property.
    /// </summary>
    public Type TypeReader { get; }

    /// <summary>
    /// Creates a new <see cref="OverrideTypeReaderAttribute"/> with the given type reader.
    /// </summary>
    /// <param name="overridenTypeReader">The <see cref="TypeReader"/> type to use.</param>
    /// <exception cref="ArgumentException">The given <paramref name="overridenTypeReader"/> does not inherit from <see cref="TypeReader"/>.</exception>
    public OverrideTypeReaderAttribute(Type overridenTypeReader)
    {
        if (!TypeReaderTypeInfo.IsAssignableFrom(overridenTypeReader.GetTypeInfo()))
            throw new ArgumentException($"{nameof(overridenTypeReader)} must inherit from {nameof(TypeReader)}.");

        TypeReader = overridenTypeReader;
    }
}
