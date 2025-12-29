using Supabase;
using WeatherApp.Models;

namespace WeatherApp.Services
{
    public class RecentSearchService
    {
        private readonly AuthenticatedSupabaseClient _authClient;

        public RecentSearchService(AuthenticatedSupabaseClient authClient)
        {
            _authClient = authClient;
        }

        public async Task<List<RecentSearch>> GetAsync()
        {
            var client = await _authClient.GetAuthenticatedClientAsync();

            var response = await client
                .From<RecentSearch>()
                .Order("created_at", Supabase.Postgrest.Constants.Ordering.Descending)
                .Get();

            return response.Models;
        }

        public async Task AddAsync(string city)
        {
            var client = await _authClient.GetAuthenticatedClientAsync();
            var user = client.Auth.CurrentUser;

            if (user == null)
                return;

            var search = new RecentSearch
            {
                Id = Guid.NewGuid(),
                City = city,
                CreatedAt = DateTime.UtcNow,
                UserId = user.Id
            };

            await client
                .From<RecentSearch>()
                .Insert(search);
        }

        public async Task DeleteAsync(Guid id)
        {
            var client = await _authClient.GetAuthenticatedClientAsync();

            await client
                .From<RecentSearch>()
                .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, id.ToString())
                .Delete();
        }


    }
}
