using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockWise.App.Models;

namespace StockWise.Data.Configurations;

public class DocumentConfiguration : IEntityTypeConfiguration<Document>
{
    public void Configure(EntityTypeBuilder<Document> builder)
    {
        builder.ToTable("Documents");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Type).HasMaxLength(20).IsRequired();
        builder.Property(x => x.Number).HasMaxLength(50).IsRequired();
        builder.Property(x => x.Status).HasMaxLength(20).IsRequired();
        builder.Property(x => x.TotalAmount).HasColumnType("decimal(18,2)");
        builder.Property(x => x.SupplierName).HasMaxLength(300);
        builder.Property(x => x.Date).HasDefaultValueSql("GETUTCDATE()");
        builder.Property(x => x.StockRefDocId);
        builder.HasIndex(x => new { x.Type, x.Number }).IsUnique();

        builder.HasOne(x => x.Customer)
            .WithMany(x => x.Documents)
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.FromWarehouse)
            .WithMany()
            .HasForeignKey(x => x.FromWarehouseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ToWarehouse)
            .WithMany()
            .HasForeignKey(x => x.ToWarehouseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasData(
            new Document { Id = 1, Type = "Income", Number = "IN-2026-0001", Date = new DateTime(2026, 7, 1, 10, 0, 0, DateTimeKind.Utc), Status = "Posted", TotalAmount = 37000m, SupplierName = "ООО Электротех", ToWarehouseId = 1, CreatedBy = 1 },
            new Document { Id = 2, Type = "Outcome", Number = "OUT-2026-0001", Date = new DateTime(2026, 7, 2, 14, 0, 0, DateTimeKind.Utc), Status = "Posted", TotalAmount = 9500m, CustomerId = 1, FromWarehouseId = 1, CreatedBy = 2 },
            new Document { Id = 3, Type = "Transfer", Number = "TRF-2026-0001", Date = new DateTime(2026, 7, 3, 9, 0, 0, DateTimeKind.Utc), Status = "Draft", TotalAmount = 0m, FromWarehouseId = 1, ToWarehouseId = 2, CreatedBy = 1 }
        );
    }
}

public class DocumentLineConfiguration : IEntityTypeConfiguration<DocumentLine>
{
    public void Configure(EntityTypeBuilder<DocumentLine> builder)
    {
        builder.ToTable("DocumentLines");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Quantity).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(x => x.Price).HasColumnType("decimal(18,2)");
        builder.Property(x => x.Amount).HasColumnType("decimal(18,2)");
        builder.Property(x => x.BatchNo).HasMaxLength(100);

        builder.HasOne(x => x.Document)
            .WithMany(x => x.Lines)
            .HasForeignKey(x => x.DocumentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Item)
            .WithMany()
            .HasForeignKey(x => x.ItemId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasData(
            new DocumentLine { Id = 1, DocumentId = 1, ItemId = 1, Quantity = 20, Price = 350m, Amount = 7000m },
            new DocumentLine { Id = 2, DocumentId = 1, ItemId = 2, Quantity = 10, Price = 2500m, Amount = 25000m },
            new DocumentLine { Id = 3, DocumentId = 1, ItemId = 8, Quantity = 10, Price = 500m, Amount = 5000m },
            new DocumentLine { Id = 4, DocumentId = 2, ItemId = 5, Quantity = 2, Price = 3500m, Amount = 7000m },
            new DocumentLine { Id = 5, DocumentId = 2, ItemId = 8, Quantity = 5, Price = 500m, Amount = 2500m },
            new DocumentLine { Id = 6, DocumentId = 3, ItemId = 1, Quantity = 10, Price = 350m, Amount = 3500m },
            new DocumentLine { Id = 7, DocumentId = 3, ItemId = 2, Quantity = 3, Price = 2500m, Amount = 7500m }
        );
    }
}
