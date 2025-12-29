using Supabase;
using WeatherApp.Models;

namespace WeatherApp.Services;

public class WeatherHistoryService
{
    private readonly Client _supabase;

    public WeatherHistoryService(Client supabase)
    {
        _supabase = supabase;
    }

    public async Task<List<WeatherHistory>> GetLast7Days(string city)
    {
        var result = await _supabase
            .From<WeatherHistory>()
            .Filter("city", Supabase.Postgrest.Constants.Operator.Equals, city)
            .Order("weather_date", Supabase.Postgrest.Constants.Ordering.Descending)
            .Limit(7)
            .Get();

        return result.Models;
    }
}
