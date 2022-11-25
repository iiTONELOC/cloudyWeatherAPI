namespace cloudyWeatherAPI.source.WeatherService
{

    public static class Helpers
    {
        /// <summary>
        /// Generates the Id for the WeatherCache object
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="lon"></param>
        /// <returns>The generated Id as a string</returns>
        public static string CreateId(string? lat, string? lon)
        {
            // convert the lat and lon to two decimal places
            double latDouble = Convert.ToDouble(lat);
            double lonDouble = Convert.ToDouble(lon);

            latDouble = Math.Round(latDouble, 2);
            lonDouble = Math.Round(lonDouble, 2);

            lat = latDouble.ToString();
            lon = lonDouble.ToString();

            // create the id
            var id = lat + ";" + lon;
         
            // if parameters were not passed we end up with 0;0
            var isValid = id != "0;0";

            if (isValid)
            {
                return id;
            }
            else
            {
                // A search wasn't conducted so lets look up the defaults
                var LAT = Environment.GetEnvironmentVariable("LAT");
                var LON = Environment.GetEnvironmentVariable("LON");
               
                if (LAT != null && LON != null)
                {
                    return CreateId(LAT, LON);
                }
                else
                {
                    // Error with retrieving the default values from the environment
                    
                    // we should never reach here, but if we do 
                    // we need to return something rather than breaking

                    // Random cords, somewhere in NH
                    return CreateId("43.05014", "-71.06788");

                }
            }

        }

    }
}
