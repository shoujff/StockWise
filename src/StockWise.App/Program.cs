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
            services.AddSingleton<ThemeService>();
            services.AddSingleton<ToastService>();
            services.AddTransient<MainWindow>();
            services.AddTransient<MainWindowViewModel>();
            services.AddTransient<ItemListViewModel>();
            services.AddTransient<ItemEditViewModel>();
            services.AddTransient<CategoryListViewModel>();

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
        }

        if (!context.Categories.Any())
        {
            var electronics = new Category { Name = "Электроника", SortOrder = 1 };
            var construction = new Category { Name = "Строительные материалы", SortOrder = 2 };
            var stationery = new Category { Name = "Канцелярия", SortOrder = 3 };

            context.Categories.AddRange(electronics, construction, stationery);
            context.SaveChanges();

            context.Categories.AddRange(
                new Category { Name = "Бытовая техника", ParentId = electronics.Id, SortOrder = 1 },
                new Category { Name = "Компьютеры", ParentId = electronics.Id, SortOrder = 2 },
                new Category { Name = "Телефоны", ParentId = electronics.Id, SortOrder = 3 },
                new Category { Name = "Электрика", ParentId = construction.Id, SortOrder = 1 },
                new Category { Name = "Сантехника", ParentId = construction.Id, SortOrder = 2 },
                new Category { Name = "Отделочные материалы", ParentId = construction.Id, SortOrder = 3 },
                new Category { Name = "Бумага", ParentId = stationery.Id, SortOrder = 1 },
                new Category { Name = "Ручки", ParentId = stationery.Id, SortOrder = 2 }
            );
        }

        if (!context.Warehouses.Any())
        {
            context.Warehouses.AddRange(
                new Warehouse { Name = "Основной склад", Address = "ул. Ленина, 10", IsActive = true },
                new Warehouse { Name = "Склад №2", Address = "ул. Промышленная, 5", IsActive = true },
                new Warehouse { Name = "Мелкооптовый", Address = "ТЦ \"Гигант\", пав. 12", IsActive = true }
            );
        }

        context.SaveChanges();
    }
}
