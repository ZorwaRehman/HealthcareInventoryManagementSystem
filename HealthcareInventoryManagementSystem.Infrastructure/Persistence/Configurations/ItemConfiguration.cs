using System;
using System.Collections.Generic;
using System.Text;
using HealthcareInventoryManagementSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HealthcareInventoryManagementSystem.Infrastructure.Persistence.Configurations
{
    public class ItemConfiguration : IEntityTypeConfiguration<ItemEntity>
    {
        public void Configure(EntityTypeBuilder<ItemEntity> builder)
        {
            builder.ToTable("Items");

            builder.HasKey(item => item.Id);

            builder.Property(item => item.Name)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(item => item.Description)
                .HasMaxLength(500);

            builder.Property(item => item.Category)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(item => item.SKU)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(item => item.SKU)
                .IsUnique();

            builder.Property(item => item.Unit)
                .IsRequired()
                .HasMaxLength(30);

            builder.Property(item => item.ReorderLevel)
                .IsRequired();

            builder.Property(item => item.ImageUrl)
                .HasMaxLength(500);

            builder.Property(item => item.IsActive)
                .HasDefaultValue(true);

            builder.Property(item => item.CreatedAt)
                .IsRequired();
        }
    }
}
