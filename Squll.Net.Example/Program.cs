using Serilog;
using Serilog.Core;
using Serilog.Sinks.SystemConsole.Themes;
using Squll.Net;
using Squll.Net.Example;
using Squll.Net.Gateway.Data;

//Configure logger to log to console and file
Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Console(theme: AnsiConsoleTheme.Code)
                .WriteTo.File($"output-{DateTime.Now:yyyy-dd-mm:hh-mm-ss}.log", rollingInterval: RollingInterval.Infinite, rollOnFileSizeLimit: true)
                .CreateLogger();

//Load configuration file values
var config = ConfigExtension.LoadConfig();
if (config == null)
{
	Log.Error("YAML file not found.");
	return;
}

Log.Debug("Config file loaded.");

//Setup gateway connection (This receives events such as MESSAGE_CREATE)
var gateway = new GatewayClient(config["Token"], new()
{
    ReconnectAttemptDelay = 2,
    Serilog = Log.Logger as Logger,
    IgnoredGatewayEvents = new() //Ignore specific events we don't plan to use
    {
        "PRESENCE_UPDATE"
    },
    Presence = new PresenceUpdateGatewayData(Squll.Net.Objects.Enums.Status.Online) //Set the default presence to online
});

//Connect to the gateway
await gateway.ConnectAsync();

//connect to the API (This allows you to create, modify and delete spaces, squads, users, etc)
var api = new ApiClient(config["Token"], new()
{
    Serilog = Log.Logger as Logger
});

//Handle the MESSAGE_CREATE event (Allows us to receive and process commands)
gateway.MessageCreate += async x =>
{
	if (x.Content == "/ping") //Listen for the /ping command
	{
		//Respond with our own message
		await api.SendMessage(x.SpaceId, new()
		{
			Content = "pong ;P"
		});
	}
};

//Keep the bot running
await Task.Delay(-1);