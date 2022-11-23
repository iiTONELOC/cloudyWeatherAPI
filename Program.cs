
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using cloudyWeatherAPI.source.db;
using cloudyWeatherAPI.source.utils;

// load our envs
// load the environment variables
var root = Directory.GetCurrentDirectory();
DotEnv.Load(Path.Combine(root, ".env").ToString());

var weatherService = new WeatherService();

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();



// returns the current, minutely, hourly, and daily weather data
app.MapGet("/current", async (
    [FromQuery(Name = "lat")] string? lat,
    [FromQuery(Name = "lon")] string? lon
    ) => await weatherService.GetCurrent(lat, lon, true));

// only returns the current weather only
app.MapGet("/current-full", async (
    [FromQuery(Name = "lat")] string? lat,
    [FromQuery(Name = "lon")] string? lon
    ) => await weatherService.GetCurrent(lat, lon));


app.Run();
