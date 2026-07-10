using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockWise.App.Models;

namespace StockWise.Data.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Number).HasMaxLength(50).IsRequired();
        builder.Property(x => x.Status).HasMaxLength(20).IsRequired();
        builder.Property(x => x.TotalAmount).HasColumnType("decimal(18,2)");
        builder.Property(x => x.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
        builder.HasIndex(x => x.Number).IsUnique();

        builder.HasOne(x => x.Customer)
            .WithMany(x => x.Orders)
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasData(
            new Order { Id = 1, Number = "ORD-2026-0001", Status = "New", TotalAmount = 17000m, CustomerId = 1, CreatedBy = 2, CreatedAt = new DateTime(2026, 7, 5, 10, 0, 0, 0, DateTimeKind.Utc) },
            new Order { Id = 2, Number = "ORD-2026-0002", Status = "InProgress", TotalAmount = 8400m, CustomerId = 2, CreatedBy = 1, CreatedAt = new DateTime(2026, 7, 7, 14, 0, 0, 0, DateTimeKind.Utc) },
            new Order { Id = 3, Number = "ORD-2026-0003", Status = "Cancelled", TotalAmount = 1750m, CustomerId = 3, CreatedBy = 2, CreatedAt = new DateTime(2026, 7, 9, 9, 0, 0, 0, DateTimeKind.Utc) }
        );
    }
}

public class OrderLineConfiguration : IEntityTypeConfiguration<OrderLine>
{
    public void Configure(EntityTypeBuilder<OrderLine> builder)
    {
        builder.ToTable("OrderLines");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Quantity).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(x => x.Price).HasColumnType("decimal(18,2)");
        builder.Property(x => x.Amount).HasColumnType("decimal(18,2)");
        builder.Property(x => x.ShippedQty).HasColumnType("decimal(18,2)");

        builder.HasOne(x => x.Order)
            .WithMany(x => x.Lines)
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Item)
            .WithMany()
            .HasForeignKey(x => x.ItemId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasData(
            new OrderLine { Id = 1, OrderId = 1, ItemId = 2, Quantity = 2m, Price = 2500m, Amount = 5000m, ShippedQty = 0m },
            new OrderLine { Id = 2, OrderId = 1, ItemId = 4, Quantity = 1m, Price = 12000m, Amount = 12000m, ShippedQty = 0m },
            new OrderLine { Id = 3, OrderId = 2, ItemId = 6, Quantity = 3m, Price = 2800m, Amount = 8400m, ShippedQty = 0m },
            new OrderLine { Id = 4, OrderId = 3, ItemId = 1, Quantity = 5m, Price = 350m, Amount = 1750m, ShippedQty = 0m }
        );
    }
}

public class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
{
    public void Configure(EntityTypeBuilder<Reservation> builder)
    {
        builder.ToTable("Reservations");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Quantity).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(x => x.Status).HasMaxLength(20).IsRequired();
        builder.Property(x => x.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

        builder.HasOne(x => x.Order)
            .WithMany(x => x.Reservations)
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.StockBalance)
            .WithMany(x => x.Reservations)
            .HasForeignKey(x => x.StockBalanceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasData(
            new Reservation { Id = 1L, OrderId = 1, StockBalanceId = 2L, Quantity = 2m, Status = "Active", CreatedAt = new DateTime(2026, 7, 5, 10, 0, 0, 0, DateTimeKind.Utc) },
            new Reservation { Id = 2L, OrderId = 1, StockBalanceId = 4L, Quantity = 1m, Status = "Active", CreatedAt = new DateTime(2026, 7, 5, 10, 0, 0, 0, DateTimeKind.Utc) },
            new Reservation { Id = 3L, OrderId = 2, StockBalanceId = 6L, Quantity = 3m, Status = "Active", CreatedAt = new DateTime(2026, 7, 7, 14, 0, 0, 0, DateTimeKind.Utc) },
            new Reservation { Id = 4L, OrderId = 3, StockBalanceId = 1L, Quantity = 5m, Status = "Released", CreatedAt = new DateTime(2026, 7, 9, 9, 0, 0, 0, DateTimeKind.Utc) }
        );
    }
}
