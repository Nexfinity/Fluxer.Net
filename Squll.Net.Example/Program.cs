using Squll.Net;
using Squll.Net.Extensions;
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

    if (x.Content.StartsWith("/"))
    {
#pragma warning disable CS8509
        var command = x.Content[1..].Split(' ').First();
        try
        {
            await (command switch
            {
                "say" => Say(x),
                "join" => Join(x),
                "help" => Help(x),
                "dev-status" => Status(x),
            });
        }
        catch (SqullException ex)
        {
            await client.SendMessage(x.SpaceId, new()
            {
                Content = $"# Error\n\n{ex.Message}\n\n```\n{ex.SqullData}\n```",
                Nonce = $"ex"
            });
        }
#pragma warning restore CS8509
    }
};

async Task Say(Message msg)
{
    var toSend = msg.Content[4..];
    await client!.SendMessage(msg.SpaceId, new()
    {
        Content = toSend,
        Nonce = $"c:1:{msg.Id}"
    });
}

async Task Join(Message msg)
{
    var invite = msg.Content[5..].Trim();
    await client!.JoinSquad(invite);
    await client.SendMessage(msg.SpaceId, new()
    {
        Content = "Joined the squad!",
        Nonce = $"c:2:{msg.Id}"
    });
}

async Task Help(Message msg)
{
    await client!.SendMessage(msg.SpaceId, new()
    {
        Content = File.ReadAllText("Help.md"),
        Nonce = $"c:3:{msg.Id}"
    });
}

async Task Status(Message msg)
{
    client!.SetStatus(msg.Content[11..].Trim());
    await client!.SendMessage(msg.SpaceId, new()
    {
        Content = $"set status to `{msg.Content[11..].Trim()}`",
        Nonce = $"c:3:{msg.Id}"
    });
}


await client.ConnectToGateway();

var squad = await client.GetSquad(72078685781950464);
Console.WriteLine(squad.Name);

await Task.Delay(-1);
