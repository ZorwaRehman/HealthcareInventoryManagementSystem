
using HealthcareInventoryManagementSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HealthcareInventoryManagementSystem.Infrastructure.Persistence.Configurations
{
    public class GoodsReceivedConfiguration
        : IEntityTypeConfiguration<GoodsReceivedEntity>
    {
        public void Configure(EntityTypeBuilder<GoodsReceivedEntity> builder)
        {
            // Set the database table name.
            builder.ToTable("GoodsReceived");

            // Configure the primary key.
            builder.HasKey(gr => gr.GoodsReceivedId);

            // Configure the unique receipt number.
            builder.Property(gr => gr.ReceiptNumber)
                .HasMaxLength(40)
                .IsRequired();

            builder.HasIndex(gr => gr.ReceiptNumber)
                .IsUnique();

            // Configure the Purchase Order relationship.
            builder.HasOne(gr => gr.PurchaseOrder)
                .WithMany()
                .HasForeignKey(gr => gr.PurchaseOrderId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure the received date.
            builder.Property(gr => gr.ReceivedDate)
                .IsRequired();

            // Configure the optional delivery note number.
            builder.Property(gr => gr.DeliveryNoteNumber)
                .HasMaxLength(100)
                .IsRequired(false);

            // Configure optional notes.
            builder.Property(gr => gr.Notes)
                .HasMaxLength(1000)
                .IsRequired(false);

            // Store the status enum as an integer.
            builder.Property(gr => gr.Status)
                .HasConversion<int>()
                .IsRequired();

            // Configure timestamps.
            builder.Property(gr => gr.CreatedAt)
                .IsRequired();

            builder.Property(gr => gr.ConfirmedAt)
                .IsRequired(false);

            // Add indexes for common searches.
            builder.HasIndex(gr => gr.PurchaseOrderId);
            builder.HasIndex(gr => gr.Status);
            builder.HasIndex(gr => gr.ReceivedDate);

            // One receipt can contain multiple received items.
            builder.HasMany(gr => gr.Items)
                .WithOne(item => item.GoodsReceived)
                .HasForeignKey(item => item.GoodsReceivedId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
