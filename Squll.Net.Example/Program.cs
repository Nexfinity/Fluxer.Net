using Squll.Net;
using Squll.Net.Objects;

var client = new SqullConnection("NzIwOTAzNjczNDIyMTUxNjg.ZLdNHg.wJ3bZVNAhY0tTE5vM8HT1SL0mtg");
Squad[] squads = Array.Empty<Squad>();
client.Ready += x =>
{
    squads = x.Squads;
};
client.MessageCreated += x =>
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine($"{x.Author.DisplayName}#{x.Author.Discriminator}: {x.Content}");
};
await client.ConnectToGateway();


var squad = await client.GetSquad(72078685781950464);
Console.WriteLine(squad.Name);

await Task.Delay(-1);
