namespace cloudyWeatherAPI.source.db
{
    using System;
    using System.Text.Json;
    using cloudyWeatherAPI.source.db.models;

    public class WeatherService
    {
        private readonly string? DefaultLattitude;
        private readonly string? DefaultLongitude;
        private readonly string? API_KEY;
        private readonly string? lang;
        private readonly string? units;
 
        public WeatherService()
        {
            DefaultLattitude = Environment.GetEnvironmentVariable("LAT") ?? "0";
            DefaultLongitude = Environment.GetEnvironmentVariable("LON") ?? "0";
            API_KEY = Environment.GetEnvironmentVariable("WEATHER_KEY");
            lang = "en";
            units = "imperial";
        }

        
        public async Task<IResult> GetCurrent(string? lat, string?lon, Boolean notFull=false)
        {
            HttpClient httpClient = new HttpClient();

            try
            {
                // fetch data from the openweathermap api
                using HttpResponseMessage weatherData = await httpClient
                    .GetAsync(lat == "0" || lon == "0" ? this.buildUrl(notFull) : this.buildUrl(lat, lon, notFull));

                // store the data as a variable, we will need to manipulate it
                var weatherResponseData = await weatherData.Content.ReadAsStringAsync();

                // event is a reserved word in C# so we have to rename the event field
                weatherResponseData = weatherResponseData.Replace("event", "_event");
                
                // create our OneCallApiData
                OneCallApiData data = JsonSerializer.Deserialize<OneCallApiData>(weatherResponseData)!;
                return Results.Ok(data);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Results.BadRequest();
            }
        
        
        }

        private string buildUrl(Boolean exclude)
        {
            var EXCLUDE = exclude ? "&exclude=minutely,hourly,daily" : "";
            return $"https://api.openweathermap.org/data/2.5/onecall?lat={DefaultLattitude}&lon={DefaultLongitude}{EXCLUDE}&appid={API_KEY}&lang={lang}&units={units}";
        }
        private string buildUrl(string? lat, string?lon, Boolean exclude)
        {
            var EXCLUDE = exclude ? "&exclude=minutely,hourly,daily" : "";

            var LAT = lat ?? DefaultLattitude ?? "0";
            var LON = lon ?? DefaultLattitude ?? "0";

            return $"https://api.openweathermap.org/data/2.5/onecall?lat={LAT}&lon={LON}{EXCLUDE}&appid={API_KEY}&lang={lang}&units={units}";
        }

       
    }
 
  
}
