builder.Services.AddAntiforgery(options =>
{
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
});
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "Cookie";
    options.DefaultChallengeScheme = "Fluxer";
})
   .AddCookie("Cookie")
   .AddFluxer(x =>
   {
       x.ClientId = "12345";
       x.ClientSecret = "secret_here";
       x.CallbackPath = "/signin-fluxer";
       x.Scope.Add("email");
       x.Scope.Add("guilds");
   });