
using HealthcareInventoryManagementSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HealthcareInventoryManagementSystem.Infrastructure.Persistence.Configurations
{
    public class PurchaseOrderItemConfiguration : IEntityTypeConfiguration<PurchaseOrderItemEntity>
    {
        public void Configure(EntityTypeBuilder<PurchaseOrderItemEntity> builder)
        {
            // Set the database table name.
            builder.ToTable("PurchaseOrderItems");

            // Configure the primary key.
            builder.HasKey(item => item.PurchaseOrderItemId);

            // Configure the relationship with the purchase order.
            builder.HasOne(item => item.PurchaseOrder)
                .WithMany(po => po.Items)
                .HasForeignKey(item => item.PurchaseOrderId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure the relationship with the existing inventory item.
            builder.HasOne(item => item.Item)
                .WithMany()
                .HasForeignKey(item => item.ItemId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure ordered quantity.
            builder.Property(item => item.OrderedQuantity)
                .IsRequired();

            // Configure received quantity.
            builder.Property(item => item.ReceivedQuantity)
                .IsRequired()
                .HasDefaultValue(0);

            // Configure unit price.
            builder.Property(item => item.UnitPrice)
                .HasPrecision(18, 2)
                .IsRequired();

            // Configure total price for this purchase order line.
            builder.Property(item => item.LineTotal)
                .HasPrecision(18, 2)
                .IsRequired();

            // Prevent adding the same inventory item twice to one order.
            builder.HasIndex(item => new
            {
                item.PurchaseOrderId,
                item.ItemId
            }).IsUnique();

            // Add an index for inventory item searches.
            builder.HasIndex(item => item.ItemId);
        }
    }
}
