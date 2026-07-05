using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockWise.App.Models;

namespace StockWise.Data.Configurations;

public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.ToTable("Transactions");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Type).HasMaxLength(20).IsRequired();
        builder.Property(x => x.Quantity).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(x => x.Direction).HasMaxLength(1).IsRequired();
        builder.Property(x => x.Price).HasColumnType("decimal(18,2)");
        builder.Property(x => x.BatchNo).HasMaxLength(100);
        builder.Property(x => x.RefDocType).HasMaxLength(20);
        builder.Property(x => x.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

        builder.HasOne(x => x.Item)
            .WithMany(x => x.Transactions)
            .HasForeignKey(x => x.ItemId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Warehouse)
            .WithMany(x => x.Transactions)
            .HasForeignKey(x => x.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.User)
            .WithMany(x => x.Transactions)
            .HasForeignKey(x => x.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasData(
            new Transaction { Id = 1001, Type = "Income", Direction = "+", Quantity = 20, Price = 350m, ItemId = 1, WarehouseId = 1, CreatedBy = 1, RefDocId = 1, RefDocType = "Income", CreatedAt = new DateTime(2026, 7, 1, 10, 0, 0, DateTimeKind.Utc) },
            new Transaction { Id = 1002, Type = "Income", Direction = "+", Quantity = 10, Price = 2500m, ItemId = 2, WarehouseId = 1, CreatedBy = 1, RefDocId = 1, RefDocType = "Income", CreatedAt = new DateTime(2026, 7, 1, 10, 0, 0, DateTimeKind.Utc) },
            new Transaction { Id = 1003, Type = "Income", Direction = "+", Quantity = 10, Price = 500m, ItemId = 8, WarehouseId = 1, CreatedBy = 1, RefDocId = 1, RefDocType = "Income", CreatedAt = new DateTime(2026, 7, 1, 10, 0, 0, DateTimeKind.Utc) },
            new Transaction { Id = 1004, Type = "Outcome", Direction = "-", Quantity = 2, Price = 3500m, ItemId = 5, WarehouseId = 1, CreatedBy = 2, RefDocId = 2, RefDocType = "Outcome", CreatedAt = new DateTime(2026, 7, 2, 14, 0, 0, DateTimeKind.Utc) },
            new Transaction { Id = 1005, Type = "Outcome", Direction = "-", Quantity = 5, Price = 500m, ItemId = 8, WarehouseId = 1, CreatedBy = 2, RefDocId = 2, RefDocType = "Outcome", CreatedAt = new DateTime(2026, 7, 2, 14, 0, 0, DateTimeKind.Utc) }
        );
    }
}
