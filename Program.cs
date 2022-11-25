using Microsoft.AspNetCore.Mvc;
using cloudyWeatherAPI.source.utils;
using cloudyWeatherAPI.source.WeatherService;

// load our environment variables
var root = Directory.GetCurrentDirectory();
DotEnv.Load(Path.Combine(root, ".env").ToString());

// bring in the weatherService
var weatherService = new WeatherService();

// build the app
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Attach our AuthMiddleware
app.Use((context, next) => AuthService.Authorize(context, next));


// ROUTES

// only returns the current weather only
app.MapGet("/current", async (
    [FromQuery(Name = "lat")] string? lat,
    [FromQuery(Name = "lon")] string? lon
    ) => await weatherService.GetCurrent(lat??"0", lon ??"0",true));

// returns the current, minutely, hourly, and daily weather data
app.MapGet("/current-full", async (
    [FromQuery(Name = "lat")] string? lat,
    [FromQuery(Name = "lon")] string? lon
    ) => await weatherService.GetCurrent(lat ?? "0", lon ?? "0"));


app.Run();
