using WeatherApp.Models;

namespace WeatherApp.Services;

public class FavoriteCityService
{
    private readonly AuthenticatedSupabaseClient _authClient;

    public FavoriteCityService(AuthenticatedSupabaseClient authClient)
    {
        _authClient = authClient;
    }

    public async Task<List<FavoriteCity>> GetAsync()
    {
        var client = await _authClient.GetAuthenticatedClientAsync();

        var result = await client
            .From<FavoriteCity>()
            .Order("created_at", Supabase.Postgrest.Constants.Ordering.Descending)
            .Get();

        return result.Models;
    }

    public async Task AddAsync(string city)
    {
        var client = await _authClient.GetAuthenticatedClientAsync();
        var user = client.Auth.CurrentUser;

        if (user == null)
            return;

        var fav = new FavoriteCity
        {
            Id = Guid.NewGuid(),
            City = city,
            UserId = user.Id,
            CreatedAt = DateTime.UtcNow
        };

        await client.From<FavoriteCity>().Insert(fav);
    }

    public async Task RemoveAsync(Guid id)
    {
        var client = await _authClient.GetAuthenticatedClientAsync();

        await client
            .From<FavoriteCity>()
            .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, id.ToString())
            .Delete();
    }
    public async Task<bool> ExistsAsync(string city)
    {
        var client = await _authClient.GetAuthenticatedClientAsync();

        var result = await client
            .From<FavoriteCity>()
            .Filter("city", Supabase.Postgrest.Constants.Operator.Equals, city)
            .Get();

        return result.Models.Any();
    }

}
