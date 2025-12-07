using WeatherApp.Models;

namespace WeatherApp.Services;

public class RecentSearchService
{
    private readonly AuthService _auth;

    public RecentSearchService(AuthService auth)
    {
        _auth = auth;
    }

    public bool CanUse => _auth.IsLoggedIn && _auth.CurrentUser != null;

    public async Task AddAsync(string city)
    {
        if (!CanUse || string.IsNullOrWhiteSpace(city))
            return;

        var userId = Guid.Parse(_auth.CurrentUser!.Id);

        var item = new RecentSearch
        {
            UserId = userId,
            City = city.Trim(),
            CreatedAt = DateTime.UtcNow   // ✅ ADD THIS LINE
        };


        await _auth.Client.From<RecentSearch>().Insert(item);
    }

    public async Task<List<RecentSearch>> GetAsync()
    {
        if (!CanUse)
            return new List<RecentSearch>();

        var userId = Guid.Parse(_auth.CurrentUser!.Id);

        var result = await _auth.Client
            .From<RecentSearch>()
            .Where(r => r.UserId == userId)
            .Order(r => r.CreatedAt, Supabase.Postgrest.Constants.Ordering.Descending)
            .Get();

        return result.Models;
    }

    public async Task DeleteAsync(Guid id)
    {
        if (!CanUse)
            return;

        // RLS ensures user can delete only their own row
        await _auth.Client
            .From<RecentSearch>()
            .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, id.ToString())
            .Delete();
    }

}
