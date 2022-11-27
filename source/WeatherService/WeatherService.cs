namespace cloudyWeatherAPI.source.WeatherService
{
    using System;
    using System.Text.Json;
    using cloudyWeatherAPI.source.models;
    using static cloudyWeatherAPI.source.models.ApiResponse;

    public class WeatherService
    {
        private readonly string? DefaultLattitude;
        private readonly string? DefaultLongitude;
        private readonly string? API_KEY;
        private readonly string? Lang;
        private readonly string? Units;
        private OpenWeatherTracker Tracker { get; set; }
        private OpenWeatherTracker DemoTracker { get; set; }

        public WeatherService()

        {
            Lang = "en";
            Units = "imperial";
            Tracker = new OpenWeatherTracker();
            DemoTracker = new OpenWeatherTracker(true);
            API_KEY = Environment.GetEnvironmentVariable("WEATHER_KEY");
            DefaultLattitude = Environment.GetEnvironmentVariable("LAT") ?? "0";
            DefaultLongitude = Environment.GetEnvironmentVariable("LON") ?? "0";
        }
             
        public async Task<IResult> GetCurrent(
            string lat, string lon, bool notFull = false, bool demo = false)
        {
            try
            {
                HttpClient httpClient = new ();                
               
                // the callback is used to fetch data from the openweather api
                async Task<OneCallApiData> callback ()
                {
                    // fetch data from the openweathermap api
                    using HttpResponseMessage weatherData = await httpClient
                        .GetAsync(lat == "0" || lon == "0" ? BuildUrl(notFull) :
                        BuildUrl(lat, lon, notFull));

                    // store the data as a variable, we will need to manipulate
                    // it
                    var weatherResponseData = await weatherData
                        .Content
                        .ReadAsStringAsync();

                    // event is a reserved word in C# so we have to rename the
                    // event field
                    weatherResponseData = weatherResponseData
                        .Replace("event", "_event");

                    // create our OneCallApiData
                    OneCallApiData data = JsonSerializer
                        .Deserialize<OneCallApiData>(weatherResponseData)!;
                    return data;
                }

                var cacheType = notFull ? CacheType.basic : CacheType.full;
                // use the API or Demo Tracker to handle our call.
                // It will return cached data, or data from the openweather api

                WeatherCacheResponse data = !demo ? await Tracker.
                    HandleOpenWeatherAPICall(callback, cacheType, lat, lon) :
                    
                    await DemoTracker
                    .HandleOpenWeatherAPICall(callback, cacheType, lat, lon);
                
                return Results.Ok(data);
            }
            catch
            {
                return Results.BadRequest();
            }
        }
      
        // TODO: Eventually refactor the buildUrl to take in a list of parameters
        // to exclude. 
        private string BuildUrl(bool exclude)
        {
            var EXCLUDE = exclude ? "&exclude=minutely,hourly,daily" : "";
            
            return $"https://api.openweathermap.org/data/2.5/onecall?lat=" +
                $"{DefaultLattitude}&lon={DefaultLongitude}{EXCLUDE}&appid=" +
                $"{API_KEY}&lang={Lang}&units={Units}";
        }
        
        private string BuildUrl(string? lat, string? lon, bool exclude)
        {
            var EXCLUDE = exclude ? "&exclude=minutely,hourly,daily" : "";

            var LAT = lat ?? DefaultLattitude ?? "0";
            var LON = lon ?? DefaultLongitude ?? "0";

            return $"https://api.openweathermap.org/data/2.5/onecall?lat={LAT}" +
                $"&lon={LON}{EXCLUDE}&appid={API_KEY}&lang={Lang}&units={Units}";
        }
    }
}
