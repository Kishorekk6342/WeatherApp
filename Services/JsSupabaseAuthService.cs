using Microsoft.JSInterop;

namespace WeatherApp.Services
{
    public class JsSupabaseAuthService
    {
        private readonly IJSRuntime _js;

        public JsSupabaseAuthService(IJSRuntime js)
        {
            _js = js;
        }

        /// <summary>
        /// Login user with email and password
        /// </summary>
        public async Task<UserDto?> LoginAsync(string email, string password)
        {
            try
            {
                var user = await _js.InvokeAsync<UserDto?>("sbLogin", email, password);

                if (user != null)
                {
                    Console.WriteLine($"✅ JS Login successful: {user.Email}");
                }

                return user;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ JS Login error: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Register new user with email, password, and username
        /// </summary>
        public async Task<bool> RegisterAsync(string email, string password, string username)
        {
            try
            {
                var user = await _js.InvokeAsync<UserDto?>("sbRegister", email, password, username);

                if (user != null)
                {
                    Console.WriteLine($"✅ JS Registration successful: {user.Email}");
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ JS Registration error: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Logout current user
        /// </summary>
        public async Task LogoutAsync()
        {
            try
            {
                await _js.InvokeVoidAsync("sbLogout");
                Console.WriteLine("✅ Logout successful");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Logout error: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Get current logged-in user
        /// </summary>
        public async Task<UserDto?> GetCurrentUserAsync()
        {
            try
            {
                var user = await _js.InvokeAsync<UserDto?>("sbGetUser");
                return user;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Get user error: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Get current session
        /// </summary>
        public async Task<SessionDto?> GetSessionAsync()
        {
            try
            {
                var session = await _js.InvokeAsync<SessionDto?>("sbGetSession");
                return session;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Get session error: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Get access token for authenticated requests
        /// </summary>
        public async Task<string?> GetAccessTokenAsync()
        {
            try
            {
                var token = await _js.InvokeAsync<string?>("sbGetAccessToken");
                return token;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Get access token error: {ex.Message}");
                return null;
            }
        }
    }

    /// <summary>
    /// DTO for User data from Supabase
    /// </summary>
    public class UserDto
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastSignInAt { get; set; }
    }

    /// <summary>
    /// DTO for Session data
    /// </summary>
    public class SessionDto
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public UserDto? User { get; set; }
        public int ExpiresIn { get; set; }
    }
}