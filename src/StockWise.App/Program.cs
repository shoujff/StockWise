using Avalonia;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using StockWise.Data;
using System;
using System.IO;
using System.Text.Json;

namespace StockWise.App;

internal class Program
{
    public static IServiceProvider ServiceProvider { get; private set; } = null!;

    [STAThread]
    public static void Main(string[] args)
    {
        var json = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "appsettings.json"));
        var settings = JsonSerializer.Deserialize<AppSettings>(json)!;
        var connString = settings.ConnectionStrings.DefaultConnection;

        var services = new ServiceCollection();
        services.AddDbContext<StockDb>(options =>
            options.UseSqlServer(connString));
        services.AddSingleton(settings);

        ServiceProvider = services.BuildServiceProvider();

        BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);
    }

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}
public class AppSettings
{
    public ConnectionStringsSection ConnectionStrings { get; set; } = new();

    public class ConnectionStringsSection
    {
        public string DefaultConnection { get; set; } = string.Empty;
    }
}