namespace Fluxer.Net.Commands.Attributes;

/// <summary>
/// Provides additional remarks or detailed information for a command.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public class RemarksAttribute : Attribute
{
	/// <summary>
	/// Gets the remarks text.
	/// </summary>
	public string Text { get; }

	/// <summary>
	/// Provides additional remarks for a command.
	/// </summary>
	/// <param name="text">The remarks text.</param>
	public RemarksAttribute(string text)
	{
		Text = text;
	}
}
