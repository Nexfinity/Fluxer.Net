using Fluxer.Net.Commands;
using Fluxer.Net.Commands.Attributes;
using Fluxer.Net.Data.Models;
using Fluxer.Net.EmbedBuilder;

namespace Fluxer.Net.Example.Modules;

/// <summary>
/// Example command module demonstrating basic commands.
/// </summary>
public class BasicCommands : ModuleBase
{
	/// <summary>
	/// Simple ping command that responds with "pong".
	/// </summary>
	[Command("ping")]
	[Summary("Check if the bot is responsive")]
	public async Task PingCommand()
	{
		await ReplyAsync("pong ;P");
	}

	/// <summary>
	/// Hello command that mentions the user.
	/// </summary>
	[Command("hello")]
	[Alias("hi", "hey")]
	[Summary("Get a friendly greeting")]
	public async Task HelloCommand()
	{
		await ReplyAsync($"Hello, <@{Context.User.Id}>! 👋");
	}

	/// <summary>
	/// Info command that shows bot information and available commands.
	/// </summary>
	[Command("info")]
	[Summary("Show bot information and available commands")]
	public async Task InfoCommand()
	{
		await ReplyAsync(
			$"**Fluxer.Net Example Bot**\n" +
			$"Version: 0.4.0\n" +
			$"Framework: .NET 7.0\n" +
			$"Library: Fluxer.Net\n\n" +
			$"Available Commands:\n" +
			$"• `/ping` - Check if bot is responsive\n" +
			$"• `/hello` - Get a friendly greeting\n" +
			$"• `/info` - Show this information\n" +
			$"• `/embed` - Show an example rich embed\n" +
			$"• `/echo <message>` - Echo back your message\n" +
			$"• `/add <a> <b>` - Add two numbers"
		);
	}

	/// <summary>
	/// Embed command that demonstrates rich embeds using EmbedBuilder.
	/// </summary>
	[Command("embed")]
	[Summary("Show an example rich embed")]
	public async Task EmbedCommand()
	{
		var embed = new Fluxer.Net.EmbedBuilder.EmbedBuilder()
			.WithTitle("Example Rich Embed")
			.WithDescription("This is a demonstration of Fluxer.Net's EmbedBuilder system, " +
			                 "based on Discord.Net's implementation. Embeds support rich formatting " +
			                 "with titles, descriptions, fields, images, and more!")
			.WithColor(0x5865F2) // Blurple color
			.WithAuthor(
				name: Context.User.Username,
				iconUrl: Context.User.Avatar != null
					? $"https://cdn.fluxer.dev/avatars/{Context.User.Id}/{Context.User.Avatar}.png"
					: null
			)
			.WithThumbnailUrl("https://avatars.githubusercontent.com/u/20194446")
			.AddField("Field 1", "This is an inline field", inline: true)
			.AddField("Field 2", "This is also inline", inline: true)
			.AddField("Field 3", "This is another inline field", inline: true)
			.AddField("Full Width Field", "This field takes up the full width because inline is false", inline: false)
			.AddField("Bot Stats", $"Guilds: 1\nChannels: 5\nUptime: {DateTime.UtcNow:HH:mm:ss}", inline: true)
			.WithFooter("Fluxer.Net v0.4.0", "https://avatars.githubusercontent.com/u/20194446")
			.WithCurrentTimestamp()
			.Build();

		await Context.Client.SendMessage(Context.ChannelId, new()
		{
			Content = "Here's an example of a rich embed:",
			Embeds = new List<Embed> { embed }
		});
	}

	/// <summary>
	/// Echo command that repeats the user's message.
	/// </summary>
	[Command("echo")]
	[Summary("Echo back your message")]
	public async Task EchoCommand([Remainder] string message)
	{
		await ReplyAsync(message);
	}

	/// <summary>
	/// Add command that adds two numbers together.
	/// </summary>
	[Command("add")]
	[Summary("Add two numbers together")]
	public async Task AddCommand(int a, int b)
	{
		await ReplyAsync($"{a} + {b} = {a + b}");
	}

	/// <summary>
	/// Example command with optional parameter.
	/// </summary>
	[Command("greet")]
	[Summary("Greet someone (or yourself)")]
	public async Task GreetCommand(string name = "stranger")
	{
		await ReplyAsync($"Hello, {name}!");
	}
}
