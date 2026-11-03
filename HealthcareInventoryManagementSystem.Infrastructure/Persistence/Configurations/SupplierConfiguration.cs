using System;
using HealthcareInventoryManagementSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HealthcareInventoryManagementSystem.Infrastructure.Persistence.Configurations
{
    public class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
    {
        public void Configure(EntityTypeBuilder<Supplier> builder)
        {
            // Table name
            builder.ToTable("Suppliers");

            // Primary key
            builder.HasKey(s => s.SupplierId);

            // Supplier name
            builder.Property(s => s.Name)
                .IsRequired()
                .HasMaxLength(150);

            // Phone number
            builder.Property(s => s.Phone)
                .IsRequired()
                .HasMaxLength(20);

            // Email address
            builder.Property(s => s.Email)
                .IsRequired()
                .HasMaxLength(150);

            // Supplier address
            builder.Property(s => s.Address)
                .IsRequired()
                .HasMaxLength(300);

            // Contact person
            builder.Property(s => s.ContactPerson)
                .HasMaxLength(150);

            // City
            builder.Property(s => s.City)
                .HasMaxLength(100);

            // Country
            builder.Property(s => s.Country)
                .HasMaxLength(100);

            // Tax registration number
            builder.Property(s => s.TaxNumber)
                .HasMaxLength(50);

            // Additional notes
            builder.Property(s => s.Notes)
                .HasMaxLength(500);

            // Supplier status
            builder.Property(s => s.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            // Creation date
            builder.Property(s => s.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            // Update date
            builder.Property(s => s.UpdatedAt)
                .IsRequired(false);
        }
    }
}
