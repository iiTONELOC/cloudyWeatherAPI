namespace cloudyWeatherAPI.source.WeatherService
{
    using cloudyWeatherAPI.source.db.models;
    using static cloudyWeatherAPI.source.WeatherService.Models;

    // This class tracks the number of calls made to the openweather api
    // We are allowed 60 calls per minute, and 1,000,000 calls per month
    // Weather data isn't updated more than once every 10 minutes, so we
    // should cache requests for 10 minutes.
    public class OpenWeatherTracker
    {
        private readonly int MaxCallsPerMinute = 60;
        private readonly int MaxCallsPerMonth = 1000000;
      
        protected int _callsThisMinute = 0;
        protected int _callsThisMonth = 0;
        protected int _monthlyCallsRemaining;
        protected int _mintueCallsRemaining;
        
        protected DateTime _lastCall;
        protected DateTime _initializedAt;
        protected DateTime _nextReset;

        protected WeatherCache Cache { get; set; }

        public OpenWeatherTracker()
        {
            _initializedAt = DateTime.Now;
            _nextReset = DateTime.Now.AddMonths(1);
            _monthlyCallsRemaining = MaxCallsPerMonth;
            _mintueCallsRemaining = MaxCallsPerMinute;
            Cache = new WeatherCache();

            // update the minuteTime, this also handles the monthly
            var minuteTimer = new System.Timers.Timer(60000);
            minuteTimer.Elapsed += (sender, e) => HandleReset();
            minuteTimer.AutoReset = true;
            minuteTimer.Enabled = true;
        }

        public async Task<WeatherCacheResponse> HandleOpenWeatherAPICall(Func<Task<OneCallApiData>> callback, string? lat, string? lon)
        {
            // holds our Response data that we will eventually return
            WeatherCacheResponse  Response = new ();

            try
            {
                // create an id we can use to check if the data is cached
                var id = Helpers.CreateId(lat, lon);
                var existingCache = Cache.Get(id);

                
                // Sets the found cache to the Response object.
                void SetCacheResponse()
                {
                    Response.StatusCode = 304;
                    Response.Data = existingCache?.ExistingData;
                }

                // Function to make the API Call.
                async Task HandleApiCall()
                {
                    // we need to check if we can make an API Call
                    var canMakeCall = CanMakeApiCall();          

                    if (canMakeCall)
                    {
                        // we need to remove the old cache item
                        Cache.Remove(id);
                        
                        // here we call the callback function to get our data
                        var _data = await callback();

                        // update the cache
                        Cache.Add(new WeatherCacheItem(id, _data));

                        // update the tracker
                        UpdateTracker();
                        
                        Response.Data = _data;
                        Response.StatusCode = 200;
                    }
                    else
                    {
                        if(existingCache != null)
                        {
                            SetCacheResponse();
                        }
                        else
                        {
                            Response.StatusCode = 429;
                            Response.Data = null;
                            Response.Error = new ApiError
                            {
                                Code = 429,
                                Message = "Too many requests"
                            };
                        }

                    }
                }
                
                
                // Look for a cached version
                // if the cached version exists and is not expired return it
                if (existingCache != null && !existingCache.IsExpired() && existingCache.ExistingData != null)
                {                    
                    SetCacheResponse();
                }
                // if the cached version exists and is expired, remove it, and update
                // OR if we don't have a requested item in the cache, make and API Call
                else if (existingCache != null && existingCache.IsExpired() && existingCache.ExistingData != null || existingCache == null)
                {
                    await HandleApiCall();
                }
                // Shouldn't Occur
                else
                {
                    Response.StatusCode = 500;
                    Response.Data = null;
                    Response.Error = new ApiError
                    {
                        Code = 500,
                        Message = "An Unknown Error Occurred"
                    };
                }

                return Response;

            }
            catch (Exception e)
            {
                Response.StatusCode = 500;
                Response.Data = null;
                Response.Error = new ApiError { Code = 500, Message = e.Message };
                return Response;
            }
        }

        private bool CanMakeApiCall()
        {
            if(_monthlyCallsRemaining > 0 && _mintueCallsRemaining > 0 )
            {
                return true;
            }
            else 
            {
                return false;
            }
        }

        private void UpdateTracker()
        {
            _callsThisMinute++;
            _callsThisMonth++;
            _monthlyCallsRemaining--;
            _mintueCallsRemaining--;

            _lastCall = DateTime.Now;
        }

        private void HandleReset()
        {
            _callsThisMinute = 0;
            _mintueCallsRemaining = MaxCallsPerMinute;

            if (DateTime.Now > _nextReset)
            {
                ResetCallsPerMonth();
            }
        }

        private void ResetCallsPerMonth()
        {
            _callsThisMonth = 0;
            _monthlyCallsRemaining = MaxCallsPerMonth;
        }
    }
}
