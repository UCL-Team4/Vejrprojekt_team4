using System;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Vejrprojekt_team4.Models;

namespace Vejrprojekt_team4.endpoint
{
    public static class GetWeather
    {
        private static string _apiKey = "9bcc17013486f1b64f663834a551d26f";

        private static WeatherResponse apiData;
        private static DateTime? lastCollectedApiCall;
        private static TimeSpan breakDuration = TimeSpan.FromMinutes(10);

        private static async Task<bool> GetApiData(decimal latitude, decimal longitude)
        {
            DateTime currentTime = DateTime.UtcNow;
            if (lastCollectedApiCall.HasValue && DateTime.UtcNow - lastCollectedApiCall < breakDuration)
            {
                return false;
            }

            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(
                    "https://api.openweathermap.org/data/2.5/weather" +
                    "?lat=" + latitude +
                    "&lon=" + longitude +
                    "&appid=" + _apiKey
                ),
            };

            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                string jsonResponse = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var weatherData = JsonSerializer.Deserialize<WeatherResponse>(jsonResponse, options);

                string baseIcon = weatherData.Weather[0].Icon;
                bool isDay = baseIcon.Contains('d');
                string baseUrl = "https://openweathermap.org/img/wn/";


                if (weatherData != null)
                {
                    lastCollectedApiCall = DateTime.UtcNow;
                }

                weatherData.Weather[0].Icon = $"{baseUrl}{baseIcon}.png";
                weatherData.Weather[0].OtherIcon = baseUrl + (isDay ? baseIcon.Replace('d', 'n') : baseIcon.Replace('n', 'd')) + ".png";

                apiData = weatherData;

                return true;
            }
        }


        public static async Task<Weather> GetWeatherAtLocation(decimal latitude = 10.5m, decimal longitude = 10.5m)
        {
            bool _ = await GetApiData(latitude, longitude);


            return apiData.Weather[0];
        }
    }
}
