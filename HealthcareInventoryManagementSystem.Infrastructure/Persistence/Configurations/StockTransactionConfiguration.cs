using HealthcareInventoryManagementSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HealthcareInventoryManagementSystem.Infrastructure.Persistence.Configurations
{
    public class StockTransactionConfiguration
        : IEntityTypeConfiguration<StockTransactionEntity>
    {
        public void Configure(EntityTypeBuilder<StockTransactionEntity> builder)
        {
            // Set the primary key.
            builder.HasKey(t => t.StockTransactionId);

            // Configure the transaction type.
            builder.Property(t => t.TransactionType)
                .HasMaxLength(30)
                .IsRequired();

            // Configure the transaction quantity.
            builder.Property(t => t.Quantity)
                .IsRequired();

            // Configure the previous and new quantities.
            builder.Property(t => t.PreviousQuantity)
                .IsRequired();

            builder.Property(t => t.NewQuantity)
                .IsRequired();

            // Configure the optional reason.
            builder.Property(t => t.Reason)
                .HasMaxLength(500);

            // Configure the transaction date.
            builder.Property(t => t.TransactionDate)
                .IsRequired();

            // Each transaction belongs to one stock batch.
            // A batch can have multiple transaction records.
            builder.HasOne(t => t.StockBatch)
                .WithMany()
                .HasForeignKey(t => t.StockBatchId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
