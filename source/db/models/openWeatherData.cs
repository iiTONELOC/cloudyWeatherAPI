namespace cloudyWeatherAPI.source.db.models
/// This file models the data returned by the OpenWeather OneCall API
{
    public class WeatherCondition
    {
        public int id { get; set; }
        public string? main { get; set; }
        public string? description { get; set; }
        public string? icon { get; set; }
    }

    public class Minutely
    {
        public int dt { get; set; }
        public double precipitation { get; set; }
    }

    public class Hourly
    {
        public int dt { get; set; }
        public double temp { get; set; }
        public double feels_like { get; set; }
        public int pressure { get; set; }
        public int humidity { get; set; }
        public double dew_point { get; set; }
        public double uvi { get; set; }
        public int clouds { get; set; }
        public int visibility { get; set; }
        public double wind_speed { get; set; }
        public int wind_deg { get; set; }
        public double wind_gust { get; set; }
        public IList<WeatherCondition>? weather { get; set; }
        public double pop { get; set; }
    }

    public class DailyTemp
    {
        public double day { get; set; }
        public double min { get; set; }
        public double max { get; set; }
        public double night { get; set; }
        public double eve { get; set; }
        public double morn { get; set; }
    }

    public class DailyFeelsLike
    {
        public double day { get; set; }
        public double night { get; set; }
        public double eve { get; set; }
        public double morn { get; set; }
    }

    public class Daily
    {
        public int dt { get; set; }
        public int sunrise { get; set; }
        public int sunset { get; set; }
        public int moonrise { get; set; }
        public int moonset { get; set; }
        public double moon_phase { get; set; }
        public DailyTemp? temp { get; set; }
        public DailyFeelsLike? feels_like { get; set; }
        public int pressure { get; set; }
        public int humidity { get; set; }
        public double dew_point { get; set; }
        public double wind_speed { get; set; }
        public int wind_deg { get; set; }
        public double wind_gust { get; set; }
        public IList<WeatherCondition>? weather { get; set; }
        public double clouds { get; set; }
        public double pop { get; set; }
        public double uvi { get; set; }
        public double rain { get; set; }
    }

    public class Current
    {
        public int dt { get; set; }
        public int sunrise { get; set; }
        public int sunset { get; set; }
        public double temp { get; set; }
        public double feels_like { get; set; }
        public int pressure { get; set; }
        public int humidity { get; set; }
        public double dew_point { get; set; }
        public double uvi { get; set; }
        public int clouds { get; set; }
        public int visibility { get; set; }
        public double wind_speed { get; set; }
        public int wind_deg { get; set; }
        public double wind_gust { get; set; }
        public IList<WeatherCondition>? weather { get; set; }
    }


    public class Alert
    {
        public string? sender_name { get; set; }
        // it is actually event but it is a reserved word
        public string? _event { get; set; }
        public int start { get; set; }
        public int end { get; set; }
        public string? description { get; set; }
        public string[]? tags { get; set; }

    }


    public class OneCallApiData
    {
        public double lat { get; set; }
        public double lon { get; set; }
        public string? timezone { get; set; }
        public int timezone_offset { get; set; }
        public Current? current { get; set; }
        public IList<Minutely>? minutely { get; set; }
        public IList<Hourly>? hourly { get; set; }
        public IList<Daily>? daily { get; set; }
        public IList<Alert>? alerts { get; set; }
    }
}
