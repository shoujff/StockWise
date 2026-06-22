using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using StockWise.App.ViewModels;
using StockWise.Data;

namespace StockWise.App;

public partial class App : Application
{

    public static IServiceProvider? Services { get; private set; }
    public static IConfiguration? Configuration { get; private set; }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {

        var host = CreateHostBuilder().Build();
        Services = host.Services;
        Configuration = Services.GetRequiredService<IConfiguration>();
        InitializeDatabase();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var mainWindow = Services.GetRequiredService<MainWindow>();
            desktop.MainWindow = mainWindow;
        }

        base.OnFrameworkInitializationCompleted();
    }
    private static IHostBuilder CreateHostBuilder()
    {
        return Host.CreateDefaultBuilder().ConfigureAppConfiguration((context, config) =>
        {
            config.SetBasePath(AppContext.BaseDirectory);
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                config.AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", optional: true);
                config.AddEnvironmentVariables();
        })
        .ConfigureServices((context, services) =>
        {
                var configuration = context.Configuration;
                services.AddSingleton(configuration);
                services.AddSingleton<MainWindow>();
                services.AddSingleton<MainWindowViewModel>();
                // TODO: Добавить остальные сервисы в следующих задачах
                services.AddDbContext<StockDb>();
                // services.AddScoped<IAuthService, AuthService>();
        });
    }
    private static void InitializeDatabase()
    {
    // TODO: Добавить в задаче 0.3
    // using var scope = Services.CreateScope();
    // var context = scope.ServiceProvider.GetRequiredService<StockDbContext>();
    // context.Database.EnsureCreated();
    }
}
