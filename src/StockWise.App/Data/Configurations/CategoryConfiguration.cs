using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockWise.App.Models;

namespace StockWise.Data.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.SortOrder).HasDefaultValue(0);

        builder.HasOne(x => x.Parent)
            .WithMany(x => x.Children)
            .HasForeignKey(x => x.ParentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasData(
            new Category { Id = 1, Name = "Электроника", SortOrder = 1 },
            new Category { Id = 2, Name = "Бытовая техника", ParentId = 1, SortOrder = 1 },
            new Category { Id = 3, Name = "Компьютеры", ParentId = 1, SortOrder = 2 },
            new Category { Id = 4, Name = "Телефоны", ParentId = 1, SortOrder = 3 },
            new Category { Id = 5, Name = "Строительные материалы", SortOrder = 2 },
            new Category { Id = 6, Name = "Электрика", ParentId = 5, SortOrder = 1 },
            new Category { Id = 7, Name = "Сантехника", ParentId = 5, SortOrder = 2 },
            new Category { Id = 8, Name = "Отделочные материалы", ParentId = 5, SortOrder = 3 },
            new Category { Id = 9, Name = "Канцелярия", SortOrder = 3 },
            new Category { Id = 10, Name = "Бумага", ParentId = 9, SortOrder = 1 },
            new Category { Id = 11, Name = "Ручки", ParentId = 9, SortOrder = 2 }
        );
    }
}
