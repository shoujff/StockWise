using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockWise.App.Models;

namespace StockWise.Data.Configurations;

public class InventoryConfiguration : IEntityTypeConfiguration<Inventory>
{
    public void Configure(EntityTypeBuilder<Inventory> builder)
    {
        builder.ToTable("Inventory");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Number).IsRequired();
        builder.Property(x => x.Status).IsRequired();
        builder.Property(x => x.TotalDiff).HasColumnType("decimal(18,2)");
        builder.Property(x => x.Date);

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Warehouse)
            .WithMany(x => x.Inventories)
            .HasForeignKey(x => x.WarehouseId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasData(
            new Inventory { Id = 1, Number = "INV-2026-0001", WarehouseId = 1, Date = new DateTime(2026, 7, 10, 9, 0, 0, 0, DateTimeKind.Utc), Status = "Draft", TotalDiff = -3m, CreatedBy = 3, UserId = 3 }
        );
    }
}

public class InventoryLineConfiguration : IEntityTypeConfiguration<InventoryLine>
{
    public void Configure(EntityTypeBuilder<InventoryLine> builder)
    {
        builder.ToTable("InventoryLine");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.ExpectedQty).HasColumnType("decimal(18,2)");
        builder.Property(x => x.ActualQty).HasColumnType("decimal(18,2)");
        builder.Property(x => x.Diff).HasColumnType("decimal(18,2)");
        builder.Property(x => x.Price).HasColumnType("decimal(18,2)");
        builder.Property(x => x.BatchNo);

        builder.HasOne(x => x.Inventory)
            .WithMany(x => x.Lines)
            .HasForeignKey(x => x.InventoryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Item)
            .WithMany()
            .HasForeignKey(x => x.ItemId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasData(
            new InventoryLine { Id = 1, InventoryId = 1, ItemId = 1, ExpectedQty = 30m, ActualQty = 28m, Diff = -2m, Price = 350m },
            new InventoryLine { Id = 2, InventoryId = 1, ItemId = 5, ExpectedQty = 6m, ActualQty = 6m, Diff = 0m, Price = 3500m },
            new InventoryLine { Id = 3, InventoryId = 1, ItemId = 8, ExpectedQty = 5m, ActualQty = 4m, Diff = -1m, Price = 500m }
        );
    }
}
