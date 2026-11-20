
using HealthcareInventoryManagementSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HealthcareInventoryManagementSystem.Infrastructure.Persistence.Configurations
{
    public class PurchaseOrderDailySequenceConfiguration
        : IEntityTypeConfiguration<PurchaseOrderDailySequenceEntity>
    {
        public void Configure(
            EntityTypeBuilder<PurchaseOrderDailySequenceEntity> builder)
        {
            builder.ToTable("PurchaseOrderDailySequences");

            // One sequence counter per Pakistan local calendar date.
            builder.HasKey(sequence => sequence.OrderDate);

            builder.Property(sequence => sequence.OrderDate)
                .HasColumnType("date")
                .IsRequired();

            builder.Property(sequence => sequence.LastSequence)
                .IsRequired();
        }
    }
}
