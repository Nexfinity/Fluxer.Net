using Fluxer.Net.OAuth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
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
    public static FluxerOAuthClient OAuth;
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        using (StreamReader reader = new StreamReader(System.AppDomain.CurrentDomain.BaseDirectory + "Config.json"))
        {
            JsonSerializer serializer = new JsonSerializer();
            Config = (Config)serializer.Deserialize(reader, typeof(Config));
        }

        // Add services to the container.
        builder.Services.AddControllers();
        builder.Services.AddAuthorization();
        builder.Services.AddFluxerOAuth(Config.Id, Config.Secret);
        builder.Services.AddAntiforgery(options =>
        {
            options.Cookie.SameSite = SameSiteMode.None;
            options.Cookie.SecurePolicy = builder.Environment.IsDevelopment() ? CookieSecurePolicy.None : CookieSecurePolicy.Always;
        });
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultScheme = "Cookie";
            options.DefaultAuthenticateScheme = "Cookie";
            options.DefaultChallengeScheme = "Fluxer";
        })
           .AddCookie("Cookie")
           .AddFluxer(x =>
           {
               x.ClientId = Config.Id.ToString();
               x.ClientSecret = Config.Secret;
               x.CallbackPath = "/signin-fluxer";
               x.Scope.Add("email");
               x.Scope.Add("guilds");
               x.Scope.Add("connections");
               x.SaveTokens = true;
           });


        var app = builder.Build();
        OAuth = app.Services.GetRequiredService<FluxerOAuthClient>();
        // Configure the HTTP request pipeline.

        app.UseRouting();
        app.UseAuthorization();
        app.UseAuthentication();

        app.MapControllers();
        app.UseAntiforgery();
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

        app.MapGet("/", (HttpContext httpContext) =>
        {
            return "Hello world";
        });

        app.MapGet("/login", [Authorize] (HttpContext httpContext) =>
        {
            return "Login";
        });

        app.MapGet("/user", async (HttpContext httpContext) =>
        {
            if (httpContext.User.Identity.IsAuthenticated)
            {
                var test = httpContext.User.GetFluxerClaims(OAuth);
                var auth = await httpContext.AuthenticateAsync();
                var access = auth.Properties.GetTokenValue("access_token");

                var refresh = auth.Properties.GetTokenValue("refresh_token");

                //var validToken = await OAuth.CheckValidTokenAsync(access);
                //var connections = await OAuth.GetConnectionsAsync(access);
                //foreach (var c in connections)
                //{
                //    Console.WriteLine($"{c.Name}");
                //}

                //var guilds = await OAuth.GetGuildsAsync(access);
                //var token = await OAuth.GetTokenAsync(access);
                //var user = await OAuth.GetUserAsync(access);
                //var valid = await OAuth.GetValidTokenAsync(access);
                var exchange = await OAuth.ExchangeRefreshTokenAsync(refresh);
                return "Is Auth: " + test.Username;
            }

            return "Login Required";
        });

        app.Run();
    }
}
