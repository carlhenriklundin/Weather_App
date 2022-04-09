using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Text.Json;

using Weather.Models;

namespace Weather.Services
{
    public class OpenWeatherService
    {
        HttpClient httpClient = new HttpClient();
        readonly string apiKey = "b7218ab5e346e4579e66e7686f6814dc"; //b7218ab5e346e4579e66e7686f6814dc

        ConcurrentDictionary<string, (DateTime, Forecast)> data = new ConcurrentDictionary<string, (DateTime, Forecast)>();
        public event EventHandler<string> WeatherForecastAvailable;

        public async Task<Forecast> GetForecastAsync(string City)
        {
            if (data.TryGetValue(City, out var _cacheCity))
            {
                if ((int)(DateTime.Now - _cacheCity.Item1).TotalSeconds <= 60)
                {
                //    WeatherForecastAvailable.Invoke(this, $"Cached weather forecast for {City} available.");
                    return _cacheCity.Item2;
                }
            }

            var language = System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            var uri = $"https://api.openweathermap.org/data/2.5/forecast?q={City}&units=metric&lang={language}&appid={apiKey}";

            Forecast forecast = await ReadWebApiAsync(uri);

            data.TryAdd(City, (DateTime.Now, forecast));

            // WeatherForecastAvailable.Invoke(this, $"New weather forecast for {City} available.");
            return forecast;

        }
        
        
        public async Task<Forecast> GetForecastAsync(double latitude, double longitude)
        {
            if (data.TryGetValue($"{latitude},{longitude}", out var _cacheCity))
            {
                if ((int)(DateTime.Now - _cacheCity.Item1).TotalSeconds <= 60)
                {
                    WeatherForecastAvailable.Invoke(this, $"Cached weather forecast for ({latitude}, {longitude}) available.");
                    return _cacheCity.Item2;
                }
            }

            var language = System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            var uri = $"https://api.openweathermap.org/data/2.5/forecast?lat={latitude}&lon={longitude}&units=metric&lang={language}&appid={apiKey}";

            Forecast forecast = await ReadWebApiAsync(uri);

            data.TryAdd($"{latitude},{longitude}", (DateTime.Now, forecast));
            
            WeatherForecastAvailable.Invoke(this, $"New weather forecast for ({latitude}, {longitude}) available.");
            return forecast;
        }



        private async Task<Forecast> ReadWebApiAsync(string uri)
        {
            HttpResponseMessage response = await httpClient.GetAsync(uri);
            response.EnsureSuccessStatusCode();
            WeatherApiData wd = await response.Content.ReadFromJsonAsync<WeatherApiData>();

            List<ForecastItem> forecastItems = wd.list.Select(x => new ForecastItem
            {
                Temperature = x.main.temp,
                DateTime = UnixTimeStampToDateTime(x.dt),
                WindSpeed = x.wind.speed,
                Description = x.weather[0].description,
                Icon = "http://openweathermap.org/img/wn/" + x.weather[0].icon + "@2x.png"
            }).ToList();

            Forecast forecast = new Forecast();
            forecast.City = wd.city.name;
            forecast.Items = forecastItems;
            
            return forecast;
        }


        private DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dateTime;
        }
    }
}
