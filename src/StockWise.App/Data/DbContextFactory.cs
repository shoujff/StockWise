using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using StockWise.Data;

namespace StockWise.App.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<StockDb>
{
    public StockDb CreateDbContext(string[] args)
    {
        var json = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json"));
        var settings = System.Text.Json.JsonSerializer.Deserialize<AppSettings>(json)!;
        var optionsBuilder = new DbContextOptionsBuilder<StockDb>();
        optionsBuilder.UseSqlServer(settings.ConnectionStrings.DefaultConnection);
        return new StockDb(optionsBuilder.Options);
    }
}