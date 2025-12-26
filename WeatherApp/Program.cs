using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using Supabase;
using WeatherApp;
using WeatherApp.Services;
using static WeatherApp.Services.AuthService;


var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");


// ✅ JavaScript-based Supabase Auth (for browser localStorage persistence)
builder.Services.AddScoped<JsSupabaseAuthService>();

// ✅ Authenticated Supabase Client Helper
builder.Services.AddScoped<AuthenticatedSupabaseClient>();

// ✅ C# Supabase Client (for database operations)
builder.Services.AddScoped(_ =>
{
    var options = new SupabaseOptions
    {
        AutoConnectRealtime = false,
        AutoRefreshToken = true
    };
    return new Client(
        "https://ukibpplazxsuqhildshh.supabase.co",
        "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InVraWJwcGxhenhzdXFoaWxkc2hoIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NjQ3MjA2NTEsImV4cCI6MjA4MDI5NjY1MX0.MjZ7OAt7BgYNzUosxi4cAMdMPRt6liAGPgMNV3U_1p0",
        options
    );
});

// ✅ C# AuthService (Singleton - maintains state across app)
builder.Services.AddSingleton<AuthService>();

// Add HttpClient for API calls
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Add MudBlazor services
builder.Services.AddMudServices();

// Add other services
builder.Services.AddScoped<WeatherService>();
builder.Services.AddScoped<RecentSearchService>();
builder.Services.AddScoped<WeatherHistoryService>();
builder.Services.AddScoped<ProfileStateService>();


await builder.Build().RunAsync();