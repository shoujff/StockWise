using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockWise.App.Models;

namespace StockWise.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.FirstName).HasMaxLength(100).IsRequired();
        builder.Property(x => x.LastName).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Login).HasMaxLength(50).IsRequired();
        builder.HasIndex(x => x.Login).IsUnique();
        builder.Property(x => x.PasswordHash).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Role).HasMaxLength(20).IsRequired();

        builder.HasData(
            new User { Id = 1, FirstName = "Администратор", LastName = "", Login = "admin", PasswordHash = "$2a$10$v7tufXv/BzNs2LFs1YlNX.M6limATLB.C5ZySCXDiK4ev8D/Nu4Qy", Role = "Admin" },
            new User { Id = 2, FirstName = "Иван", LastName = "Петров", Login = "manager", PasswordHash = "$2a$10$b/irvmKoiR3SRXJKOFpZUebCb9267sh9lMWDroW1rgtk5T2ByUXlK", Role = "Manager" },
            new User { Id = 3, FirstName = "Сергей", LastName = "Иванов", Login = "warehouse", PasswordHash = "$2a$10$aTcj/v7LGvvOc4r1Qs5nqOm7wUMrFKjO4luqNBSfRlU0Ul3uJBcXK", Role = "Warehouse" },
            new User { Id = 4, FirstName = "Анна", LastName = "Сидорова", Login = "viewer", PasswordHash = "$2a$10$.syaB/VMqXxB.xM8ZF4FBOef/sqnbAuXXLzkXCJZ8pwLm3QJgVI1y", Role = "Viewer" }
        );
    }
}
