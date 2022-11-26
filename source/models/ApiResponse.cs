namespace cloudyWeatherAPI.source.models
{
    public static class ApiResponse
    {
        public class ApiError
        {
            public int Code { get; set; }
            public string? Message { get; set; }
        }

        public class WeatherCacheResponse
        {
            public int StatusCode { get; set; }
            public OneCallApiData? Data { get; set; }
            public ApiError? Error { get; set; }
        }
    }
}
