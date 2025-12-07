    using System.Net.Http.Json;
using WeatherApp.Models;

namespace WeatherApp.Services;

public class WeatherService
{
    private readonly HttpClient _http;
    private const string ApiKey = "b315c4f34144624bd6b24d7ac126f080"; // put your key

    public WeatherService(HttpClient http)
    {
        _http = http;
    }

    public async Task<WeatherResponse?> GetByCityAsync(string city)
    {
        var url = $"https://api.openweathermap.org/data/2.5/weather?q={city}&units=metric&appid={ApiKey}";
        return await _http.GetFromJsonAsync<WeatherResponse>(url);
    }

    public async Task<WeatherResponse?> GetByCoordsAsync(double lat, double lon)
    {
        var url = $"https://api.openweathermap.org/data/2.5/weather?lat={lat}&lon={lon}&units=metric&appid={ApiKey}";
        return await _http.GetFromJsonAsync<WeatherResponse>(url);
    }
}
