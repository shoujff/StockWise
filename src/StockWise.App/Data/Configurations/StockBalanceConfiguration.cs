using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockWise.App.Models;

namespace StockWise.Data.Configurations;

public class StockBalanceConfiguration : IEntityTypeConfiguration<StockBalance>
{
    public void Configure(EntityTypeBuilder<StockBalance> builder)
    {
        builder.ToTable("StockBalances");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Quantity).HasColumnType("decimal(18,2)");
        builder.Property(x => x.ReservedQty).HasColumnType("decimal(18,2)").HasDefaultValue(0);
        builder.Property(x => x.Price).HasColumnType("decimal(18,2)");
        builder.Property(x => x.BatchNo).HasMaxLength(100);
        builder.Property(x => x.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");

        builder.HasIndex(x => new { x.ItemId, x.WarehouseId, x.BatchNo }).IsUnique()
            .HasFilter("[BatchNo] IS NOT NULL");

        builder.HasOne(x => x.Item)
            .WithMany(x => x.StockBalances)
            .HasForeignKey(x => x.ItemId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Warehouse)
            .WithMany(x => x.StockBalances)
            .HasForeignKey(x => x.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasData(
            new StockBalance { Id = 1, ItemId = 1, WarehouseId = 1, Quantity = 30, ReservedQty = 0, Price = 350m, UpdatedAt = new DateTime(2026, 7, 1, 10, 0, 0, DateTimeKind.Utc) },
            new StockBalance { Id = 2, ItemId = 2, WarehouseId = 1, Quantity = 10, ReservedQty = 2, Price = 2500m, UpdatedAt = new DateTime(2026, 7, 1, 10, 0, 0, DateTimeKind.Utc) },
            new StockBalance { Id = 3, ItemId = 3, WarehouseId = 1, Quantity = 5, ReservedQty = 0, Price = 4500m, UpdatedAt = new DateTime(2026, 7, 1, 10, 0, 0, DateTimeKind.Utc) },
            new StockBalance { Id = 4, ItemId = 4, WarehouseId = 1, Quantity = 2, ReservedQty = 1, Price = 12000m, UpdatedAt = new DateTime(2026, 7, 1, 10, 0, 0, DateTimeKind.Utc) },
            new StockBalance { Id = 5, ItemId = 5, WarehouseId = 1, Quantity = 6, ReservedQty = 0, Price = 3500m, UpdatedAt = new DateTime(2026, 7, 2, 14, 0, 0, DateTimeKind.Utc) },
            new StockBalance { Id = 6, ItemId = 6, WarehouseId = 2, Quantity = 15, ReservedQty = 3, Price = 2800m, UpdatedAt = new DateTime(2026, 7, 1, 10, 0, 0, DateTimeKind.Utc) },
            new StockBalance { Id = 7, ItemId = 7, WarehouseId = 2, Quantity = 3, ReservedQty = 0, Price = 35000m, UpdatedAt = new DateTime(2026, 7, 1, 10, 0, 0, DateTimeKind.Utc) },
            new StockBalance { Id = 8, ItemId = 8, WarehouseId = 1, Quantity = 5, ReservedQty = 0, Price = 500m, UpdatedAt = new DateTime(2026, 7, 2, 14, 0, 0, DateTimeKind.Utc) },
            new StockBalance { Id = 9, ItemId = 9, WarehouseId = 1, Quantity = 60, ReservedQty = 0, Price = 300m, UpdatedAt = new DateTime(2026, 7, 1, 10, 0, 0, DateTimeKind.Utc) },
            new StockBalance { Id = 10, ItemId = 10, WarehouseId = 1, Quantity = 15, ReservedQty = 0, Price = 850m, UpdatedAt = new DateTime(2026, 7, 1, 10, 0, 0, DateTimeKind.Utc) },
            new StockBalance { Id = 11, ItemId = 11, WarehouseId = 1, Quantity = 50, ReservedQty = 0, Price = 150m, BatchNo = "LED-2026-01", ExpiryDate = new DateOnly(2026, 12, 31), UpdatedAt = new DateTime(2026, 7, 1, 10, 0, 0, DateTimeKind.Utc) },
            new StockBalance { Id = 12, ItemId = 11, WarehouseId = 1, Quantity = 30, ReservedQty = 0, Price = 160m, BatchNo = "LED-2026-02", ExpiryDate = new DateOnly(2027, 6, 30), UpdatedAt = new DateTime(2026, 7, 1, 10, 0, 0, DateTimeKind.Utc) },
            new StockBalance { Id = 13, ItemId = 12, WarehouseId = 2, Quantity = 4, ReservedQty = 0, Price = 3500m, UpdatedAt = new DateTime(2026, 7, 1, 10, 0, 0, DateTimeKind.Utc) },
            new StockBalance { Id = 14, ItemId = 13, WarehouseId = 2, Quantity = 12, ReservedQty = 0, Price = 450m, UpdatedAt = new DateTime(2026, 7, 1, 10, 0, 0, DateTimeKind.Utc) },
            new StockBalance { Id = 15, ItemId = 14, WarehouseId = 2, Quantity = 8, ReservedQty = 0, Price = 350m, UpdatedAt = new DateTime(2026, 7, 1, 10, 0, 0, DateTimeKind.Utc) },
            new StockBalance { Id = 16, ItemId = 15, WarehouseId = 2, Quantity = 3, ReservedQty = 0, Price = 1200m, UpdatedAt = new DateTime(2026, 7, 1, 10, 0, 0, DateTimeKind.Utc) },
            new StockBalance { Id = 17, ItemId = 16, WarehouseId = 1, Quantity = 20, ReservedQty = 0, Price = 550m, UpdatedAt = new DateTime(2026, 7, 1, 10, 0, 0, DateTimeKind.Utc) },
            new StockBalance { Id = 18, ItemId = 16, WarehouseId = 3, Quantity = 30, ReservedQty = 0, Price = 520m, UpdatedAt = new DateTime(2026, 7, 1, 10, 0, 0, DateTimeKind.Utc) },
            new StockBalance { Id = 19, ItemId = 17, WarehouseId = 3, Quantity = 150, ReservedQty = 0, Price = 25m, UpdatedAt = new DateTime(2026, 7, 1, 10, 0, 0, DateTimeKind.Utc) },
            new StockBalance { Id = 20, ItemId = 18, WarehouseId = 3, Quantity = 80, ReservedQty = 0, Price = 85m, UpdatedAt = new DateTime(2026, 7, 1, 10, 0, 0, DateTimeKind.Utc) },
            new StockBalance { Id = 21, ItemId = 1, WarehouseId = 2, Quantity = 20, ReservedQty = 0, Price = 350m, UpdatedAt = new DateTime(2026, 7, 1, 10, 0, 0, DateTimeKind.Utc) }
        );
    }
}
