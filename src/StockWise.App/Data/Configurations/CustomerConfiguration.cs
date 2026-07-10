using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockWise.App.Models;

namespace StockWise.Data.Configurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("Customer");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(300).IsRequired();
        builder.Property(x => x.INN).HasMaxLength(20);
        builder.Property(x => x.ContactPerson).HasMaxLength(200);
        builder.Property(x => x.Phone).HasMaxLength(50);
        builder.Property(x => x.Email).HasMaxLength(200);

        builder.HasData(
            new Customer { Id = 1, Name = "ООО Розница", INN = "7701234567", ContactPerson = "Иван Иванов", Phone = "+7 (495) 123-45-67", Email = "info@roznica.ru" },
            new Customer { Id = 2, Name = "ООО ТехноМир", INN = "7702345678", ContactPerson = "Петр Петров", Phone = "+7 (495) 234-56-78", Email = "info@technomir.ru" },
            new Customer { Id = 3, Name = "ИП Сидоров А.В.", INN = "7703456789", ContactPerson = "Алексей Сидоров", Phone = "+7 (495) 345-67-89", Email = "sidorov@mail.ru" }
        );
    }
}
