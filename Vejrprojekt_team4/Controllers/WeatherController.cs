using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
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

                if (weatherData == null)
                {
                    return View(weatherData);
                }

                string baseIcon = weatherData.Weather[0].Icon;
                bool isDay = baseIcon.Contains('d');
                string baseUrl = "https://openweathermap.org/img/wn/";


                weatherData.Weather[0].Icon = $"{baseUrl}{baseIcon}.png";
                weatherData.Weather[0].OtherIcon = baseUrl + (isDay ? baseIcon.Replace('d', 'n') : baseIcon.Replace('n', 'd')) + ".png";

                return View(weatherData);
            }
        }
    }
}
