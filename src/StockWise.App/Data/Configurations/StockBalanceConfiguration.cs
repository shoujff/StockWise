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
    }
}
