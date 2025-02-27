using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Vejrprojekt_team4.Models;

namespace Vejrprojekt_team4.Controllers
{
    public class WeatherController : Controller
    {
        public async Task<IActionResult> Index()
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(
                    "https://api.openweathermap.org/data/2.5/weather" +
                    "?lat=44.34" +
                    "&lon=10.99" +
                    "&appid=9bcc17013486f1b64f663834a551d26f"
                ),
            };

            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                string jsonResponse = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var weatherData = JsonSerializer.Deserialize<WeatherResponse>(jsonResponse, options);

                return View(weatherData);
            }
        }
    }
}
