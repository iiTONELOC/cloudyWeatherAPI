namespace cloudyWeatherAPI.source.utils
{
    // can be used on its own but the AuthService class provides an
    // Authorize method that can be used as a middleware
    class Auth
    {
        private readonly string[]? ACCESS_KEYS;
        private readonly string DEMO_KEY;
    
        
        public Auth()
        {
            ACCESS_KEYS = Environment
                .GetEnvironmentVariable("AUTH_ACCESS_KEYS")?
                .Split(";", StringSplitOptions.RemoveEmptyEntries);

            DEMO_KEY = Environment.GetEnvironmentVariable("DEMO_KEY") ?? "";
        } 

        public bool IsAuthorized(string? key)
        {
            return ACCESS_KEYS?.Contains(key) ?? false;
        }

        public bool IsDemo(HttpContext _context, string? authKey)
        {
            // we need to see if the auth key hash matches the demo key hash

            bool validDemoKey ()
            {
                if (authKey == DEMO_KEY && DEMO_KEY != null) return true;
                else return false;                
            }




            // check if the request is for the landing page
            if (_context.Request.Path == "/"
                || _context.Request.Path == "/current-demo"
                && validDemoKey()         
                || _context.Request.Path == "/current-full-demo"
                && validDemoKey())
            {
                // if it is, then we can allow the request                    
                return true;
            }
            else
            {
                return false;
            }

        }
    }

    // AuthMiddleware
    public static class AuthService
    {
        private static readonly Auth auth = new();
        public static bool IsAuthorized(string? key) => auth
            .IsAuthorized(key);

        public static Task Authorize(HttpContext context,Func<Task> next)
        {
            // extract the token from the Authorization header
            // can use Bearer or just the token
            var key = context.Request.Headers["Authorization"]
            .ToString()
            .Replace("Bearer ", "");

            // if the key is not authorized, return a 401
            if (auth.IsAuthorized(key) || auth.IsDemo(context, key))
            {
                return next();
            }
            else
            {
                context.Response.StatusCode = 401;
                return context.Response.WriteAsync("Unauthorized");
            }
           
        }

        
    }
} 
