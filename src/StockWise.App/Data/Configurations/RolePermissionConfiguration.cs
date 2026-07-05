using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockWise.App.Models;

namespace StockWise.Data.Configurations;

public class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder.ToTable("RolePermissions");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Role).HasMaxLength(50).IsRequired();
        builder.Property(x => x.Permission).HasMaxLength(100).IsRequired();
        builder.HasIndex(x => new { x.Role, x.Permission }).IsUnique();

        builder.HasData(
            // Admin — полный доступ
            new RolePermission { Id = 1, Role = "Admin", Permission = "Items.Create" },
            new RolePermission { Id = 2, Role = "Admin", Permission = "Items.Edit" },
            new RolePermission { Id = 3, Role = "Admin", Permission = "Items.Delete" },
            new RolePermission { Id = 4, Role = "Admin", Permission = "Items.View" },
            new RolePermission { Id = 5, Role = "Admin", Permission = "Warehouse.Create" },
            new RolePermission { Id = 6, Role = "Admin", Permission = "Warehouse.Edit" },
            new RolePermission { Id = 7, Role = "Admin", Permission = "Warehouse.View" },
            new RolePermission { Id = 8, Role = "Admin", Permission = "Documents.Create" },
            new RolePermission { Id = 9, Role = "Admin", Permission = "Documents.Post" },
            new RolePermission { Id = 10, Role = "Admin", Permission = "Documents.View" },
            new RolePermission { Id = 11, Role = "Admin", Permission = "Documents.Cancel" },
            new RolePermission { Id = 12, Role = "Admin", Permission = "Orders.Create" },
            new RolePermission { Id = 13, Role = "Admin", Permission = "Orders.Edit" },
            new RolePermission { Id = 14, Role = "Admin", Permission = "Orders.View" },
            new RolePermission { Id = 15, Role = "Admin", Permission = "Reports.View" },
            new RolePermission { Id = 16, Role = "Admin", Permission = "Admin.Users" },
            // Manager
            new RolePermission { Id = 17, Role = "Manager", Permission = "Items.Create" },
            new RolePermission { Id = 18, Role = "Manager", Permission = "Items.Edit" },
            new RolePermission { Id = 19, Role = "Manager", Permission = "Items.View" },
            new RolePermission { Id = 20, Role = "Manager", Permission = "Documents.Create" },
            new RolePermission { Id = 21, Role = "Manager", Permission = "Documents.Post" },
            new RolePermission { Id = 22, Role = "Manager", Permission = "Documents.View" },
            new RolePermission { Id = 23, Role = "Manager", Permission = "Orders.Create" },
            new RolePermission { Id = 24, Role = "Manager", Permission = "Orders.Edit" },
            new RolePermission { Id = 25, Role = "Manager", Permission = "Orders.View" },
            new RolePermission { Id = 26, Role = "Manager", Permission = "Reports.View" },
            // Warehouse
            new RolePermission { Id = 27, Role = "Warehouse", Permission = "Items.View" },
            new RolePermission { Id = 28, Role = "Warehouse", Permission = "Warehouse.View" },
            new RolePermission { Id = 29, Role = "Warehouse", Permission = "Documents.View" },
            new RolePermission { Id = 30, Role = "Warehouse", Permission = "Inventory.Create" },
            new RolePermission { Id = 31, Role = "Warehouse", Permission = "Inventory.Edit" },
            // Viewer
            new RolePermission { Id = 32, Role = "Viewer", Permission = "Items.View" },
            new RolePermission { Id = 33, Role = "Viewer", Permission = "Warehouse.View" },
            new RolePermission { Id = 34, Role = "Viewer", Permission = "Documents.View" },
            new RolePermission { Id = 35, Role = "Viewer", Permission = "Reports.View" }
        );
    }
}
