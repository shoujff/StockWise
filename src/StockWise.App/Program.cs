using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using StockWise.App.Models;
using StockWise.App.ViewModels;
using StockWise.App.Views;
using StockWise.App.Repositories;
using StockWise.App.Services;
using StockWise.Data;

namespace StockWise.App;

internal class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();
        App.Services = host.Services;

        InitializeDatabase();

        BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);
    }

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();

    private static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args).ConfigureAppConfiguration((context, config) =>
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

            var connString = configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<StockDb>(options =>
                options.UseSqlServer(connString));

            services.AddSingleton<DapperContext>();

            services.AddScoped<UserRepository>();
            services.AddScoped<CategoryRepository>();
            services.AddScoped<ItemRepository>();
            services.AddScoped<WarehouseRepository>();
            services.AddScoped<StockBalanceRepository>();
            services.AddScoped<TransactionRepository>();
            services.AddScoped<RolePermissionRepository>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IItemService, ItemService>();
        });
    }

    private static void InitializeDatabase()
    {
        using var scope = App.Services!.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<StockDb>();
        context.Database.Migrate();

        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var adminLogin = configuration.GetSection("Auth:DefaultAdminUsername").Get<string>() ?? "admin";
        var adminPassword = configuration.GetSection("Auth:DefaultAdminPassword").Get<string>() ?? "Admin123!";

        if (!context.Users.Any(u => u.Login == adminLogin))
        {
            context.Users.Add(new User
            {
                Login = adminLogin,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(adminPassword, workFactor: 12),
                FirstName = "Администратор",
                LastName = "",
                Role = "Admin"
            });
            context.SaveChanges();
        }
    }
}
