using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using Supabase;
using WeatherApp.Models;

namespace WeatherApp.Services;

public class WeatherService
{
    private readonly HttpClient _http;
    private readonly Client _supabase;
    private readonly string _apiKey;

    public WeatherService(
        HttpClient http,
        Client supabase,
        IConfiguration configuration)
    {
        _http = http;
        _supabase = supabase;
        _apiKey = configuration["OpenWeather:ApiKey"]
            ?? throw new InvalidOperationException("OpenWeather API key missing");
    }

    public async Task<WeatherResponse?> GetByCityAsync(string city)
    {
        var url =
            $"https://api.openweathermap.org/data/2.5/weather" +
            $"?q={city}" +
            $"&units=metric" +
            $"&appid={_apiKey}" +
            $"&ts={DateTimeOffset.UtcNow.ToUnixTimeSeconds()}";

        var result = await _http.GetFromJsonAsync<WeatherResponse>(url);

        if (result != null)
            await SaveTodayWeather(result);

        return result;
    }
   
    public async Task<WeatherResponse?> GetByCoordsAsync(double lat, double lon)
    {
        var url =
            $"https://api.openweathermap.org/data/2.5/weather" +
            $"?lat={lat}&lon={lon}" +
            $"&units=metric" +
            $"&appid={_apiKey}" +
            $"&ts={DateTimeOffset.UtcNow.ToUnixTimeSeconds()}";

        var result = await _http.GetFromJsonAsync<WeatherResponse>(url);

        if (result != null)
            await SaveTodayWeather(result);

        return result;
    }

    public async Task<string> GetCityFromCoordinates(GeoPosition position)
    {
        var url =
            $"https://api.openweathermap.org/geo/1.0/reverse" +
            $"?lat={position.Latitude}&lon={position.Longitude}&limit=1&appid={_apiKey}";

        var response =
            await _http.GetFromJsonAsync<List<GeoLocationResult>>(url);

        return response?.FirstOrDefault()?.Name ?? "Unknown";
    }
    private async Task SaveTodayWeather(WeatherResponse data)
    {
        var today = DateTime.UtcNow.Date;
        var city = data.Name ?? "Unknown";

        var existing = await _supabase
            .From<WeatherHistory>()
            .Filter("city", Supabase.Postgrest.Constants.Operator.Equals, city)
            .Filter(
                "weather_date",
                Supabase.Postgrest.Constants.Operator.Equals,
                today.ToString("yyyy-MM-dd")
            )
            .Get();

        if (existing.Models.Count > 0)
            return;

        var history = new WeatherHistory
        {
            City = city,
            WeatherDate = today,
            Temp_Min = (decimal)(data.Main?.Temp ?? 0),
            Temp_Max = (decimal)(data.Main?.Temp ?? 0),
            Temp_Avg = (decimal)(data.Main?.Temp ?? 0),
            Humidity = (decimal)(data.Main?.Humidity ?? 0),
            Wind_Speed = (decimal)(data.Wind?.Speed ?? 0),
            Condition = "Normal"
        };

        await _supabase.From<WeatherHistory>().Insert(history);
    }
}
