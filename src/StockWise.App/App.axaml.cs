using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using StockWise.App.Views;
using StockWise.App.Services;

namespace StockWise.App;

public partial class App : Application
{
    public static IServiceProvider? Services { get; set; }

    public override void Initialize()
    {
        AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
        {
            File.AppendAllText("crash.log",
                $"[{DateTime.Now}] Unhandled: {args.ExceptionObject}{Environment.NewLine}");
        };
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var themeService = Services!.GetRequiredService<ThemeService>();
        themeService.Initialize();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var authService = Services!.GetRequiredService<IAuthService>();
            var loginWindow = new LoginWindow(authService);
            desktop.MainWindow = loginWindow;
            loginWindow.Show();
        }

        base.OnFrameworkInitializationCompleted();
    }
}
