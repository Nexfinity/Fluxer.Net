public class TestModule : ModuleBase
{
    [Command("test")]
    public async Task Test()
    {
        // This will send a message in the current channel.
        await ReplyAsync("Hello World!");
    }
}