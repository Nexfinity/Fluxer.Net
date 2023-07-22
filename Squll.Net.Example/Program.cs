using Squll.Net;
using Squll.Net.Objects;

var client = new SqullConnection("NzIwOTAzNjczNDIyMTUxNjg.ZLdNHg.wJ3bZVNAhY0tTE5vM8HT1SL0mtg");
Squad[] squads = Array.Empty<Squad>();
client.Ready += x =>
{
    squads = x.Squads;
};
client.MessageCreated += async x =>
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine($"{x.Author.DisplayName}#{x.Author.Discriminator}: {x.Content}");

    if (x.Content.StartsWith("/say"))
    {
        var toSend = x.Content[4..];
        await client.SendMessage(x.SpaceId, new()
        {
            Content = toSend,
            Nonce = "73315849653989376"
        });
    }
};
await client.ConnectToGateway();

var squad = await client.GetSquad(72078685781950464);
Console.WriteLine(squad.Name);

await Task.Delay(-1);
