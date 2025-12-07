using Supabase;
using Supabase.Gotrue.Exceptions;

namespace WeatherApp.Services;

// Simple profile object we keep in memory
public class AuthProfile
{
    public string? Id { get; set; }
    public string? Email { get; set; }
    public string? FullName { get; set; }
}

public class AuthService
{
    private readonly Supabase.Client _client;
    public Supabase.Client Client => _client;

    public bool IsLoggedIn => CurrentProfile is not null;
    public Supabase.Gotrue.User? CurrentUser { get; private set; }
    public AuthProfile? CurrentProfile { get; private set; }
    public string? LastError { get; private set; }

    private const string SupabaseUrl = "https://ukibpplazxsuqhildshh.supabase.co";
    private const string SupabaseAnonKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InVraWJwcGxhenhzdXFoaWxkc2hoIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NjQ3MjA2NTEsImV4cCI6MjA4MDI5NjY1MX0.MjZ7OAt7BgYNzUosxi4cAMdMPRt6liAGPgMNV3U_1p0";

    public AuthService()
    {
        var options = new SupabaseOptions
        {
            AutoRefreshToken = true,
            AutoConnectRealtime = false
        };

        _client = new Supabase.Client(SupabaseUrl, SupabaseAnonKey, options);

        // Initialize client & restore session if exists
        _ = InitializeAsync();
    }

    private async Task InitializeAsync()
    {
        await _client.InitializeAsync();

        var user = _client.Auth.CurrentUser;
        if (user != null)
        {
            SetFromUser(user);
        }
    }

    private void SetFromUser(Supabase.Gotrue.User user)
    {
        CurrentUser = user;

        string? fullName = null;

        if (user.UserMetadata != null &&
            user.UserMetadata.TryGetValue("full_name", out var fullNameObj))
        {
            fullName = fullNameObj?.ToString();
        }

        if (string.IsNullOrWhiteSpace(fullName))
        {
            fullName = user.Email ?? "User";
        }

        CurrentProfile = new AuthProfile
        {
            Id = user.Id,
            Email = user.Email,
            FullName = fullName
        };
    }

    // ---------------- REGISTER ----------------

    public async Task<bool> RegisterAsync(string email, string password, string fullName)
    {
        LastError = null;

        try
        {
            var options = new Supabase.Gotrue.SignUpOptions
            {
                Data = new Dictionary<string, object>
                {
                    ["full_name"] = fullName
                }
            };

            var result = await _client.Auth.SignUp(email, password, options);

            if (result.User == null)
            {
                LastError = "Sign-up failed.";
                return false;
            }

            return true;
        }
        catch (GotrueException ex)
        {
            LastError = ex.Message;
            return false;
        }
    }

    // ---------------- LOGIN (UPDATED) ----------------

    public async Task<bool> LoginAsync(string email, string password)
    {
        LastError = null;

        try
        {
            var session = await _client.Auth.SignIn(email, password);

            if (session?.User == null)
            {
                LastError = "Invalid email or password.";
                return false;
            }

            SetFromUser(session.User);
            return true;
        }
        catch (GotrueException ex)
        {
            var msg = ex.Message ?? string.Empty;

            // 🔴 IMPORTANT: let the UI see this specific error
            if (msg.Contains("email_not_confirmed", StringComparison.OrdinalIgnoreCase))
                throw;

            // For invalid_credentials and other normal login errors,
            // we swallow the exception and just return false.
            LastError = ex.Message;
            return false;
        }
    }

    // ---------------- LOGOUT ----------------

    public async Task LogoutAsync()
    {
        LastError = null;

        await _client.Auth.SignOut();
        CurrentUser = null;
        CurrentProfile = null;
    }
}
