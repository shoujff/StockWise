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
    }
}
