using Microsoft.JSInterop;
using Supabase;
using System.Text.Json.Serialization;

namespace WeatherApp.Services
{
    public class AuthenticatedSupabaseClient
    {
        private readonly IJSRuntime _jsRuntime;
        private readonly Client _supabaseClient;

        public AuthenticatedSupabaseClient(IJSRuntime jsRuntime, Client supabaseClient)
        {
            _jsRuntime = jsRuntime;
            _supabaseClient = supabaseClient;
        }

        public async Task<Client> GetAuthenticatedClientAsync()
        {
            try
            {
                var session = await _jsRuntime.InvokeAsync<JsSession?>("sbGetSession");

                if (session != null &&
                    !string.IsNullOrEmpty(session.AccessToken) &&
                    !string.IsNullOrEmpty(session.RefreshToken))
                {
                    await _supabaseClient.Auth.SetSession(
                        session.AccessToken,
                        session.RefreshToken
                    );
                    Console.WriteLine("✅ Supabase C# client authenticated");
                }
                else
                {
                    Console.WriteLine("⚠️ JS session missing or invalid");
                    throw new InvalidOperationException("No valid session available");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error syncing auth token: {ex.Message}");
                throw;
            }

            return _supabaseClient;
        }
    }

    // ✅ Match the JavaScript session object structure
    public class JsSession
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; } = "";

        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; } = "";

        [JsonPropertyName("expires_in")]
        public int? ExpiresIn { get; set; }

        [JsonPropertyName("token_type")]
        public string? TokenType { get; set; }

        [JsonPropertyName("user")]
        public JsUser? User { get; set; }
    }

    public class JsUser
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = "";

        [JsonPropertyName("email")]
        public string Email { get; set; } = "";
    }
}