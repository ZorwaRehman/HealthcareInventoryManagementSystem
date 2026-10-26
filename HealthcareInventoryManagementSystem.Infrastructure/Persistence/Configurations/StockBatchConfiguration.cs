using HealthcareInventoryManagementSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HealthcareInventoryManagementSystem.Infrastructure.Persistence.Configurations
{
    // Configures how stock batches are stored in the database.
    public class StockBatchConfiguration : IEntityTypeConfiguration<StockBatchEntity>
    {
        public void Configure(EntityTypeBuilder<StockBatchEntity> builder)
        {
            // Set the primary key.
            builder.HasKey(b => b.StockBatchId);

            // Configure the batch number.
            builder.Property(b => b.BatchNumber)
                .HasMaxLength(50)
                .IsRequired();

            // Configure the quantity.
            builder.Property(b => b.QuantityInStock)
                .IsRequired();

            // Store unit price with two decimal places.
            builder.Property(b => b.UnitPrice)
                .HasPrecision(18, 2);

            // Configure the relationship with ItemEntity.
            builder.HasOne(b => b.Item)
                .WithMany()
                .HasForeignKey(b => b.ItemId)
                .OnDelete(DeleteBehavior.Restrict);

            // Prevent duplicate batch numbers for the same item.
            builder.HasIndex(b => new { b.ItemId, b.BatchNumber })
                .IsUnique();
        }
    }
}
