
using HealthcareInventoryManagementSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HealthcareInventoryManagementSystem.Infrastructure.Persistence.Configurations
{
    public class GoodsReceivedItemConfiguration
        : IEntityTypeConfiguration<GoodsReceivedItemEntity>
    {
        public void Configure(EntityTypeBuilder<GoodsReceivedItemEntity> builder)
        {
            // Set the database table name.
            builder.ToTable("GoodsReceivedItems");

            // Configure the primary key.
            builder.HasKey(item => item.GoodsReceivedItemId);

            // Configure the Goods Received document relationship.
            builder.HasOne(item => item.GoodsReceived)
                .WithMany(gr => gr.Items)
                .HasForeignKey(item => item.GoodsReceivedId)
                .OnDelete(DeleteBehavior.Restrict);

            // Link the received item to its exact Purchase Order item.
            builder.HasOne(item => item.PurchaseOrderItem)
                .WithMany()
                .HasForeignKey(item => item.PurchaseOrderItemId)
                .OnDelete(DeleteBehavior.Restrict);

            // Link the optional existing stock batch.
            builder.HasOne(item => item.StockBatch)
                .WithMany()
                .HasForeignKey(item => item.StockBatchId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            // Configure the supplier's batch or lot number.
            builder.Property(item => item.BatchNumber)
                .HasMaxLength(50)
                .IsRequired();

            // Configure all quantity fields.
            builder.Property(item => item.DeliveredQuantity)
                .IsRequired();

            builder.Property(item => item.AcceptedQuantity)
                .IsRequired();

            builder.Property(item => item.RejectedQuantity)
                .IsRequired();

            builder.Property(item => item.QuarantinedQuantity)
                .IsRequired();

            // Configure optional manufacturing and expiry dates.
            builder.Property(item => item.ManufactureDate)
                .IsRequired(false);

            builder.Property(item => item.ExpiryDate)
                .IsRequired(false);

            // Configure optional storage location.
            builder.Property(item => item.StorageLocation)
                .HasMaxLength(100)
                .IsRequired(false);

            // Add indexes for frequently queried relationships.
            builder.HasIndex(item => item.GoodsReceivedId);
            builder.HasIndex(item => item.PurchaseOrderItemId);
            builder.HasIndex(item => item.StockBatchId);
        }
    }
}
