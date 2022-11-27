using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace cloudyWeatherAPI.Pages
{
    public class IndexModel : PageModel
    {
        public void OnGet()
        {
        }

        public void GetAccessKey()
        {
            {
                // get the access key from the environment variables
                var ACCESS_KEY = System.Environment.GetEnvironmentVariable("DEMO_KEY");
                ViewData["ACCESS_KEY"] = ACCESS_KEY;
            }
        }
    }
}
