using System;
using Eris.Serilog.Formatting.Json;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using Squll.Net;
using Squll.Net.Extensions;
using Squll.Net.Objects;

var v2Client = new SqullConnectionV2("**redacted**", new()
{
    ReconnectAttemptDelay = 2,
    SerilogConfig = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Console(theme: AnsiConsoleTheme.Code)
                .WriteTo.File(new SerilogJsonFormatter(), $"output-{DateTime.Now:yyyy-dd-mm:hh-mm-ss}.log", rollingInterval: RollingInterval.Infinite, rollOnFileSizeLimit: true)
});
// await v2Client.LeaveSquad(72070065598038016);
await v2Client.ConnectAsync();

v2Client.MessageCreate += async x =>
{
    if (x.Content == "/ping")
        await v2Client.SendMessage(x.SpaceId, new()
        {
            Content = "pong ;P",
            MentionUsers = new ulong[1] { x.AuthorId }
        });
};

await Task.Delay(-1);



// var client = new SqullConnection("**redacted**");
// Squad[] squads = Array.Empty<Squad>();
// client.Ready += x =>
// {
//     squads = x.Squads;
// };
// client.MessageCreated += async x =>
// {
//     Console.ForegroundColor = ConsoleColor.Green;
//     Console.WriteLine($"{x.Author.DisplayName}#{x.Author.Discriminator}: {x.Content}");

//     if (x.Content.StartsWith("/"))
//     {
// #pragma warning disable CS8509
//         var command = x.Content[1..].Split(' ').First();
//         try
//         {
//             await (command switch
//             {
//                 "say" => Say(x),
//                 "join" => Join(x),
//                 "help" => Help(x),
//                 "dev-status" => Status(x),
//             });
//         }
//         catch (SqullApiException ex)
//         {
//             await client.SendMessage(x.SpaceId, new()
//             {
//                 Content = $"# Error\n\n{ex.Message}\n\n```\n{ex.SqullData}\n```",
//                 Nonce = $"ex"
//             });
//         }
//         catch (Exception ex)
//         {
//             await client.SendMessage(x.SpaceId, new()
//             {
//                 Content = $"# Error\n\n{ex.Message}\n\n",
//                 Nonce = $"ex"
//             });
//         }

// #pragma warning restore CS8509
//     }
// };

// async Task Say(Message msg)
// {
//     var toSend = msg.Content[4..];
//     await client!.SendMessage(msg.SpaceId, new()
//     {
//         Content = toSend,
//         Nonce = $"c:1:{msg.Id}"
//     });
// }

// async Task Join(Message msg)
// {
//     var invite = msg.Content[5..].Trim();
//     await client!.JoinSquad(invite);
//     await client.SendMessage(msg.SpaceId, new()
//     {
//         Content = "Joined the squad!",
//         Nonce = $"c:2:{msg.Id}"
//     });
// }

// async Task Help(Message msg)
// {
//     await client!.SendMessage(msg.SpaceId, new()
//     {
//         Content = File.ReadAllText("Help.md"),
//         Nonce = $"c:3:{msg.Id}"
//     });
// }

// async Task Status(Message msg)
// {
//     if (msg.Author.Id is not 72076505658228736 or 72068936915025920)
//         throw new AccessViolationException("You are not the bots developer.");
//     client!.SetStatus(msg.Content[11..].Trim());
//     await client!.SendMessage(msg.SpaceId, new()
//     {
//         Content = $"set status to `{msg.Content[11..].Trim()}`",
//         Nonce = $"c:3:{msg.Id}"
//     });
// }


// await client.ConnectToGateway();

// var squad = await client.GetSquad(72078685781950464);
// Console.WriteLine(squad.Name);

// await Task.Delay(-1);
