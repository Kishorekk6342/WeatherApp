using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

using Supabase;                                 // Supabase.Client, SupabaseOptions
using Supabase.Gotrue;                          // SignUpOptions, SignIn/SignUp API surface
using Supabase.Gotrue.Exceptions;               // GotrueException
using Supabase.Postgrest;                       // From<>, Get/Update helpers
using Supabase.Postgrest.Exceptions;            // PostgrestException
using static Supabase.Postgrest.Constants;      // Operator enum
using WeatherApp.Models;


namespace WeatherApp.Services
{
    public class ProfileStateService
    {
        public string? Username { get; private set; }
        public string? AvatarUrl { get; private set; }

        public event Action? OnChange;

        public void SetProfile(string? username, string? avatarUrl)
        {
            Username = username;
            AvatarUrl = avatarUrl;
            OnChange?.Invoke();
        }
    }
}

// Profile model

namespace WeatherApp.Services
{
    // Simple profile object we keep in memory (you already had this, kept for convenience)
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

        // TODO: keep keys out of source for production
         public bool IsInitialized { get; private set; }

        public AuthService(Supabase.Client client)
        {
            _client = client;
        }




        public async Task InitializeAsync()
        {
            try
            {
                await _client.InitializeAsync();

                var user = _client.Auth.CurrentUser;
                if (user != null)
                {
                    SetFromUser(user);
                    await LoadProfileAsync();
                }
            }
            catch (Exception ex)
            {
                LastError = ex.Message;
                Console.WriteLine("AuthService.InitializeAsync error: " + ex.Message);
            }
            finally
            {
                IsInitialized = true;
            }
        }


        private void SetFromUser(Supabase.Gotrue.User user)
        {
            CurrentUser = user;

            var email = user.Email ?? "user@unknown.com";
            var fullName = email;

            if (user.UserMetadata != null &&
                user.UserMetadata.TryGetValue("full_name", out var nameObj) &&
                nameObj != null)
            {
                fullName = nameObj.ToString()!;
            }

            CurrentProfile = new AuthProfile
            {
                Id = user.Id,
                Email = email,
                FullName = fullName
            };
        }



        // ---------------- REGISTER ----------------
        // ---------------- REGISTER ----------------
        public async Task<bool> RegisterAsync(string email, string password, string username)
        {
            LastError = null;

            try
            {
                var options = new Supabase.Gotrue.SignUpOptions
                {
                    Data = new Dictionary<string, object>
                    {
                        ["username"] = username,
                        ["full_name"] = username // Can be updated later in profile
                    }
                };

                var result = await _client.Auth.SignUp(email, password, options);

                if (result.User == null)
                {
                    LastError = "Sign-up failed.";
                    return false;
                }

                // The trigger will automatically create the profile
                Console.WriteLine($"✅ User registered: {email}");

                // Don't auto-login here. Return true to indicate registration initiated.
                return true;
            }
            catch (GotrueException ex)
            {
                LastError = ex.Message;
                Console.WriteLine($"❌ Registration error: {ex.Message}");
                return false;
            }
        }


        // ---------------- LOGIN ----------------
        public async Task<bool> LoginAsync(string email, string password)
        {
            LastError = null;

            try
            {
                // SignIn may throw GotrueException on invalid credentials
                var session = await _client.Auth.SignIn(email, password);

                if (session?.User == null)
                {
                    LastError = "Invalid email or password.";
                    return false;
                }

                SetFromUser(session.User);

                // load profile row to populate CurrentProfile fuller data from profiles table
                await LoadProfileAsync();

                return true;
            }
            catch (GotrueException ex)
            {
                // Bubble up confirmation-specific error to UI if needed
                var msg = ex.Message ?? string.Empty;

                if (msg.Contains("email_not_confirmed", StringComparison.OrdinalIgnoreCase))
                {
                    // store last error and rethrow so UI can treat specially
                    LastError = ex.Message;
                    throw;
                }

                LastError = ex.Message;
                return false;
            }
        }

        // ---------------- LOGOUT ----------------
        public async Task LogoutAsync()
        {
            LastError = null;

            try
            {
                await _client.Auth.SignOut();
            }
            catch
            {
                // ignore signout errors
            }

            CurrentUser = null;
            CurrentProfile = null;
        }

        // ---------------- Load profile row from "profiles" table ----------------
        // Attempts to load the public.profiles row for the authenticated user.
        private async Task LoadProfileAsync()
        {
            if (CurrentUser == null)
            {
                CurrentProfile = null;
                return;
            }

            try
            {
                // CurrentUser.Id is already the UUID string; do not pass a Guid object
                var userId = CurrentUser.Id;

                var resp = await _client
                    .From<ProfileModel>()
                    .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, userId)
                    .Get();

                var model = resp?.Models?.FirstOrDefault();
                if (model != null)
                {
                    CurrentProfile = new AuthProfile
                    {
                        Id = model.Id.ToString(),
                        Email = model.Email ?? CurrentUser.Email,
                    };
                }
                else
                {
                    SetFromUser(CurrentUser);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("LoadProfileAsync error: " + ex.Message);
                if (CurrentUser != null)
                    SetFromUser(CurrentUser);
            }
        }


        // ---------------- Update profile (full_name) in public.profiles ----------------
        public async Task<bool> UpdateProfileAsync(string fullName)
        {
            if (CurrentUser == null)
                return false;

            try
            {
                var userId = CurrentUser.Id; // keep as string

                if (!Guid.TryParse(userId, out var guid))
                    return false;

                var patchProfile = new ProfileModel
                {
                    Id = guid,
                };


                var resp = await _client
                    .From<ProfileModel>()
                    .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, userId)  // pass id as string
                    .Update(patchProfile);

                await LoadProfileAsync();
                return true;
            }
            catch (PostgrestException pex)
            {
                LastError = pex.Message;
                Console.WriteLine($"Profile update Postgrest error: {pex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                LastError = ex.Message;
                Console.WriteLine($"Profile update error: {ex.Message}");
                return false;
            }
        }

    }
}
    