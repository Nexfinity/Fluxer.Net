using Squll.Net;

var client = new SqullConnection("NzIwOTAzNjczNDIyMTUxNjg.ZLdNHg.wJ3bZVNAhY0tTE5vM8HT1SL0mtg");
await client.ConnectToGateway();


var squad = await client.GetSquad(72078685781950464);
Console.WriteLine(squad.Name);

await Task.Delay(-1);
