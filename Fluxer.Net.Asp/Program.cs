using Fluxer.Net.OAuth;
using Newtonsoft.Json;

namespace Fluxer.Net.Asp;

public class Config
{
    public ulong Id { get; set; }
    public string Secret { get; set; }
}
public class Program
{
    public static Config Config;
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        using (StreamReader reader = new StreamReader(System.AppDomain.CurrentDomain.BaseDirectory + "Config.json"))
        {
            JsonSerializer serializer = new JsonSerializer();
            Config = (Config)serializer.Deserialize(reader, typeof(Config));
        }

        // Add services to the container.
        builder.Services.AddAuthorization();
        builder.Services.AddFluxerOAuth(Config.Id, Config.Secret);




        var app = builder.Build();

        // Configure the HTTP request pipeline.

        app.UseAuthorization();

        //var summaries = new[]
        //{
        //    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        //};

        //app.MapGet("/weatherforecast", (HttpContext httpContext) =>
        //{
        //    var forecast = Enumerable.Range(1, 5).Select(index =>
        //        new WeatherForecast
        //        {
        //            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
        //            TemperatureC = Random.Shared.Next(-20, 55),
        //            Summary = summaries[Random.Shared.Next(summaries.Length)]
        //        })
        //        .ToArray();
        //    return forecast;
        //});

        app.Run();
    }
}
