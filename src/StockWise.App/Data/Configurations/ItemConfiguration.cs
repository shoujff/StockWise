using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockWise.App.Models;

namespace StockWise.Data.Configurations;

public class ItemConfiguration : IEntityTypeConfiguration<Item>
{
    public void Configure(EntityTypeBuilder<Item> builder)
    {
        builder.ToTable("Items");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(300).IsRequired();
        builder.Property(x => x.Article).HasMaxLength(50).IsRequired();
        builder.HasIndex(x => x.Article).IsUnique();
        builder.Property(x => x.Unit).HasMaxLength(20).IsRequired();
        builder.Property(x => x.Price).HasColumnType("decimal(18,2)");
        builder.Property(x => x.MinStock).HasColumnType("decimal(18,2)");
        builder.Property(x => x.MaxStock).HasColumnType("decimal(18,2)");
        builder.Property(x => x.Barcode).HasMaxLength(50);
        builder.Property(x => x.ImagePath).HasMaxLength(500);
        builder.Property(x => x.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

        builder.HasOne(x => x.Category)
            .WithMany(x => x.Items)
            .HasForeignKey(x => x.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasData(
            new Item { Id = 1, Name = "HDMI кабель 3м", Article = "HDMI-001", Unit = "шт", Price = 350m, MinStock = 5, MaxStock = 50, CategoryId = 3, CreatedAt = new DateTime(2026, 7, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Item { Id = 2, Name = "USB-C Hub 7in1", Article = "USB-HUB-001", Unit = "шт", Price = 2500m, MinStock = 2, MaxStock = 20, CategoryId = 3, CreatedAt = new DateTime(2026, 7, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Item { Id = 3, Name = "Чайник Bosch TWK7201", Article = "BOSCH-T-001", Unit = "шт", Price = 4500m, MinStock = 2, MaxStock = 10, CategoryId = 2, CreatedAt = new DateTime(2026, 7, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Item { Id = 4, Name = "Микроволновка Samsung MW3500", Article = "SAMS-MW-001", Unit = "шт", Price = 12000m, MinStock = 1, MaxStock = 5, CategoryId = 2, CreatedAt = new DateTime(2026, 7, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Item { Id = 5, Name = "Клавиатура Logitech K480", Article = "LOG-KB-001", Unit = "шт", Price = 3500m, MinStock = 3, MaxStock = 15, CategoryId = 3, CreatedAt = new DateTime(2026, 7, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Item { Id = 6, Name = "Мышь Razer DeathAdder", Article = "RAZER-MS-001", Unit = "шт", Price = 2800m, MinStock = 3, MaxStock = 20, CategoryId = 3, CreatedAt = new DateTime(2026, 7, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Item { Id = 7, Name = "Монитор Dell 27\" S2722QC", Article = "DELL-MON-001", Unit = "шт", Price = 35000m, MinStock = 1, MaxStock = 5, CategoryId = 3, CreatedAt = new DateTime(2026, 7, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Item { Id = 8, Name = "Чехол iPhone 15 Pro", Article = "IP15-CASE-001", Unit = "шт", Price = 500m, MinStock = 10, MaxStock = 50, CategoryId = 4, CreatedAt = new DateTime(2026, 7, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Item { Id = 9, Name = "Защитное стекло iPhone 15 Pro", Article = "IP15-GLASS-001", Unit = "шт", Price = 300m, MinStock = 20, MaxStock = 100, CategoryId = 4, CreatedAt = new DateTime(2026, 7, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Item { Id = 10, Name = "Удлинитель 5м 6 розеток", Article = "EXT-5M-001", Unit = "шт", Price = 850m, MinStock = 5, MaxStock = 30, CategoryId = 6, CreatedAt = new DateTime(2026, 7, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Item { Id = 11, Name = "Лампочка LED 12W", Article = "LED-12W-001", Unit = "шт", Price = 150m, MinStock = 20, MaxStock = 100, IsBatch = true, CategoryId = 6, CreatedAt = new DateTime(2026, 7, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Item { Id = 12, Name = "Смеситель Grohe Eurosmart", Article = "GROHE-MIX-001", Unit = "шт", Price = 3500m, MinStock = 2, MaxStock = 10, CategoryId = 7, CreatedAt = new DateTime(2026, 7, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Item { Id = 13, Name = "Шланг душевой 1.5м", Article = "HOSE-1.5-001", Unit = "шт", Price = 450m, MinStock = 5, MaxStock = 30, CategoryId = 7, CreatedAt = new DateTime(2026, 7, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Item { Id = 14, Name = "Шпатлёвка Vetonit 5кг", Article = "VETONIT-001", Unit = "меш", Price = 350m, MinStock = 5, MaxStock = 30, CategoryId = 8, CreatedAt = new DateTime(2026, 7, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Item { Id = 15, Name = "Краска белая 3л", Article = "PAINT-WH-001", Unit = "бан", Price = 1200m, MinStock = 3, MaxStock = 15, CategoryId = 8, CreatedAt = new DateTime(2026, 7, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Item { Id = 16, Name = "Бумага А4 \"Снегурочка\" 500л", Article = "PAPER-A4-001", Unit = "пач", Price = 550m, MinStock = 10, MaxStock = 50, CategoryId = 10, CreatedAt = new DateTime(2026, 7, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Item { Id = 17, Name = "Ручка шариковая синяя", Article = "PEN-BLUE-001", Unit = "шт", Price = 25m, MinStock = 50, MaxStock = 200, CategoryId = 11, CreatedAt = new DateTime(2026, 7, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Item { Id = 18, Name = "Маркер перманентный чёрный", Article = "MARKER-BLK-001", Unit = "шт", Price = 85m, MinStock = 20, MaxStock = 100, CategoryId = 11, CreatedAt = new DateTime(2026, 7, 1, 0, 0, 0, DateTimeKind.Utc) }
        );
    }
}
