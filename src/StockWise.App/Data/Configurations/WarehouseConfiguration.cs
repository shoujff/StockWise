using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockWise.App.Models;

namespace StockWise.Data.Configurations;

public class WarehouseConfiguration : IEntityTypeConfiguration<Warehouse>
{
    public void Configure(EntityTypeBuilder<Warehouse> builder)
    {
        builder.ToTable("Warehouses");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Address).HasMaxLength(500);
        builder.Property(x => x.IsActive).HasDefaultValue(true);

        builder.HasData(
            new Warehouse { Id = 1, Name = "Основной склад", Address = "ул. Ленина, 10", IsActive = true },
            new Warehouse { Id = 2, Name = "Склад №2", Address = "ул. Промышленная, 5", IsActive = true },
            new Warehouse { Id = 3, Name = "Мелкооптовый", Address = "ТЦ \"Гигант\", пав. 12", IsActive = true }
        );
    }
}
