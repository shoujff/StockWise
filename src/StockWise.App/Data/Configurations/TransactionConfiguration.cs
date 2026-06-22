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
    }
}
