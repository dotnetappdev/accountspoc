using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using ZXing.Net.Maui.Controls;
using Microsoft.EntityFrameworkCore;
using AccountsPOC.MauiApp.Data;
using AccountsPOC.MauiApp.Services;

namespace AccountsPOC.MauiApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseBarcodeReader()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        // Add Blazor WebView
        builder.Services.AddMauiBlazorWebView();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        // Configure SQLite database for offline storage
        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "accountspoc.db3");
        builder.Services.AddDbContext<LocalDbContext>(options =>
            options.UseSqlite($"Data Source={dbPath}"));

        // Register services
        builder.Services.AddSingleton(Connectivity.Current);
        builder.Services.AddScoped<ISyncService, SyncService>();

        // Register HTTP client for API calls
        builder.Services.AddHttpClient("AccountsPOCAPI", client =>
        {
            // Configure your API base address here
            // For development, you might use: http://10.0.2.2:5000 for Android emulator
            // or https://your-api-url.com for production
            client.BaseAddress = new Uri("https://localhost:5001/");
        });

        return builder.Build();
    }
}
