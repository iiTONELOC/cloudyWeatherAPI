namespace cloudyWeatherAPI.source.models
{
    public class WeatherCacheItem
    {
        // Id is the lat and lon set to two decimal places
        // concatenated together as one string with a ';'
        // as the delimiter

        // Example: 40.12, -105.23 -> 4012;-10523
        public string? Id { get; set; }
        public double Lat { get; set; }
        public double Lon { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }

        private readonly int CacheTimeInMins = 10;

        // constructor
        public OneCallApiData? ExistingData { get; set; }

        public WeatherCacheItem(string id, OneCallApiData apiData)
        {
            double[] coords = GetLatAndLon(id);

            Id = id;
            Lat = coords[0];
            Lon = coords[1];
            ExistingData = apiData;
            CreatedAt = DateTime.Now;
            ExpiresAt = DateTime.Now.AddMinutes(CacheTimeInMins);
        }

        public bool IsExpired()
        {
            return DateTime.Now > ExpiresAt;
        }

        static double[] GetLatAndLon(string _id)
        {
            var latAndLon = _id.Split(';');
            var lat = Convert.ToDouble(latAndLon[0]);
            var lon = Convert.ToDouble(latAndLon[1]);

            return new double[] { lat, lon };
        }

    }


    // class that contains a dictionary of our cached weather data
    // uses the id as an index
    // methods for manipulating the cache are included here.
    public class WeatherCache
    {
        protected Dictionary<string, WeatherCacheItem>? _weatherCache;

        public WeatherCache()
        {
            _weatherCache = new Dictionary<string, WeatherCacheItem>();
        }

        public void Add(WeatherCacheItem item)
        {
            if (_weatherCache != null && item?.Id != null)
            {
                _weatherCache.Add(item.Id, item);
            }
        }

        public WeatherCacheItem? Get(string id)
        {
            if (_weatherCache != null && id != null && _weatherCache
                .TryGetValue(id, out WeatherCacheItem? item))
            {
                return item;
            }
            else
            {
                return null;
            }
        }

        public void Remove(string id)
        {
            if (_weatherCache != null && id != null)
            {
                _weatherCache.Remove(id);
            }
        }
    }
}
