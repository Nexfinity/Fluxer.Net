[Group("dev"), RequireOwner]
public class TestModule : ModuleBase
{
    [Command("say")]
    public async Task Say([Remainder] string text)
    {
        // This will respond back with the text you give it.
        await ReplyAsync(text);
    }
}