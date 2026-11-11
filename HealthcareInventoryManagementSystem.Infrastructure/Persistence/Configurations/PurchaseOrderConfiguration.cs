using HealthcareInventoryManagementSystem.Domain.Enums;
using HealthcareInventoryManagementSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HealthcareInventoryManagementSystem.Infrastructure.Persistence.Configurations
{
    public class PurchaseOrderConfiguration : IEntityTypeConfiguration<PurchaseOrderEntity>
    {
        public void Configure(EntityTypeBuilder<PurchaseOrderEntity> builder)
        {
            // Set the database table name.
            builder.ToTable("PurchaseOrders");

            // Configure the primary key.
            builder.HasKey(po => po.PurchaseOrderId);

            // Configure the unique purchase order number.
            builder.Property(po => po.OrderNumber)
                .IsRequired()
                .HasMaxLength(40);

            builder.HasIndex(po => po.OrderNumber)
                .IsUnique();

            // Configure the supplier relationship.
            builder.HasOne(po => po.Supplier)
                .WithMany()
                .HasForeignKey(po => po.SupplierId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure order dates.
            builder.Property(po => po.OrderDate)
                .IsRequired();

            builder.Property(po => po.ExpectedDeliveryDate)
                .IsRequired(false);

            // Store the enum status as an integer.
            builder.Property(po => po.Status)
                .IsRequired()
                .HasConversion<int>();

            // Configure financial amounts.
            builder.Property(po => po.Subtotal)
                .HasPrecision(18, 2)
                .IsRequired();

            // Configure the tax percentage.
            builder.Property(po => po.TaxRate)
                .HasPrecision(5, 2)
                .IsRequired();

            builder.Property(po => po.TaxAmount)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(po => po.TotalAmount)
                .HasPrecision(18, 2)
                .IsRequired();

            // Configure optional notes.
            builder.Property(po => po.Notes)
                .HasMaxLength(1000)
                .IsRequired(false);

            // Configure creation and update dates.
            builder.Property(po => po.CreatedAt)
                .IsRequired();

            builder.Property(po => po.UpdatedAt)
                .IsRequired(false);

            builder.Property(po => po.ApprovedAt)
                .IsRequired(false);

            // Add indexes for common purchase order searches.
            builder.HasIndex(po => po.SupplierId);

            builder.HasIndex(po => po.Status);

            builder.HasIndex(po => po.OrderDate);

            // One purchase order can contain multiple purchase order items.
            builder.HasMany(po => po.Items)
                .WithOne(item => item.PurchaseOrder)
                .HasForeignKey(item => item.PurchaseOrderId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
